using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;

namespace Aurum.Xpo
{
    /// <summary>
    /// Интерфейс объекта, осуществляющего настраиваемое управление данными в базе данных
    /// </summary>
    public interface ICustomPersistent
    {
        /// <summary>
        /// Возвращает таблицу, которая используется в запросах
        /// </summary>
        /// <param name="dataStore">Объект, выполняющий операции с данными</param>
        /// <returns>Таблица базы данных, которая используется в запросах</returns>
        DBTableEx GetTable(IDataStore dataStore);

        /// <summary>
        /// Выполняет добавление данных
        /// </summary>
        /// <param name="dataStore">Объект, выполняющий операции с данными</param>
        /// <param name="statement">Выражение добавления данных</param>
        /// <returns>Идентификатор новой записи</returns>
        ParameterValue Insert(IDataStore dataStore, InsertStatement statement);
        
        /// <summary>
        /// Выполняет изменение данных
        /// </summary>
        /// <param name="dataStore">Объект, выполняющий операции с данными</param>
        /// <param name="statement">Выражение изменения данных</param>
        void Update(IDataStore dataStore, UpdateStatement statement);

        /// <summary>
        /// Выполняет удаление данных
        /// </summary>
        /// <param name="dataStore">Объект, выполняющий операции с данными</param>
        /// <param name="statement">Выражение удаления данных</param>
        void Delete(IDataStore dataStore, DeleteStatement statement);
    }

    /// <summary>
    /// Интерфейс объекта, осуществляющего настраиваемые выборку и управление данными в базе данных
    /// </summary>
    public interface ICustomPersistentQuery 
    {
        /// <summary>
        /// Выполняет запрос данных
        /// </summary>
        /// <param name="dataStore">Объект, выполняющий операции с данными</param>
        /// <param name="statement">Выражение запроса данных</param>
        /// <returns>Результат запроса данных</returns>
        SelectStatementResult Select(IDataStore dataStore, SelectStatement statement);
    }

    /// <summary>
    /// Коллекция констрейнтов базы данных
    /// </summary>
    public class DBConstraintCollection : List<DBConstraintBase> { }

    /// <summary>
    /// Схема объектов базы данных
    /// </summary>
    public class DBSchema
    {
        private Dictionary<string, Lazy<DBTableEx>> dictionary = new Dictionary<string, Lazy<DBTableEx>>();

        /// <summary>
        /// Добавляет в справочник таблицу с указанным названием и отложенным получением
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <param name="tableGetter">Объект, возвращающий таблицу в момент необходимости</param>
        internal void Add(string tableName, Lazy<DBTableEx> tableGetter)
        {
            if (!dictionary.ContainsKey(tableName))
                dictionary.Add(tableName, tableGetter);
        }

        /// <summary>
        /// Определяет, содержит ли схема таблицу с указанным названием
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <returns>True, если схема содержит таблицу с указанным названием, иначе false</returns>
        public bool Contains(string tableName)
        {
            return dictionary.ContainsKey(tableName);
        }

        /// <summary>
        /// Список названий всех таблиц схемы
        /// </summary>
        public IEnumerable<string> TableNames
        {
            get { return dictionary.Keys; }
        }

        /// <summary>
        /// Ищет в справочнике таблицу по указанному названию
        /// </summary>
        /// <param name="tableName">Название таблицы (транслированное)</param>
        /// <returns>Таблица с дополнительной информацией по констрейнтам или null, если таблица не найдена в справочнике</returns>
        /// <remarks>Выполняется инициализация значения таблицы и обновление информации из исходной таблицы, 
        /// если есть отличия (<see cref="Aurum.Xpo.DBTableEx.Refresh"/>).</remarks>
        public DBTableEx FindTable(string tableName)
        {
            Lazy<DBTableEx> getter;
            if (dictionary.TryGetValue(tableName, out getter))
            {
                getter.Value.Refresh();
                return getter.Value;
            }
            return null;
        }

        /// <summary>
        /// Возвращает таблицу по указанному названию
        /// </summary>
        /// <param name="tableName">Название таблицы (транслированное)</param>
        /// <returns>Таблица с дополнительной информацией по констрейнтам</returns>
        /// <exception cref="KeyNotFoundException">Вызывается, если таблица с указанным названием в справочнике не найдена</exception>
        public DBTableEx GetTable(string tableName)
        {
            DBTableEx table = FindTable(tableName);
            if (table == null)
                throw new KeyNotFoundException("Table " + tableName + " is not found. Check XPDictionaryInformer registration");
            return table;
        }
    }

    /// <summary>
    /// Таблица базы данных с дополнительной информацией о констрейнтах
    /// </summary>
    public class DBTableEx : DBTable
    {
        private DBTable table;
        private DBTableEx parent;
        private DBConstraintCollection constraints = new DBConstraintCollection();

        /// <summary>
        /// Оригинальная таблица
        /// </summary>
        internal DBTable OriginalTable
        {
            get { return table; }
        }

        /// <summary>
        /// Таблица родительского класса
        /// </summary>
        internal DBTableEx ParentTable
        {
            get { return parent; }
            set { parent = value; }
        }

        /// <summary>
        /// Название оригинальной таблицы
        /// </summary>
        /// <value>Если оригинальная таблица отсутствует, то null</value>
        public string OriginalName
        {
            get { return table != null ? table.Name : null; }
        }

        /// <summary>
        /// Указывает, что управление данными таблицы переопределено
        /// </summary>
        /// <value>True, если соответствующий класс имеет атрибут <see cref="CustomPersistentAttribute"/></value>
        public bool IsCustom { get { return CustomPersistent != null; } }

        /// <summary>
        /// Объект базы данных с настраиваемым управлением, если управление данными таблицы переопределено
        /// </summary>
        public ICustomPersistent CustomPersistent;

        /// <summary>Конструктор</summary>        
        public DBTableEx() { }

        /// <summary>Конструктор с указанием названия таблицы</summary>
        /// <param name="name">Название таблицы</param>
        public DBTableEx(string name) : base(name) { }

        /// <summary>
        /// Конструктор на основе существующей таблицы без констрейнтов
        /// </summary>
        /// <param name="name">Название таблицы в базе данных</param>
        /// <param name="table">Исходная таблица, на основе которой создается расширенная копия</param>
        public DBTableEx(string name, DBTable table)
        {
            this.table = table;
            Name = name;
            CustomPersistent = null;
            IsView = table.IsView;
            PrimaryKey = table.PrimaryKey;
            Update();
        }

        /// <summary>
        /// Конструктор с указанием названия таблицы и колонок
        /// </summary>
        /// <param name="name">Название</param>
        /// <param name="columns">Колонки</param>
        public DBTableEx(string name, params DBColumn[] columns)
        {
            Name = name;
            Columns.AddRange(columns);
            DBColumnCollection pk = new DBColumnCollection();
            pk.AddRange(columns.Where(col => col.IsKey));
            if (pk.Count > 0) PrimaryKey = new DBPrimaryKey(pk);
        }

        /// <summary>
        /// Обновление информации из исходной таблицы
        /// </summary>
        public void Update()
        {
            if (table == null) return;
            Columns.Clear();
            Indexes.Clear();
            ForeignKeys.Clear();
            Columns.AddRange(table.Columns);
            Indexes.AddRange(table.Indexes);
            ForeignKeys.AddRange(table.ForeignKeys.Select(fk => new DBForeignKey(fk.Columns,
                XPDictionaryInformer.TranslateTableName(fk.PrimaryKeyTable), fk.PrimaryKeyTableKeyColumns)));
        }

        /// <summary>
        /// Обновление информации из исходной таблицы, если есть отличия от исходной таблицы
        /// </summary>
        /// <remarks>Эта операция выполняется при запросе таблицы из схемы <see cref="Aurum.Xpo.DBSchema.FindTable"/>.
        /// Изменения исходной таблицы возможны, если у класса этой таблицы есть наследники с атрибутом 
        /// <c>[MapInheritance(MapInheritanceType.ParentTable)]</c> и обращение к ним произошло позже, 
        /// чем создание данного объекта DBTableEx. Наследники добавляют в родительскую таблицу свои поля и ссылки, 
        /// которые нужно синхронизировать с текущим объектом.
        /// </remarks>
        public void Refresh()
        {
            if (table != null && (
                (Columns.Count != table.Columns.Count) ||
                (Indexes.Count != table.Indexes.Count) || 
                (ForeignKeys.Count != table.ForeignKeys.Count)))
            {
                // Ситуация, которая не должна возникать в обычном пользовательском режиме. 
                // Может возникать, если не было выполнено обновление.
                Trace.WriteLineIf(xpoSwitch.TraceWarning, string.Format("DBTableEx is changed: {0}", Name), "XPO");
                Update();
            }
        }

        static TraceSwitch xpoSwitch = new TraceSwitch("XPO", "");

        /// <summary>
        /// Констрейнты таблицы
        /// </summary>
        public List<DBConstraintBase> Constraints
        {
            get { return constraints; }
        }

        /// <summary>
        /// Добавить указанный констрейнт
        /// </summary>
        /// <param name="constraint">Добавляемый констрейнт</param>
        public void AddConstraint(DBConstraintBase constraint)
        {
            constraints.Add(constraint);
        }

        /// <summary>
        /// Определяет, должна ли указанная колонка содержать непустое значение
        /// </summary>
        /// <param name="column">Колонка</param>
        /// <returns>True, если колонка является ключом или есть констрейнт <see cref="DBNotNullConstraint"/> для указанной колонки</returns>
        public virtual bool ColumnIsNotNull(DBColumn column)
        {
            return column.IsKey || 
                constraints.Exists(c => c is DBNotNullConstraint && ((DBNotNullConstraint)c).Column.Name == column.Name);
        }

        /// <summary>
        /// Определяет, есть ли запрет на редактирование указанной колонки
        /// </summary>
        /// <param name="column">Колонка</param>
        /// <returns>Значение, указывающее наличие констрейнта <see cref="DBReadOnlyConstraint"/> для указанной колонки</returns>
        public virtual bool ColumnIsReadOnly(DBColumn column)
        {
            return constraints.Exists(c => c is DBReadOnlyConstraint && ((DBReadOnlyConstraint)c).Column.Name == column.Name);
        }

        /// <summary>
        /// Определяет, могут ли указанные колонки быть ссылкой на данную таблицу
        /// </summary>
        /// <param name="columns">Колонки, для которых определяется возможность ссылки на данную таблицу</param>
        /// <returns>True - если указанные колонки могут ссылаться на данную таблицу, false - если у таблицы нет первичного ключа
        /// или число колонок не совпадает с первичным ключом или типы колонок не совпадают с типами колонок первичного ключа</returns>
        public bool CanReferences(DBColumnCollection columns)
        {
            if (table.PrimaryKey == null || columns == null || columns.Count == 0 || table.PrimaryKey.Columns.Count != columns.Count)
                return false;
            for (int i = 0; i < columns.Count; i++)
                if (columns[i].ColumnType != table.GetColumn(table.PrimaryKey.Columns[i]).ColumnType)
                    return false;
            return true;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// Колонка, принадлежащая определенной таблице
    /// </summary>
    public class DBTableColumn : DBColumn
    {
        private DBTableEx table;

        /// <summary>
        /// Таблица, которой принадлежит колонка
        /// </summary>
        public DBTableEx Table { get { return table; } }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="table">Таблица, которой принадлежит колонка</param>
        /// <param name="column">Исходная колонка таблицы</param>
        /// <exception cref="NullReferenceException">Не указана мсходная колонка таблицы</exception>
        /// <exception cref="ArgumentNullException">Не указана таблица, которой принадлежит колонка</exception>
        /// <exception cref="ArgumentException">Указанная колонка <paramref name="column"/> не принадлежит таблице <paramref name="table"/></exception>
        public DBTableColumn(DBTableEx table, DBColumn column)
            : base(column.Name, column.IsKey, column.DBTypeName, column.Size, column.ColumnType)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (table.GetColumn(Name) == null) throw new ArgumentException("column");
            IsIdentity = column.IsIdentity;
            this.table = table;
        }
    }

    /// <summary>
    /// Класс сравнения колонок таблицы по именам
    /// </summary>
    public class DBColumnComparerByNames : IEqualityComparer<DBColumn>
    {
        /// <summary>Статический экземпляр класса сравнения колонок таблицы по именам</summary>
        public static readonly DBColumnComparerByNames Instance = new DBColumnComparerByNames();

        /// <contentfrom cref="M:System.Collections.Generic.IEqualityComparer`1.Equals"/>
        public bool Equals(DBColumn x, DBColumn y) { return string.Equals(x.Name, y.Name); }

        /// <contentfrom cref="M:System.Collections.Generic.IEqualityComparer`1.GetHashCode"/>
        public int GetHashCode(DBColumn obj) { return obj.Name.GetHashCode(); }
    }

    /// <summary>
    /// Базовый класс констрейнтов базы данных
    /// </summary>
    /// <remarks>Констрейнт определяет целостность данных в базе данных, например, констрейнт not null. 
    /// Часть базовых констрейнтов, такие как первичный ключ и ссылки, определены самой таблицей <see cref="DBTable"/>
    /// и не входят список констрейнтов, наследуемых от класса <b>DBConstraint</b></remarks>
    public abstract class DBConstraintBase
    {
    }

    /// <summary>
    /// Констрейнт колонки таблицы
    /// </summary>
    public abstract class DBColumnConstraint : DBConstraintBase
    {
        /// <summary>Колонка таблицы, к которой относится констрейнт</summary>
        public DBColumn Column;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="column">Колонка таблицы</param>
        public DBColumnConstraint(DBColumn column)
        {
            this.Column = column;
        }
    }

    /// <summary>
    /// Колонка таблицы с формулой вычисления данных
    /// </summary>
    public class DBCriteriaColumn : DBColumn
    {
        /// <summary>Формула для вычисления колонки</summary>
        public CriteriaOperator Criteria;

        /// <summary>Конструктор</summary>
        public DBCriteriaColumn() : base() { }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Название колонки</param>
        /// <param name="isKey">Флаг ключевого поля</param>
        /// <param name="dBTypeName">Тип данных в базе данных</param>
        /// <param name="size">Размер колонки</param>
        /// <param name="type">Тип данных (универсальный)</param>
        /// <param name="criteria">Формула расчета данных</param>
        public DBCriteriaColumn(string name, bool isKey, string dBTypeName, int size, DBColumnType type, CriteriaOperator criteria)
            : base(name, isKey, dBTypeName, size, type)
        {
            this.Criteria = criteria;
        }

        /// <summary>
        /// Возвращает копию формулы с установкой алиаса колонок
        /// </summary>
        /// <param name="aliasNode">Алиас колонок</param>
        /// <returns>Формула вычисления данных колонки с использованием указанного алиаса</returns>
        public CriteriaOperator GetCriteria(string aliasNode)
        {
            CriteriaOperator result = CriteriaOperator.Clone(Criteria);
            AliasProcessor.Execute(result, a => aliasNode);
            return result;
        }
    }

    /// <summary>
    /// Констрейнт колонки, указывающий, что колонка не может содержать пустое значение
    /// </summary>
    public class DBNotNullConstraint : DBColumnConstraint
    {
        /// <summary>Конструктор</summary>
        /// <param name="column">Колонка таблицы</param>
        public DBNotNullConstraint(DBColumn column) : base(column) { }
    }

    /// <summary>
    /// Констрейнт колонки, указывающий, что колонка доступна только для чтения
    /// </summary>
    public class DBReadOnlyConstraint : DBColumnConstraint
    {
        /// <summary>Конструктор</summary>
        /// <param name="column">Колонка таблицы</param>
        public DBReadOnlyConstraint(DBColumn column) : base(column) { }
    }

    /// <summary>
    /// Констрейнт таблицы, определяющий последовательность периодов
    /// </summary>
    public abstract class DBPeriodConstraint : DBConstraintBase
    {
        /// <summary>Колонки, определяющие ключ периода</summary>
        public DBColumnCollection PeriodKey;

        /// <summary>Колонка начала периода</summary>
        public DBColumn DateIn;

        /// <summary>Колонка конца периода</summary>
        public DBColumn DateOut;

        /// <summary>Указывает, включено ли начало в период</summary>
        public bool IncludeIn;

        /// <summary>Указывает, включен ли конец в период</summary>
        public bool IncludeOut;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="periodKey">Колонки, определяющие ключ периода</param>
        /// <param name="dateIn">Колонка начала периода</param>
        /// <param name="dateOut">Колонка конца периода</param>
        /// <param name="includeIn">Указывает, включено ли начало в период</param>
        /// <param name="includeOut">Указывает, включен ли конец в период</param>
        protected DBPeriodConstraint(DBColumnCollection periodKey, DBColumn dateIn, DBColumn dateOut, bool includeIn, bool includeOut)
        {
            this.PeriodKey = periodKey;
            this.DateIn = dateIn;
            this.DateOut = dateOut;
            this.IncludeIn = includeIn;
            this.IncludeOut = includeOut;
        }
    }

    /// <summary>
    /// Констрейнт таблицы, определяющий непересекающуюся последовательность периодов
    /// </summary>
    /// <remarks>Непересекающаяся последовательность периодов означает, что периоды этой последовательности не пересекаются между собой</remarks>
    public class DBConsistentPeriodConstraint : DBPeriodConstraint
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="periodKey">Колонки, определяющие ключ периода</param>
        /// <param name="dateIn">Колонка начала периода</param>
        /// <param name="dateOut">Колонка конца периода</param>
        /// <param name="includeIn">Указывает, включено ли начало в период</param>
        /// <param name="includeOut">Указывает, включен ли конец в период</param>
        public DBConsistentPeriodConstraint(DBColumnCollection periodKey, DBColumn dateIn, DBColumn dateOut, bool includeIn, bool includeOut)
            : base(periodKey, dateIn, dateOut, includeIn, includeOut)
        {
        }
    }

    /// <summary>
    /// Констрейнт таблицы, определяющий непрерывную последовательность периодов
    /// </summary>
    /// <remarks>Непрерывная последовательность периодов означает, что между периодами нет перерывов</remarks>
    public class DBContinuousPeriodConstraint : DBPeriodConstraint
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="periodKey">Колонки, определяющие ключ периода</param>
        /// <param name="dateIn">Колонка начала периода</param>
        /// <param name="dateOut">Колонка конца периода</param>
        /// <param name="includeIn">Указывает, включено ли начало в период</param>
        /// <param name="includeOut">Указывает, включен ли конец в период</param>
        public DBContinuousPeriodConstraint(DBColumnCollection periodKey, DBColumn dateIn, DBColumn dateOut, bool includeIn, bool includeOut)
            : base(periodKey, dateIn, dateOut, includeIn, includeOut)
        {
        }
    }

    /// <summary>
    /// Констрейнт таблицы, определяющий иерархию
    /// </summary>
    public abstract class DBHierarchyConstraint : DBConstraintBase
    {
        /// <summary>Колонки, определяющие ссылку на родительскую запись</summary>
        public DBColumnCollection Parent;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="parent">Колонки, определяющие ссылку на родительскую запись</param>
        public DBHierarchyConstraint(DBColumnCollection parent)
        {
            this.Parent = parent;
        }
    }

    /// <summary>
    /// Констрейнт таблицы, определяющий иерархию без циклов
    /// </summary>
    public class DBNoCycleHierarchyConstraint : DBHierarchyConstraint
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="parent">Колонки, определяющие ссылку на родительскую запись</param>
        public DBNoCycleHierarchyConstraint(DBColumnCollection parent)
            : base(parent)
        {
        }
    }

    /// <summary>
    /// Констрейнт таблицы, определяющий уникальность колонок
    /// </summary>
    public class DBUniqueConstraint : DBConstraintBase
    {
        /// <summary>Колонки, определяющие уникальность записей</summary>
        public DBColumnCollection Columns;

        /// <summary>Конструктор</summary>
        /// <param name="columns">Колонки, определяющие уникальность записей</param>
        public DBUniqueConstraint(DBColumnCollection columns) { this.Columns = columns; }
    }

    /// <summary>
    /// Констрейнт таблицы с указанным критерием целостности данных
    /// </summary>
    public class DBCriteriaConstraint : DBConstraintBase
    {
        private CriteriaOperator criteria;
        private string tableAlias;

        /// <summary>Критерий целостности данных</summary>
        public CriteriaOperator Criteria { get { return criteria;  } }

        /// <summary>Алиас таблицы, используемый в выражении критерия</summary>
        public string TableAlias { get { return tableAlias; } }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="criteria">Критерий целостности данных</param>
        public DBCriteriaConstraint(CriteriaOperator criteria)
        {
            this.criteria = criteria;
        }

        /// <summary>
        /// Конструктор с указанием алиаса таблицы
        /// </summary>
        /// <param name="criteria">Критерий целостности данных</param>
        /// <param name="tableAlias">Алиас таблицы, используемый в выражении критерия</param>
        public DBCriteriaConstraint(CriteriaOperator criteria, string tableAlias)
        {
            this.criteria = criteria;
            this.tableAlias = tableAlias;
        }
    }

    /// <summary>
    /// Констрейнт таблицы с указанным запросом целостности данных
    /// </summary>
    /// <remarks>Проверяется только при изменении данных основной таблицы запроса</remarks>
    public class DBSelectConstraint : DBConstraintBase
    {
        private SelectStatement select;
        
        /// <summary>Запрос целостности данных</summary>
        public SelectStatement Select { get { return select; } }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="select">Запрос целостности данных</param>
        public DBSelectConstraint(SelectStatement select)
        {
            this.select = select;
        }
    }

    /// <summary>
    /// Констрейнт таблицы с указанным запросом безопасности данных
    /// </summary>
    public class DBSecurityConstraint : DBConstraintBase
    {
        private SelectStatement select;
        private DBOperations operations;
        
        /// <summary>Запрос безопасности данных</summary>
        public SelectStatement Select { get { return select; } }

        /// <summary>Операции, к которым применяется констрейнт</summary>
        /// <value>Значение по умолчанию <code>DBOperations.AddEdit</code></value>
        public DBOperations Operations { get { return operations; } }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="select">Запрос безопасности данных</param>
        /// <param name="operations">Операции, к которым применяется констрейнт</param>
        public DBSecurityConstraint(SelectStatement select, DBOperations operations)
        {
            this.select = select;
            this.operations = operations;
        }
    }

    /// <summary>
    /// Констрейнт таблицы в базе данных
    /// </summary>
    /// <remarks>Используется для получения метаданных таблицы из базы данных</remarks>
    class DBCheckConstraint : DBConstraintBase
    {
        public string Name;
        public string Condition;
        public DBCheckConstraint(string name, string condition) { Name = name; Condition = condition; }
    }

    /// <summary>
    /// Типы операций с данными
    /// </summary>
    [Flags]
    public enum DBOperations
    {
        /// <summary>Чтение (select)</summary>
        Read = 1,

        /// <summary>Добавление (insert)</summary>
        Add = 2,

        /// <summary>Изменение (update)</summary>
        Edit = 4,

        /// <summary>Удаление (delete)</summary>
        Delete = 8,

        /// <summary>Добавление или изменение</summary>
        AddEdit = Add | Edit 
    }
}
