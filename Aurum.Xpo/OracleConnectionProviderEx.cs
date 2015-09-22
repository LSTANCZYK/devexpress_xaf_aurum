using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Helpers;

namespace Aurum.Xpo
{
    /// <summary>
    /// Поставщик данных Oracle с дополнительными возможностями (запись в скрипт, безопасный доступ к данным и др.)
    /// </summary>
    /// <remarks>Для использования поставщика данных OracleConnectionProviderEx необходимо зарегистрировать используемые 
    /// справочники метаданных в <see cref="T:XPDictionaryInformer"/>, из которого поставщик данных получает названия схем таблиц, заданных 
    /// атрибутами <see cref="T:PersistentSchemaAttribute"/>. Также, если используется создание поставщика на основе строки соединения, 
    /// то необходимо зарегистрировать самого поставщика <see cref="M:Register"/>.</remarks>
    /// <example><code title="Пример простой регистрации поставщика данных в Xaf-приложении">
    /// protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args)
    /// {
    ///     // Регистрация класса поставщика данных
    ///     OracleConnectionProviderEx.Register();
    ///     // Строка соединения: XpoProvider=SecuredODP;...
    ///     XPObjectSpaceProvider provider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection);  
    ///     // Регистрация справочника метаданных вместе со сборкой, содержащей атрибуты схем системных модулей DevExpress
    ///     XPDictionaryInformer.Register(provider.XPDictionary, typeof(MyXafWindowsFormsApplication).Assembly);
    ///     // Установка определений схем базы данных из конфигурационного файла
    ///     XPDictionaryInformer.SetupSchemaAttributes(TablePrefixes);
    ///     // Возвращаем результат
    ///     args.ObjectSpaceProvider = provider;
    /// }
    /// </code></example>
    public class OracleConnectionProviderEx : ODPConnectionProvider, 
        ISecuredSqlGeneratorFormatter, ISqlDataStoreSafe, ISqlDataStoreCancelling, ISqlDataStoreSecurity
    {
        /// <summary>
        /// Конструктор поставщика данных с указанием соединения и опции автосоздания структуры данных
        /// </summary>
        /// <param name="connection">Соединение с базой данных</param>
        /// <param name="autoCreateOption">Опция автосоздания структуры данных</param>
        public OracleConnectionProviderEx(IDbConnection connection, AutoCreateOption autoCreateOption)
            : base(connection, autoCreateOption)
        {
            OnOpenConnection();
        }

        #region Создание поставщика на основе строки соединения (DatabaseConnectionString)

        /// <summary>Тип поставщика данных</summary>
        public new const string XpoProviderTypeString = "SecuredODP";
        
        /// <summary>Название параметра файла скрипта в строке соединения</summary>
        public const string ScriptParameterName = "Script";

        /// <summary>Название параметра опций обновления в строке соединения</summary>
        /// <seealso cref="UpdateSchemaOptions"/>
        public const string UpdateOptionsParameterName = "Options";

        /// <summary>Название параметра регистронезависимости условий в строке соединения</summary>
        /// <remarks>Параметр устанавливает параметры сессии <b>NLS_SORT</b> и <b>NLS_COMP</b></remarks>
        public const string InsesnsitiveParameterName = "Insensitive";

        /// <summary>Название параметра табличного пространства индексов в строке соединения</summary>
        public const string IndexTablespaceParameterName = "IndexTablespace";

        /// <summary>
        /// Возвращает строку соединения с поставщиком данных с указанными параметрами
        /// </summary>
        /// <param name="server">Сервер базы данных</param>
        /// <param name="userid">Идентификатор (логин) пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <returns>Строка соединения с поставщиком данных</returns>
        public static new string GetConnectionString(string server, string userid, string password)
        {
            return String.Format("{3}={4};Data Source={0};user id={1}; password={2};", server, userid, password, DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString);
        }

        /// <summary>
        /// Создает поставщика данных на основе строки соединения
        /// </summary>
        /// <param name="connectionString">Строка соединения с поставщиком данных</param>
        /// <param name="autoCreateOption">Опция автосоздания структуры базы данных</param>
        /// <param name="objectsToDisposeOnDisconnect">Объекты, требующие удаления после отключения соединения</param>
        /// <returns>Экземпляр поставщика данных OracleConnectionProviderEx</returns>
        public static new IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect)
        {
            // Файл для записи скрипта
            string scriptPath = null;
            UpdateSchemaOptions updateOptions = UpdateSchemaOptions.Default;
            bool? insensitive = null;
            string indexTablespace = null;
            ConnectionStringParser parser = new ConnectionStringParser(connectionString);
            if (parser.PartExists(ScriptParameterName))
            {
                scriptPath = parser.GetPartByName(ScriptParameterName);
                parser.RemovePartByName(ScriptParameterName);
                connectionString = parser.GetConnectionString();
            }

            // Опции обновления
            if (parser.PartExists(UpdateOptionsParameterName))
            {
                updateOptions = (UpdateSchemaOptions)Convert.ToInt32(parser.GetPartByName(UpdateOptionsParameterName));
                parser.RemovePartByName(UpdateOptionsParameterName);
                connectionString = parser.GetConnectionString();
            }

            // Регистронезависимость в условиях
            if (parser.PartExists(InsesnsitiveParameterName))
            {
                insensitive = Convert.ToBoolean(parser.GetPartByName(InsesnsitiveParameterName));
                parser.RemovePartByName(InsesnsitiveParameterName);
                connectionString = parser.GetConnectionString();
            }

            // Табличное пространство для индексов
            if (parser.PartExists(IndexTablespaceParameterName))
            {
                indexTablespace = parser.GetPartByName(IndexTablespaceParameterName);
                parser.RemovePartByName(IndexTablespaceParameterName);
                connectionString = parser.GetConnectionString();
            }

            // Соединение
            IDbConnection connection = CreateConnection(connectionString);
            objectsToDisposeOnDisconnect = new IDisposable[] { connection };
            return scriptPath == null && updateOptions == UpdateSchemaOptions.Default && !insensitive.HasValue && indexTablespace == null ? 
                CreateProviderFromConnection(connection, autoCreateOption) : 
                new OracleConnectionProviderEx(connection, autoCreateOption, scriptPath, updateOptions, insensitive, indexTablespace);
        }

        /// <summary>
        /// Создает поставщика данных на основе соединения с базой данных
        /// </summary>
        /// <param name="connection">Соединение с базой данных</param>
        /// <param name="autoCreateOption">Опция автосоздания структуры базы данных</param>
        /// <returns>Экземпляр поставщика данных OracleConnectionProviderEx</returns>
        public static new IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption)
        {
            return new OracleConnectionProviderEx(connection, autoCreateOption);
        }

        static OracleConnectionProviderEx()
        {
			RegisterDataStoreProvider(XpoProviderTypeString, new DataStoreCreationFromStringDelegate(CreateProviderFromString));
            // ODPConnectionProvider тоже регистрирует свой делегат под этим именем, поэтому неизвестно какой из них будет создан,
            // но со строкой этой проблемы нет, так как тип поставщика данных указывается явно
            RegisterDataStoreProvider("Oracle.DataAccess.Client.OracleConnection", new DataStoreCreationFromConnectionDelegate(CreateProviderFromConnection));
        }

        /// <summary>Регистрация поставщика данных</summary>
        public static new void Register() { }

        /// <summary>Регистронезависимость в строковых параметрах условий</summary>
        private bool? insensitive = null;

        /// <inheritdoc/>
        protected override void OpenConnectionInternal()
        {
            base.OpenConnectionInternal();
            OnOpenConnection();
        }

        /// <summary>
        /// Вызывается при создании или открытии соединения
        /// </summary>
        protected virtual void OnOpenConnection()
        {
            if (Connection.State != ConnectionState.Open) return;

            // Установка параметров сессии
            if (insensitive == true)
            {
                ExecuteCommand("alter SESSION set NLS_SORT=BINARY_CI");
                ExecuteCommand("alter SESSION set NLS_COMP=LINGUISTIC");
            }
            if (insensitive == false)
            {
                ExecuteCommand("alter SESSION set NLS_COMP=BINARY");
            }
        }

        #endregion

        #region Адаптер команд

        private bool scriptMode = false;
        private string scriptPath;
        private StreamWriter scriptWriter;
        private int scriptCommands;
        private StringBuilder scriptUnsafe;

        /// <summary>
        /// Конструктор базы данных с указанием соединения, опции автосоздания структуры данных и потока для записи скрипта
        /// </summary>
        /// <param name="connection">Соединение с базой данных</param>
        /// <param name="autoCreateOption">Опция автосоздания структуры данных</param>
        /// <param name="scriptWriter">Поток для записи изменений структуры в скрипт</param>
        /// <param name="updateOptions">Опции обновления структуры данных</param>
        /// <param name="insensitive">Регистронезависимость условий</param>
        /// <param name="indexTablespace">Табличное пространство для индексов</param>
        public OracleConnectionProviderEx(IDbConnection connection, AutoCreateOption autoCreateOption, StreamWriter scriptWriter, 
            UpdateSchemaOptions updateOptions = UpdateSchemaOptions.Default, bool? insensitive = null, string indexTablespace = null)
            : base(connection, autoCreateOption)
        {
            this.scriptMode = scriptWriter != null;
            this.scriptWriter = scriptWriter;
            this.updateOptions = updateOptions;
            this.insensitive = insensitive;
            this.indexTablespace = indexTablespace;
            OnOpenConnection();
        }

        /// <summary>
        /// Конструктор базы данных с указанием соединения, опции автосоздания структуры данных и пути файла для записи скрипта
        /// </summary>
        /// <param name="connection">Соединение с базой данных</param>
        /// <param name="autoCreateOption">Опция автосоздания структуры данных</param>
        /// <param name="scriptPath">Путь файла для записи изменений структуры в скрипт</param>
        /// <param name="updateOptions">Опции обновления структуры данных</param>
        /// <param name="insensitive">Регистронезависимость условий</param>
        /// <param name="indexTablespace">Табличное пространство для индексов</param>
        public OracleConnectionProviderEx(IDbConnection connection, AutoCreateOption autoCreateOption, string scriptPath,
            UpdateSchemaOptions updateOptions = UpdateSchemaOptions.Default, bool? insensitive = null, string indexTablespace = null)
            : base(connection, autoCreateOption)
        {
            this.scriptMode = !string.IsNullOrEmpty(scriptPath);
            this.scriptPath = scriptPath;
            this.updateOptions = updateOptions;
            this.insensitive = insensitive;
            this.indexTablespace = indexTablespace;
            OnOpenConnection();
        }

        /// <summary>
        /// Переопределяет создание команды базы данных, добавляя возможность создания адаптера.
        /// Если адаптер команды не создан, то возвращается оригинальная команда.
        /// </summary>
        /// <returns>Команда базы данных или ее адаптер</returns>
        public override IDbCommand CreateCommand()
        {
            IDbCommand command = base.CreateCommand();
            if (scriptMode && scriptWriter != null)
            {
                OracleCommandScript adapter = new OracleCommandScript(command, scriptWriter);
                adapter.BeforeWrite = OnBeforeWrite;
                return adapter;
            }
            return command; 
        }

        private void OnBeforeWrite(object sender, EventArgs e)
        {
            if (scriptCommands == 0 && scriptWriter != null)
                scriptWriter.WriteLine(string.Format("-- {0} Update schema auto script", DateTime.Now));
            scriptCommands++;
        }

        /// <summary>
        /// Устанавливает параметры для создания адаптера команды
        /// </summary>
        protected virtual void SetCommandAdapterParameters()
        {
            if (scriptMode && scriptPath != null)
                scriptWriter = new StreamWriter(scriptPath, true);
            scriptCommands = 0;
            if (scriptMode && ((updateOptions & UpdateSchemaOptions.UnsafeChanges) != 0))
                scriptUnsafe = new StringBuilder();
        }

        /// <summary>
        /// Освобождает параметры адаптера команды
        /// </summary>
        protected virtual void FreeCommandAdapterParameters()
        {
            if (scriptMode && scriptPath != null && scriptWriter != null)
            {
                if (scriptCommands > 0)
                    scriptWriter.WriteLine(string.Format("-- {0} End of script. Total commands: {1}", DateTime.Now, scriptCommands));
                scriptWriter.Close();
                scriptWriter.Dispose();
                scriptWriter = null;
            }
            scriptUnsafe = null;
        }

        #endregion

        #region Форматирование названий объектов БД, элементов sql-запросов (исключение кавычек, нижний регистр, invariant)

        /// <remarks>Возвращает макисмальную длину названия таблицы 26 символов для возможности добавления префикса (vw_, pkg_)</remarks>
        protected override int GetSafeNameTableMaxLength()
        {
            return 26;
        }

        /// <inheritdoc/>
        /// <remarks>В случае, когда название схемы не задано, то она определяется с учетом атрибута <see cref="PersistentSchemaAttribute"/>. 
        /// Для этого требуется обязательная регистрация поставщиков метаданных в информаторе <see cref="XPDictionaryInformer"/>.</remarks>
        public override string ComposeSafeSchemaName(string tableName)
        {
            return base.ComposeSafeSchemaName(tableName).ToUpper();
        }

        /// <summary>
        /// Возвращает фактическое название схемы для указанной таблицы в формате sql-команды
        /// </summary>
        /// <param name="tableName">Исходное название таблицы</param>
        /// <returns>Название схемы в формате sql-команды, заданное в исходном названии таблицы, 
        /// или имя текущего пользователя, если схема в названии таблицы не указана</returns>
        public string FormatRealSchemaName(string tableName)
        {
            string schemaName = ComposeSafeSchemaName(tableName);
            return (string.IsNullOrEmpty(schemaName) ? CurrentUserName : schemaName).ToLowerInvariant();
        }

        private static readonly HashSet<string> reservedWords = new HashSet<string>(new string[] { 
            // http://docs.oracle.com/cd/E11882_01/appdev.112/e10830/appb.htm
            "access", "add", "all", "alter", "and", "any", "arraylen", "as", "asc", "audit", "between", "by", "char", "check", "cluster", "column", "comment", "compress", "connect", "create", "current", "date", "decimal", "default", "delete", "desc", "distinct", "drop", 
            "else", "exclusive", "exists", "file", "float", "for", "from", "grant", "group", "having", "identified", "immediate", "in", "increment", "index", "initial", "insert", "integer", "intersect", "into", "is", "level", "like", "lock", "long", "maxextents", "minus", "mode", 
            "modify", "noaudit", "nocompress", "not", "notfound", "nowait", "null", "number", "of", "offline", "on", "online", "option", "or", "order", "pctfree", "prior", "privileges", "public", "raw", "rename", "resource", "revoke", "row", "rowid", "rowlabel", "rownum", "rows", 
            "start", "select", "session", "set", "share", "size", "smallint", "sqlbuf", "successful", "synonym", "sysdate", "table", "then", "to", "trigger", "uid", "union", "unique", "update", "user", "validate", "values", "varchar", "varchar2", "view", "whenever", "where", "with",
            // Дополнительно 
            "role", "time", "type" });

        /// <summary>
        /// Возвращает модифицированное название с учетом зарезирвированных слов
        /// </summary>
        /// <param name="name">Оригинальное название</param>
        /// <returns>Если название совпадает с зарезервированным словом, то результат модифицируется, иначе возвращает оригинальное название</returns>
        protected string ModifyReservedWords(string name)
        {
            if (name == null) return name;
            return reservedWords.Contains(name.ToLower()) ? "f" + name : name;
        }

        /// <inheritdoc/>
        public override string ComposeSafeTableName(string tableName)
        {
            int dot = tableName.IndexOf('.');
            if (dot > 0) tableName = tableName.Remove(0, dot + 1);
            tableName = ModifyReservedWords(tableName);
            tableName = GetSafeObjectName(tableName, GetSafeNameRoot(tableName), GetSafeNameTableMaxLength());
            return tableName.ToUpper();
        }

        /// <inheritdoc/>
        public override string ComposeSafeColumnName(string columnName)
        {
            return base.ComposeSafeColumnName(ModifyReservedWords(columnName)).ToUpper();
        }

        /// <inheritdoc/>
        public override string ComposeSafeConstraintName(string constraintName)
        {
            return base.ComposeSafeConstraintName(constraintName).ToUpper();
        }

        /// <inheritdoc/>
        public override string FormatTable(string schema, string tableName)
        {
            return (string.IsNullOrEmpty(schema) ? tableName : string.Concat(schema, ".", tableName)).ToLowerInvariant();
        }

        /// <inheritdoc/>
        public override string FormatTable(string schema, string tableName, string tableAlias)
        {
            return (string.IsNullOrEmpty(schema) ? string.Concat(tableName, " ", tableAlias) : 
                string.Concat(schema, ".", tableName, " ", tableAlias)).ToLowerInvariant();
        }

        /// <inheritdoc/>
        public override string FormatColumn(string columnName)
        {
            return columnName.ToLowerInvariant();
        }

        /// <inheritdoc/>
        public override string FormatColumn(string columnName, string tableAlias)
        {
            return string.Concat(tableAlias, ".", columnName).ToLowerInvariant();
        }

        /// <inheritdoc/>
        public override string FormatConstraint(string constraintName)
        {
            return constraintName.ToLowerInvariant();
        }

        /// <inheritdoc/>
        protected override string GetSeqName(string tableName)
        {
            string schema = ComposeSafeSchemaName(tableName);
            string table = ComposeSafeTableName(tableName);
            return FormatSequence(schema, table);
        }

        /// <inheritdoc/>
        protected override string GetPrimaryKeyName(DBPrimaryKey cons, DBTable table)
        {
            if (cons.Name != null) return cons.Name;
            return string.Concat("pk_", ComposeSafeTableName(table.Name));
        }

        /// <inheritdoc/>
        protected override string GetForeignKeyName(DBForeignKey cons, DBTable table)
        {
            if (cons.Name != null) return cons.Name;
            StringBuilder sb = new StringBuilder();
            sb.Append("fk_");
            sb.Append(ComposeSafeTableName(table.Name));
            foreach (string col in cons.Columns) sb.Append(col);
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string GetIndexName(DBIndex cons, DBTable table)
        {
            if (cons.Name != null) return cons.Name;
            StringBuilder sb = new StringBuilder();
            sb.Append("idx_");
            sb.Append(ComposeSafeTableName(table.Name));
            foreach (string col in cons.Columns) sb.Append(col);
            return sb.ToString();
        }

        /// <summary>
        /// Возвращает название констрейнта по указанному выражению
        /// </summary>
        /// <param name="expression">Выражение констрейнта</param>
        /// <param name="table">Таблица</param>
        /// <returns>Название констрейнта</returns>
        protected virtual string GetConstraintName(string expression, DBTable table)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("chk_");
            sb.Append(ComposeSafeTableName(table.Name));
            char[] criteriaChars = expression.ToCharArray();
            foreach (char criteriaChar in criteriaChars)
                if (char.IsLetterOrDigit(criteriaChar)) sb.Append(criteriaChar);
            return sb.ToString();
        }

        #endregion

        #region Безопасный доступ к данным (ISecuredSqlGeneratorFormatter)

        /// <contentfrom cref="ISecuredSqlGeneratorFormatter.FormatView(string,string)" />
        public virtual string FormatView(string schema, string tableName)
        {
            return (string.IsNullOrEmpty(schema) ? string.Concat("vw_", tableName) : 
                string.Concat(schema, ".vw_", tableName)).ToLowerInvariant();
        }

        /// <contentfrom cref="ISecuredSqlGeneratorFormatter.FormatView(string,string,string)" />
        public virtual string FormatView(string schema, string tableName, string viewAlias)
        {
            return (string.IsNullOrEmpty(schema) ? string.Concat("vw_", tableName, " ", viewAlias) : 
                string.Concat(schema, ".vw_", tableName, " ", viewAlias)).ToLowerInvariant();
        }

        /// <contentfrom cref="ISecuredSqlGeneratorFormatter.FormatPackage" />
        public virtual string FormatPackage(string schema, string tableName)
        {
            return (string.IsNullOrEmpty(schema) ? string.Concat("pkg_", tableName) :
                string.Concat(schema, ".pkg_", tableName)).ToLowerInvariant();
        }

        /// <summary>Формат стандартной процедуры</summary>
        private string FormatStandartProcedure(string schema, string tableName, string suffix)
        {
            if (!string.IsNullOrEmpty(schema)) schema = string.Concat(schema, ".");
            return string.Concat(schema, "p_", tableName, suffix).ToLowerInvariant();
        }

        /// <contentfrom cref="ISecuredSqlGeneratorFormatter.FormatProcedureAdd" />
        public virtual string FormatProcedureAdd(string schema, string tableName)
        {
            return FormatStandartProcedure(schema, tableName, "_a");
        }

        /// <contentfrom cref="ISecuredSqlGeneratorFormatter.FormatProcedureEdit" />
        public virtual string FormatProcedureEdit(string schema, string tableName)
        {
            return FormatStandartProcedure(schema, tableName, "_e");
        }

        /// <contentfrom cref="ISecuredSqlGeneratorFormatter.FormatProcedureDelete" />
        public virtual string FormatProcedureDelete(string schema, string tableName)
        {
            return FormatStandartProcedure(schema, tableName, "_d");
        }

        /// <contentfrom cref="ISecuredSqlGeneratorFormatter.FormatProcedureParameter" />
        public virtual string FormatProcedureParameter(string columnName)
        {
            return string.Concat("p", GetSafeObjectName(columnName, GetSafeNameRoot(columnName), GetSafeNameColumnMaxLength() - 1));
        }

        /// <contentfrom cref="ISecuredSqlGeneratorFormatter.FormatSequence" />
        public string FormatSequence(string schema, string tableName)
        {
            string seqname = ComposeSafeConstraintName(string.Concat("seq_", tableName));
            return (string.IsNullOrEmpty(schema) ? seqname : string.Concat(schema, ".", seqname)).ToLowerInvariant();
        }

        /// <contentfrom cref="ISecuredSqlGeneratorFormatter.FormatSelectInto" />
        public string FormatSelectInto(string propertiesSql, string intoSql, string fromSql, string whereSql)
        {
            whereSql = whereSql == null ? null : "\r\n  where " + whereSql;
            return string.Format(CultureInfo.InvariantCulture, "select {0} into {1}\r\n  from {2}{3}",
                propertiesSql, intoSql, fromSql, whereSql);
        }

        /// <contentfrom cref="ISecuredSqlGeneratorFormatter.FormatCustomTable" />
        public string FormatCustomTable(string tableName, string tableAlias)
        {
            return string.IsNullOrEmpty(tableAlias) ? tableName : string.Concat(tableName, " ", tableAlias);
        }

        /// <contentfrom cref="ISecuredSqlGeneratorFormatter.FormatCustomColumn" />
        public string FormatCustomColumn(string columnName, string tableAlias)
        {
            return string.IsNullOrEmpty(tableAlias) ? columnName : string.Concat(tableAlias, ".", columnName);
        }

        /// <summary>Получить название представления для sql-запроса для указанной таблицы</summary>
        /// <param name="table">Таблица</param>
        /// <returns>Название представления таблицы <paramref name="table"/> в формате sql-запроса</returns>
        public string FormatView(DBTable table)
        {
            return FormatView(ComposeSafeSchemaName(table.Name), ComposeSafeTableName(table.Name));
        }

        /// <summary>Получить название пакета для sql-команды для указанной таблицы</summary>
        /// <param name="table">Таблица</param>
        /// <returns>Название пакета таблицы <paramref name="table"/> в формате sql-команды</returns>
        public string FormatPackage(DBTable table)
        {
            return FormatPackage(ComposeSafeSchemaName(table.Name), ComposeSafeTableName(table.Name));
        }

        #endregion

        #region Отмена команд sql-запросов (ISqlDataStoreCancelling)

        private object selectCancelLocker = new object();
        private IDbCommand selectCommand;

        /// <contentfrom cref="ISqlDataStoreCancelling.CanCancelSelect" />
        public bool CanCancelSelect
        {
            get { return true; }
        }

        /// <contentfrom cref="ISqlDataStoreCancelling.CancelSelect" />
        public void CancelSelect()
        {
            lock (selectCancelLocker)
            {
                if (selectCommand != null)
                    selectCommand.Cancel();
            }
        }

        #endregion

        #region Выборка и модификация данных

        /// <summary>
        /// Выполняет выборку данных
        /// </summary>
        protected override SelectStatementResult ProcessSelectData(SelectStatement selects)
        {
            // Кастомизация выборки данных
            DBTableEx root = XPDictionaryInformer.TranslateAndGet(selects.TableName);
            if (root.IsCustom && root.CustomPersistent is ICustomPersistentQuery)
                return ((ICustomPersistentQuery)root.CustomPersistent).Select(this, selects);
            StringCollection customAliases;
            CustomPersistentSelectProcessor.Execute(selects, this, out customAliases);

            // Выборка данных
            Query query = new SecuredSelectSqlGenerator(this, customAliases).GenerateSql(selects);
            return SelectData(query, selects.Operands);
        }

        /// <summary>
        /// Выполняет модификацию данных
        /// </summary>
        protected override ModificationResult ProcessModifyData(params ModificationStatement[] dmlStatements)
        {
            BeginTransaction();
            try
            {
                List<ParameterValue> result = new List<ParameterValue>();
                TaggedParametersHolder identities = new TaggedParametersHolder();
                foreach (ModificationStatement root in dmlStatements)
                {
                    ParameterValue res = ModifyRecords(root, identities);
                    if (!object.ReferenceEquals(res, null)) result.Add(res);
                }
                CommitTransaction();
                return new ModificationResult(result);
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
        }

        /// <summary>
        /// Модификация записей по указанному выражению
        /// </summary>
        /// <param name="root">Выражение на модификацию записей</param>
        /// <param name="identities">Держатель одинаковых идентификаторов</param>
        /// <returns>Результат модификации</returns>
        /// <todo>Пакетное добавление данных (batch insert)</todo>
        protected virtual ParameterValue ModifyRecords(ModificationStatement root, TaggedParametersHolder identities)
        {
            // Кастомизированная модификация
            DBTableEx table = XPDictionaryInformer.TranslateAndGet(root.TableName);
            if (table.IsCustom)
            {
                ParameterValue result = null;
                if (root is InsertStatement) result = table.CustomPersistent.Insert(this, (InsertStatement)root);
                if (root is UpdateStatement) table.CustomPersistent.Update(this, (UpdateStatement)root);
                if (root is DeleteStatement) table.CustomPersistent.Delete(this, (DeleteStatement)root); 
                return result;
            }

            // Обычная модификация
            Dictionary<OperandValue, string> parameters = new Dictionary<OperandValue, string>();
            Query query = new OracleSecuredModifySqlGenerator(this, identities, parameters).GenerateSql(table, root);
            ParameterValue id = root is InsertStatement ? ((InsertStatement)root).IdentityParameter : null;
            ExecuteBlock(query, id);
            return id;
        }

        /// <summary>
        /// Исполнение блока sql-команд
        /// </summary>
        /// <param name="query">Sql-команда</param>
        /// <param name="parameterID">Параметр, возвращающий значение новой записи</param>
        /// <param name="commandType">Тип команды, если не указан, то определяется по содержанию ключевого слова <b>begin</b></param>
        protected void ExecuteBlock(Query query, ParameterValue parameterID, System.Data.CommandType? commandType = null)
        {
            IDbCommand command = CreateCommand(query);
            if (commandType.HasValue)
                command.CommandType = commandType.Value;
            else if (!command.CommandText.ToLower().Contains("begin"))
                command.CommandType = System.Data.CommandType.StoredProcedure;
            IDataParameter id = null;
            if (!object.ReferenceEquals(parameterID, null) && command.Parameters.Count > 0)
            {
                id = (IDataParameter)command.Parameters[0];
                id.Direction = ParameterDirection.Output;
                id.DbType = DbType.Int32;
            }
            try
            {
                command.ExecuteNonQuery();
                if (id != null) parameterID.Value = id.Value;
            }
            catch (Exception e)
            {
                throw WrapException(e, command);
            }
        }

        /// <summary>
        /// Исполнение sql-команды
        /// </summary>
        /// <param name="sql">Текст sql-команды</param>
        protected void ExecuteCommand(string sql)
        {
            ExecuteBlock(new Query(sql), null, System.Data.CommandType.Text);
        }

        /// <inheritdoc/>
        protected override IDataParameter CreateParameter(IDbCommand command, object value, string name)
        {
            IDataParameter param = base.CreateParameter(command, value, name);
            // По умолчанию Oracle устанавливает Varchar2 для пустого значения и Raw для непустого и большого значения, 
            // а потом выдает ORA-1460: unimplemented or unreasonable conversion requested
            if (value is byte[])
            {
                object byteType = ((byte[])value).Length < MaximumBinarySize ? oracleDbTypeRaw : oracleDbTypeBlob;
                if (!object.Equals(getOracleDbType(param), byteType))
                    setOracleDbType(param, byteType);
            }
            return param;
        }

        private SetPropertyValueDelegate setOracleDbType;
        private GetPropertyValueDelegate getOracleDbType;
        private object oracleDbTypeBlob;
        private object oracleDbTypeRaw;

        /// <inheritdoc/>
        protected override void PrepareDelegates()
        {
            base.PrepareDelegates();
            Type oracleParameterType = ConnectionHelper.GetType("Oracle.DataAccess.Client.OracleParameter");
            Type oracleDbTypeType = ConnectionHelper.GetType("Oracle.DataAccess.Client.OracleDbType");
            Type oracleCommandType = ConnectionHelper.GetType("Oracle.DataAccess.Client.OracleCommand");
            oracleDbTypeBlob = Enum.Parse(oracleDbTypeType, "Blob");
            oracleDbTypeRaw = Enum.Parse(oracleDbTypeType, "Raw");
            ReflectConnectionHelper.CreatePropertyDelegates(oracleParameterType, "OracleDbType", out setOracleDbType, out getOracleDbType);
        }

        /// <inheritdoc/>
        protected override IDbCommand CreateCommand(Query query)
        {
            IDbCommand command = CreateCommand();
            PrepareParameters(command, query);
            command.CommandText = SqlDictionary.Default != null ? SqlDictionary.Default.TransformSql(query.Sql) : query.Sql;
#if !CF
            Trace.WriteLineIf(xpoSwitch.TraceInfo, new DbCommandTracer(command));
#endif
            return command;
        }

        #endregion

        #region Изменение структуры данных

        /// <summary>Опции обновления структуры данных</summary>
        private UpdateSchemaOptions updateOptions = UpdateSchemaOptions.Default;

        /// <summary>Табличное пространство для индексов</summary>
        private string indexTablespace = null;

        /// <contentfrom cref="ISqlDataStoreSafe.UpdateSchema" />
        public UpdateSchemaResult UpdateSchema(UpdateSchemaMode mode, bool dontCreateIfFirstTableNotExist, params DBTable[] tables)
        {
            using (IDisposable c1 = new PerformanceCounters.QueueLengthCounter(PerformanceCounters.SqlDataStoreTotalRequests, PerformanceCounters.SqlDataStoreTotalQueue, PerformanceCounters.SqlDataStoreSchemaUpdateRequests, PerformanceCounters.SqlDataStoreSchemaUpdateQueue))
            {
                lock (SyncRoot)
                {
                    return ProcessUpdateSchema(mode, dontCreateIfFirstTableNotExist, tables);
                }
            }
        }

        /// <inheritdoc />
        protected override UpdateSchemaResult ProcessUpdateSchema(bool skipIfFirstTableNotExists, params DBTable[] tables)
        {
            return ProcessUpdateSchema(UpdateSchemaMode.Full, skipIfFirstTableNotExists, tables); 
        }

        /// <summary>
        /// Выполняет изменение структуры данных с возможностью записи скрипта
        /// </summary>
        protected UpdateSchemaResult ProcessUpdateSchema(UpdateSchemaMode mode, bool skipIfFirstTableNotExists, params DBTable[] tables)
        {
            // Изменение структуры реальной базы данных
            SetCommandAdapterParameters();
            UpdateSchemaResult result;
            try
            {
                result = ProcessUpdateSchema(mode, skipIfFirstTableNotExists, ConvertTables(tables));
                
                // Небезопасные команды изменения структуры, записываемые в комментариях в отдельном блоке
                if (scriptMode && scriptWriter != null && scriptUnsafe != null && scriptUnsafe.Length > 0)
                {
                    scriptWriter.WriteLine("/* Unsafe changes");
                    scriptWriter.WriteLine(scriptUnsafe.ToString());
                    scriptWriter.WriteLine("*/");
                }
            }
            finally
            {
                FreeCommandAdapterParameters();
            }
            if (scriptMode && scriptPath != null && scriptCommands > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Please execute the script and press <Enter>.");
                Console.ResetColor();
                Console.ReadLine();
            }
            return result;
        }

        // Конвертация таблиц в таблицы с информацией о констрейнтах
        private DBTableEx[] ConvertTables(DBTable[] tables)
        {
            DBTableEx[] result = new DBTableEx[tables.Length];
            for (int i = 0; i < tables.Length; i++)
                result[i] = XPDictionaryInformer.TranslateAndGet(tables[i].Name);
            return result;
        }

        /// <summary>
        /// Выполняет изменение структуры данных
        /// </summary>
        protected UpdateSchemaResult ProcessUpdateSchema(UpdateSchemaMode mode, bool skipIfFirstTableNotExists, params DBTableEx[] tables)
        {
            // Конвертация исходных таблиц в таблицы с информацией о констрейнтах
            ICollection collectedTables = CollectTablesToCreate(tables);
            Hashtable createdTables = new Hashtable();
            Hashtable recreateSafe = new Hashtable();
            // Проверка существования первой таблицы
            if (skipIfFirstTableNotExists && collectedTables.Count > 0)
            {
                IEnumerator te = tables.GetEnumerator();
                IEnumerator ce = collectedTables.GetEnumerator();
                te.MoveNext();
                ce.MoveNext();
                if (object.ReferenceEquals(te.Current, ce.Current))
                    return UpdateSchemaResult.FirstTableNotExists;
            }
            // Изменение структуры данных
            if (CanCreateSchema)
                BeginTransaction();
            try
            {
                // Создание таблиц
                if (!CanCreateSchema && collectedTables.Count > 0)
                {
                    IEnumerator ce = collectedTables.GetEnumerator();
                    ce.MoveNext();
                    throw new SchemaCorrectionNeededException("Table '" + ComposeSafeTableName(((DBTable)ce.Current).Name) + "' not found");
                }
                else
                {
                    foreach (DBTableEx table in collectedTables)
                    {
                        if (table.IsView || table.IsCustom) continue;
                        CreateTable(table);
                        CreateSequence(table);
                        CreatePrimaryKey(table);
                        createdTables[table] = table;
                        recreateSafe[table] = table;
                    }
                }
                // Модификация таблиц
                Hashtable realTables = new Hashtable();
                foreach (DBTableEx table in tables)
                {
                    // Пропуск представлений, уже созданных таблиц 
                    // и таблиц с настраиваемой модификацией данных
                    if (table.IsView || createdTables[table] != null || table.IsCustom) continue;
                    DBTableEx realTable = new DBTableEx(table.Name);
                    bool collectIndexes = false;
                    bool collectFKs = false;
                    bool collectConstraints = false;
                    if (CanCreateSchema)
                    {
                        collectIndexes = table.Indexes.Count > 0;
                        collectFKs = table.ForeignKeys.Count > 0;
                        collectConstraints = table.Constraints.Exists(c => c is DBCriteriaConstraint);
                        if (NeedsIndexForForeignKey)
                            collectIndexes = collectIndexes || collectFKs;
                    }
                    GetTableSchema(realTable, collectIndexes, collectFKs, collectConstraints);
                    realTables[table] = realTable;
                    // Новые колонки
                    foreach (DBColumn column in table.Columns)
                    {
                        // Создание колонки
                        DBColumn realColumn = FindColumnByName(realTable, column);
                        if (realColumn == null)
                        {
                            if (!CanCreateSchema) throw new SchemaCorrectionNeededException(string.Format(
                                "Column '{0}' not found in table '{1}'", ComposeSafeColumnName(column.Name), ComposeSafeTableName(table.Name)));
                            CreateColumn(table, column);
                            recreateSafe[table] = table;
                        }
                        // Изменение типа колонки
                        if (realColumn != null && GetDBTableColumnType(table, column) != realColumn.DBTypeName)
                        {
                            if (!CanCreateSchema) throw new SchemaCorrectionNeededException(string.Format(
                                "Type of column '{0}' in table '{1}' is changed to {2}", ComposeSafeColumnName(column.Name), ComposeSafeTableName(table.Name), GetSqlCreateColumnType(table, column)));
                            ChangeColumn(table, realColumn, column);
                            recreateSafe[table] = table;
                        }
                        // Изменение констрейнта обязательности
                        bool notNull = table.ColumnIsNotNull(column);
                        if (realColumn != null && notNull != realTable.ColumnIsNotNull(realColumn))
                        {
                            ChangeColumnNullable(table, column, notNull);
                            recreateSafe[table] = table;
                        }
                    }
                    // Старые колонки
                    if (CanCreateSchema && (updateOptions & UpdateSchemaOptions.UnsafeChanges) != 0)
                    {
                        List<DBColumn> realColumns = new List<DBColumn>(realTable.Columns);
                        foreach (DBColumn column in table.Columns)
                        {
                            // Поиск по колонкам заданной таблицы table, чтобы учесть преобразование имен колонок
                            DBColumn realColumn = FindColumnByName(realTable, column);
                            if (realColumn != null) realColumns.Remove(realColumn);
                        }
                        foreach (DBColumn realColumn in realColumns) 
                            DeleteColumn(realTable, realColumn);
                    }
                    // Последовательность для первичного ключа
                    if (CanCreateSchema && GetTableSequenceColumn(table) != null && !GetSequence(table))
                    {
                        CreateSequence(table);
                        recreateSafe[table] = table;
                    }
                    // Первичный ключ
                    if (CanCreateSchema && table.PrimaryKey != null && realTable.PrimaryKey == null)
                    {
                        CreatePrimaryKey(table);
                        recreateSafe[table] = table;
                    }
                    // Невалидные или отсутствующие представления и пакеты
                    if (recreateSafe[table] == null && !GetValidSafeObjects(table))
                        recreateSafe[table] = table;
                }
                // Индексы, ссылки, констрейнты
                if (CanCreateSchema)
                {
                    foreach (DBTableEx table in tables)
                    {
                        // Новая или измененная таблица
                        DBTableEx realTable = (DBTableEx)realTables[table];
                        if (realTable == null && createdTables[table] != null)
                        {
                            realTable = new DBTableEx(table.Name);
                            realTable.Columns.AddRange(table.Columns);
                        }
                        if (realTable == null) continue;
                        // Индексы
                        foreach (DBIndex index in table.Indexes)
                        {
                            if (!IsIndexExists(realTable, index))
                            {
                                CreateIndex(table, index);
                                realTable.AddIndex(index);
                            }
                        }
                        // Индексы ссылок
                        if (NeedsIndexForForeignKey)
                        {
                            foreach (DBForeignKey fk in table.ForeignKeys)
                            {
                                DBIndex index = new DBIndex(fk.Columns, false);
                                if (!IsIndexExists(realTable, index) &&
                                    (table.PrimaryKey == null || (table.PrimaryKey != null && !IsColumnsEqual(table.PrimaryKey.Columns, index.Columns))))
                                {
                                    CreateIndex(table, index);
                                    realTable.AddIndex(index);
                                }
                            }
                        }
                        // Ссылки
                        foreach (DBForeignKey fk in table.ForeignKeys)
                        {
                            if (!IsForeignKeyExists(realTable, fk))
                                CreateForeignKey(table, fk);
                        }
                        // Констрейнты
                        foreach (DBCriteriaConstraint cons in table.Constraints.Where(c => c is DBCriteriaConstraint))
                        {
                            string expression = OracleCheckConstraintGenerator.GenerateExpression(this, cons.Criteria);
                            if (expression != null && !realTable.Constraints.Exists(
                                c => c is DBCheckConstraint && ((DBCheckConstraint)c).Condition == expression))
                                CreateConstraint(table, expression);
                            if (expression == null) //&& !IsConstraintExists - сложные констрейнты реализованы в пакетах
                                recreateSafe[table] = table;
                        }
                        if (table.Constraints.Exists(c => !(c is DBCriteriaConstraint || c is DBNotNullConstraint)))
                            recreateSafe[table] = table;
                    }
                }
                // Представления и пакеты
                if (CanCreateSchema)
                {
                    bool allSafeObjects = (updateOptions & UpdateSchemaOptions.AllSafeObjects) == UpdateSchemaOptions.AllSafeObjects;
                    foreach (DBTableEx table in tables)
                    {
                        if (table.IsView || table.IsCustom || (!allSafeObjects && recreateSafe[table] == null)) continue;
                        CreateOrReplaceView(table);
                        CreateOrReplacePackage(table);
                    }
                }
                // Завершение транзакции
                if (CanCreateSchema)
                    CommitTransaction();
            }
            catch
            {
                if (CanCreateSchema)
                    RollbackTransaction();
                throw;
            }
            return UpdateSchemaResult.SchemaExists;
        }

        /// <summary>
        /// Определение таблиц, которые отсутствуют в базе данных и должны быть созданы
        /// </summary>
        /// <param name="tables">Исходный набор таблиц</param>
        /// <returns>Список таблиц, которые должны быть созданы</returns>
        public override ICollection CollectTablesToCreate(ICollection tables)
        {
            Dictionary<string, bool> dbTables = new Dictionary<string, bool>();
            Dictionary<string, bool> dbSchemaTables = new Dictionary<string, bool>();

            // if (SysUsersAvailable) - основывается на доступе к sys.user$, в котором нет особой необходимости 
            try
            {
                string queryString =
                    "select o.table_name, o.owner from all_tables o " +
                    "where o.owner <> 'SYS' and o.owner <> 'SYSTEM' and o.table_name in ({0})";
                foreach (SelectStatementResultRow row in GetDataForTables(tables, null, queryString).Rows)  // избавиться от GetDataForTables
                {
                    if (row.Values[0] is DBNull) continue;
                    string tableName = (string)row.Values[0];
                    string schemaName = (string)row.Values[1];
                    string fullName = string.Concat(schemaName, ".", tableName);
                    if (!dbSchemaTables.ContainsKey(fullName))
                        dbSchemaTables.Add(fullName, false);
                }
                queryString =
                    "select o.view_name, o.owner from all_views o " +
                    "where o.owner <> 'SYS' and o.owner <> 'SYSTEM' and o.view_name in ({0})";
                foreach (SelectStatementResultRow row in GetDataForTables(tables, null, queryString).Rows)
                {
                    if (row.Values[0] is DBNull) continue;
                    string tableName = (string)row.Values[0];
                    string schemaName = (string)row.Values[1];
                    string fullName = string.Concat(schemaName, ".", tableName);
                    if (!dbSchemaTables.ContainsKey(fullName))
                        dbSchemaTables.Add(fullName, true);
                }
            }
            catch (SchemaCorrectionNeededException) { }
            foreach (SelectStatementResultRow row in GetDataForTables(tables, null,
                "select table_name from user_tables where table_name in ({0})").Rows)
            {
                string name = (string)row.Values[0]; 
                if (!dbTables.ContainsKey(name)) dbTables.Add(name, false);
            }
            foreach (SelectStatementResultRow row in GetDataForTables(tables, null, 
                "select view_name from user_views where view_name in ({0})").Rows)
            {
                string name = (string)row.Values[0];
                if (!dbTables.ContainsKey(name)) dbTables.Add(name, true);
            }
            ArrayList list = new ArrayList();
            foreach (DBTableEx table in tables)
            {
                string tableName = ComposeSafeTableName(table.Name);
                string schemaName = ComposeSafeSchemaName(table.Name);
                bool isView = false;
                if (!dbSchemaTables.TryGetValue(string.Concat(schemaName, ".", tableName), out isView) &&
                    // Добавляем проверку названия схемы для таблиц текущего пользователя 
                    !((string.IsNullOrEmpty(schemaName) || schemaName == CurrentUserName) && dbTables.TryGetValue(tableName, out isView)))
                    list.Add(table);
                else
                    table.IsView = isView;
            }
            return list;
        }

        #region Получение метаданных таблицы

        /// <summary>
        /// Получает метаданные указанной таблицы
        /// </summary>
        /// <param name="table">Таблица, в которой обновляются метаданные из базы данных</param>
        /// <param name="checkIndexes">Флаг сбора данных об индексах</param>
        /// <param name="checkForeignKeys">Флаг сбора данных о ссылках</param>
        /// <param name="checkConstraints">Флаг сбора данных о констрейнтах</param>
        public virtual void GetTableSchema(DBTableEx table, bool checkIndexes, bool checkForeignKeys, bool checkConstraints)
        {
            GetColumns(table);
            GetPrimaryKey(table);
            if (checkIndexes)
                GetIndexes(table);
            if (checkForeignKeys)
                GetForeignKeys(table);
            if (checkConstraints) 
                GetConstraints(table);
        }

        private DBColumnType GetTypeFromString(string typeName, int size, int precision, int? scale)
        {
            switch (typeName.ToLower())
            {
                case "int":
                    return DBColumnType.Int32;
                case "blob":
                case "raw":
                    return DBColumnType.ByteArray;
                case "number":
                    if (precision == 0) return scale == 0 ? DBColumnType.Int32 : DBColumnType.Decimal;
                    if (precision == 1) return DBColumnType.Boolean;
                    if (precision <= 3) return DBColumnType.Byte;
                    if (precision <= 5) return DBColumnType.Int16;
                    if (precision <= 10) return DBColumnType.UInt32;
                    if (precision <= 19) return DBColumnType.Decimal;
                    return scale == 0 ? DBColumnType.UInt64 : DBColumnType.Decimal;
                case "nchar":
                case "char":
                    return size > 1 ? DBColumnType.String : DBColumnType.Char;
                case "float":
                    return DBColumnType.Double;
                case "nvarchar":
                case "varchar":
                case "varchar2":
                case "nvarchar2":
                    return DBColumnType.String;
                case "date":
                    return DBColumnType.DateTime;
                case "clob":
                case "nclob":
                    return DBColumnType.String;
            }
            return DBColumnType.Unknown;
        }

        private void GetColumns(DBTable table)
        {
            string schema = ComposeSafeSchemaName(table.Name);
            string tableName = ComposeSafeTableName(table.Name);
            const string selectFields = 
@"select column_name, data_type, char_col_decl_length, data_precision, data_scale,
    case when data_type='NUMBER' and data_precision is null and data_scale is null then data_type
    when data_type='NUMBER' and data_precision is null and data_scale = 0 then 'INT'
    when data_type='NUMBER' then data_type||'('||data_precision||','||data_scale||')'
    when data_type in ('CHAR','NCHAR','VARCHAR2','NVARCHAR2') then data_type||'('||char_col_decl_length||')'
    when data_type='RAW' then data_type||'('||data_length||')'
    else data_type end full_type, nullable";
            Query query = string.IsNullOrEmpty(schema) ?
                new Query(selectFields + " from user_tab_columns where table_name = :p0", 
                    new QueryParameterCollection(new OperandValue(tableName)), new string[] { ":p0" }) :
                new Query(selectFields + " from all_tab_columns where owner = :p0 and table_name = :p1", 
                    new QueryParameterCollection(new OperandValue(schema), new OperandValue(tableName)), new string[] { ":p0", ":p1" });
            foreach (SelectStatementResultRow row in SelectData(query).Rows)
            {
                string name = (string)row.Values[0];
                int size = row.Values[2] != DBNull.Value ? ((IConvertible)row.Values[2]).ToInt32(CultureInfo.InvariantCulture) : 0;
                int precision = row.Values[3] != DBNull.Value ? ((IConvertible)row.Values[3]).ToInt32(CultureInfo.InvariantCulture) : 0;
                int? scale = row.Values[4] != DBNull.Value ? ((IConvertible)row.Values[4]).ToInt32(CultureInfo.InvariantCulture) : (int?)null;
                DBColumnType type = GetTypeFromString((string)row.Values[1], size, precision, scale);
                string dbType = (string)row.Values[5];
                string nullable = (string)row.Values[6];
                DBColumn column = new DBColumn(name, false, dbType, type == DBColumnType.String ? size : 0, type);
                table.AddColumn(column);
                if (nullable == "N" && table is DBTableEx) 
                    ((DBTableEx)table).AddConstraint(new DBNotNullConstraint(column));
            }
        }

        private void GetPrimaryKey(DBTable table)
        {
            string schema = ComposeSafeSchemaName(table.Name);
            string safeTableName = ComposeSafeTableName(table.Name);
            Query query;
            if (schema == string.Empty)
                query = new Query(
                    "select tc.column_name from user_cons_columns tc inner join user_constraints c " + 
                        "on tc.constraint_name = c.constraint_name and tc.table_name = c.table_name " +
                    "where c.constraint_type = 'P' and tc.table_name = :p0",
                new QueryParameterCollection(new OperandValue(safeTableName)), new string[] { ":p0" });
            else
                query = new Query(
                    "select tc.column_name from all_cons_columns tc " +
                        "inner join all_constraints c on tc.constraint_name = c.constraint_name " +
                        "and tc.owner = c.owner and tc.table_name = c.table_name " +
                    "where c.constraint_type = 'P' and tc.owner = :p0  and tc.table_name = :p1",
                    new QueryParameterCollection(new OperandValue(schema), new OperandValue(safeTableName)), new string[] { ":p0", ":p1" });
            SelectStatementResult data = SelectData(query);
            if (data.Rows.Length > 0)
            {
                StringCollection cols = new StringCollection();
                for (int i = 0; i < data.Rows.Length; i++)
                {
                    string columnName = (string)data.Rows[i].Values[0];
                    DBColumn column = table.GetColumn(columnName);
                    if (column != null)
                        column.IsKey = true;
                    cols.Add(columnName);
                }
                table.PrimaryKey = new DBPrimaryKey(table.Name, cols);
            }
        }

        private void GetIndexes(DBTable table)
        {
            string schema = ComposeSafeSchemaName(table.Name);
            string safeTableName = ComposeSafeTableName(table.Name);
            Query query;
            if (schema == string.Empty)
                query = new Query(
                    "select ind.index_name, cols.column_name, cols.column_position, ind.uniqueness " +
                    "from user_indexes ind inner join user_ind_columns cols " + 
                        "on ind.index_name = cols.index_name and cols.table_name = ind.table_name " +
                    "where ind.table_name = :p0 order by ind.index_name, cols.column_position",
                    new QueryParameterCollection(new OperandValue(safeTableName)), new string[] { ":p0" });
            else
                query = new Query(
                    "select ind.index_name, cols.column_name, cols.column_position, ind.uniqueness " + 
                    "from all_indexes ind inner join all_ind_columns cols on ind.index_name = cols.index_name " +
                        "and cols.index_owner = ind.table_owner and cols.table_name = ind.table_name " +
                    "where ind.table_owner = :p0 and ind.table_name = :p1 " +
                    "order by ind.index_name, cols.column_position",
                    new QueryParameterCollection(new OperandValue(schema), new OperandValue(safeTableName)), new string[] { ":p0", ":p1" });
            SelectStatementResult data = SelectData(query);
            DBIndex index = null;
            foreach (SelectStatementResultRow row in data.Rows)
            {
                if (Convert.ToDecimal(row.Values[2]) == 1m)
                {
                    StringCollection list = new StringCollection();
                    list.Add((string)row.Values[1]);
                    index = new DBIndex((string)row.Values[0], list, string.Equals(row.Values[3], "UNIQUE"));
                    table.Indexes.Add(index);
                }
                else
                    index.Columns.Add((string)row.Values[1]);
            }
        }

        private void GetForeignKeys(DBTable table)
        {
            string schema = ComposeSafeSchemaName(table.Name);
            string safeTableName = ComposeSafeTableName(table.Name);
            Query query;
            if (schema == string.Empty)
                query = new Query(
                    "select tc.position, tc.column_name, fc.column_name, fc.table_name from user_constraints c " +
                        "inner join user_cons_columns tc on tc.constraint_name = c.constraint_name and tc.table_name = c.table_name " +
                        "inner join user_cons_columns fc on c.r_constraint_name = fc.constraint_name and tc.position = fc.position " +
                    "where c.table_name = :p0 order by c.constraint_name, tc.position",
                new QueryParameterCollection(new OperandValue(safeTableName)), new string[] { ":p0" });
            else
                query = new Query(
                    "select tc.position, tc.column_name, fc.column_name, fc.owner||'.'||fc.table_name from all_constraints c " +
                        "inner join all_cons_columns tc on tc.constraint_name = c.constraint_name and tc.owner = c.owner and tc.table_name = c.table_name " +
                        "inner join all_cons_columns fc on c.r_constraint_name = fc.constraint_name and tc.position = fc.position and fc.owner = c.r_owner " +
                    "where c.owner = :p0 and c.table_name = :p1 order by c.constraint_name, tc.position",
                new QueryParameterCollection(new OperandValue(schema), new OperandValue(safeTableName)), new string[] { ":p0", ":p1" });
            SelectStatementResult data = SelectData(query);
            DBForeignKey fk = null;
            foreach (SelectStatementResultRow row in data.Rows)
            {
                if (Convert.ToDecimal(row.Values[0]) == decimal.One)
                {
                    StringCollection pkc = new StringCollection();
                    StringCollection fkc = new StringCollection();
                    pkc.Add((string)row.Values[2]);
                    fkc.Add((string)row.Values[1]);
                    fk = new DBForeignKey(fkc, (string)row.Values[3], pkc);
                    table.ForeignKeys.Add(fk);
                }
                else
                {
                    fk.Columns.Add((string)row.Values[1]);
                    fk.PrimaryKeyTableKeyColumns.Add((string)row.Values[2]);
                }
            }
        }

        private void GetConstraints(DBTableEx table)
        {
			string schemaName = ComposeSafeSchemaName(table.Name);
			string tableName = ComposeSafeTableName(table.Name);
            bool allOrUser = !string.IsNullOrEmpty(schemaName);
            string constraintsTable = allOrUser ? "all_constraints" : "user_constraints";
            string realSchemaName = allOrUser ? schemaName : CurrentUserName;
            Query query = new Query(
                // (Оба представления содержат поле owner)
                "select constraint_name from " + constraintsTable + " " +
                "where owner = :p0 and table_name = :p1 and constraint_type = 'C' and generated = 'USER NAME'",
                new QueryParameterCollection(new OperandValue(realSchemaName), new OperandValue(tableName)), new string[] { ":p0", ":p1" });
            SelectStatementResult data = SelectData(query);
            foreach (SelectStatementResultRow row in data.Rows)
            {
                string name = (string)row.Values[0];
                string condition = GetLong(new Query(
                    "select search_condition from " + constraintsTable + " " +
                    "where owner = :p0 and table_name = :p1 and constraint_name = :p2",
                    new QueryParameterCollection(new OperandValue(realSchemaName), 
                        new OperandValue(tableName), new OperandValue(name)), new string[] { ":p0", ":p1", ":p2" }));
                table.AddConstraint(new DBCheckConstraint(name, condition));
            }
        }

        private string GetLong(Query query, bool nullOnError = true)
        {
            // Выборка данных независимо от режима scriptMode
            IDbCommand command = base.CreateCommand(); 
            PrepareParameters(command, query);
            command.CommandText = 
                "declare l long; begin " + query.Sql.Replace("from", "into l from") + "; :pLong := l; end;";
            IDbDataParameter clob = command.CreateParameter();
            clob.DbType = DbType.String;
            clob.Direction = ParameterDirection.Output;
            clob.ParameterName = ":pLong";
            clob.Size = 32000;
            command.Parameters.Add(clob);
            try
            {
                command.ExecuteNonQuery();
                return (string)clob.Value;
            }
            catch (Exception e)
            {
                if (nullOnError) return null;
                throw WrapException(e, command);
            }
        }

        private bool GetSequence(DBTable table)
        {
            string realSchemaName = FormatRealSchemaName(table.Name).ToUpper();
            string sequenceName = FormatSequence(null, ComposeSafeTableName(table.Name)).ToUpper();
            SelectStatementResult data = SelectData(new Query(
                "select count(*) from dba_sequences where sequence_owner = :p0 and sequence_name = :p1",
                new QueryParameterCollection(new OperandValue(realSchemaName), new OperandValue(sequenceName)), new string[] { ":p0", ":p1" }));
            return Convert.ToInt32(data.Rows[0].Values[0]) == 1;
        }

        private bool GetValidSafeObjects(DBTable table)
        {
            string realSchemaName = FormatRealSchemaName(table.Name).ToUpper();
            string tableName = ComposeSafeTableName(table.Name);
            SelectStatementResult data = SelectData(new Query(
                "select count(*) from dba_objects where owner = :p0 and status = 'VALID' " +
                    "and object_name in (:p1, :p2, :p3, :p4, :p5) and ((object_name = :p1 and object_type = 'VIEW') " + 
                    "or (object_name = :p2 and object_type in ('PACKAGE', 'PACKAGE BODY')) or (object_name in (:p3, :p4, :p5) and object_type = 'PROCEDURE'))",
                new QueryParameterCollection(new OperandValue(realSchemaName),
                    new OperandValue(FormatView(null, tableName).ToUpper()),
                    new OperandValue(FormatPackage(null, tableName).ToUpper()),
                    new OperandValue(FormatProcedureAdd(null, tableName).ToUpper()),
                    new OperandValue(FormatProcedureEdit(null, tableName).ToUpper()),
                    new OperandValue(FormatProcedureDelete(null, tableName).ToUpper())),
                new string[] { ":p0", ":p1", ":p2", ":p3", ":p4", ":p5" }));
            return Convert.ToInt32(data.Rows[0].Values[0]) == 6;
        }

        private string GetViewText(DBTable table)
        {
            string realSchemaName = FormatRealSchemaName(table.Name).ToUpper();
            string tableName = ComposeSafeTableName(table.Name);
            Query query = new Query(
                "select text from dba_views where owner=:p0 and view_name=:p1",
                new QueryParameterCollection(new OperandValue(realSchemaName), new OperandValue(FormatView(null, tableName).ToUpper())),
                new string[] { ":p0", ":p1" });
            return GetLong(query, nullOnError: true);
        }

        private string GetPackageBody(DBTable table)
        {
            string realSchemaName = FormatRealSchemaName(table.Name).ToUpper();
            string tableName = ComposeSafeTableName(table.Name);
            Query query = new Query(
                "select text from dba_source where owner=:p0 and name =:p1 and type='PACKAGE BODY' order by line",
                new QueryParameterCollection(new OperandValue(realSchemaName), new OperandValue(FormatPackage(null, tableName).ToUpper())), 
                new string[] { ":p0", ":p1" });
            StringBuilder sb = new StringBuilder();
            foreach (SelectStatementResultRow row in SelectData(query).Rows) 
                sb.Append((string)row.Values[0]);
            return sb.ToString();
        }

        #endregion

        /// <inheritdoc/>
        public override string GetSqlCreateColumnFullAttributes(DBTable table, DBColumn column)
        {
            string result = GetSqlCreateColumnType(table, column);
            bool notNull = table is DBTableEx ? ((DBTableEx)table).ColumnIsNotNull(column) : column.IsKey;
            return notNull ? string.Concat(result, OracleTemplater.Space, OracleTemplater.NotNull) : result;
        }

        /// <summary>Возвращает тип данных колонки в формате, хранимом в структуре таблицы БД</summary>
        /// <param name="table">Таблица, которой принадлежит колонка</param>
        /// <param name="column">Колонка, для которой нужно определить тип данных</param>
        /// <returns>Тип данных колонки в формате, хранимом в БД</returns>
        protected string GetDBTableColumnType(DBTable table, DBColumn column) 
        {
            string result = GetSqlCreateColumnType(table, column).ToUpper()
                .Replace(" ", string.Empty)
                .Replace("INTEGER", "INT")
                .Replace("NUMERIC", "NUMBER")
                .Replace("DOUBLEPRECISION", "FLOAT");
            if (result == "CHAR" || result == "NCHAR") result = result + "(1)";
            if (result.StartsWith("NUMBER(") && result.IndexOf(',') < 0)
                result = result.Substring(0, result.Length - 1) + ",0)";
            return result;
		}

        /// <summary>
        /// Получить для указанной таблицы колонку первичного ключа с автоинкрементом
        /// </summary>
        /// <param name="table">Таблица</param>
        /// <returns>Колонка первичного ключа таблицы table с автоинкрементом или null, если нет такой колонки</returns>
        protected DBColumn GetTableSequenceColumn(DBTable table)
        {
            return OracleTemplater.GetTableSequenceColumn(table);
        }

        /// <summary>
        /// Получить для указанной колонки таблицы тип параметра процедуры в формате sql-команды
        /// </summary>
        /// <param name="table">Таблица</param>
        /// <param name="column">Колонка</param>
        /// <returns>Тип параметра процедуры в формате sql-команды</returns>
        /// <remarks>По умолчанию удаляет из типа колонки (см. <see cref="ConnectionProviderSql.GetSqlCreateColumnType"/>) значения, указанные в скобках</remarks>
        protected virtual string GetSqlProcedureParameterType(DBTable table, DBColumn column)
        {
            if (column == OracleTemplater.ColumnRowid) return OracleTemplater.Rowid;
            string result = GetSqlCreateColumnType(table, column);
            int par = result.IndexOf('(');
            return par > 0 ? result.Remove(par) : result;
        }

        /// <summary>
        /// Создает указанную таблицу
        /// </summary>
        /// <param name="table">Таблица</param>
        public override void CreateTable(DBTable table)
        {
            ExecuteSqlSchemaUpdate("Table", table.Name, string.Empty,
                new OracleTemplater(this, table).CreateTableCommand(col => GetSqlCreateColumnFullAttributes(table, col)));
        }

        /// <summary>
        /// Создание последовательности для первичного ключа указанной таблицы
        /// </summary>
        /// <param name="table">Таблица, для которой создается последовательность</param>
        public virtual void CreateSequence(DBTable table)
        {
            DBColumn key = GetTableSequenceColumn(table);
            if (key != null && !GetSequence(table))
            {
                string sequence = GetSeqName(table.Name);
                ExecuteSqlSchemaUpdate("Sequence", sequence, table.Name, 
                    new OracleTemplater(this, table).CreateSequenceCommand(sequence));
            }
        }

        /// <summary>
        /// Создание первичного ключа для указанной таблицы
        /// </summary>
        /// <param name="table">Таблица, для которой создается первичный ключ</param>
        public override void CreatePrimaryKey(DBTable table)
        {
            if (table.PrimaryKey == null) return;
            base.CreatePrimaryKey(table);
        }

        /// <summary>Шаблон создания индекса с указанием табличного пространства</summary>
        protected override string CreateIndexTemplate { get { return "create {0} index {1} on {2}({3}) {4}"; } }

        /// <summary>
        /// Создание индекса для таблицы
        /// </summary>
        /// <param name="table">Таблица, для которой создается индекс</param>
        /// <param name="index">Создаваемый индекс</param>
        public override void CreateIndex(DBTable table, DBIndex index)
        {
            StringBuilder indexColumns = new StringBuilder();
            foreach (string indexColumn in index.Columns)
            {
                if (indexColumns.Length > 0) indexColumns.Append(", ");
                indexColumns.Append(FormatColumnSafe(indexColumn));
            }
            string schemaName = ComposeSafeSchemaName(table.Name);
            string tableFullName = FormatTableSafe(table);
            string indexName = GetIndexName(index, table);
            string indexFullName = ComposeSafeConstraintName(indexName);
            indexFullName = (string.IsNullOrEmpty(schemaName) ? indexFullName : string.Concat(schemaName, ".", indexFullName)).ToLowerInvariant();
            ExecuteSqlSchemaUpdate("Index", indexName, table.Name, 
                String.Format(CultureInfo.InvariantCulture, CreateIndexTemplate,
                index.IsUnique ? "unique" : string.Empty, indexFullName, tableFullName, indexColumns,
                string.IsNullOrEmpty(indexTablespace) ? string.Empty : "tablespace " + indexTablespace));
        }

        /// <summary>
        /// Создание ссылки на другую таблицу
        /// </summary>
        /// <param name="table">Таблица со ссылкой</param>
        /// <param name="fk">Ссылка на другую таблицу</param>
        public override void CreateForeignKey(DBTable table, DBForeignKey fk)
        {
            // Если ссылка на таблицу с настраиваемой модификацией, то пропускаем
            DBTableEx foreignTableEx = XPDictionaryInformer.Schema.GetTable(fk.PrimaryKeyTable);
            if (foreignTableEx.IsCustom) return;

            // Если ссылка на таблицу из другой схемы, то необходимо дать разрешение
            string tableSchema = FormatRealSchemaName(table.Name);
            string foreignSchema = FormatRealSchemaName(fk.PrimaryKeyTable);
            if (tableSchema != foreignSchema)
            {
                string foreignTable = FormatTable(foreignSchema, ComposeSafeTableName(fk.PrimaryKeyTable));
                ExecuteSqlSchemaUpdate("ForeignKey", GetForeignKeyName(fk, table), table.Name,
                    String.Format(CultureInfo.InvariantCulture, "grant references on {0} to {1}", foreignTable, tableSchema));
            }

            base.CreateForeignKey(table, fk);
        }

        /// <summary>
        /// Создание констрейнта для таблицы
        /// </summary>
        /// <param name="table">Таблица</param>
        /// <param name="expression">Выражение констрейнта</param>
        public virtual void CreateConstraint(DBTable table, string expression)
        {
            string constraintName = GetConstraintName(expression, table);
            ExecuteSqlSchemaUpdate("Constraint", constraintName, table.Name,
                new OracleTemplater(this, table).CreateConstraintCommand(FormatConstraintSafe(constraintName), expression));
        }

        /// <summary>
        /// Изменение типа колонки
        /// </summary>
        /// <param name="table">Таблица</param>
        /// <param name="column">Колонка таблицы, которая подлежит изменению</param>
        /// <param name="newColumn">Колонка, которая является шаблоном для изменений</param>
        public virtual void ChangeColumn(DBTable table, DBColumn column, DBColumn newColumn)
        {
            string command = String.Format(CultureInfo.InvariantCulture, "alter table {0} modify ({1} {2})",
                FormatTableSafe(table), FormatColumnSafe(column.Name), GetSqlCreateColumnType(table, newColumn));
            if (IsColumnChangeSafe(column, newColumn))
                ExecuteSqlSchemaUpdate("Column", column.Name, table.Name, command);
            else
                UnsafeSchemaUpdate("Column type", column.Name, table.Name, command + ";");
            column.ColumnType = newColumn.ColumnType;
            column.Size = newColumn.Size;
        }

        /// <summary>
        /// Изменение констрейнта обязательности колонки (not null)
        /// </summary>
        /// <param name="table">Таблица</param>
        /// <param name="column">Колонка таблицы, которая подлежит изменению</param>
        /// <param name="notNull">Значение констрейнта обязательности: true - обязательные значения, false - необязательные значения</param>
        public virtual void ChangeColumnNullable(DBTable table, DBColumn column, bool notNull)
        {
            string command = String.Format(CultureInfo.InvariantCulture, "alter table {0} modify ({1} {2})",
                FormatTableSafe(table), FormatColumnSafe(column.Name), notNull ? "not null" : "null");
            if (!notNull)
                ExecuteSqlSchemaUpdate("Column", column.Name, table.Name, command);
            else
                UnsafeSchemaUpdate("Column mandatory", column.Name, table.Name, command + ";");
        }

        /// <summary>
        /// Удаление колонки
        /// </summary>
        /// <param name="table">Таблица</param>
        /// <param name="column">Колонка таблицы, которая подлежит удалению</param>
        public virtual void DeleteColumn(DBTable table, DBColumn column)
        {
            string command = String.Format(CultureInfo.InvariantCulture, "alter table {0} drop column {1};",
                FormatTableSafe(table), FormatColumnSafe(column.Name));
            UnsafeSchemaUpdate("Old column", column.Name, table.Name, command);
        }

        // Сравнение содержания объекта безопасности из скрипта и из базы данных
        private bool CompareSafeContent(string script, string content, bool firstLine)
        {
            if (string.IsNullOrEmpty(script) || string.IsNullOrEmpty(content)) return false;
            var newLineIndex = script.IndexOf(Environment.NewLine);
            if (newLineIndex >= 0)
            {
                script = script.Substring(newLineIndex);
            }
            if (firstLine)
            {
                newLineIndex = content.IndexOf('\n');
                if (newLineIndex >= 0)
                {
                    content = content.Substring(newLineIndex);
                }
            }
            string s1 = new string(script.Where(c => !char.IsWhiteSpace(c)).ToArray());
            string s2 = new string(content.Where(c => !char.IsWhiteSpace(c)).ToArray());
            return s1 == s2;
        }

        /// <summary>
        /// Создание или пересоздание представления на основе указанной таблицы
        /// </summary>
        /// <param name="table">Таблица, на основе которой создается представление</param>
        public virtual void CreateOrReplaceView(DBTable table)
        {
            string view = FormatView(table);
            string script = new OracleTemplater(this, table).CreateViewCommand();
            // Проверка изменений
            bool skip = false;
            if ((updateOptions & UpdateSchemaOptions.CheckSafeContent) == UpdateSchemaOptions.CheckSafeContent)
                skip = CompareSafeContent(script, GetViewText(table), false);
            // Выполнение скрипта
            if (!skip) 
                ExecuteSqlSchemaUpdate("View", view, table.Name, script);
        }

        /// <summary>
        /// Создание или пересоздание пакета процедур на основе указанной таблицы
        /// </summary>
        /// <param name="table">Таблица, на основе которой создается пакет процедур</param>
        public virtual void CreateOrReplacePackage(DBTable table)
        {
            string package = FormatPackage(table);
            string script = new OracleTemplater(this, table).CreateStandartPackageCommand(package, 
                col => GetSqlProcedureParameterType(table, col));
            // Проверка изменений
            bool skip = false;
            if ((updateOptions & UpdateSchemaOptions.CheckSafeContent) == UpdateSchemaOptions.CheckSafeContent)
            {
                string body = null;
                foreach (string command in ParseScript(script))
                    if (command != null && command.ToUpper().Contains("PACKAGE BODY")) { body = command; break; }
                skip = CompareSafeContent(body, GetPackageBody(table), true);
            }
            // Выполнение скрипта
            if (!skip)
            {
                if (scriptMode)
                    ExecuteSqlSchemaUpdate("Package", package, table.Name, script);
                else
                    foreach (string command in ParseScript(script))
                        ExecuteSqlSchemaUpdate("Package and procedures", package, table.Name, command);
            }
        }

        /// <summary>
        /// Разбирает скрипт на отдельные команды
        /// </summary>
        /// <param name="script">Скрипт</param>
        /// <returns>Коллекция команд скрипта</returns>
        protected StringCollection ParseScript(string script)
        {
            StringCollection result = new StringCollection();
            foreach (string block in script.Split('/'))
            {
                List<string> commands = new List<string>();
                string lower = block.ToLower();
                int posPackage = lower.IndexOf("package");
                int posProcedure = lower.IndexOf("procedure");
                int posDeclare = lower.IndexOf("declare");
                int posBegin = lower.IndexOf("begin");
                int posStart = posDeclare >= 0 ? posDeclare : posBegin;
                if (posPackage >= 0 || posProcedure >= 0)
                    posStart = lower.LastIndexOf("create", posPackage >= 0 ? posPackage : posProcedure);
                if (posStart >= 0)
                {
                    if (posStart > 0)
                        commands.AddRange(block.Substring(0, posStart - 1).Split(';'));
                    commands.Add(block.Substring(posStart));
                }
                else
                    commands.AddRange(block.Split(';'));
                foreach (string command in commands)
                {
                    string commandText = command.Trim();
                    if (!string.IsNullOrEmpty(commandText)) 
                        result.Add(commandText);
                }
            }
            return result;
        }

        /// <summary>
        /// Определяет, является ли изменение типа колонки безопасным
        /// </summary>
        /// <param name="column">Колонка таблицы, которая подлежит изменению</param>
        /// <param name="newColumn">Колонка, которая является шаблоном для изменений</param>
        /// <returns>True, если изменение типа колонки безопасное, иначе false</returns>
        public virtual bool IsColumnChangeSafe(DBColumn column, DBColumn newColumn)
        {
            return newColumn.ColumnType == column.ColumnType && newColumn.Size > column.Size && column.Size > 0;
        }

        /// <summary>
        /// Запись небезопасного изменения структуры БД в скрипт в виде комментариев
        /// </summary>
        /// <param name="objectTypeName">Название типа изменяемого объекта</param>
        /// <param name="objectName">Название объекта</param>
        /// <param name="parentObjectName">Название родительского объекта</param>
        /// <param name="command">Команда по небезопасному изменению структуры БД</param>
        protected void UnsafeSchemaUpdate(string objectTypeName, string objectName, string parentObjectName, string command)
        {
            if (scriptMode && scriptUnsafe != null)
            {
                scriptUnsafe.AppendFormat("-- {0} {1} of {2}", objectTypeName, objectName, parentObjectName);
                scriptUnsafe.AppendLine();
                scriptUnsafe.AppendLine(command);
                scriptUnsafe.AppendLine();
            }
        }

        #endregion

        #region Администрирование безопасности

        private Dictionary<string, SecurityObject> tablesPrivileges;
        private HashSet<string> currentUserPrivileges;
        private string currentUser;

        /// <summary>Возвращает привилегию таблицы</summary>
        private bool GetTablePrivilege(string privilegeName, out SecurityObject tablePrivilege)
        {
            if (tablesPrivileges == null)
            {
                tablesPrivileges = new Dictionary<string, SecurityObject>();
                foreach (string tableName in XPDictionaryInformer.Schema.TableNames)
                {
                    string schema = ComposeSafeSchemaName(tableName);
                    string table = ComposeSafeTableName(tableName);
                    tablesPrivileges[FormatView(schema, table).ToUpper()] = new SecurityObject(tableName, SecurityTableRights.Select);
                    tablesPrivileges[FormatProcedureAdd(schema, table).ToUpper()] = new SecurityObject(tableName, SecurityTableRights.Insert);
                    tablesPrivileges[FormatProcedureEdit(schema, table).ToUpper()] = new SecurityObject(tableName, SecurityTableRights.Update);
                    tablesPrivileges[FormatProcedureDelete(schema, table).ToUpper()] = new SecurityObject(tableName, SecurityTableRights.Delete);
                }
            }
            return tablesPrivileges.TryGetValue(privilegeName, out tablePrivilege);
        }

        /// <summary>Возвращает флаг привилегии текущего пользователя</summary>
        private bool GetUserPrivilege(string privilegeName)
        {
            if (currentUserPrivileges == null)
            {
                currentUserPrivileges = new HashSet<string>();
                foreach (string tableName in new string[] { 
                    "dba_sys_privs", "dba_role_privs", "dba_tab_privs",
                    "role_role_privs", "role_tab_privs", 
                    "user_sys_privs", "user_role_privs", 
                    "dba_users", "all_users", "user_users", "dba_roles",
                    "all_views", "all_procedures"
                })
                {
                    if (TestSelect(tableName)) currentUserPrivileges.Add(tableName);
                }
                if (currentUserPrivileges.Contains("user_role_privs"))
                    if (SelectData(new Query("select 1 from user_role_privs where granted_role = 'DBA'")).Rows.Count() > 0)
                        currentUserPrivileges.Add("dba");
                if (currentUserPrivileges.Contains("user_sys_privs"))
                    foreach (SelectStatementResultRow row in SelectData(new Query("select privilege from user_sys_privs")).Rows)
                        currentUserPrivileges.Add(row.Values[0].ToString().ToLower());
            }
            return currentUserPrivileges.Contains(privilegeName);
        }

        /// <summary>Текущий пользователь</summary>
        public string CurrentUserName 
        { 
            get { if (currentUser == null) currentUser = GetCurrentUser(); return currentUser; } 
        }

        /// <contentfrom cref="ISqlDataStoreSecurity.AdminSecurity" />
        public SecurityResult AdminSecurity(params SecurityStatement[] statements)
        {
            List<SecurityObject> result = new List<SecurityObject>();
            if (statements != null)
                foreach (SecurityStatement statement in statements)
                {
                    statement.CheckValidity();
                    IEnumerable<SecurityObject> statementResult = AdminSecurity(statement);
                    if (statementResult != null) result.AddRange(statementResult);
                }
            return new SecurityResult(result);
        }

        /// <contentfrom cref="ISqlDataStoreSecurity.CheckAccess" />
        public bool[] CheckAccess(params SecurityStatement[] statements)
        {
            List<bool> result = new List<bool>();
            if (statements != null)
                foreach (SecurityStatement statement in statements)
                {
                    statement.CheckValidity();
                    bool access = false, limits = false;
                    SecurityObjectTypes leftType = statement.LeftOperand != null ? statement.LeftOperand.ObjectType : SecurityObjectTypes.Table;
                    string leftObject = statement.LeftOperand != null ? statement.LeftOperand.ObjectName : null;
                    bool isCurrentUser = leftType == SecurityObjectTypes.User && string.Compare(leftObject, CurrentUserName, true) == 0;
                    if (GetUserPrivilege("dba"))
                        access = true;
                    else
                    {
                        switch (statement.Operation)
                        {
                            case AdminSecurityOperations.Create:
                                access = leftType == SecurityObjectTypes.User ?
                                    GetUserPrivilege("create user") && GetUserPrivilege("grant any privilege") : GetUserPrivilege("create role");
                                break;
                            case AdminSecurityOperations.Drop:
                                access = leftType == SecurityObjectTypes.User ? GetUserPrivilege("drop user") : GetUserPrivilege("drop any role");
                                break;
                            case AdminSecurityOperations.GrantTo:
                            case AdminSecurityOperations.RevokeFrom:
                                access = leftType == SecurityObjectTypes.Table ? GetUserPrivilege("grant any object privilege") : GetUserPrivilege("grant any role");
                                break;
                            case AdminSecurityOperations.SetUserInfo:
                                access = GetUserPrivilege("alter user"); 
                                break;
                            case AdminSecurityOperations.GetRolePrivileges:
                                access = GetUserPrivilege("dba_role_privs") || GetUserPrivilege("role_tab_privs");
                                break;
                            case AdminSecurityOperations.GetCurrentPrivileges:
                                access = GetUserPrivilege("all_views") && GetUserPrivilege("all_procedures");
                                break;
                            case AdminSecurityOperations.GetTable:
                                if (statement.RightOperand != null)
                                {
                                    SecurityObjectTypes rightType = statement.RightOperand.ObjectType;
                                    if (leftType == SecurityObjectTypes.Role && rightType == SecurityObjectTypes.Role)
                                        access = (GetUserPrivilege("dba_role_privs") && GetUserPrivilege("dba_roles")) || GetUserPrivilege("role_role_privs");
                                    if ((leftType == SecurityObjectTypes.Role && rightType == SecurityObjectTypes.User) || 
                                        (leftType == SecurityObjectTypes.User && rightType == SecurityObjectTypes.Role))
                                        access = (GetUserPrivilege("dba_role_privs") && GetUserPrivilege("dba_roles")) || GetUserPrivilege("user_role_privs");
                                }
                                access = leftType == SecurityObjectTypes.User ?
                                    GetUserPrivilege("dba_users") || GetUserPrivilege("all_users") || GetUserPrivilege("user_users") :
                                    leftType == SecurityObjectTypes.Role ?
                                    GetUserPrivilege("dba_roles") || GetUserPrivilege("user_role_privs") :
                                    leftType == SecurityObjectTypes.Table ?
                                    GetUserPrivilege("dba_tab_privs") || GetUserPrivilege("role_tab_privs") : false;
                                break;
                        }
                    }
                    switch (statement.Operation)
                    {
                        case AdminSecurityOperations.Drop:
                            limits = isCurrentUser; 
                            break;
                        case AdminSecurityOperations.GrantTo:
                        case AdminSecurityOperations.RevokeFrom:
                            if (statement.RightOperand != null && statement.RightOperand.ObjectType == SecurityObjectTypes.User)
                                limits = string.Compare(statement.RightOperand.ObjectName, CurrentUserName, true) == 0;
                            break;
                    }
                    result.Add(access && !limits);
                }
            return result.ToArray();
        }

        /// <summary>
        /// Выполняет указанную команду администрирования безопасности
        /// </summary>
        /// <param name="statement">Команда администрирования безопасности</param>
        /// <returns>Объекты безопасности в результате выполнения указанной команды в зависимости от типа команды <see cref="AdminSecurityOperations"/></returns>
        protected virtual IEnumerable<SecurityObject> AdminSecurity(SecurityStatement statement)
        {
            // Команда запроса объектов администрирования
            if (statement.Operation == AdminSecurityOperations.GetRolePrivileges ||
                statement.Operation == AdminSecurityOperations.GetCurrentPrivileges ||
                statement.Operation == AdminSecurityOperations.GetTable)
            {
                List<SecurityObject> result = new List<SecurityObject>();
                SecurityObjectTypes objectType = statement.LeftOperand != null ? statement.LeftOperand.ObjectType : SecurityObjectTypes.UserInfo;
                string objectName = statement.LeftOperand != null ? statement.LeftOperand.ObjectName : null;
                SecurityObject tableSecurity = null;
                switch (statement.Operation)
                {
                    // Запрос привилегий роли
                    case AdminSecurityOperations.GetRolePrivileges:
                        foreach (SelectStatementResultRow row in SelectSimple(
                            GetUserPrivilege("dba_role_privs") ?
                            "select owner||'.'||table_name table_name from dba_tab_privs where grantee = :pRole" :
                            "select owner||'.'||table_name table_name from role_tab_privs where role = :pRole",
                            ":pRole", objectName, "table_name").Rows)
                            if (GetTablePrivilege((string)row.Values[0], out tableSecurity)) result.Add(tableSecurity);
                        break;
                    // Запрос текущих привилегий пользователя
                    case AdminSecurityOperations.GetCurrentPrivileges:
                        foreach (SelectStatementResultRow row in SelectData(new Query(
                            "select owner||'.'||view_name from all_views where owner <> 'SYS' and owner <> 'SYSTEM' and view_name like 'VW:_%' escape ':' union all " +
                            "select owner||'.'||object_name from all_procedures where owner <> 'SYS' and owner <> 'SYSTEM' and object_name like 'P:_%' escape ':'")).Rows)
                            if (GetTablePrivilege((string)row.Values[0], out tableSecurity)) result.Add(tableSecurity);
                        break;
                    // Запрос таблицы для объектов
                    case AdminSecurityOperations.GetTable:
                        // Таблица пользователей (Id, IsActive, IsExpired, Locked, Created)
                        if (objectType == SecurityObjectTypes.User && statement.RightOperand == null)
                        {
                            DBColumn username = new DBColumn("username", true, null, 30, DBColumnType.String);
                            DBColumn isActive = new DBColumn("is_active", false, null, 0, DBColumnType.Boolean);
                            DBColumn isExpired = new DBColumn("is_expired", false, null, 0, DBColumnType.Boolean);
                            DBColumn locked = new DBColumn("lock_date", false, null, 0, DBColumnType.DateTime);
                            DBColumn created = new DBColumn("created", false, null, 0, DBColumnType.DateTime);
                            DBColumn nullValue = new DBCriteriaColumn("null", false, null, 0, DBColumnType.String, new OperandValue(null));
                            tableSecurity =
                                GetSecurityTable("(select username, " +
                                    "cast(decode(instr(account_status,'LOCKED'),0,1,0) as number(1)) is_active, " +
                                    "cast(decode(instr(account_status,'EXPIRED'),0,0,1) as number(1)) is_expired, " +
                                    "lock_date, created from dba_users)",
                                    new string[] { "dba_users" }, username, isActive, isExpired, locked, created) ??
                                // isActive, isExpired не имеет смысла брать из user_users
                                GetSecurityTable("all_users", null, username, nullValue, nullValue, nullValue, created) ??
                                GetSecurityTable("user_users", null, username, nullValue, nullValue, locked, created);
                        }
                        // Таблица ролей (Id)
                        else if (objectType == SecurityObjectTypes.Role && statement.RightOperand == null)
                        {
                            tableSecurity =
                                GetSecurityTable("dba_roles", "role") ??
                                // Выделена в запрос, чтобы не путать с использованием в таблице ролей пользователей с тремя полями
                                GetSecurityTable("(select granted_role from user_role_privs)", "granted_role", new string[] { "user_role_privs" });
                        }
                        // Таблица иерархии ролей (Parent, Child)
                        else if (objectType == SecurityObjectTypes.Role && statement.RightOperand != null &&
                            statement.RightOperand.ObjectType == SecurityObjectTypes.Role)
                        {
                            tableSecurity =
                                GetSecurityTable("(select grantee, granted_role from dba_role_privs, dba_roles where grantee = role)",
                                    "grantee", "granted_role", new string[] { "dba_role_privs", "dba_roles" }) ??
                                GetSecurityTable("role_role_privs", "role", "granted_role");
                        }
                        // Таблица ролей пользователей (Parent, Child)
                        else if (statement.Operation == AdminSecurityOperations.GetTable && statement.RightOperand != null && (
                            (statement.LeftOperand.ObjectType == SecurityObjectTypes.Role && statement.RightOperand.ObjectType == SecurityObjectTypes.User) ||
                            (statement.LeftOperand.ObjectType == SecurityObjectTypes.User && statement.RightOperand.ObjectType == SecurityObjectTypes.Role)))
                        {
                            tableSecurity =
                                GetSecurityTable("(select grantee, granted_role from dba_role_privs, dba_users where grantee = username)",
                                    "grantee", "granted_role", new string[] { "dba_role_privs", "dba_users" }) ??
                                GetSecurityTable("user_role_privs", "username", "granted_role");
                        }
                        if (tableSecurity != null) result.Add(tableSecurity);
                        break;
                }
                return result;
            }

            // Команда модификации объектов администрирования
            foreach (string sql in new AdminSecurityGenerator(statement, this).GenerateSqlCommands())
            {
                IDbCommand command = CreateCommand();
                command.CommandText = sql;
                try
                {
                    command.ExecuteNonQuery();
                    Trace.WriteLineIf(xpoSwitch.TraceInfo, sql, "AdminSecurity");
                }
                catch (Exception e)
                {
                    throw WrapException(e, command);
                }
            }
            return null;
        }

        // Простая выборка полей по указанному запросу
        private SelectStatementResult SelectSimple(string sql, string parameterName, object parameterValue, params string[] columnNames)
        {
            StringCollection parametersNames = new StringCollection();
            parametersNames.Add(parameterName);
            CriteriaOperatorCollection selectFields = new CriteriaOperatorCollection(columnNames.Length);
            foreach (string columnName in columnNames)
                selectFields.Add(new QueryOperand(columnName, null));
            Query query = new Query(sql, new QueryParameterCollection(new OperandValue(parameterValue)), parametersNames);
            return SelectData(query, selectFields);
        }

        // Проверка доступа к таблице через попытку запроса
        private bool TestSelect(string tableName)
        {
            try
            {
                SelectData(new Query("select 1 from " + tableName + " where 1=0"));
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Получение таблицы для объекта безопасности
        private SecurityObject GetSecurityTable(string tableName, string[] privs, params DBColumn[] columns)
        {
            DBTableEx table = XPDictionaryInformer.Schema.FindTable(tableName);
            if (table == null && (privs == null ? GetUserPrivilege(tableName) : privs.All(priv => GetUserPrivilege(priv))))
            {
                table = new DBTableEx(tableName, columns);
                XPDictionaryInformer.RegisterTable(tableName, table);
            }
            if (table != null)
                return new SecurityObject(SecurityObjectTypes.Table, tableName);
            return null;
        }

        // Получение таблицы для объекта безопасности с одним ключевым полем
        private SecurityObject GetSecurityTable(string tableName, string keyColumn, string[] privs = null)
        {
            return GetSecurityTable(tableName, privs,
                new DBColumn(keyColumn, true, null, 30, DBColumnType.String));
        }

        // Получение таблицы для ассоциации объектов безопасности
        private SecurityObject GetSecurityTable(string tableName, string column1, string column2, string[] privs = null)
        {
            return GetSecurityTable(tableName, privs,
                new DBCriteriaColumn("custom_key", true, null, 30, DBColumnType.String,
                    new FunctionOperator(FunctionOperatorType.Concat, new QueryOperand(column1, null), 
                        new OperandValue("."), new QueryOperand(column2, null))),
                new DBColumn(column1, false, null, 30, DBColumnType.String),
                new DBColumn(column2, false, null, 30, DBColumnType.String));
        }

        #endregion

        #region Обработка исключений

        /// <inheritdoc/>
        protected override Exception WrapException(Exception e, IDbCommand query)
        {
            Trace.WriteLineIf(xpoSwitch.TraceError, e, "XPO");
            object codeObject;
            if (ConnectionHelper.TryGetExceptionProperty(e, "Number", out codeObject))
            {
                int code = (int)codeObject;
                switch (code)
                {
                    case 1013: throw new UserCancelException();
                    case 1918: throw new UserNotExistsException();
                    case 1919: throw new RoleNotExistsException();
                    case 1920: throw new UserConflictsException();
                    case 1921: throw new RoleConflictsException();
                    case 1927: throw new ObjectNotGrantedException();
                    case 1934: throw new RoleCircularGrantException();
                }
            };
            return base.WrapException(e, query);
        }

        /// <summary>
        /// Возвращает код ошибки Oracle, если это возможно
        /// </summary>
        /// <param name="e">Исключение</param>
        /// <param name="code">Возвращаемый код ошибки</param>
        /// <returns>True, если это ошибка Oracle и код получен успешно</returns>
        public bool TryGetExceptionNumber(Exception e, out int code)
        {
            object codeObject;
            if (ConnectionHelper.TryGetExceptionProperty(e, "Code", out codeObject))
            {
                code = (int)codeObject;
                return true;
            }
            code = 0;
            return false;
        }

        #endregion

        #region Утилиты частного кода ConnectionProviderSql

        // ReleaseFromPool
        private void DisposeCommand(IDbCommand command)
        {
            command.Dispose();
        }

        private void DoReconnect()
        {
            OpenConnectionInternal();
            OnReconnected();
        }

        private SelectStatementResult SelectData(Query query, CriteriaOperatorCollection targets)
        {
            if (query.ConstantValues != null && query.OperandIndexes != null && query.ConstantValues.Count > 0)
            {
                CriteriaOperatorCollection customTargets = new CriteriaOperatorCollection();
                if (query.OperandIndexes.Count == 0)
                {
                    customTargets.Add(new OperandValue(1));
                }
                else
                {
                    CriteriaOperator[] trgts = new CriteriaOperator[query.OperandIndexes.Count];
                    for (int i = 0; i < targets.Count; i++)
                    {
                        if (query.OperandIndexes.ContainsKey(i))
                        {
                            trgts[query.OperandIndexes[i]] = targets[i];
                        }
                    }
                    customTargets.AddRange(trgts);
                }
                SelectStatementResult queryResult = SelectDataSimple(query, customTargets, false)[0];
                SelectStatementResultRow[] rows = new SelectStatementResultRow[queryResult.Rows.Length];
                for (int ri = 0; ri < rows.Length; ri++)
                {
                    object[] values = new object[targets.Count];
                    for (int i = 0; i < targets.Count; i++)
                    {
                        if (query.OperandIndexes.ContainsKey(i))
                        {
                            values[i] = queryResult.Rows[ri].Values[query.OperandIndexes[i]];
                        }
                        else
                        {
                            values[i] = query.ConstantValues[i].Value;
                        }
                    }
                    rows[ri] = new SelectStatementResultRow(values);
                }
                return new SelectStatementResult(rows);
            }
            return SelectDataSimple(query, targets, false)[0];
        }

        private SelectStatementResult[] SelectDataSimple(Query query, CriteriaOperatorCollection targets, bool includeMetadata)
        {
            // Команда выборки доступна как поле в процедуре отмены команды CancelSelect
            selectCommand = CreateCommand(query);
            try
            {
                SelectStatementResult[] result;
                for (int tryCount = 1; ; ++tryCount)
                {
                    try
                    {
                        result = InternalGetData(selectCommand, targets, query.SkipSelectedRecords, query.TopSelectedRecords, includeMetadata);
                        break;
                    }
                    catch (Exception e)
                    {
                        if (Transaction == null && IsConnectionBroken(e) && tryCount <= 1)
                        {
                            try { DoReconnect(); continue; }
                            catch { }
                        }
                        throw WrapException(e, selectCommand);
                    }
                }
                Trace.WriteLineIf(xpoSwitch.TraceInfo, new SelectStatementResultTracer(targets, result));
                return result;
            }
            finally
            {
                // Блокировка переменной selectCommand 
                lock (selectCancelLocker)
                {
                    DisposeCommand(selectCommand);
                    selectCommand = null;
                }
            }
        }

        private DBColumn FindColumnByName(DBTable table, DBColumn column)
        {
            string columnName = ComposeSafeColumnName(column.Name);
            for (int i = 0; i < table.Columns.Count; i++)
                if (string.Compare(table.Columns[i].Name, columnName, true) == 0)
                    return table.Columns[i];
            return null;
        }

        private bool IsColumnExists(DBTable table, DBColumn column)
        {
            return FindColumnByName(table, column) != null;
        }

        private bool IsColumnsEqual(StringCollection first, StringCollection second)
        {
            if (first.Count != second.Count)
                return false;
            for (int i = 0; i < first.Count; i++)
                if (String.Compare(ComposeSafeColumnName(first[i]), ComposeSafeColumnName(second[i]), true) != 0)
                    return false;
            return true;
        }

        private bool IsIndexExists(DBTable table, DBIndex index)
        {
            foreach (DBIndex i in table.Indexes)
            {
                if (IsColumnsEqual(i.Columns, index.Columns))
                    return true;
            }
            return false;
        }

        private bool IsForeignKeyExists(DBTable table, DBForeignKey foreignKey)
        {
            foreach (DBForeignKey fk in table.ForeignKeys)
            {
                if (string.Compare(ComposeSafeTableName(foreignKey.PrimaryKeyTable), ComposeSafeTableName(fk.PrimaryKeyTable), true) == 0 && 
                    string.Compare(ComposeSafeSchemaName(foreignKey.PrimaryKeyTable), ComposeSafeSchemaName(fk.PrimaryKeyTable), true) == 0 &&
                    IsColumnsEqual(fk.Columns, foreignKey.Columns) && 
                    IsColumnsEqual(fk.PrimaryKeyTableKeyColumns, foreignKey.PrimaryKeyTableKeyColumns))
                    return true;
            }
            return false;
        }

        #endregion

        #region Утилиты частного кода BaseOracleConnectionProvider и ODPConnectionProvider
        
        delegate bool TablesFilter(DBTable table);
        private SelectStatementResult GetDataForTables(ICollection tables, TablesFilter filter, string queryText)
        {
            QueryParameterCollection parameters = new QueryParameterCollection();
            List<SelectStatementResult> resultList = new List<SelectStatementResult>();
            int paramIndex = 0;
            int pos = 0;
            int count = tables.Count;
            int currentSize = 0;
            StringCollection inGroup = null;
            foreach (DBTable table in tables)
            {
                if (currentSize == 0)
                {
                    if (inGroup == null)
                    {
                        inGroup = new StringCollection();
                    }
                    else
                    {
                        if (inGroup.Count == 0)
                        {
                            resultList.Add(new SelectStatementResult());
                        }
                        resultList.Add(SelectData(new Query(string.Format(CultureInfo.InvariantCulture, queryText, StringListHelper.DelimitedText(inGroup, ",")), parameters, inGroup)));
                        inGroup.Clear();
                        parameters.Clear();
                    }
                    paramIndex = 0;
                    currentSize = (pos < count) ? (count - pos < 15 ? count - pos : 15) : 0;
                }
                if (filter == null || filter(table))
                {
                    parameters.Add(new OperandValue(ComposeSafeTableName(table.Name)));
                    inGroup.Add(":p" + paramIndex.ToString(CultureInfo.InvariantCulture));
                    ++paramIndex;
                    --currentSize;
                }
                ++pos;
            }
            if (inGroup != null && inGroup.Count > 0)
            {
                resultList.Add(SelectData(new Query(string.Format(CultureInfo.InvariantCulture, queryText, StringListHelper.DelimitedText(inGroup, ",")), parameters, inGroup)));
            }
            if (resultList.Count == 0) return new SelectStatementResult();
            if (resultList.Count == 1) return resultList[0];
            int fullResultSize = 0;
            for (int i = 0; i < resultList.Count; i++)
            {
                fullResultSize += resultList[i].Rows.Length;
            }
            if (fullResultSize == 0) return new SelectStatementResult();
            SelectStatementResultRow[] fullResult = new SelectStatementResultRow[fullResultSize];
            int copyPos = 0;
            for (int i = 0; i < resultList.Count; i++)
            {
                Array.Copy(resultList[i].Rows, 0, fullResult, copyPos, resultList[i].Rows.Length);
                copyPos += resultList[i].Rows.Length;
            }
            return new SelectStatementResult(fullResult);
        }

        ReflectConnectionHelper helper;
        ReflectConnectionHelper ConnectionHelper
        {
            get
            {
                if (helper == null)
                    helper = new ReflectConnectionHelper(Connection, "Oracle.DataAccess.Client.OracleException");
                return helper;
            }
        }

        #endregion
    }

    /// <summary>
    /// Адаптер команды Oracle для записи в скрипт
    /// </summary>
    public class OracleCommandScript : CommandScript
    {        
        /// <summary>
        /// Конструктор адаптера на основе оригинальной команды с указанием потока для записи скрипта
        /// </summary>
        /// <param name="command">Оригинальная команда базы данных</param>
        /// <param name="writer">Поток для записи скрипта</param>
        public OracleCommandScript(IDbCommand command, StreamWriter writer)
            : base(command, writer)
        {
        }

        /// <summary>
        /// Получить терминатор для указанной команды
        /// </summary>
        /// <param name="commandText">Текст команды</param>
        /// <returns>Возвращает &quot;;&quot;, если команда не завершается одним из этих знаков &quot;;&quot;, &quot;/&quot;</returns>
        public override string GetCommandTerminator(string commandText)
        {
            commandText = commandText != null ? commandText.Trim() : null;
            if (string.IsNullOrEmpty(commandText)) return null;
            char end = commandText[commandText.Length - 1];
            return end == ';' || end == '/' ? null : ";";
        }
    }

    /// <summary>
    /// Исключение при выполнении команд
    /// </summary>
    public class OracleConnectionProviderException : Exception
    {
        /// <inheritdoc/>
        public OracleConnectionProviderException() { }
        /// <inheritdoc/>
        public OracleConnectionProviderException(string message) : base(message) { }
    }

    /// <summary>
    /// Опции обновления схемы
    /// </summary>
    [Flags]
    public enum UpdateSchemaOptions
    {
        /// <summary>По умолчанию</summary>
        /// <remarks>Обновляются только те объекты безопасности, в таблицах которых есть новые поля или сложные констрейнты</remarks>
        Default = 0,

        /// <summary>Обновление всех объектов безопасности (независимо от наличия сложных констрейнтов)</summary>
        AllSafeObjects = 1,

        /// <summary>Проверка изменений в содержании объектов безопасности</summary>
        /// <remarks>Выполняется проверка содержания <b>package body</b> и <b>view</b>, если содержание в БД отличается, 
        /// то соответствующие объекты безопасности (представления и/или пакет+процедуры) обновляются, иначе пропускаются</remarks>
        CheckSafeContent = 2,

        /// <summary>Добавление в скрипт блока в виде комментария с небезопасными изменениями структуры</summary>
        /// <remarks>Небезопасными изменениями структуры считаются те, что могут повлиять на существующие данные или 
        /// выполниться с ошибкой или могут затронуть объекты структуры, созданные вручную. Например, это удаление 
        /// полей, индексов или констрейнтов, отличных от заданных в атрибутах, установка для поля констрейнта not null и т.д.
        /// На данный момент реализовано: изменение типа колонок таблиц, удаление устаревших колонок, установка not null.</remarks>
        UnsafeChanges = 4
    }
}
