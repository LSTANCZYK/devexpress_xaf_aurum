using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;

namespace Aurum.Xpo
{
    /// <summary>
    /// Шаблонизатор структуры базы данных Oracle
    /// </summary>
    /// <remarks>Содержит шаблоны и генерирует текст команд для создания основных 
    /// структурных элементов БД Oracle (таблиц, представлений, пакетов и т.д.)</remarks>
    class OracleTemplater
    {
        private ISecuredSqlGeneratorFormatter formatter;
        private DBTable table;
        private DBTableEx tableEx;
        private DBColumn sequenceColumn;
        private DBColumn gcRecordColumn;
        private DBColumnCollection defaultKey;
        
        /// <summary>Имена стандартных процедур базы данных</summary>
        public static Dictionary<StoredProcedureTypes, string> StoredProcedureNames;

        /// <summary>Колонка rowid</summary>
        public static readonly DBColumn ColumnRowid;

        #region Символы разделителей
        
        /// <summary>Пробел</summary>
        public const string Space = " ";

        /// <summary>Двойной пробел</summary>
        public const string DoubleSpace = "  ";

        /// <summary>2-кратный двойной пробел</summary>
        public const string DoubleSpace2 = "    ";

        /// <summary>3-кратный двойной пробел</summary>
        public const string DoubleSpace3 = "      ";

        /// <summary>Разделитель строк (колонок, параметров) со знаком &quot;,&quot; и переносом строки</summary>
        public const string StringsDelimiter = ",\r\n  ";

        /// <summary>Разделитель строк (колонок, параметров) со знаком &quot;,&quot; и пробелом</summary>
        public const string StringsDelimiterInline = ", ";

        /// <summary>Разделитель строк текста с отступом в 2 пробела</summary>
        public const string LinesDelimiter = "\r\n  ";

        /// <summary>Окончание инструкции</summary>
        public const string InstructionEnd = ";";

        /// <summary>Открытая скобка</summary>
        public const string ParenthesisOpen = "(";

        /// <summary>Закрытая скобка</summary>
        public const string ParenthesisClose = ")";

        #endregion

        #region Зарезервированные слова, операторы и константы

        /// <summary>Null</summary>
        public const string Null = "null";

        /// <summary>Констрейнт Not null</summary>
        public const string NotNull = "not null";

        /// <summary>Направление исходящего параметра процедуры</summary>
        public const string Out = "out";

        /// <summary>Зарезервированное слово rowid</summary>
        public const string Rowid = "rowid";

        /// <summary>Следующее значение sequence со знаком &quot;.&quot;</summary>
        public const string SequenceNextval = ".nextval";
        
        /// <summary>Условие and</summary>
        public const string And = "and";

        /// <summary>Условие and с пробелами</summary>
        public const string AndSpace = Space + And + Space;

        /// <summary>Условие or</summary>
        public const string Or = "or";

        /// <summary>Условие or с пробелами</summary>
        public const string OrSpace = Space + Or + Space;

        /// <summary>Условие not</summary>
        public const string ConditionNot = "not";

        /// <summary>Условие is null</summary>
        public const string IsNull = "is null";

        /// <summary>Условие is not null</summary>
        public const string IsNotNull = "is not null";

        /// <summary>Оператор конкатинации строк</summary>
        public const string ConcatOperator = "||";

        /// <summary>Значение, равное одной секунде</summary>
        public const string OneSecondValue = "1/86400";

        /// <summary>Параметр флагов установки значений</summary>
        public const string SetValueFlagsParameter = "SetValueFlags";

        /// <summary>Знак присваивания</summary>
        public const string Assignment = ":=";

        #endregion

        /// <summary>Шаблон создания таблицы: 0 - название таблицы, 1 - колонки</summary>
        public const string CreateTableTemplate = "create table {0} (\r\n  {1}\r\n)";

        /// <summary>Шаблон создания сиквенса: 0 - название сиквенса</summary>
        public const string CreateSequenceTemplate = "create sequence {0} start with 1 increment by 1 nocache";

        /// <summary>Шаблон создания представления: 0 - название представления, 1 - запрос</summary>
        public const string CreateViewTemplate = "create or replace view {0} as \r\n{1}\r\nwith read only";

        /// <summary>Шаблон создания констрейнта: 0 - название таблицы, 1 - название констрейнта, 2 - выражение</summary>
        public const string CreateConstraintTemplate = "alter table {0} add constraint {1} check ({2})";

        /// <summary>Шаблон создания пакета: 0 - название пакета, 1 - определения процедур, 2 - имплементация процедур</summary>
        public const string CreatePackageTemplate =
@"create or replace package {0} as
-- Autocreation

{1}

end;
/

create or replace package body {0} as

{2}

end;
/";
        
        /// <summary>Шаблон создания публичной процедуры: 0 - название публичной процедуры с параметрами, 1 - вызов частной процедуры</summary>
        public const string CreatePublicProcedureTemplate = "create or replace procedure {0} \r\nas begin {1}; end;\r\n/";

        /// <summary>Шаблон процедуры: 0 - название процедуры, 1 - параметры, 2 - переменные, 3 - инструкции</summary>
        public const string ProcedureTemplate = 
@"procedure {0}{1} as
{2}begin
  {3}
end;";

        /// <summary>Шаблон инструкции добавления записи: 0 - название таблицы, 1 - поля, 2 - значения</summary>
        public const string InsertInstructionTemplate = "insert into {0} ({1})\r\n  values({2})";

        /// <summary>Шаблон инструкции изменения записи: 0 - название таблицы, 1 - присваивание значений, 2 - обязательное условие</summary>
        public const string UpdateInstructionTemplate = "update {0} set\r\n    {1}\r\n  where {2}";

        /// <summary>Шаблон инструкции удаления записи: 0 - название таблицы, 1 - обязательное условие</summary>
        public const string DeleteInstructionTemplate = "delete from {0} where {1}";

        /// <summary>Шаблон возврата значений при добавлении или изменении записи: 0 - поля, 1 - возвращаемые значения</summary>
        public const string ReturningTemplate = "returning {0} into {1}";

        /// <summary>Шаблон присваивания или проверки равенства колонки и значения: 0 - колонка, 1 - значение</summary>
        public const string EqualTemplate = "{0} = {1}";

        /// <summary>Шаблон проверки равенства колонки и значения с учетом пустых значений null: 0 - колонка, 1 - значение</summary>
        public const string EqualNullableTemplate = "({0} = {1} or ({0} is null and {1} is null))";

        /// <summary>Шаблон проверки неравенства колонки и значения: 0 - колонка, 1 - значение</summary>
        public const string InequalTemplate = "{0} <> {1}";

        /// <summary>Шаблон получения значений полей в переменные: 0 - название таблицы, 1 - поля, 2 - переменные, 3 - условие</summary>
        public const string SelectVarsTemplate = "select {1} into {2}\r\n  from {0} where {3}";

        /// <summary>Шаблон установки значения с возможностью пропуска: 0 - колонка, 1 - значение, 2 - параметр с битами полей, 3 - бит поля</summary>
        public const string SetValueOrSkip = "{0} = decode(bitand({3},{2}),{3},{1},{0})";

        /// <summary>Шаблон соединения таблиц с обязательным условием: 0 - первая таблица, 1 - вторая таблица, 2 - условие соединения</summary>
        public const string InnerJoinTemplate = "{0} inner join {1} on {2}";

        /// <summary>Шаблон инструкции условия: 0 - условие, 1 - инструкции</summary>
        public const string IfThenTemplate = "if {0} then {1} end if;";

        /// <summary>Максимальное количество бит в типе number (безопасное значение)</summary>
        public const int MaxBitsInNumber = 30;

        /// <summary>Шаблон инструкции проверки количества измененных или удаленных записей: 0 - название таблицы, 1 - ключ</summary>
        public const string CheckRowCountInstructionTemplate = 
@"if sql%rowcount <> 1 then 
    raise_application_error(-20001, 'Record {0}('||{1}||') is not found');
  end if";

        /// <summary>Шаблон констрейнта непересекающейся последовательности периодов: 
        /// 0 - название таблицы, 1 - DateIn, 2 - DateOut, 3 - условие ключа периода, 4 - +1sec, 5 - '='</summary>
        public const string ConsistentPeriodConstraintTemplate =
@"vDateIn date;
  vDateOut date;
  vFirst boolean := true;
begin
  for q in (
    select {1} datein, {2} dateout 
    from {0} 
    where {3}
    order by {1} nulls first)
  loop
    if vFirst then
      vFirst := false;
    elsif (vDateIn{4} <{5} q.dateout or vDateIn is null or q.dateout is null) and
          (q.datein{4} <{5} vDateOut or q.datein is null or vDateOut is null) then
      raise_application_error(-20001, 'Period of {0} is not consistent: '||vDateIn||' - '||vDateOut);
    end if;
    vDateIn := q.datein; vDateOut := q.dateout;
  end loop;
end;";

        /// <summary>Шаблон констрейнта непрерывной последовательности периодов: 
        /// 0 - название таблицы, 1 - DateIn, 2 - DateOut, 3 - условие ключа периода, 4 - +1sec, 5 - '='</summary>
        public const string ContinuousPeriodConstraintTemplate =
@"vDateOut date;
  vFirst boolean := true;
begin
  for q in (
    select {1} datein, {2} dateout 
    from {0} 
    where {3}
    order by {1} nulls first)
  loop
    exit when q.dateout is null;
    if vFirst then
      vDateOut := q.dateout;
      vFirst := false;
    elsif (vDateOut <{5} q.datein{4}) then
      raise_application_error(-20001, 'Period of {0} is not continuous: '||q.datein||' - '||vDateOut);
    end if;
    if q.dateout > vDateOut then vDateOut := q.dateout; end if;
  end loop;
end;";

        /// <summary>Шаблон констрейнта иерархии без циклов: 0 - название таблицы, 1 - условие ключа, 2 - условие иерархии, 3 - дополнительные условия</summary>
        public const string NoCycleHierarchyConstraintTemplate =
@"vCycle number;
begin
  select max(connect_by_iscycle) into vCycle 
  from {0} {3}
  connect by nocycle {2}
  start with {1};
  if vCycle = 1 then
    raise_application_error(-20001, 'Hierarchy of {0} has cycles');
  end if;
end;";

        /// <summary>Шаблон констрейнта уникальности: 0 - название таблицы, 1 - условие ключа</summary>
        public const string UniqueConstraintTemplate =
@"vCount number;
begin
  select count(*) into vCount
  from {0}
  where {1};
  if vCount > 1 then
    raise_application_error(-20001, 'Uniqueness of {0} is breaked');
  end if;
end;";

        /// <summary>Шаблон констрейнта запроса: 0 - название таблицы, 1 - название констрейнта, 2 - запрос</summary>
        public const string SelectConstraintTemplate =
@"vTemp number;
begin
  {2};
exception when no_data_found or too_many_rows then
  raise_application_error(-20001, 'Constraint ""{1}"" of {0} is not executed');
end;";

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="formatter">Представитель форматирования sql-запросов с безопасным доступом к данным</param>
        /// <param name="table">Таблица, для которой создаются инструкции</param>
        public OracleTemplater(ISecuredSqlGeneratorFormatter formatter, DBTable table)
        {
            this.formatter = formatter;
            this.table = table;
            this.tableEx = table as DBTableEx;
            this.TableName = GetTableName(table);
            this.sequenceColumn = GetTableSequenceColumn(table);
            this.gcRecordColumn = GetActiveRecordsColumn(table);
            this.GCRecordTableName = gcRecordColumn != null ? GetTableName(gcRecordColumn) : null;
            this.defaultKey = GetDefaultKey(table);
        }

        static OracleTemplater()
        {
            StoredProcedureNames = new Dictionary<StoredProcedureTypes, string>();
            StoredProcedureNames.Add(StoredProcedureTypes.Add, "Add");
            StoredProcedureNames.Add(StoredProcedureTypes.Edit, "Edit");
            StoredProcedureNames.Add(StoredProcedureTypes.Delete, "Del");
            ColumnRowid = new DBColumn(Rowid, true, Rowid, 0, DBColumnType.Unknown);
        }

        /// <summary>
        /// Возвращает название таблицы
        /// </summary>
        /// <param name="table">Таблица</param>
        /// <returns>Название таблицы в формате команды БД</returns>
        protected string GetTableName(DBTable table)
        {
            return formatter.FormatTable(formatter.ComposeSafeSchemaName(table.Name), formatter.ComposeSafeTableName(table.Name));
        }

        /// <summary>
        /// Возвращает название таблицы указанной колонки
        /// </summary>
        /// <param name="column">Колонка</param>
        /// <returns>Название таблицы в формате команды БД колонки <paramref name="column"/></returns>
        protected string GetTableName(DBColumn column)
        {
            return column is DBTableColumn ? GetTableName(((DBTableColumn)column).Table) : TableName;
        }

        /// <summary>
        /// Получить ключ по умолчанию для указанной таблицы
        /// </summary>
        /// <param name="table">Таблица</param>
        /// <returns>Ключ таблицы или rowid, если у таблицы нет ключа</returns>
        protected DBColumnCollection GetDefaultKey(DBTable table)
        {
            DBColumnCollection key = new DBColumnCollection();
            if (table.PrimaryKey == null)
                key.Add(ColumnRowid);
            else
                foreach (string keyName in table.PrimaryKey.Columns)
                    key.Add(table.GetColumn(keyName));
            return key;
        }

        /// <summary>
        /// Возвращает название последовательности для указанной таблицы
        /// </summary>
        /// <param name="table">Таблица</param>
        /// <returns>Название последовательности для указанной таблицы</returns>
        protected string GetSequenceName(DBTable table)
        {
            return formatter.FormatSequence(formatter.ComposeSafeSchemaName(table.Name), formatter.ComposeSafeTableName(table.Name));
        }

        /// <summary>
        /// Получить колонку последовательности для указанной таблицы
        /// </summary>
        /// <param name="table">Таблица</param>
        /// <returns>Возвращает колонку ключа, если есть ключ с одной колонкой, автоинкрементом и целочисленного типа</returns>
        public static DBColumn GetTableSequenceColumn(DBTable table)
        {
            if (table.PrimaryKey == null) return null;
            DBColumn key = table.GetColumn(table.PrimaryKey.Columns[0]);
            return (key.IsKey && key.IsIdentity && table.PrimaryKey.Columns.Count == 1 &&
                (key.ColumnType == DBColumnType.Int32 || key.ColumnType == DBColumnType.Int64)) ? key : null;
        }

        /// <summary>Название таблицы в формате команды БД</summary>
        protected readonly string TableName;

        /// <summary>Название таблицы, содержащей колонку логического удаления 
        /// <see cref="DevExpress.Xpo.DeferredDeletionAttribute"/>, в формате команды БД</summary>
        protected readonly string GCRecordTableName;

        /// <summary>
        /// Возвращает название колонки
        /// </summary>
        /// <param name="column">Колонка</param>
        /// <returns>Название колонки в формате команды БД</returns>
        protected string ColumnName(DBColumn column)
        {
            return formatter.FormatColumn(formatter.ComposeSafeColumnName(column.Name));
        }

        /// <summary>
        /// Возвращает название колонки с указанным алиасом таблицы
        /// </summary>
        /// <param name="column">Колонка</param>
        /// <param name="alias">Алиас таблицы, которой принадлежит колонка</param>
        /// <returns>Название колонки в формате команды БД с учетом указанного алиаса</returns>
        protected string ColumnName(DBColumn column, string alias)
        {
            string result = ColumnName(column);
            return string.IsNullOrEmpty(alias) ? result : string.Concat(alias, ".", result);
        }

        /// <summary>
        /// Возвращает название колонки с алиасом из указанного справочника алиасов
        /// </summary>
        /// <param name="column">Колонка</param>
        /// <param name="aliases">Справочник алиасов, из которого нужно брать алиас колонки <paramref name="column"/></param>
        /// <returns>Название колонки в формате команды БД с учетом алиаса из указанного справочника алиасов</returns>
        protected string ColumnName(DBColumn column, AliasDictionary aliases)
        {
            string alias = aliases != null ? aliases.Find(GetTableName(column)) : null;
            return ColumnName(column, alias);
        }

        /// <summary>
        /// Определяет, запрещены ли для указанной колонки пустые значения
        /// </summary>
        /// <param name="column">Колонка, для которой определяется возможность пустых значений</param>
        /// <returns>True - если колонка не может содержать пустых значений, false - если могут быть пустые значения</returns>
        protected bool ColumnIsNotNull(DBColumn column)
        {
            return column is DBTableColumn ? ((DBTableColumn)column).Table.ColumnIsNotNull(column) : 
                tableEx != null && tableEx.ColumnIsNotNull(column);
        }

        /// <summary>
        /// Возвращает название представления
        /// </summary>
        /// <param name="table">Таблица</param>
        /// <returns>Название представления в формате команды БД</returns>
        protected string ViewName(DBTable table)
        {
            return formatter.FormatView(formatter.ComposeSafeSchemaName(table.Name), formatter.ComposeSafeTableName(table.Name));
        }

        /// <summary>
        /// Возвращает название пакета
        /// </summary>
        /// <param name="table">Таблица</param>
        /// <returns>Название пакета в формате команды БД</returns>
        protected string PackageName(DBTable table)
        {
            return formatter.FormatPackage(formatter.ComposeSafeSchemaName(table.Name), formatter.ComposeSafeTableName(table.Name));
        }

        /// <summary>
        /// Возвращает название параметра процедуры
        /// </summary>
        /// <param name="column">Колонка</param>
        /// <returns>Название параметра процедуры в формате команды БД</returns>
        protected string ParameterName(DBColumn column)
        {
            return formatter.FormatProcedureParameter(column.Name);
        }

        /// <summary>
        /// Определяет, включен ли параметр в стандартной процедуре для указанной колонки
        /// </summary>
        /// <param name="procedureType">Тип стандартной процедуры</param>
        /// <param name="column">Колонка, для которой проверяется наличие параметра</param>
        /// <returns>True, если параметр для указанной колонки включен в стандартную процедуру указанного типа</returns>
        protected bool ParameterIncluded(StoredProcedureTypes procedureType, DBColumn column)
        {
            if (column == ColumnRowid)
                return table.PrimaryKey == null && procedureType != StoredProcedureTypes.Add;
            if (column.IsKey)
                return true;
            if (procedureType == StoredProcedureTypes.Delete) 
                return false;
            bool isLocal = !(column is DBTableColumn);
            if (procedureType == StoredProcedureTypes.Add)
                return isLocal;
            return isLocal && (tableEx == null || !tableEx.ColumnIsReadOnly(column));
        }

        /// <summary>
        /// Определяет, является ли параметр исходящим в стандартной процедуре для указанной колонки
        /// </summary>
        /// <param name="procedureType">Тип стандартной процедуры</param>
        /// <param name="column">Колонка, для которой проверяется направление параметра</param>
        /// <returns>True, если параметр является исходящим</returns>
        protected bool ParameterOut(StoredProcedureTypes procedureType, DBColumn column)
        {
            if (procedureType == StoredProcedureTypes.Add && column == sequenceColumn)
                return true;
            return false;
        }

        /// <summary>
        /// Возвращает значение параметра добавляемой или изменяемой записи для стандартной процедуры
        /// </summary>
        /// <param name="procedureType">Тип стандартной процедуры</param>
        /// <param name="column">Колонка, для которой вычисляется значение параметра</param>
        /// <returns>Текст значения параметра добавляемой или изменяемой записи, по умолчанию сам параметр</returns>
        protected string ParameterValue(StoredProcedureTypes procedureType, DBColumn column)
        {
            return ParameterName(column);
        }

        /// <summary>
        /// Возвращает название переменной для указанной колонки
        /// </summary>
        /// <param name="column">Колонка, для которой вычисляется название переменной</param>
        /// <returns>Название переменной для указанной колонки</returns>
        protected string VariableName(DBColumn column)
        {
            return string.Concat("v", column.Name.Length <= 29 ? column.Name : formatter.ComposeSafeColumnName(column.Name));
        }

        /// <summary>
        /// Возвращает тип переменной для указанной колонки
        /// </summary>
        /// <param name="column">Колонка, для которой вычисляется тип переменной</param>
        /// <returns>Тип переменной для указанной колонки</returns>
        protected string VariableType(DBColumn column)
        {
            return column == ColumnRowid ? Rowid : 
                string.Format("{0}.{1}%type", GetTableName(column), ColumnName(column));
        }

        /// <summary>
        /// Возвращает строку колонок с указанным форматированием
        /// </summary>
        /// <param name="columns">Колонки</param>
        /// <param name="format">Формат строки для каждой колонки</param>
        /// <param name="delimiter">Разделитель колонок</param>
        /// <returns>Строка колонок с указанным форматированием, разделенных указанным разделителем</returns>
        protected string ColumnsConcat(IEnumerable<DBColumn> columns, Func<DBColumn, string> format, string delimiter)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DBColumn column in columns)
            {
                string s = format(column);
                if (string.IsNullOrEmpty(s)) continue;
                if (sb.Length > 0) sb.Append(delimiter);
                sb.Append(s);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Возвращает строку колонок с указанным форматированием
        /// </summary>
        /// <param name="format">Формат строки для каждой колонки</param>
        /// <param name="delimiter">Разделитель колонок</param>
        /// <returns>Строка колонок с указанным форматированием, разделенных указанным разделителем</returns>
        protected string ColumnsConcat(Func<DBColumn, string> format, string delimiter)
        {
            return ColumnsConcat(table.Columns, format, delimiter);
        }

        /// <summary>
        /// Возвращает строку колонок с указанным форматированием
        /// </summary>
        /// <param name="format">Формат строки для каждой колонки</param>
        /// <returns>Строка колонок с указанным форматированием, разделенных стандартным разделителем строк <see cref="StringsDelimiter"/></returns>
        protected string ColumnsConcat(Func<DBColumn, string> format)
        {
            return ColumnsConcat(table.Columns, format, StringsDelimiter);
        }

        /// <summary>
        /// Возвращает строку колонок и параметров ключа с указанным форматированием для каждой колонки ключа
        /// </summary>
        /// <param name="format">Формат строки для каждой пары колонки и параметра ключа, где 0 - колонка ключа, 1 - параметр ключа</param>
        /// <param name="delimiter">Разделитель строк</param>
        /// <returns>Строка колонок и параметров ключа с указанным форматированием</returns>
        protected string DefaultKeyConcat(string format, string delimiter)
        {
            return ColumnsConcat(defaultKey, col => string.Format(format, ColumnName(col), ParameterName(col)), delimiter);
        }

        /// <summary>
        /// Возвращает колонку логического удаления указанной таблицы
        /// </summary>
        /// <param name="table">Таблица, для которой нужно вернуть колонку логического удаления</param>
        /// <returns>Колонка логического удаления таблицы <paramref name="table"/> или null, если колонка отсуствует</returns>
        protected DBColumn GetActiveRecordsColumn(DBTable table)
        {
            DBColumn result = table.GetColumn(DevExpress.Xpo.Metadata.Helpers.GCRecordField.StaticName);
            if (result == null && table is DBTableEx)
            {
                DBTableEx parent = ((DBTableEx)table).ParentTable;
                while (parent != null && result == null)
                {
                    result = parent.GetColumn(DevExpress.Xpo.Metadata.Helpers.GCRecordField.StaticName);
                    if (result != null) result = new DBTableColumn(parent, result); else parent = parent.ParentTable;
                }
            }
            return result;
        }

        /// <summary>
        /// Возвращает условие активных (неудаленных) записей
        /// </summary>
        /// <param name="alias">Алиас таблицы, содержащей колонку отложенного удаления</param>
        /// <returns>Условие активных записей, если таблица имеет колонку логического удаления <see cref="DevExpress.Xpo.DeferredDeletionAttribute"/>, иначе null</returns>
        /// <remarks>Если алиас не указан, то условие возвращается только в случае принадлежности колонки текущей таблице</remarks>
        protected string GetActiveRecordsCondition(string alias)
        {
            return gcRecordColumn != null && !(string.IsNullOrEmpty(alias) && gcRecordColumn is DBTableColumn) ? 
                string.Concat(ColumnName(gcRecordColumn, alias), Space, IsNull) : null;
        }

        /// <summary>
        /// Возвращает объединение указанных условий
        /// </summary>
        /// <param name="conditions">Условия, которые необходимо объединить в одно</param>
        /// <returns>Условие, объединяющее указанные в <paramref name="conditions"/>.</returns>
        protected string JoinConditions(params string[] conditions)
        {
            return string.Join(AndSpace, conditions.Where(c => !string.IsNullOrEmpty(c)).ToArray());
        }

        /// <summary>
        /// Возвращает соединение таблиц из указанного справочника алиасов
        /// </summary>
        /// <param name="aliases">Справочник алиасов таблиц, для которых требуется выражение соединения</param>
        /// <returns>Соединение таблиц, содержащихся в справочнике алиасов <see cref="TableName"/></returns>
        protected string GetTableFrom(AliasDictionary aliases)
        {
            StringBuilder result = new StringBuilder();
            string first = null;
            foreach (string table in aliases.Keys)
            {
                string alias = aliases[table];
                if (result.Length == 0)
                {
                    result.Append(table + Space + alias);
                    first = alias;
                }
                else
                {
                    string on = ColumnsConcat(defaultKey, 
                        col => string.Format(EqualTemplate, ColumnName(col, alias), ColumnName(col, first)), AndSpace);
                    result.AppendFormat(InnerJoinTemplate, null, table + Space + alias, on);
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Возвращает команду создания таблицы
        /// </summary>
        /// <param name="columnDefinition">Функция получения типа колонки</param>
        /// <returns>Команда создания таблицы</returns>
        public string CreateTableCommand(Func<DBColumn, string> columnDefinition)
        {
            return string.Format(CultureInfo.InvariantCulture, CreateTableTemplate, TableName,
                ColumnsConcat(col => string.Concat(ColumnName(col), Space, columnDefinition(col))));
        }

        /// <summary>
        /// Возвращает команду создания сиквенса
        /// </summary>
        /// <param name="sequenceName">Название сиквенса</param>
        /// <returns>Команда создания сиквенса</returns>
        public string CreateSequenceCommand(string sequenceName)
        {
            return string.Format(CultureInfo.InvariantCulture, CreateSequenceTemplate, sequenceName);
        }

        /// <summary>
        /// Возвращает команду создания представления
        /// </summary>
        /// <returns>Команда создания представления</returns>
        public string CreateViewCommand()
        {
            // Объединение условий безопасности данных
            SelectStatement select = new SelectStatement(table, null);
            JoinNodeCollection nodes = new JoinNodeCollection();
            GroupOperator condition = new GroupOperator();
            int sequrityIndex = 0;
            if (tableEx != null)
                foreach (DBConstraintBase constraint in tableEx.Constraints)
                    if (constraint is DBSecurityConstraint)
                    {
                        DBSecurityConstraint sequrity = (DBSecurityConstraint)constraint;
                        if ((sequrity.Operations & DBOperations.Read) != DBOperations.Read) continue;
                        if (select.Alias == null) select.Alias = sequrity.Select.Alias;
                        AliasProcessor.Execute(sequrity.Select, a => 
                            a == sequrity.Select.Alias ? select.Alias : string.Format("{0}r{1}", a, sequrityIndex));
                        if (sequrity.Select.SubNodes != null)
                            nodes.AddRange(sequrity.Select.SubNodes);
                        if (!ReferenceEquals(sequrity.Select.Condition, null))
                            condition.Operands.Add(sequrity.Select.Condition);
                        sequrityIndex++;
                    }

            // Итоговый запрос и представление
            select.Operands.AddRange(table.Columns.Select(col => new QueryOperand(col, select.Alias)));
            if (nodes.Count > 0) select.SubNodes = nodes;
            if (condition.Operands.Count > 0) select.Condition = condition;
            string sql = new SelectSqlGeneratorBase(formatter, false, null, false, null).GenerateSql(select).Sql;
            return string.Format(CultureInfo.InvariantCulture, CreateViewTemplate, ViewName(table), sql);
        }

        /// <summary>
        /// Возвращает строку создания констрейнта
        /// </summary>
        /// <param name="constraintName">Название констрейнта</param>
        /// <param name="expression">Выражение</param>
        /// <returns>Команда создания констрейнта</returns>
        public string CreateConstraintCommand(string constraintName, string expression)
        {
            return string.Format(CultureInfo.InvariantCulture, CreateConstraintTemplate,
                TableName, constraintName, expression);
        }

        /// <summary>
        /// Возвращает команду создания пакета
        /// </summary>
        /// <param name="package">Название пакета</param>
        /// <param name="publicProcedures">Публичные процедуры</param>
        /// <param name="privateProcedures">Частные процедуры</param>
        /// <returns>Команда создания пакета</returns>
        public string CreatePackageCommand(string package, string[] publicProcedures, string[] privateProcedures)
        {
            // Определения публичных процедур
            string defs = string.Join("\r\n\r\n", publicProcedures.Select(p => {
                return ProcedureHeader(p) + ";"; 
            }));

            // Список всех процедур
            List<string> procedures = new List<string>();
            if (privateProcedures != null)
                procedures.AddRange(privateProcedures);
            procedures.AddRange(publicProcedures);

            // Пакет
            return string.Format(CultureInfo.InvariantCulture, CreatePackageTemplate,
                package, defs, string.Join("\r\n\r\n", procedures));
        }

        /// <summary>
        /// Возвращает заголовок процедуры с параметрами
        /// </summary>
        /// <param name="procedure">Полный текст процедуры</param>
        /// <returns>Заголовок процедуры, включая параметры</returns>
        protected string ProcedureHeader(string procedure)
        {
            int asis = procedure.IndexOf(" as");
            if (asis < 0) asis = procedure.IndexOf(" is");
            return procedure.Substring(0, asis);
        }

        /// <summary>
        /// Получить текст процедуры в формате команды БД
        /// </summary>
        /// <param name="procedure">Название процедуры</param>
        /// <param name="parameters">Параметры (без скобок)</param>
        /// <param name="vars">Переменные</param>
        /// <param name="instructions">Инструкции</param>
        /// <returns>Текст процедуры в формате команды БД</returns>
        public string ProcedureFormat(string procedure, string parameters, string vars, string instructions)
        {
            return string.Format(ProcedureTemplate, procedure, parameters, vars, instructions);
        }

        /// <summary>
        /// Возвращает строку инструкций
        /// </summary>
        /// <param name="instructions">Инструкции</param>
        /// <returns>Строка инструкций, разделенных стандартным разделителем <see cref="LinesDelimiter"/>.</returns>
        /// <remarks>Окончание для каждой инструкции <see cref="InstructionEnd"/> подставляется автоматически, если его нет.
        /// Пустые инструкции исключаются.</remarks>
        public string InstructionsConcat(IEnumerable<string> instructions)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string instruction in instructions)
            {
                if (string.IsNullOrEmpty(instruction)) continue;
                if (sb.Length > 0) sb.Append(LinesDelimiter);
                sb.Append(instruction);
                if (!instruction.EndsWith(InstructionEnd))
                    sb.Append(InstructionEnd);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Возвращает строку параметров стандартной процедуры
        /// </summary>
        /// <param name="type">Тип стандартной процедуры</param>
        /// <param name="parameterType">Функция, возвращающая тип для параметра соответствующей колонки</param>
        /// <param name="mode">Режим описания параметров</param>
        /// <returns>Строка параметров стандартной процедуры</returns>
        protected string GetStandartProcedureParameters(StoredProcedureTypes type, Func<DBColumn, string> parameterType, ParametersModes mode)
        {
            List<DBColumn> parametersColumns = new List<DBColumn>();
            parametersColumns.Add(ColumnRowid);
            parametersColumns.AddRange(table.Columns);
            StringBuilder parameters = new StringBuilder();
            int setValuesCount = 0;
            bool declareMode = mode != ParametersModes.PackageCall;
            foreach (DBColumn col in parametersColumns)
            {
                if (!ParameterIncluded(type, col)) continue;
                if (parameters.Length > 0) 
                    parameters.Append(declareMode ? StringsDelimiter : StringsDelimiterInline);
                parameters.Append(ParameterName(col));
                if (declareMode)
                {
                    parameters.Append(Space);
                    if (ParameterOut(type, col))
                        parameters.Append(Out + Space);
                    parameters.Append(parameterType(col));
                    
                    // Поля таблицы, принадлежащие наследованным классам, могут быть неизвестны на момент вызова,
                    // поэтому, чтобы не было ошибки несоответствия количества параметров при вызове, добавляется значение по умолчанию
                    if (mode == ParametersModes.ProcedureDeclare && tableEx != null && !tableEx.ColumnIsNotNull(col))
                        parameters.Append(Space + Assignment + Space + Null);
                }
                if (!col.IsKey) setValuesCount++;
            }
            if (type == StoredProcedureTypes.Edit)
            {
                int reminder, groupCount = Math.DivRem(setValuesCount, MaxBitsInNumber, out reminder);
                for (int group = 0; group <= groupCount; group++)
                {
                    if (parameters.Length > 0)
                        parameters.Append(declareMode ? StringsDelimiter : StringsDelimiterInline);
                    parameters.Append(SetValueFlagsParameter + (group == 0 ? string.Empty : group.ToString()));
                    if (declareMode)
                        parameters.AppendFormat(" number := {0}", 
                            Convert.ToInt32(Math.Pow(2, group == groupCount ? reminder : MaxBitsInNumber) - 1));
                }
            }
            if (parameters.Length > 0)
            {
                parameters.Insert(0, ParenthesisOpen);
                if (declareMode) parameters.Insert(1, LinesDelimiter);
                if (declareMode) parameters.Append("\r\n");
                parameters.Append(ParenthesisClose);
            }
            return parameters.ToString();
        }

        /// <summary>
        /// Возвращает текст стандартной процедуры с указанными параметрами и основными инструкциями
        /// </summary>
        /// <param name="type">Тип стандартной процедуры</param>
        /// <param name="parameterType">Функция, возвращающая тип для параметра соответствующей колонки</param>
        /// <param name="checks">Процедуры проверки</param>
        /// <returns>Текст процедуры в формате команды БД</returns>
        public string CreateStandartProcedureCommand(StoredProcedureTypes type, Func<DBColumn, string> parameterType, List<CheckProcedure> checks)
        {
            // Название процедуры
            string procedureName = StoredProcedureNames[type];
            List<string> instructions = new List<string>();

            // Параметры
            string parameters = GetStandartProcedureParameters(type, parameterType, ParametersModes.PackageDeclare);

            // Переменные
            List<DBColumn> varsOld = new List<DBColumn>(), varsNew = new List<DBColumn>();
            bool checkOld = false, checkNew = false;
            foreach (CheckProcedure check in checks)
            {
                if (!check.IsCalling(type)) continue;
                if (check.IsCalling(type, CheckProcedureCallValues.OldValues)) { checkOld |= true; varsOld.AddRange(check.Columns); };
                if (check.IsCalling(type, CheckProcedureCallValues.NewValues)) { checkNew |= true; varsNew.AddRange(check.Columns); };
            }
            varsOld = new List<DBColumn>(varsOld.Distinct());
            varsNew = new List<DBColumn>(varsNew.Distinct());
            varsOld.RemoveAll(var => var.IsKey && ParameterIncluded(type, var));
            varsNew.RemoveAll(var => ParameterIncluded(type, var));
            List<DBColumn> vars = new List<DBColumn>(varsOld.Union(varsNew));
            string varsDefinition = vars.Count > 0 ? DoubleSpace +
                ColumnsConcat(vars, var => VariableName(var) + Space + VariableType(var), ";" + LinesDelimiter) + ";\r\n" : null;

            // Старые значения записи и вызов процедур проверок перед базовой инструкцией
            if (checkOld && type != StoredProcedureTypes.Add)
            {
                instructions.Add(CreateSelectVarsInstruction(varsOld));
                foreach (CheckProcedure check in checks)
                    if (check.IsCalling(type, CheckProcedureCallOrder.Before, CheckProcedureCallValues.OldValues))
                    {
                        // Инструкция перед логическим удалением записи
                        string callCheck = CreateCheckProcedureCallInstruction(check, varsOld);
                        if ((check.CallType & CheckProcedureCallTypes.OnSetDeleted) != 0 &&
                            (check.CallType & CheckProcedureCallTypes.OnEditValues) == 0 &&
                            gcRecordColumn != null && ParameterIncluded(type, gcRecordColumn))
                        {
                            callCheck = string.Format(IfThenTemplate, 
                                ParameterName(gcRecordColumn) + Space + IsNotNull, 
                                callCheck + (!callCheck.EndsWith(InstructionEnd) ? InstructionEnd : null));
                        }
                        instructions.Add(callCheck);
                    }
            }

            // Базовая инструкция
            string baseInstruction = null; bool update = true;
            switch (type)
            {
                case StoredProcedureTypes.Add: baseInstruction = CreateStandartInsertInstruction(varsNew); break;
                case StoredProcedureTypes.Edit: baseInstruction = CreateStandartUpdateInstruction(out update); break;
                case StoredProcedureTypes.Delete: baseInstruction = CreateStandartDeleteInstruction(); break;
            }
            instructions.Add(baseInstruction);

            // Проверка количества измененных записей
            if (type != StoredProcedureTypes.Add && update)
                instructions.Add(CreateCheckRowCountInstruction());

            // Вызов процедур проверок со старыми значениями
            if (checkOld && type != StoredProcedureTypes.Add)
                foreach (CheckProcedure check in checks)
                    if (check.IsCalling(type, CheckProcedureCallOrder.After, CheckProcedureCallValues.OldValues))
                        instructions.Add(CreateCheckProcedureCallInstruction(check, varsOld));

            // Вызов процедур проверок с новыми значениями
            if (checkNew && type != StoredProcedureTypes.Delete)
            {
                instructions.Add(CreateSelectVarsInstruction(varsNew));
                foreach (CheckProcedure check in checks)
                    if (check.IsCalling(type, CheckProcedureCallOrder.After, CheckProcedureCallValues.NewValues))
                        instructions.Add(CreateCheckProcedureCallInstruction(check, varsNew));
            }

            return ProcedureFormat(procedureName, parameters.ToString(), varsDefinition, InstructionsConcat(instructions));
        }

        /// <summary>
        /// Возвращает текст пакета со стандартными процедурами и публичными процедурами вызова
        /// </summary>
        /// <param name="packageName">Название пакета</param>
        /// <param name="parameterType">Функция, возвращающая тип для параметра соответствующей колонки</param>
        /// <returns>Текст пакета со стандартными процедурами</returns>
        public string CreateStandartPackageCommand(string packageName, Func<DBColumn, string> parameterType)
        {
            List<CheckProcedure> checkProcedures = CreateCheckProcedures();
            string[] privateProcedures = checkProcedures.Select(p => CreateCheckProcedureCommand(p, parameterType)).ToArray();
            string procedureAdd = CreateStandartProcedureCommand(StoredProcedureTypes.Add, parameterType, checkProcedures);
            string procedureEdit = CreateStandartProcedureCommand(StoredProcedureTypes.Edit, parameterType, checkProcedures);
            string procedureDelete = CreateStandartProcedureCommand(StoredProcedureTypes.Delete, parameterType, checkProcedures);
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(CreatePackageCommand(packageName, new string[] { procedureAdd, procedureEdit, procedureDelete }, privateProcedures));
            sb.AppendLine(CreateStandartPublicProcedureCommand(packageName, StoredProcedureTypes.Add, parameterType));
            sb.AppendLine(CreateStandartPublicProcedureCommand(packageName, StoredProcedureTypes.Edit, parameterType));
            sb.AppendLine(CreateStandartPublicProcedureCommand(packageName, StoredProcedureTypes.Delete, parameterType));
            return sb.ToString();
        }

        /// <summary>
        /// Возвращает текст стандартной публичной процедуры вызова 
        /// </summary>
        /// <param name="packageName">Название пакета</param>
        /// <param name="type">Тип стандартной процедуры</param>
        /// <param name="parameterType">Функция, возвращающая тип для параметра соответствующей колонки</param>
        /// <returns>Текст стандартной публичной процедуры вызова</returns>
        protected string CreateStandartPublicProcedureCommand(string packageName, StoredProcedureTypes type, Func<DBColumn, string> parameterType)
        {
            // Название публичной процедуры с параметрами
            string schema = formatter.ComposeSafeSchemaName(table.Name);
            string tableName = formatter.ComposeSafeTableName(table.Name);
            string publicName = null;
            switch (type)
            {
                case StoredProcedureTypes.Add: publicName = formatter.FormatProcedureAdd(schema, tableName); break;
                case StoredProcedureTypes.Edit: publicName = formatter.FormatProcedureEdit(schema, tableName); break;
                case StoredProcedureTypes.Delete: publicName = formatter.FormatProcedureDelete(schema, tableName); break;
            }
            string publicParameters = GetStandartProcedureParameters(type, parameterType, ParametersModes.ProcedureDeclare);

            // Вызов частной процедуры с параметрами
            string privateName = string.Concat(packageName, ".", StoredProcedureNames[type]);
            string privateParameters = GetStandartProcedureParameters(type, parameterType, ParametersModes.PackageCall);

            return string.Format(CultureInfo.InvariantCulture, CreatePublicProcedureTemplate, 
                publicName + publicParameters, privateName + privateParameters);
        }

        /// <summary>
        /// Возвращает текст процедуры проверки
        /// </summary>
        /// <param name="procedure">Процедура проверки</param>
        /// <param name="parameterType">Функция, возвращающая тип для параметра соответствующей колонки</param>
        /// <returns>Текст процедуры проверки</returns>
        protected string CreateCheckProcedureCommand(CheckProcedure procedure, Func<DBColumn, string> parameterType)
        {
            string parameters = ColumnsConcat(procedure.Columns,
                col => ParameterName(col) + Space + parameterType(col), StringsDelimiterInline);
            if (!string.IsNullOrEmpty(parameters)) parameters = string.Concat(ParenthesisOpen, parameters, ParenthesisClose);
            return string.Format("procedure {0}{1} as\r\n  {2}", procedure.Name, parameters, procedure.Content);
        }

        /// <summary>
        /// Возвращает текст инструкции вызова указанной процедуры проверки
        /// </summary>
        /// <param name="procedure">Процедура проверки</param>
        /// <param name="vars">Переменные, которые нужно подставить вместо параметров стандартной процедуры</param>
        /// <returns>Текст инструкции вызова указанной процедуры проверки</returns>
        protected string CreateCheckProcedureCallInstruction(CheckProcedure procedure, List<DBColumn> vars)
        {
            string parameters = ColumnsConcat(procedure.Columns, 
                col => vars.Contains(col) ? VariableName(col) : ParameterName(col), StringsDelimiterInline);
            if (procedure.Columns.Count > 0) parameters = ParenthesisOpen + parameters + ParenthesisClose;
            return procedure.Name + parameters;
        }

        /// <summary>
        /// Возвращает текст инструкции добавления записи в указанную таблицу для стандартной процедуры
        /// </summary>
        /// <param name="vars">Список переменных процедуры</param>
        /// <returns>Текст инструкции добавления записи в указанную таблицу для стандартной процедуры</returns>
        /// <remarks>Если есть колонка ключа с последовательностью или переменная rowid, то в инструкции возвращается значение в эту колонку или переменную</remarks>
        public string CreateStandartInsertInstruction(List<DBColumn> vars)
        {
            string sequenceNextVal = sequenceColumn != null ? GetSequenceName(table) + SequenceNextval : null;
            string insert = string.Format(InsertInstructionTemplate, TableName,
                ColumnsConcat(col => ParameterIncluded(StoredProcedureTypes.Add, col) ? ColumnName(col) : null, StringsDelimiterInline),
                ColumnsConcat(col => ParameterIncluded(StoredProcedureTypes.Add, col) ? (col == sequenceColumn ? sequenceNextVal : 
                    ParameterValue(StoredProcedureTypes.Add, col)) : null, StringsDelimiterInline));
            DBColumn returningColumn = sequenceColumn ?? (vars.Contains(ColumnRowid) ? ColumnRowid : null);
            return returningColumn == null ? insert : string.Concat(insert, LinesDelimiter,
                string.Format(ReturningTemplate, ColumnName(returningColumn), ParameterName(returningColumn)));
        }

        /// <summary>
        /// Возвращает текст инструкции изменения записи в таблице
        /// </summary>
        /// <returns>Текст инструкции изменения записи в указанной таблице</returns>
        public string CreateStandartUpdateInstruction(out bool update)
        {
            update = table.Columns.Exists(col => ParameterIncluded(StoredProcedureTypes.Edit, col) && !col.IsKey);
            if (!update) return Null;
            int groupIndex = 0, bitIndex = 0;
            return string.Format(UpdateInstructionTemplate, TableName,
                ColumnsConcat(col => 
                    {
                        if (!ParameterIncluded(StoredProcedureTypes.Edit, col) || col.IsKey) return null;
                        string result = string.Format(SetValueOrSkip, 
                            ColumnName(col), 
                            ParameterValue(StoredProcedureTypes.Edit, col), 
                            SetValueFlagsParameter + (groupIndex == 0 ? string.Empty : groupIndex.ToString()),
                            Convert.ToInt32(Math.Pow(2, bitIndex)));
                        bitIndex++;
                        if (bitIndex == OracleTemplater.MaxBitsInNumber) { groupIndex++; bitIndex = 0; }
                        return result;
                    }, 
                    StringsDelimiter + DoubleSpace), 
                CreateDefaultKeyCondition());
        }

        /// <summary>
        /// Возвращает текст инструкции удаления записи в таблице
        /// </summary>
        /// <returns>Текст инструкции удаления записи в таблице</returns>
        public string CreateStandartDeleteInstruction()
        {
            return string.Format(DeleteInstructionTemplate, TableName, CreateDefaultKeyCondition());
        }

        /// <summary>
        /// Возвращает строку условия по ключу по умолчанию
        /// </summary>
        /// <returns>Строка условия по ключу по умолчанию</returns>
        protected string CreateDefaultKeyCondition()
        {
            return DefaultKeyConcat(EqualTemplate, AndSpace);
        }

        /// <summary>
        /// Возвращает текст инструкции выборки значений полей в переменные из указанного списка
        /// </summary>
        /// <param name="vars">Список переменных, включающий возвращаемые значения и ключи</param>
        /// <returns>Текст инструкции выборки полей в переменные из указанного списка, не являющиеся ключами</returns>
        protected string CreateSelectVarsInstruction(IEnumerable<DBColumn> vars)
        {
            if (vars == null || vars.Count() == 0 || vars.All(var => var.IsKey)) return null;
            AliasDictionary aliases = new AliasDictionary();
            aliases.Append(TableName);
            aliases.Append(vars.Select(col => GetTableName(col)));
            string from = GetTableFrom(aliases);
            return string.Format(SelectVarsTemplate, from,
                ColumnsConcat(vars, col => !col.IsKey ? ColumnName(col, aliases) : null, StringsDelimiterInline),
                ColumnsConcat(vars, col => !col.IsKey ? VariableName(col) : null, StringsDelimiterInline),
                ColumnsConcat(defaultKey, col => string.Format(EqualTemplate, ColumnName(col, aliases), 
                    vars.Contains(col) ? VariableName(col) : ParameterName(col)), AndSpace));
        }

        /// <summary>
        /// Возвращает текст инструкции проверки количества записей
        /// </summary>
        /// <returns>Текст инструкции проверки количества измененных или удаленных записей.</returns> 
        /// <remarks>Если количество измененных или удаленных записей не равно 1, инструкция вызывает исключение.</remarks>
        public string CreateCheckRowCountInstruction()
        {
            string ids = string.Join(string.Concat(ConcatOperator, "','", ConcatOperator), 
                defaultKey.Select(c => ParameterName(c)).ToArray());
            string check = string.Format(CheckRowCountInstructionTemplate, TableName, ids);
            return string.Concat(check, InstructionEnd);
        }

        /// <summary>
        /// Возвращает список процедур проверки
        /// </summary>
        /// <returns>Процедуры проверки для констрейнтов текущей таблицы</returns>
        public List<CheckProcedure> CreateCheckProcedures()
        {
            List<CheckProcedure> list = new List<CheckProcedure>();
            if (tableEx != null)
                foreach (DBConstraintBase constraint in tableEx.Constraints)
                {
                    if (constraint is DBConsistentPeriodConstraint)
                        list.Add(CreateConsistentPeriodCheck((DBConsistentPeriodConstraint)constraint));
                    if (constraint is DBContinuousPeriodConstraint)
                        list.Add(CreateContinuousPeriodCheck((DBContinuousPeriodConstraint)constraint));
                    if (constraint is DBNoCycleHierarchyConstraint)
                        list.Add(CreateNoCycleHierarchyCheck((DBNoCycleHierarchyConstraint)constraint));
                    if (constraint is DBUniqueConstraint)
                        list.Add(CreateUniqueCheck((DBUniqueConstraint)constraint));
                    if (constraint is DBCriteriaConstraint)
                    {
                        // Если констрейнт критерия не был преобразован в выражение констрейнта БД, то добавляем его как констрейнт запроса
                        DBCriteriaConstraint cons = (DBCriteriaConstraint)constraint;
                        if (OracleCheckConstraintGenerator.GenerateExpression(formatter, cons.Criteria) == null)
                        {
                            DBTable originalTable = tableEx != null ? (tableEx.OriginalTable ?? table) : table;
                            SelectStatement select = new SelectStatement(originalTable, cons.TableAlias);
                            select.Condition = cons.Criteria;
                            DBSelectConstraint selectConstraint = new DBSelectConstraint(select);
                            list.Add(CreateSelectCheck(selectConstraint));
                        }
                    }
                    if (constraint is DBSelectConstraint)
                        list.Add(CreateSelectCheck((DBSelectConstraint)constraint));
                    if (constraint is DBSecurityConstraint)
                        list.Add(CreateSecurityCheck((DBSecurityConstraint)constraint));
                }

            // Удаление невызываемых процедур
            list.RemoveAll(check => !check.IsCalling());

            // Уникальность имен
            HashSet<string> uniqueNames = new HashSet<string>();
            foreach (CheckProcedure check in list)
            {
                if (uniqueNames.Contains(check.Name))
                    for (int i = 1; ; i++)
                        if (!uniqueNames.Contains(check.Name + i.ToString()))
                        { check.Name = check.Name + i.ToString(); break; }
                uniqueNames.Add(check.Name);
            }
            return list;
        }

        /// <summary>
        /// Создание процедуры проверки на основе констрейнта непересекающихся периодов
        /// </summary>
        /// <param name="constraint">Констрейнт непересекающихся периодов</param>
        /// <returns>Процедура проверки на основе указанного констрейнта</returns>
        public CheckProcedure CreateConsistentPeriodCheck(DBConsistentPeriodConstraint constraint)
        {
            AliasDictionary aliases = new AliasDictionary();
            aliases.Append(TableName);
            aliases.Append(constraint.PeriodKey.Select(col => GetTableName(col)));
            aliases.Append(GetTableName(constraint.DateIn));
            aliases.Append(GetTableName(constraint.DateOut));
            if (gcRecordColumn != null) aliases.Append(GCRecordTableName);
            string from = GetTableFrom(aliases);
            string periodKey = ColumnsConcat(constraint.PeriodKey,
                col => string.Format(
                    ColumnIsNotNull(col) ? EqualTemplate : EqualNullableTemplate, 
                    ColumnName(col, aliases), ParameterName(col)), AndSpace);
            string activeRecords = GetActiveRecordsCondition(aliases.Find(GCRecordTableName));
            return new CheckProcedure("CheckConsistentPeriod", constraint.PeriodKey, 
                string.Format(ConsistentPeriodConstraintTemplate, from, 
                    ColumnName(constraint.DateIn, aliases), ColumnName(constraint.DateOut, aliases), 
                    JoinConditions(periodKey, activeRecords), 
                    constraint.IncludeIn || constraint.IncludeOut ? null : "+" + OneSecondValue,
                    constraint.IncludeIn && constraint.IncludeOut ? "=" : null));
        }

        /// <summary>
        /// Создание процедуры проверки на основе констрейнта непрерывных периодов
        /// </summary>
        /// <param name="constraint">Констрейнт непрерывных периодов</param>
        /// <returns>Процедура проверки на основе указанного констрейнта</returns>
        public CheckProcedure CreateContinuousPeriodCheck(DBContinuousPeriodConstraint constraint)
        {
            AliasDictionary aliases = new AliasDictionary();
            aliases.Append(TableName);
            aliases.Append(constraint.PeriodKey.Select(col => GetTableName(col)));
            aliases.Append(GetTableName(constraint.DateIn));
            aliases.Append(GetTableName(constraint.DateOut));
            if (gcRecordColumn != null) aliases.Append(GCRecordTableName);
            string from = GetTableFrom(aliases);
            string periodKey = ColumnsConcat(constraint.PeriodKey,
                col => string.Format(
                    ColumnIsNotNull(col) ? EqualTemplate : EqualNullableTemplate, 
                    ColumnName(col, aliases), ParameterName(col)), AndSpace);
            string activeRecords = GetActiveRecordsCondition(aliases.Find(GCRecordTableName));
            return new CheckProcedure("CheckContinuousPeriod", constraint.PeriodKey,
                string.Format(ContinuousPeriodConstraintTemplate, from,
                    ColumnName(constraint.DateIn, aliases), ColumnName(constraint.DateOut, aliases), 
                    JoinConditions(periodKey, activeRecords),
                    constraint.IncludeIn && constraint.IncludeOut ? "+" + OneSecondValue : null,
                    !constraint.IncludeIn && !constraint.IncludeOut ? "=" : null),
                CheckProcedureCallTypes.OnEditAfter | CheckProcedureCallTypes.OnDeleteAfter);
        }

        /// <summary>
        /// Создание процедуры проверки на основе констрейнта иерархии без циклов
        /// </summary>
        /// <param name="constraint">Констрейнт иерархии без циклов</param>
        /// <returns>Процедура проверки на основе указанного констрейнта</returns>
        public CheckProcedure CreateNoCycleHierarchyCheck(DBNoCycleHierarchyConstraint constraint)
        {
            string parentConnect = ColumnsConcat(constraint.Parent, 
                col => string.Format("{0} = prior {1}", ColumnName(col), ColumnName(defaultKey[constraint.Parent.IndexOf(col)])), AndSpace);
            string activeRecords = GetActiveRecordsCondition(null);
            return new CheckProcedure("CheckNoCycleHierarchy", defaultKey,
                string.Format(NoCycleHierarchyConstraintTemplate, TableName, CreateDefaultKeyCondition(), parentConnect,
                string.IsNullOrEmpty(activeRecords) ? null : "where " + activeRecords),
                CheckProcedureCallTypes.OnEditNewValues);
        }

        /// <summary>
        /// Создание процедуры проверки на основе констрейнта уникальности
        /// </summary>
        /// <param name="constraint">Констрейнт уникальности</param>
        /// <returns>Процедура проверки на основе указанного констрейнта</returns>
        public CheckProcedure CreateUniqueCheck(DBUniqueConstraint constraint)
        {
            AliasDictionary aliases = new AliasDictionary();
            aliases.Append(TableName);
            aliases.Append(constraint.Columns.Select(col => GetTableName(col)));
            if (gcRecordColumn != null) aliases.Append(GCRecordTableName);
            string from = GetTableFrom(aliases);
            string key = ColumnsConcat(constraint.Columns,
                col => string.Format(
                    // Уникальность согласно Oracle: 
                    // 1 колонка - пустые значения игнорируются, несколько колонок - null участвуют в определении уникальности
                    constraint.Columns.Count == 1 || ColumnIsNotNull(col) ? EqualTemplate : EqualNullableTemplate, 
                    ColumnName(col, aliases), ParameterName(col)), AndSpace);
            string activeRecords = GetActiveRecordsCondition(aliases.Find(GCRecordTableName));
            return new CheckProcedure("CheckUnique", constraint.Columns,
                string.Format(UniqueConstraintTemplate, from, JoinConditions(key, activeRecords)));
        }

        /// <summary>
        /// Создание процедуры проверки на основе констрейнта запроса целостности данных
        /// </summary>
        /// <param name="constraint">Констрейнт запроса целостности данных</param>
        /// <returns>Процедура проверки на основе указанного констрейнта</returns>
        public CheckProcedure CreateSelectCheck(DBSelectConstraint constraint)
        {
            return CreateSelectCheck(constraint.Select, CheckProcedureCallTypes.Default);
        }

        /// <summary>
        /// Создание процедуры проверки на основе констрейнта запроса безопасности данных
        /// </summary>
        /// <param name="constraint">Констрейнт запроса безопасности данных</param>
        /// <returns>Процедура проверки на основе указанного констрейнта</returns>
        public CheckProcedure CreateSecurityCheck(DBSecurityConstraint constraint)
        {
            return CreateSelectCheck(constraint.Select,
                ((constraint.Operations & DBOperations.Add) == DBOperations.Add ? CheckProcedureCallTypes.OnAdd : CheckProcedureCallTypes.None) |
                ((constraint.Operations & DBOperations.Edit) == DBOperations.Edit ? CheckProcedureCallTypes.OnEditValues : CheckProcedureCallTypes.None) |
                ((constraint.Operations & DBOperations.Delete) == DBOperations.Delete ? (gcRecordColumn == null ? 
                    CheckProcedureCallTypes.OnDeleteBefore : CheckProcedureCallTypes.OnSetDeleted) : CheckProcedureCallTypes.None));
        }

        /// <summary>Создание процедуры проверки на основе запроса</summary>
        private CheckProcedure CreateSelectCheck(SelectStatement selectStatement, CheckProcedureCallTypes callType)
        {
            SelectIntoStatement select = new SelectIntoStatement(selectStatement);
            Dictionary<OperandValue, string> parameters = new Dictionary<OperandValue, string>();
            
            // Выбрать константу 1 в переменную vTemp
            OperandParameter temp = new OperandParameter("vTemp", 0);
            parameters.Add(temp, temp.ParameterName);
            select.Operands.Clear();
            select.Operands.Add(new ConstantValue(1));
            select.Into.Add(temp);

            // Добавить к условию фильтр по ключу
            CriteriaOperatorCollection condition = new CriteriaOperatorCollection();
            condition.Add(select.Condition);
            foreach (DBColumn key in defaultKey)
            {
                OperandParameter keyParameter = new OperandParameter(ParameterName(key), 0);
                parameters.Add(keyParameter, keyParameter.ParameterName);
                condition.Add(new BinaryOperator(new QueryOperand(key, select.Alias), keyParameter, BinaryOperatorType.Equal));
            }

            // Добавить к условию фильтр активных (неудаленных) записей
            DBColumn gcRecord = table.GetColumn(DevExpress.Xpo.Metadata.Helpers.GCRecordField.StaticName);
            if (gcRecord != null) condition.Add(new UnaryOperator(UnaryOperatorType.IsNull, new QueryOperand(gcRecord, select.Alias)));
            select.Condition = new GroupOperator(GroupOperatorType.And, condition);

            // Запрос и процедура проверки
            string sql = new SelectIntoSqlGenerator(formatter, parameters, true).GenerateSql(select).Sql;
            return new CheckProcedure("CheckSelect", defaultKey,
                string.Format(SelectConstraintTemplate, TableName, condition.ToString().Replace('\'', '"'), sql), callType);
        }

        /// <summary>
        /// Процедура проверки
        /// </summary>
        public class CheckProcedure
        {
            private DBColumnCollection columns;

            /// <summary>Название</summary>
            public string Name;

            /// <summary>Колонки, на которых основывается проверка</summary>
            public DBColumnCollection Columns { get { return columns; } }

            /// <summary>Содержание процедуры</summary>
            public string Content;

            /// <summary>Определяет, когда вызывается процедура проверки</summary>
            public CheckProcedureCallTypes CallType;

            /// <summary>
            /// Конструктор процедуры проверки
            /// </summary>
            /// <param name="name">Название</param>
            /// <param name="columns">Колонки, на которых основывается проверка</param>
            /// <param name="content">Содержание процедуры</param>
            public CheckProcedure(string name, DBColumnCollection columns, string content)
                : this(name, columns, content, CheckProcedureCallTypes.Default)
            {
            }

            /// <summary>
            /// Конструктор процедуры проверки
            /// </summary>
            /// <param name="name">Название</param>
            /// <param name="columns">Колонки, на которых основывается проверка</param>
            /// <param name="content">Содержание процедуры</param>
            /// <param name="callType">Определяет, когда вызывается процедура проверки</param>
            public CheckProcedure(string name, DBColumnCollection columns, string content, CheckProcedureCallTypes callType)
            {
                this.Name = name;
                this.Content = content;
                this.columns = new DBColumnCollection();
                if (columns != null)
                    this.columns.AddRange(columns.Distinct());
                this.CallType = callType;
            }

            /// <summary>
            /// Определяет, должна ли быть вызвана процедура проверки в стандартной процедуре указанного типа
            /// </summary>
            /// <param name="procedureType">Тип стандартной процедуры изменения данных</param>
            /// <param name="order">Порядок вызова процедуры проверки</param>
            /// <param name="values">Значения параметров в вызове процедуры проверки</param>
            /// <returns>True, если процедура проверки должна быть вызвана в стандартной процедуре указанного типа</returns>
            public bool IsCalling(StoredProcedureTypes procedureType, CheckProcedureCallOrder order, CheckProcedureCallValues values)
            {
                switch (procedureType)
                {
                    case StoredProcedureTypes.Add: 
                        return order == CheckProcedureCallOrder.After && values == CheckProcedureCallValues.NewValues && 
                            (CallType & CheckProcedureCallTypes.OnAdd) != 0;
                    case StoredProcedureTypes.Edit:
                        if (order == CheckProcedureCallOrder.Before)
                            return values == CheckProcedureCallValues.OldValues &&
                                (CallType & CheckProcedureCallTypes.OnEditBefore) != 0;
                        else
                            return (CallType & (values == CheckProcedureCallValues.OldValues ? 
                                CheckProcedureCallTypes.OnEditOldValues : CheckProcedureCallTypes.OnEditNewValues)) != 0;
                    case StoredProcedureTypes.Delete: 
                        return values == CheckProcedureCallValues.OldValues &&
                            (CallType & (order == CheckProcedureCallOrder.Before ? 
                            CheckProcedureCallTypes.OnDeleteBefore : CheckProcedureCallTypes.OnDeleteAfter)) != 0;
                }
                return false;
            }

            /// <summary>
            /// Определяет, должна ли быть вызвана процедура проверки в стандартной процедуре указанного типа
            /// </summary>
            /// <param name="procedureType">Тип стандартной процедуры изменения данных</param>
            /// <param name="values">Значения параметров в вызове процедуры проверки</param>
            /// <returns>True, если процедура проверки должна быть вызвана в стандартной процедуре указанного типа</returns>
            public bool IsCalling(StoredProcedureTypes procedureType, CheckProcedureCallValues values)
            {
                return IsCalling(procedureType, CheckProcedureCallOrder.Before, values) || IsCalling(procedureType, CheckProcedureCallOrder.After, values);
            }

            /// <summary>
            /// Определяет, должна ли быть вызвана процедура проверки в стандартной процедуре указанного типа
            /// </summary>
            /// <param name="procedureType">Тип стандартной процедуры изменения данных</param>
            /// <returns>True, если процедура проверки должна быть вызвана в стандартной процедуре указанного типа</returns>
            public bool IsCalling(StoredProcedureTypes procedureType)
            {
                switch (procedureType)
                {
                    case StoredProcedureTypes.Add: return (CallType & CheckProcedureCallTypes.OnAdd) != 0;
                    case StoredProcedureTypes.Edit: return (CallType & CheckProcedureCallTypes.OnEdit) != 0;
                    case StoredProcedureTypes.Delete: return (CallType & CheckProcedureCallTypes.OnDelete) != 0;
                }
                return false;
            }

            /// <summary>Определяет, вызывается ли процедура проверки в стандартных процедурах</summary>
            public bool IsCalling() 
            { 
                return CallType != CheckProcedureCallTypes.None;
            }
        }

        /// <summary>
        /// Типы вызова процедуры проверки
        /// </summary>
        public enum CheckProcedureCallTypes
        {
            /// <summary>Процедура проверки не вызывается</summary>
            None = 0,

            /// <summary>Вызов процедуры после добавления записи</summary>
            OnAdd = 1,

            /// <summary>Вызов процедуры перед изменением записи</summary>
            OnEditValues = 2,

            /// <summary>Вызов процедуры после изменения записи со старыми значениями</summary>
            OnEditOldValues = 4,

            /// <summary>Вызов процедуры после изменения записи с новыми значениями</summary>
            OnEditNewValues = 8,

            /// <summary>Вызов процедуры перед удалением записи</summary>
            OnDeleteBefore = 16,

            /// <summary>Вызов процедуры после удаления со значениями удаленной записи</summary>
            OnDeleteAfter = 32,

            /// <summary>Вызов процедуры перед логическим удалением записи (GCRecord = value)</summary>
            OnSetDeleted = 64,

            /// <summary>Вызов процедуры перед изменением записи</summary>
            OnEditBefore = OnEditValues | OnSetDeleted,

            /// <summary>Вызов процедуры после изменения записи со старыми и новыми значениями</summary>
            OnEditAfter = OnEditOldValues | OnEditNewValues,

            /// <summary>Вызов процедуры перед изменением записи и после изменения со старыми и новыми значениями</summary>
            OnEdit = OnEditBefore | OnEditAfter,

            /// <summary>Вызов процедуры перед удалением и после удаления со значениями удаленной записи</summary>
            OnDelete = OnDeleteBefore | OnDeleteAfter,

            /// <summary>Вызов процедуры по умолчанию: после добавления и после изменения с новыми значениями</summary>
            Default = OnAdd | OnEditNewValues
        }

        /// <summary>Значения параметров в вызове процедуры проверки</summary>
        public enum CheckProcedureCallValues
        {
            /// <summary>Вызов процедуры проверки со старыми значениями</summary>
            OldValues,

            /// <summary>Вызов процедуры проверки с новыми значениями</summary>
            NewValues
        }

        /// <summary>Порядок вызова процедуры проверки</summary>
        public enum CheckProcedureCallOrder
        {
            /// <summary>Вызов процедуры проверки до основной инструкции</summary>
            Before,

            /// <summary>Вызов процедуры проверки после основной инструкции</summary>
            After
        }

        /// <summary>Справочник алиасов таблиц</summary>
        public class AliasDictionary : Dictionary<string, string> 
        {
            private int index = 0;

            /// <summary>Добавляет указанную таблицу к справочнику алиасов, если таблица отсутствует</summary>
            /// <param name="table">Таблица, для которой нужен алиас в справочнике алиасов</param>
            public void Append(string table)
            {
                if (!ContainsKey(table))
                {
                    Add(table, "n" + index.ToString());
                    index++;
                }
            }

            /// <summary>Добавляет указанные таблицы к справочнику алиасов, если какие-то таблицы отсутствуют</summary>
            /// <param name="tables">Таблицы, для которых нужны алиасы в справочнике алиасов</param>
            public void Append(IEnumerable<string> tables)
            {
                if (tables != null)
                    foreach (string table in tables) Append(table);
            }

            /// <summary>Возвращает алиас указанной таблицы, если она есть в справочнике алиасов</summary>
            /// <param name="table">Таблица, для которой нужен алиас из справочника</param>
            /// <returns>Алиас для таблицы <paramref name="table"/> или null, если таблица отсутствует в справочнике алиасов</returns>
            public string Find(string table)
            {
                string alias;
                return !string.IsNullOrEmpty(table) && TryGetValue(table, out alias) ? alias : null;
            }
        }
    }

    /// <summary>
    /// Типы стандартных процедур базы данных с безопасным доступом к данным
    /// </summary>
    public enum StoredProcedureTypes
    {
        /// <summary>Процедура добавления новой записи таблицы</summary>
        Add,

        /// <summary>Процедура изменения записи таблицы</summary>
        Edit,

        /// <summary>Процедура удаления записи таблицы</summary>
        Delete
    }

    /// <summary>
    /// Режимы описания параметров процедуры
    /// </summary>
    public enum ParametersModes
    {
        /// <summary>Декларация в пакете</summary>
        PackageDeclare,

        /// <summary>Декларация в процедуре</summary>
        ProcedureDeclare,

        /// <summary>Вызов процедуры в пакете</summary>
        PackageCall
    }
}
