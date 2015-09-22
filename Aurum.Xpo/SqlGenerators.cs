using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;

namespace Aurum.Xpo
{
    /// <summary>
    /// Предоставляет форматирование элементов sql-запросов, связанных с безопасным получением и изменением данных
    /// </summary>
    public interface ISecuredSqlGeneratorFormatter : ISqlGeneratorFormatter
    {
        /// <summary>
        /// Форматирует название представления для указанной таблицы в указанной схеме для sql-запроса
        /// </summary>
        /// <param name="schema">Название схемы</param>
        /// <param name="tableName">Название таблицы</param>
        /// <returns>Название представления в формате sql-запроса</returns>
        string FormatView(string schema, string tableName);

        /// <summary>
        /// Форматирует название представления для указанной таблицы и схемы и с указанным алиасом для sql-запроса
        /// </summary>
        /// <param name="schema">Название схемы</param>
        /// <param name="tableName">Название таблицы</param>
        /// <param name="viewAlias">Алиас представления</param>
        /// <returns>Название представления вместе с алиасом в формате sql-запроса</returns>
        string FormatView(string schema, string tableName, string viewAlias);

        /// <summary>
        /// Форматирует название пакета для указанных таблицы и схемы для sql-запроса
        /// </summary>
        /// <param name="schema">Название схемы</param>
        /// <param name="tableName">Название таблицы</param>
        /// <returns>Название пакета в формате sql-команды</returns>
        string FormatPackage(string schema, string tableName);

        /// <summary>
        /// Форматирует название процедуры добавления для указанных таблицы и схемы для sql-запроса
        /// </summary>
        /// <param name="schema">Название схемы</param>
        /// <param name="tableName">Название таблицы</param>
        /// <returns>Название процедуры в формате sql-команды</returns>
        string FormatProcedureAdd(string schema, string tableName);

        /// <summary>
        /// Форматирует название процедуры изменения для указанных таблицы и схемы для sql-запроса
        /// </summary>
        /// <param name="schema">Название схемы</param>
        /// <param name="tableName">Название таблицы</param>
        /// <returns>Название процедуры в формате sql-команды</returns>
        string FormatProcedureEdit(string schema, string tableName);

        /// <summary>
        /// Форматирует название процедуры удаления для указанных таблицы и схемы для sql-запроса
        /// </summary>
        /// <param name="schema">Название схемы</param>
        /// <param name="tableName">Название таблицы</param>
        /// <returns>Название процедуры в формате sql-команды</returns>
        string FormatProcedureDelete(string schema, string tableName);

        /// <summary>
        /// Форматирует название параметра процедуры для sql-запроса
        /// </summary>
        /// <param name="columnName">Исходное название колонки, соответствующей параметру процедуры</param>
        /// <returns>Название параметра процедуры в формате sql-команды</returns>
        string FormatProcedureParameter(string columnName);

        /// <summary>
        /// Форматирует название последовательности для указанных таблицы и схемы для sql-запроса
        /// </summary>
        /// <param name="schema">Название схемы</param>
        /// <param name="tableName">Название таблицы</param>
        /// <returns>Название последовательности в формате sql-команды</returns>
        string FormatSequence(string schema, string tableName);

        /// <summary>
        /// Форматирует строку sql-запроса с возвратом значений в переменные для sql-запроса
        /// </summary>
        /// <param name="propertiesSql">Строка полей</param>
        /// <param name="intoSql">Строка переменных</param>
        /// <param name="fromSql">Строка таблиц</param>
        /// <param name="whereSql">Строка условия</param>
        /// <returns>Строка sql-запроса</returns>
        string FormatSelectInto(string propertiesSql, string intoSql, string fromSql, string whereSql);

        /// <summary>
        /// Форматирует название настраиваемой таблицы для sql-запроса
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <param name="tableAlias">Алиас таблицы</param>
        /// <returns>Название настраиваемой таблицы в формате sql-команды</returns>
        string FormatCustomTable(string tableName, string tableAlias);

        /// <summary>
        /// Форматирует название колонки настраиваемой таблицы для sql-запроса
        /// </summary>
        /// <param name="columnName">Название колонки таблицы</param>
        /// <param name="tableAlias">Алиас таблицы</param>
        /// <returns>Название колонки настраиваемой таблицы в формате sql-команды</returns>
        string FormatCustomColumn(string columnName, string tableAlias);
    }

    /// <summary>
    /// Генератор sql-запросов с настройкой безопасности и форматирования запросов
    /// </summary>
    public class SelectSqlGeneratorBase : BaseSqlGeneratorWithParameters, IQueryCriteriaVisitor
    {
        /// <summary>Флаг перевода имен таблиц</summary>
        private bool translateNames;
        
        /// <summary>Флаг безопасности (использовать представления или таблицы)</summary>
        private bool secured;

        /// <summary>Коллекция алиасов таблиц с настраиваемым управлением данными</summary>
        private StringCollection customAliases;

        /// <summary>Выражение запроса</summary>
        new protected SelectStatement Root { get { return (SelectStatement)base.Root; } }

        /// <summary>Представитель форматирования sql-запросов с безопасным доступом к данным</summary>
        protected readonly ISecuredSqlGeneratorFormatter formatterSequred;

        /// <summary>
        /// Конструктор с выбором режима безопасности
        /// </summary>
        /// <param name="formatter">Представитель форматирования sql-запросов с безопасным доступом к данным</param>
        /// <param name="secured">Флаг безопасности, true - использовать представления, false - использовать таблицы</param>
        /// <param name="parameters">Справочник параметров</param>
        /// <param name="translateNames">Флаг перевода имен таблиц</param>
        /// <param name="customAliases">Коллекция алиасов таблиц с настраиваемым управлением данными</param>
        /// <remarks>Используется для внутренних целей при построении запросов в процедурах</remarks>
        public SelectSqlGeneratorBase(ISecuredSqlGeneratorFormatter formatter, bool secured,
            Dictionary<OperandValue, string> parameters, bool translateNames, StringCollection customAliases)
            : base(formatter, new TaggedParametersHolder(), parameters ?? new Dictionary<OperandValue, string>())
        {
            this.formatterSequred = formatter;
            this.secured = secured;
            this.translateNames = translateNames;
            this.customAliases = customAliases;
        }

        private string GetTableOrView(JoinNode node)
        {
            string nodeName = translateNames ? XPDictionaryInformer.TranslateTableName(node.TableName) : node.TableName;
            bool isCustom = customAliases != null && customAliases.Contains(node.Alias ?? string.Empty);
            if (isCustom) return formatterSequred.FormatCustomTable(nodeName, node.Alias);
            string nodeSchema = formatter.ComposeSafeSchemaName(nodeName);
            string nodeTable = formatter.ComposeSafeTableName(nodeName);
            return secured ?
                formatterSequred.FormatView(nodeSchema, nodeTable, node.Alias) :
                formatterSequred.FormatTable(nodeSchema, nodeTable, node.Alias);
        }

        private void AppendJoinNode(JoinNode node, StringBuilder joins)
        {
            if (formatter.BraceJoin)
                joins.Insert(0, "(");
            joins.Append("\n ");
            joins.Append(node.Type == JoinType.Inner ? "inner" : "left");
            joins.Append(" join ");
            joins.Append(GetTableOrView(node));
            joins.Append(" on ");
            joins.Append(Process(node.Condition));
            if (formatter.BraceJoin)
                joins.Append(')');
            foreach (JoinNode subNode in node.SubNodes)
                AppendJoinNode(subNode, joins);
        }

        /// <summary>
        /// Построение соединения таблиц выборки
        /// </summary>
        /// <returns>Соединение таблиц выборки</returns>
        protected new StringBuilder BuildJoins()
        {
            StringBuilder joins = new StringBuilder();
            joins.Append(GetTableOrView(Root));
            foreach (JoinNode subNode in Root.SubNodes)
                AppendJoinNode(subNode, joins);
            return joins;
        }

        /// <summary>
        /// Переопределяет генерацию sql-выражения
        /// </summary>
        /// <returns>Sql-выражение</returns>
        protected override string InternalGenerateSql()
        {
            string groupBySql = BuildGrouping();
            string propertiesSql = BuildProperties();
            string fromSql = BuildJoins().ToString();
            string whereSql = BuildCriteria();
            string havingSql = BuildGroupCriteria();
            string sortingSql = BuildSorting();
            int skipSelectedRecords = ((SelectStatement)Root).SkipSelectedRecords;
            int topSelectedRecords = ((SelectStatement)Root).TopSelectedRecords;
            fromSql = string.Concat(fromSql, BuildOuterApply());
            return FormatSelect(propertiesSql, fromSql, whereSql, sortingSql, groupBySql, havingSql, skipSelectedRecords, topSelectedRecords);
        }

        /// <summary>
        /// Форматирование результирующей строки sql-запроса
        /// </summary>
        /// <param name="propertiesSql">Строка полей</param>
        /// <param name="fromSql">Строка таблиц</param>
        /// <param name="whereSql">Строка условия</param>
        /// <param name="sortingSql">Строка сортировки</param>
        /// <param name="groupBySql">Строка группировки</param>
        /// <param name="havingSql">Строка условия после группировки</param>
        /// <param name="skipSelectedRecords">Количество записей, которые нужно пропустить</param>
        /// <param name="topSelectedRecords">Количество записей, которые нужно выбрать</param>
        /// <returns>Строка sql-запроса</returns>
        protected virtual string FormatSelect(string propertiesSql, string fromSql, string whereSql, string sortingSql, string groupBySql, string havingSql,
            int skipSelectedRecords, int topSelectedRecords)
        {
            if (skipSelectedRecords != 0)
            {
                ISqlGeneratorFormatterSupportSkipTake skipFormatter = SelectSqlGenerator.GetSkipTakeImpl(formatter);
                if (skipFormatter != null)
                    return skipFormatter.FormatSelect(propertiesSql, fromSql, whereSql, sortingSql, groupBySql, havingSql, skipSelectedRecords, topSelectedRecords);
                else if (topSelectedRecords != 0)
                    return formatter.FormatSelect(propertiesSql, fromSql, whereSql, sortingSql, groupBySql, havingSql, skipSelectedRecords + topSelectedRecords);
            }
            return formatter.FormatSelect(propertiesSql, fromSql, whereSql, sortingSql, groupBySql, havingSql, topSelectedRecords);
        }

        /// <summary>
        /// Переопределяет создание запроса
        /// </summary>
        /// <param name="sql">Sql-выражение</param>
        /// <param name="parameters">Параметры запроса</param>
        /// <param name="parametersNames">Имена параметров запроса</param>
        /// <returns>Запрос на основе исходного</returns>
        protected override Query CreateQuery(string sql, QueryParameterCollection parameters, IList parametersNames)
        {
            return new Query(sql, parameters, parametersNames, Root.SkipSelectedRecords, Root.TopSelectedRecords, constantValues, operandIndexes);
        }

        /// <contentfrom cref="IQueryCriteriaVisitor.Visit(QuerySubQueryContainer)" />
        object IQueryCriteriaVisitor.Visit(QuerySubQueryContainer container)
        {
            Aggregate agg = container.AggregateType;
            if (container.Node == null)
                return SubSelectSqlGenerator.GetSelectValue(container.AggregateProperty, agg, this);
            CriteriaOperator prop = (container.Node.Operands.Count > 0) ? container.Node.Operands[0] : null;
            string subSelect = new SubSelectSqlGenerator(this, prop, agg).GenerateSql(container.Node).Sql;
            string exists = agg == Aggregate.Exists ? "exists" : string.Empty;
            return string.Format(CultureInfo.InvariantCulture, "{1}({0})", subSelect, exists);
        }

        /// <contentfrom cref="IQueryCriteriaVisitor.Visit(QueryOperand)" />
        object IQueryCriteriaVisitor.Visit(QueryOperand operand)
        {
            bool isCustom = customAliases != null && customAliases.Contains(operand.NodeAlias ?? string.Empty);
            if (isCustom) return formatterSequred.FormatCustomColumn(operand.ColumnName, operand.NodeAlias);
            string columnName = formatter.ComposeSafeColumnName(operand.ColumnName);
            return operand.NodeAlias == null ? formatter.FormatColumn(columnName) : formatter.FormatColumn(columnName, operand.NodeAlias);
        }

        /// <summary>
        /// Генератор подзапроса
        /// </summary>
        class SubSelectSqlGenerator : SelectSqlGeneratorBase
        {
            private SelectSqlGeneratorBase parent;
            private Aggregate aggregate;
            private CriteriaOperator aggregateProperty;
            private static string[] agg;
            private static Dictionary<OperandValue, string> parametersEmpty;
            
            static SubSelectSqlGenerator()
            {
                agg = new string[6];
                agg[(int)Aggregate.Max] = "max({0})";
                agg[(int)Aggregate.Min] = "min({0})";
                agg[(int)Aggregate.Avg] = "avg({0})";
                agg[(int)Aggregate.Count] = "count({0})";
                agg[(int)Aggregate.Sum] = "sum({0})";
                agg[(int)Aggregate.Exists] = "{0}";
                parametersEmpty = new Dictionary<OperandValue, string>();
            }

            // Возвращает формат агрегированного значения указанного выражения
            public static string GetSelectValue(CriteriaOperator aggregateProperty, Aggregate aggregate, SelectSqlGeneratorBase generator)
            {
                string property = ReferenceEquals(aggregateProperty, null) ? "*" : (string)aggregateProperty.Accept(generator);
                return String.Format(agg[(int)aggregate], property);
            }

            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="parent">Родительский генератор запроса</param>
            /// <param name="aggregateProperty">Агрегатное свойство</param>
            /// <param name="aggregate">Тип агрегации</param>
            public SubSelectSqlGenerator(SelectSqlGeneratorBase parent, CriteriaOperator aggregateProperty, Aggregate aggregate)
                : base(parent.formatterSequred, parent.secured, parametersEmpty, parent.translateNames, parent.customAliases)
            {
                this.parent = parent;
                this.aggregate = aggregate;
                this.aggregateProperty = aggregateProperty;
            }

            /// <inheritdoc />
            protected override string FormatSelect(string propertiesSql, string fromSql, string whereSql, string sortingSql, string groupBySql, string havingSql,
                int skipSelectedRecords, int topSelectedRecords)
            {
                string selectValue = GetSelectValue(aggregateProperty, aggregate, this);
                return base.FormatSelect(selectValue, fromSql, whereSql, sortingSql, groupBySql, havingSql, skipSelectedRecords, topSelectedRecords);
            }

            /// <inheritdoc />
            public override string GetNextParameterName(OperandValue parameter)
            {
                return parent.GetNextParameterName(parameter);
            }
        }

        #region Утилиты частного кода SelectSqlGenerator

        private Dictionary<int, OperandValue> constantValues = new Dictionary<int, OperandValue>();
        private Dictionary<int, int> operandIndexes = new Dictionary<int, int>();
        private Dictionary<string, bool> groupProperties;

        private string BuildGrouping()
        {
            if (Root.GroupProperties.Count == 0)
                return null;
            groupProperties = new Dictionary<string, bool>();
            StringBuilder list = new StringBuilder();
            foreach (CriteriaOperator sp in Root.GroupProperties)
            {
                string groupProperty = Process(sp);
                groupProperties[groupProperty] = true;
                list.AppendFormat(CultureInfo.InvariantCulture, "{0},", groupProperty);
            }
            return list.ToString(0, list.Length - 1);
        }

        private string BuildGroupCriteria()
        {
            return Process(Root.GroupCondition, true);
        }

        private string BuildSorting()
        {
            if (Root.SortProperties.Count == 0)
                return null;
            StringBuilder list = new StringBuilder();
            for (int i = 0; i < Root.SortProperties.Count; i++)
            {
                SortingColumn sp = Root.SortProperties[i];
                int j;
                for (j = 0; j < i; j++)
                    if (Root.SortProperties[j].Property.Equals(sp.Property))
                        break;
                if (j < i)
                    continue;
                list.Append(formatter.FormatOrder(Process(sp.Property), sp.Direction));
                list.Append(',');
            }
            return list.ToString(0, list.Length - 1);
        }

        private string BuildProperties()
        {
            int operandIndex = 0;
            StringBuilder list = new StringBuilder();
            for (int i = 0; i < Root.Operands.Count; i++)
            {
                CriteriaOperator mic = Root.Operands[i];
                if (mic is OperandValue)
                {
                    constantValues.Add(i, (OperandValue)mic);
                }
                else
                {
                    if (list.Length > 0)
                        list.Append(',');
                    string property = Process(mic);
                    list.Append(property);
                    operandIndexes.Add(i, operandIndex++);
                }
            }
            if (operandIndex == 0) return "1";
            return list.ToString();
        }

        #endregion
    }

    /// <summary>
    /// Генератор sql-запросов на получение данных с безопасным доступом
    /// </summary>
    public class SecuredSelectSqlGenerator : SelectSqlGeneratorBase
    {
        /// <summary>
        /// Конструктор с указанным представителем форматирования sql-запросов
        /// </summary>
        /// <param name="formatter">Представитель форматирования sql-запросов с безопасным доступом к данным</param>
        /// <param name="customAliases">Коллекция алиасов таблиц с настраиваемым управлением данными</param>
        public SecuredSelectSqlGenerator(ISecuredSqlGeneratorFormatter formatter, StringCollection customAliases)
            : base(formatter, true, null, true, customAliases)
        {
        }
    }

    /// <summary>
    /// Выражение выборки данных с возвратом значений в список переменных
    /// </summary>
    public class SelectIntoStatement : SelectStatement
    {
        /// <summary>Список переменных, в которые выполняется выборка</summary>
        public CriteriaOperatorCollection Into = new CriteriaOperatorCollection();

        /// <summary>Конструктор</summary>
        public SelectIntoStatement() : base() { }

        /// <summary>Конструктор с указанной таблицей и алиасом</summary>
        /// <param name="table">Таблица</param>
        /// <param name="alias">Алиас</param>
        public SelectIntoStatement(DBTable table, string alias) : base(table, alias) { }

        /// <summary>Конструктор на основе запроса выборки данных без возврата значений</summary>
        /// <param name="statement">Выражение запроса</param>
        public SelectIntoStatement(SelectStatement statement)
        {
            this.TableName = statement.TableName;
            this.Alias = statement.Alias;
            // select
            this.Operands = statement.Operands;
            // from
            this.Type = statement.Type;
            this.SubNodes = statement.SubNodes;
            // where
            this.Condition = statement.Condition;
            // group by, having
            this.GroupCondition = statement.GroupCondition;
            if (statement.GroupProperties != null)
                this.GroupProperties.AddRange(statement.GroupProperties);
            // sort by
            if (statement.SortProperties != null)
                this.SortProperties.AddRange(statement.SortProperties);
            this.SkipSelectedRecords = statement.SkipSelectedRecords;
            this.TopSelectedRecords = statement.TopSelectedRecords;
        }
    }

    /// <summary>
    /// Генератор sql-запросов для процедур с возвратом значений в указанные переменные
    /// </summary>
    public class SelectIntoSqlGenerator : SelectSqlGeneratorBase
    {
        private string intoSql;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="formatter">Представитель форматирования sql-запросов с безопасным доступом к данным</param>
        /// <param name="parameters">Справочник параметров</param>
        /// <param name="translateNames">Флаг перевода имен таблиц</param>
        public SelectIntoSqlGenerator(ISecuredSqlGeneratorFormatter formatter, Dictionary<OperandValue, string> parameters, bool translateNames)
            : base(formatter, false, parameters, translateNames, null)
        {
        }

        // Построить строку переменных, в которые возвращаются значения
        private string BuildInto()
        {
            SelectIntoStatement statementInto = (SelectIntoStatement)Root;
            StringBuilder sb = new StringBuilder();
            foreach (CriteriaOperator var in statementInto.Into)
            {
                if (sb.Length > 0) sb.Append(", ");
                sb.Append(Process(var));
            }
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string InternalGenerateSql()
        {
            intoSql = BuildInto();
            return base.InternalGenerateSql();
        }

        /// <inheritdoc/>
        protected override string FormatSelect(string propertiesSql, string fromSql, string whereSql, string sortingSql, string groupBySql, string havingSql, int skipSelectedRecords, int topSelectedRecords)
        {
            return formatterSequred.FormatSelectInto(propertiesSql, intoSql, fromSql, whereSql);
        }
    }

    /// <summary>
    /// Генератор sql-команды на модификацию данных с безопасным доступом для БД Oracle
    /// </summary>
    public class OracleSecuredModifySqlGenerator : BaseObjectSqlGenerator, ICriteriaVisitor, IQueryCriteriaVisitor
    {
        /// <summary>
        /// Базовое выражение на модификацию данных
        /// </summary>
        protected new ModificationStatement Root 
        { 
            get { return (ModificationStatement)base.Root; } 
        }

        /// <summary>
        /// Представитель форматирования sql-запросов с безопасным доступом к данным
        /// </summary>
        protected readonly ISecuredSqlGeneratorFormatter formatterSequred;

        private TaggedParametersHolder identitiesByTag;
        private QueryParameterCollection commandParams;
        private List<string> commandParamsNames;

        // Переменные для форматирования блока с циклом вызова процедуры по условию
        private const string aliasName = "t";
        private DBTableEx table;
        private string whereCondition;
        private StringCollection keyProcessed;
        private StringBuilder keyColumns;
        private StringBuilder parametersVars;
        private string schemaName;
        private string tableName;

        /// <summary>
        /// Конструктор с указанным представителем форматирования запросов и параметрами
        /// </summary>
        /// <param name="formatter">Представитель форматирования запросов</param>
        /// <param name="identities">Представитель идентификации одинаковых параметров</param>
        /// <param name="parameters">Параметры</param>
        public OracleSecuredModifySqlGenerator(ISecuredSqlGeneratorFormatter formatter, TaggedParametersHolder identities, Dictionary<OperandValue, string> parameters) 
            : base(formatter, identities, parameters) 
        {
            this.formatterSequred = formatter;
            this.identitiesByTag = identities;
            this.commandParams = new QueryParameterCollection();
            this.commandParamsNames = new List<string>();
        }

        /// <summary>
        /// Возвращает запрос модификации данных
        /// </summary>
        /// <param name="table">Таблица, данные которой модифицируются</param>
        /// <param name="node">Выражение модификации данных</param>
        /// <returns>Запрос модификации данных</returns>
        public Query GenerateSql(DBTableEx table, BaseStatement node)
        {
            this.table = table;
            return GenerateSql(node);
        }

        /// <contentfrom cref="IQueryCriteriaVisitor.Visit(QueryOperand)"/>
        object IQueryCriteriaVisitor.Visit(QueryOperand theOperand)
        {
            keyProcessed.Add(theOperand.ColumnName);
            return formatter.FormatColumn(theOperand.ColumnName);
        }

        /// <contentfrom cref="ICriteriaVisitor.Visit(OperandValue)"/>
        object ICriteriaVisitor.Visit(OperandValue theOperand)
        {
            return AddParameter(theOperand, null);
        }

        /// <summary>Добавить параметр</summary>
        /// <param name="value">Значение</param>
        /// <param name="columnName">Название колонки, если это простой вызов процедуры</param>
        /// <param name="formatName">Признак необходимости форматирования названия параметра</param>
        /// <returns>Название параметра</returns>
        private string AddParameter(OperandValue value, string columnName, bool formatName = true)
        {
            string name = whereCondition == null && columnName != null ? 
                (formatName ? formatterSequred.FormatProcedureParameter(columnName) : columnName) : 
                ":p" + commandParamsNames.Count.ToString();
            value = identitiesByTag.ConsolidateParameter(value);
            commandParamsNames.Add(name);
            commandParams.Add(value);
            return name;
        }

        // Подготовка ключа для обновления или удаления записи
        private void PrepareKey()
        {
            // Условие единственной записи по ключу, обрабатывается как прямой вызов процедуры 
            // (cложный ключ обрабатывается как произвольное условие)
            BinaryOperator binary = Root.Condition as BinaryOperator;
            QueryOperand field = !object.ReferenceEquals(binary, null) ? binary.LeftOperand as QueryOperand : null;
            OperandValue value = !object.ReferenceEquals(binary, null) ? binary.RightOperand as OperandValue : null;
            if (!ReferenceEquals(field, null) && !ReferenceEquals(value, null) && binary.OperatorType == BinaryOperatorType.Equal &&
                table.PrimaryKey != null && table.PrimaryKey.Columns.Count == 1 && table.PrimaryKey.Columns[0] == field.ColumnName &&
                !table.Columns.Exists(col => col.ColumnType == DBColumnType.ByteArray))
            {
                AddParameter(value, field.ColumnName);
                keyProcessed.Add(field.ColumnName);
                return;
            }

            // Произвольное условие
            whereCondition = Process(Root.Condition);
            StringCollection keys = table.PrimaryKey == null ? new StringCollection() : table.PrimaryKey.Columns;
            if (keys.Count == 0) keys.Add(OracleTemplater.Rowid);
            foreach (string key in keys)
            {
                if (keyColumns.Length > 0) keyColumns.Append(", ");
                if (parametersVars.Length > 0) parametersVars.Append(", ");
                string keyColumn = formatter.FormatColumn(key);
                keyColumns.Append(keyColumn);
                parametersVars.AppendFormat("{0}.{1}", aliasName, keyColumn);
            };
        }

        // Подготовка параметров для добавления или обновления записи
        private void PrepareParameters(bool edit)
        {
            List<int> setValues = new List<int>(); setValues.Add(0);
            int groupIndex = 0, bitIndex = 0;
            foreach (DBColumn column in table.Columns)
            {
                // Ключ
                if (column.IsKey && keyProcessed.Contains(column.Name)) continue;   // processed key

                // Переданное значение
                OperandValue value = null;
                for (int i = 0; i < Root.Operands.Count; i++)
                    if (Root.Operands[i] is QueryOperand && ((QueryOperand)Root.Operands[i]).ColumnName == column.Name)
                    {
                        value = Root.Parameters[i];
                        break;
                    }

                // Поле readonly
                bool hasValue = !ReferenceEquals(value, null);
                if (edit && table.ColumnIsReadOnly(column))                        // readonly
                    if (hasValue) throw new OracleConnectionProviderException(
                        string.Format("Table '{0}.{1}', column '{2}' is readonly", schemaName, tableName, column.Name)); 
                    else continue;

                // Значение по умолчанию
                if (ReferenceEquals(value, null))
                    value = new OperandValue(null);

                // Обход bug ORA-1460 (см. OracleConnectionProviderEx.CreateParameter)
                if (column.ColumnType == DBColumnType.ByteArray && value.Value == null) value.Value = new byte[0];

                // Параметр
                string parameter = AddParameter(value, column.Name);
                if (parametersVars.Length > 0) parametersVars.Append(", ");
                parametersVars.Append(parameter);
                
                // Биты редактирования полей
                if (edit && !column.IsKey)
                {
                    if (hasValue) setValues[groupIndex] += Convert.ToInt32(Math.Pow(2, bitIndex));
                    bitIndex++;
                    if (bitIndex == OracleTemplater.MaxBitsInNumber) { setValues.Add(0); groupIndex++; bitIndex = 0; }
                }
            }
            if (edit)
            {
                for (groupIndex = 0; groupIndex < setValues.Count; groupIndex++)
                {
                    string parameter = AddParameter(new OperandValue(setValues[groupIndex]),
                        OracleTemplater.SetValueFlagsParameter + (groupIndex == 0 ? string.Empty : groupIndex.ToString()), false);
                    if (parametersVars.Length > 0) parametersVars.Append(", ");
                    parametersVars.Append(parameter);
                }
            }
        }

        /// <summary>
        /// Переопределяет генерацию sql-команды
        /// </summary>
        /// <returns>Sql-команда</returns>
        protected override string InternalGenerateSql()
        {
            if (table == null) table = XPDictionaryInformer.TranslateAndGet(Root.TableName);
            string rootName = table.Name;
            schemaName = formatter.ComposeSafeSchemaName(rootName);
            tableName = formatter.ComposeSafeTableName(rootName);
            whereCondition = null;
            keyProcessed = new StringCollection();
            keyColumns = new StringBuilder();
            parametersVars = new StringBuilder();

            // Добавление
            if (Root is InsertStatement)
            {
                InsertStatement ins = (InsertStatement)Root;
                if (!ReferenceEquals(ins.IdentityParameter, null))
                {
                    identitiesByTag.ConsolidateIdentity(ins.IdentityParameter);
                    commandParams.Add(ins.IdentityParameter);
                    commandParamsNames.Add(formatterSequred.FormatProcedureParameter(ins.IdentityColumn));
                    keyProcessed.Add(ins.IdentityColumn);
                }
                PrepareParameters(false);
                return formatterSequred.FormatProcedureAdd(schemaName, tableName);
            }
            // Изменение
            else if (Root is UpdateStatement)
            {
                PrepareKey();
                PrepareParameters(true);
                return FormatBlock(formatterSequred.FormatProcedureEdit(schemaName, tableName));
            }
            // Удаление
            else if (Root is DeleteStatement)
            {
                PrepareKey();
                return FormatBlock(formatterSequred.FormatProcedureDelete(schemaName, tableName));
            }
            else
                new ArgumentException("Incorrect type of modification statement");
            return null;
        }

        /// <summary>
        /// Переопределяет создание команды
        /// </summary>
        /// <param name="sql">Sql-команда</param>
        /// <param name="parameters">Параметры команды</param>
        /// <param name="parametersNames">Имена параметров запроса</param>
        /// <returns>Запрос на основе исходного</returns>
        protected override Query CreateQuery(string sql, QueryParameterCollection parameters, IList parametersNames)
        {
            return new Query(sql, commandParams, commandParamsNames);
        }

        private string FormatBlock(string procedureName)
        {
            // Простой вызов процедуры
            if (whereCondition == null) return procedureName;

            // Вызов в цикле блока с условием
            string viewName = formatterSequred.FormatView(schemaName, tableName);
            string selectSql = formatter.FormatSelect(keyColumns.ToString(), viewName, whereCondition, null, null, null, 0);
            return string.Format(CultureInfo.InvariantCulture,
                "begin for {3} in (\r\n{2}\r\n) loop {0}({1}); end loop; end;", procedureName, parametersVars, selectSql, aliasName);
        }
    }

    /// <summary>
    /// Генератор выражения констрейнта проверки для БД Oracle
    /// </summary>
    public class OracleCheckConstraintGenerator : OracleWhereGenerator, IQueryCriteriaVisitor
    {
        /// <summary>Исключение, в случае невозможности преобразования критерия в выражение констрейнта</summary>
        class CheckConstraintImpossible : Exception { public CheckConstraintImpossible(string message) : base(message) { } };

        private ISqlGeneratorFormatter formatter;
        private ISqlGeneratorFormatterEx formatterEx;

        // Процесс обработки операнда
        string ProcessOperand(object operand)
        {
            return Process((CriteriaOperator)operand);
        }

        // Конструктор с указанным представителем форматирования
        private OracleCheckConstraintGenerator(ISqlGeneratorFormatter formatter)
            : base(false)
        {
            this.formatter = formatter;
            this.formatterEx = formatter as ISqlGeneratorFormatterEx;
        }

        /// <summary>
        /// Генерирует выражение констрейнта для указанного критерия
        /// </summary>
        /// <param name="formatter">Представитель форматирования sql-запросов</param>
        /// <param name="criteria">Критерий, для которого генерируется выражение констрейнта</param>
        /// <returns>Строка выражения констрейнта для указанного критерия или null, если критерий не может быть преобразован в констрейнт</returns>
        public static string GenerateExpression(ISqlGeneratorFormatter formatter, CriteriaOperator criteria)
        {
            try { return new OracleCheckConstraintGenerator(formatter).Process(criteria); }
            catch (CheckConstraintImpossible) { return null; }
        }

        /// <summary>
        /// Генерирует выражение констрейнта для указанного критерия
        /// </summary>
        /// <param name="formatter">Представитель форматирования sql-запросов</param>
        /// <param name="criteria">Критерий, для которого генерируется выражение констрейнта</param>
        /// <param name="reason">Причина невозможности преобразования критерия в констрейнт, если операция завершилась неуспешно</param>
        /// <returns>Строка выражения констрейнта для указанного критерия или null, если критерий не может быть преобразован в констрейнт</returns>
        public static string GenerateExpression(ISqlGeneratorFormatter formatter, CriteriaOperator criteria, out string reason)
        {
            reason = null;
            try { return new OracleCheckConstraintGenerator(formatter).Process(criteria); }
            catch (CheckConstraintImpossible ex) { reason = ex.Message; return null; }
        }

        /// <contentfrom cref="IQueryCriteriaVisitor.Visit(QuerySubQueryContainer)"/>
        object IQueryCriteriaVisitor.Visit(QuerySubQueryContainer theOperand)
        {
            throw new CheckConstraintImpossible(string.Format("Cannot contain subqueries ({0})", theOperand));
        }

        /// <contentfrom cref="IQueryCriteriaVisitor.Visit(QueryOperand)"/>
        object IQueryCriteriaVisitor.Visit(QueryOperand theOperand)
        {
            return formatter.FormatColumn(formatter.ComposeSafeColumnName(theOperand.ColumnName));
        }

        /// <contentfrom cref="ICriteriaVisitor.Visit(FunctionOperator)"/>
        object ICriteriaVisitor.Visit(FunctionOperator theOperator)
        {
            // Фильтр невозможных для констрейнта функций
            switch (theOperator.OperatorType)
            {
                case FunctionOperatorType.Now:
                case FunctionOperatorType.Today:
                case FunctionOperatorType.UtcNow:
                case FunctionOperatorType.LocalDateTimeThisYear:
                case FunctionOperatorType.LocalDateTimeThisMonth:
                case FunctionOperatorType.LocalDateTimeLastWeek:
                case FunctionOperatorType.LocalDateTimeThisWeek:
                case FunctionOperatorType.LocalDateTimeYesterday:
                case FunctionOperatorType.LocalDateTimeToday:
                case FunctionOperatorType.LocalDateTimeNow:
                case FunctionOperatorType.LocalDateTimeTomorrow:
                case FunctionOperatorType.LocalDateTimeDayAfterTomorrow:
                case FunctionOperatorType.LocalDateTimeNextWeek:
                case FunctionOperatorType.LocalDateTimeTwoWeeksAway:
                case FunctionOperatorType.LocalDateTimeNextMonth:
                case FunctionOperatorType.LocalDateTimeNextYear:
                case FunctionOperatorType.IsThisMonth:
                case FunctionOperatorType.IsThisWeek:
                case FunctionOperatorType.IsThisYear:
                    throw new CheckConstraintImpossible(string.Format("Impossible function type ", theOperator));
            }
            // Настраиваемая функция
            string customFunction = null;
            if (theOperator.OperatorType == FunctionOperatorType.Custom || theOperator.OperatorType == FunctionOperatorType.CustomNonDeterministic)
            {
                if (theOperator.Operands.Count < 1 || !(theOperator.Operands[0] is OperandValue) || !(((OperandValue)theOperator.Operands[0]).Value is string))
                    throw new InvalidOperationException(string.Format("Invalid custom function operands ", theOperator));
                else
                    customFunction = (string)((OperandValue)theOperator.Operands[0]).Value;
            }
            // С процедурой обработки параметров
            if (formatterEx != null)
            {
                object[] operands = new object[theOperator.Operands.Count];
                Array.Copy(theOperator.Operands.ToArray(), operands, operands.Length);
                if (customFunction != null) operands[0] = customFunction;
                return formatterEx.FormatFunction(ProcessOperand, theOperator.OperatorType, operands);
            }
            // Без процедуры обработки параметров
            else
            {
                string[] operands = new string[theOperator.Operands.Count];
                int i = customFunction == null ? 0 : 1;
                if (customFunction != null) operands[0] = customFunction;
                for (; i < theOperator.Operands.Count; i++)
                    operands[i] = Process((CriteriaOperator)theOperator.Operands[i]);
                return formatter.FormatFunction(theOperator.OperatorType, operands);
            }
        }
    }

    /// <summary>
    /// Служебный класс для обработки алиасов в операторе условия
    /// </summary>
    class AliasProcessor : IQueryCriteriaVisitor
    {
        /// <summary>
        /// Делегат обработки алиаса
        /// </summary>
        /// <param name="alias">Исходный алиас</param>
        /// <returns>Измененный алиас</returns>
        public delegate string AliasProcess(string alias);

        private void Process(CriteriaOperator operand)
        {
            if (!ReferenceEquals(operand, null))
                operand.Accept(this);
        }

        private void Process(CriteriaOperatorCollection operands)
        {
            if (operands != null)
                foreach (CriteriaOperator operand in operands) Process(operand);
        }

        private void Process(JoinNode node)
        {
            if (node == null) return;
            node.Alias = ProcessAlias(node.Alias);
            Process(node.Condition);
            if (node.SubNodes != null)
                foreach (JoinNode sub in node.SubNodes)
                    Process(sub);
        }

        private void Process(BaseStatement statement)
        {
            if (statement == null) return;
            Process((JoinNode)statement);
            Process(statement.Operands);
        }

        private AliasProcess aliasProcess;
        private string ProcessAlias(string alias)
        {
            if (aliasProcess != null)
                return aliasProcess(alias);
            return alias;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="process">Делегат обработки алиаса</param>
        public AliasProcessor(AliasProcess process)
        {
            this.aliasProcess = process;
        }

        /// <summary>
        /// Выполнить обработку алиасов в указанном выражении
        /// </summary>
        /// <param name="statement">Выражение БД</param>
        /// <param name="process">Процедура обработки алиасов</param>
        public static void Execute(BaseStatement statement, AliasProcess process)
        {
            AliasProcessor processor = new AliasProcessor(process);
            processor.Process(statement);
        }

        /// <summary>
        /// Выполнить обработку алиасов в указанном операторе условия
        /// </summary>
        /// <param name="operand">Оператор условия</param>
        /// <param name="process">Процедура обработки алиасов</param>
        public static void Execute(CriteriaOperator operand, AliasProcess process)
        {
            AliasProcessor processor = new AliasProcessor(process);
            processor.Process(operand);
        }

        #region IQueryCriteriaVisitor, ICriteriaVisitor

        /// <contentfrom cref="IQueryCriteriaVisitor.Visit(QuerySubQueryContainer)"/>
        public object Visit(QuerySubQueryContainer theOperand)
        {
            Process(theOperand.Node);
            Process(theOperand.AggregateProperty);
            return null;
        }

        /// <contentfrom cref="IQueryCriteriaVisitor.Visit(QueryOperand)"/>
        public object Visit(QueryOperand theOperand)
        {
            theOperand.NodeAlias = ProcessAlias(theOperand.NodeAlias);
            return null;
        }

        /// <contentfrom cref="ICriteriaVisitor.Visit(FunctionOperator)"/>
        public object Visit(FunctionOperator theOperator)
        {
            Process(theOperator.Operands);
            return null;
        }

        /// <contentfrom cref="ICriteriaVisitor.Visit(OperandValue)"/>
        public object Visit(OperandValue theOperand)
        {
            return null;
        }

        /// <contentfrom cref="ICriteriaVisitor.Visit(GroupOperator)"/>
        public object Visit(GroupOperator theOperator)
        {
            Process(theOperator.Operands);
            return null;
        }

        /// <contentfrom cref="ICriteriaVisitor.Visit(InOperator)"/>
        public object Visit(InOperator theOperator)
        {
            Process(theOperator.LeftOperand);
            Process(theOperator.Operands);
            return null;
        }

        /// <contentfrom cref="ICriteriaVisitor.Visit(UnaryOperator)"/>
        public object Visit(UnaryOperator theOperator)
        {
            Process(theOperator.Operand);
            return null;
        }

        /// <contentfrom cref="ICriteriaVisitor.Visit(BinaryOperator)"/>
        public object Visit(BinaryOperator theOperator)
        {
            Process(theOperator.LeftOperand);
            Process(theOperator.RightOperand);
            return null;
        }

        /// <contentfrom cref="ICriteriaVisitor.Visit(BetweenOperator)"/>
        public object Visit(BetweenOperator theOperator)
        {
            Process(theOperator.TestExpression);
            Process(theOperator.BeginExpression);
            Process(theOperator.EndExpression);
            return null;
        }

        #endregion
    }

    /// <summary>
    /// Генератор sql-команд администрирования безопасности базы данных
    /// </summary>
    public class AdminSecurityGenerator
    {
        private SecurityStatement root;
        private ISecuredSqlGeneratorFormatter formatter;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="statement">Команда администрирования безопасности</param>
        /// <param name="formatter">Представитель форматирования sql-запросов с безопасным доступом к данным</param>
        public AdminSecurityGenerator(SecurityStatement statement, ISecuredSqlGeneratorFormatter formatter)
        {
            this.root = statement;
            this.formatter = formatter;
        }

        /// <summary>
        /// Возвращает sql-инструкции для выполнения команды администрирования безопасности
        /// </summary>
        /// <returns>Sql-инструкции администрирования безопасности</returns>
        public IEnumerable<string> GenerateSqlCommands()
        {
            List<string> commands = new List<string>();
            switch (root.Operation)
            {
                case AdminSecurityOperations.Create:
                case AdminSecurityOperations.Drop:
                    string operation = root.Operation == AdminSecurityOperations.Create ? "create" : "drop";
                    string objectType = root.LeftOperand.ObjectType == SecurityObjectTypes.Role ? "role" : "user";
                    string objectName = root.LeftOperand.ObjectName;
                    if (root.Operation == AdminSecurityOperations.Create && root.LeftOperand.ObjectType == SecurityObjectTypes.User)
                    {
                        commands.Add(string.Format("create user {0} identified by {0}123", objectName));
                        commands.Add(string.Format("grant create session to {0}", objectName));
                    }
                    else
                        commands.Add(string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", operation, objectType, objectName));
                    break;
                case AdminSecurityOperations.GrantTo:
                case AdminSecurityOperations.RevokeFrom:
                    operation = root.Operation == AdminSecurityOperations.GrantTo ? "grant {0} to {1}" : "revoke {0} from {1}";
                    Action<string> commandsAdd = s => commands.Add(
                        string.Format(CultureInfo.InvariantCulture, operation, s, root.RightOperand.ObjectName));
                    if (root.LeftOperand.ObjectType == SecurityObjectTypes.Table)
                    {
                        SecurityTableRights rights = root.LeftOperand.TableRights;
                        string schema = formatter.ComposeSafeSchemaName(root.LeftOperand.ObjectName);
                        string table = formatter.ComposeSafeTableName(root.LeftOperand.ObjectName);
                        if ((rights & SecurityTableRights.Select) != 0) 
                            commandsAdd("select on " + formatter.FormatView(schema, table));
                        if ((rights & SecurityTableRights.Insert) != 0) 
                            commandsAdd("execute on " + formatter.FormatProcedureAdd(schema, table));
                        if ((rights & SecurityTableRights.Update) != 0) 
                            commandsAdd("execute on " + formatter.FormatProcedureEdit(schema, table));
                        if ((rights & SecurityTableRights.Delete) != 0) 
                            commandsAdd("execute on " + formatter.FormatProcedureDelete(schema, table));
                    }
                    else
                        commandsAdd(root.LeftOperand.ObjectName);
                    break;
                case AdminSecurityOperations.SetUserInfo:
                    string userName = root.LeftOperand.ObjectName;
                    SecurityUserInfo userInfo = (SecurityUserInfo)root.RightOperand;
                    if (userInfo.IsActive.HasValue)
                        commands.Add(string.Format(
                            "alter user {0} account {1}", userName, userInfo.IsActive.Value ? "unlock" : "lock"));
                    if (userInfo.IsExpired.HasValue)
                        commands.Add(string.Format(userInfo.IsExpired.Value ?
                            "alter user {0} password expire" : "alter user {0} identified by {0}123", userName));
                    break;
            }
            return commands;
        }
    }

    /// <summary>
    /// Служебный класс, заменяющий названия таблиц и полей для объектов с настраиваемым управлением данными
    /// </summary>
    class CustomPersistentSelectProcessor : IQueryCriteriaVisitor
    {
        private IDataStore dataStore;
        private Dictionary<string, Tuple<DBTableEx, DBTableEx>> tables;

        private CriteriaOperator Process(CriteriaOperator operand)
        {
            if (!ReferenceEquals(operand, null))
                return (CriteriaOperator)operand.Accept(this);
            return null;
        }

        private void Process(CriteriaOperatorCollection operands)
        {
            if (operands != null)
                for (int i = 0; i < operands.Count; i++)
                    operands[i] = Process(operands[i]);
        }

        private void Process(JoinNode node)
        {
            if (node == null) return;
            string alias = node.Alias ?? string.Empty;
            if (tables.ContainsKey(alias))
            {
                Tuple<DBTableEx, DBTableEx> replace = tables[alias];
                node.TableName = replace.Item2.Name;
            }
            Process(node.Condition);
            if (node.SubNodes != null)
                foreach (JoinNode sub in node.SubNodes)
                    Process(sub);
        }

        private void Process(BaseStatement statement)
        {
            if (statement == null) return;
            CollectTables(statement);
            Process((JoinNode)statement);
            Process(statement.Operands);
        }

        private void CollectTables(JoinNode node)
        {
            string alias = node.Alias ?? string.Empty;
            if (!tables.ContainsKey(alias))
            {
                DBTableEx table = XPDictionaryInformer.TranslateAndGet(node.TableName);
                if (table != null && table.IsCustom)
                {
                    DBTableEx table2 = table.CustomPersistent.GetTable(dataStore);
                    tables.Add(alias, new Tuple<DBTableEx, DBTableEx>(table, table2));
                }
            }
            if (node.SubNodes != null)
                foreach (JoinNode subNode in node.SubNodes)
                    CollectTables(subNode);
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dataStore">Объект, выполняющий операции с данными</param>
        public CustomPersistentSelectProcessor(IDataStore dataStore)
        {
            this.dataStore = dataStore;
            this.tables = new Dictionary<string, Tuple<DBTableEx, DBTableEx>>();
        }

        /// <summary>
        /// Выполнить обработку алиасов в указанном выражении
        /// </summary>
        /// <param name="statement">Выражение выборки или изменения данных</param>
        /// <param name="dataStore">Объект, выполняющий операции с данными</param>
        /// <param name="customAliases">Список алиасов таблиц с настраиваемым управлением данными</param>
        public static void Execute(BaseStatement statement, IDataStore dataStore, out StringCollection customAliases)
        {
            CustomPersistentSelectProcessor processor = new CustomPersistentSelectProcessor(dataStore);
            processor.Process(statement);
            customAliases = new StringCollection();
            if (processor.tables.Count > 0)
                customAliases.AddRange(processor.tables.Keys.ToArray());
        }

        #region IQueryCriteriaVisitor, ICriteriaVisitor

        /// <contentfrom cref="IQueryCriteriaVisitor.Visit(QuerySubQueryContainer)"/>
        public object Visit(QuerySubQueryContainer theOperand)
        {
            Process(theOperand.Node);
            theOperand.AggregateProperty = Process(theOperand.AggregateProperty);
            return theOperand;
        }

        /// <contentfrom cref="IQueryCriteriaVisitor.Visit(QueryOperand)"/>
        public object Visit(QueryOperand theOperand)
        {
            string alias = theOperand.NodeAlias ?? string.Empty;
            if (tables.ContainsKey(alias))
            {
                Tuple<DBTableEx, DBTableEx> replace = tables[alias];
                int columnIndex = replace.Item1.Columns.FindIndex(col => col.Name == theOperand.ColumnName);

                if (columnIndex >= 0 && columnIndex < replace.Item2.Columns.Count)
                {
                    DBColumn column = replace.Item2.Columns[columnIndex];
                    if (column is DBCriteriaColumn)
                        return ((DBCriteriaColumn)column).GetCriteria(theOperand.NodeAlias);
                    return new QueryOperand(column.Name, theOperand.NodeAlias, theOperand.ColumnType);
                }
                else
                    throw new InvalidOperationException(string.Format(
                        "Query with custom persistent table {0}. Column index {1} is out of range",
                        replace.Item1.Name, columnIndex));
            }
            return theOperand;
        }

        /// <contentfrom cref="ICriteriaVisitor.Visit(FunctionOperator)"/>
        public object Visit(FunctionOperator theOperator)
        {
            Process(theOperator.Operands);
            return theOperator;
        }

        /// <contentfrom cref="ICriteriaVisitor.Visit(OperandValue)"/>
        public object Visit(OperandValue theOperand)
        {
            return theOperand;
        }

        /// <contentfrom cref="ICriteriaVisitor.Visit(GroupOperator)"/>
        public object Visit(GroupOperator theOperator)
        {
            Process(theOperator.Operands);
            return theOperator;
        }

        /// <contentfrom cref="ICriteriaVisitor.Visit(InOperator)"/>
        public object Visit(InOperator theOperator)
        {
            theOperator.LeftOperand = Process(theOperator.LeftOperand);
            Process(theOperator.Operands);
            return theOperator;
        }

        /// <contentfrom cref="ICriteriaVisitor.Visit(UnaryOperator)"/>
        public object Visit(UnaryOperator theOperator)
        {
            theOperator.Operand = Process(theOperator.Operand);
            return theOperator;
        }

        /// <contentfrom cref="ICriteriaVisitor.Visit(BinaryOperator)"/>
        public object Visit(BinaryOperator theOperator)
        {
            theOperator.LeftOperand = Process(theOperator.LeftOperand);
            theOperator.RightOperand = Process(theOperator.RightOperand);
            return theOperator;
        }

        /// <contentfrom cref="ICriteriaVisitor.Visit(BetweenOperator)"/>
        public object Visit(BetweenOperator theOperator)
        {
            theOperator.TestExpression = Process(theOperator.TestExpression);
            theOperator.BeginExpression = Process(theOperator.BeginExpression);
            theOperator.EndExpression = Process(theOperator.EndExpression);
            return theOperator;
        }

        #endregion
    }
}
