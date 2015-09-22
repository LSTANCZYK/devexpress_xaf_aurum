using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Generators;
using DevExpress.Xpo.Exceptions;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;

namespace Aurum.Xpo
{
    /// <summary>
    /// Служебный класс для сбора метаданных о констрейнтах базы данных
    /// </summary>
    static class DBTableHelperEx
    {
        /// <summary>
        /// Колонки таблицы
        /// </summary>
        private class TableColumns : Tuple<DBTableEx, List<string>>
        {
            public TableColumns(DBTableEx table) : base(table, new List<string>()) { }
        }

        /// <summary>
        /// Справочник колонок таблиц
        /// </summary>
        private class ColumnDictionary : Dictionary<string, TableColumns>
        {
            public void Add(string propertyName, DBTableEx table)
            {
                Add(propertyName, new TableColumns(table));
            }

            public List<string> GetColumns(string propertyName)
            {
                return this[propertyName].Item2;
            }
        }

        /// <summary>
        /// Собирает метаданные класса и преобразует их в констрейнты, добавляя в указанный список
        /// </summary>
        /// <param name="table">Таблица класса</param>
        /// <param name="classInfo">Метаданные класса</param>
        public static void ProcessClassInfo(DBTableEx table, ReflectionClassInfoEx classInfo)
        {
            // Справочник свойств и колонок 
            ColumnDictionary propertiesColumns = new ColumnDictionary();
            ProcessMembers(table, classInfo, propertiesColumns);
            ProcessClass(table, classInfo, propertiesColumns);
        }

        #region Соответствие DBTableHelper для полного соответствия названий колонок

        // Собирает метаданные свойств класса, преобразуя их в констрейнты. Попутно составляется справочник свойств/полей и колонок.
        private static void ProcessMembers(DBTableEx table, ReflectionClassInfoEx classInfo, ColumnDictionary propertiesColumns)
        {
            foreach (XPMemberInfo memberInfo in classInfo.SourceInfo.PersistentProperties)
            {
                // Таблица, которой принадлежат колонки свойства
                DBTableEx persistentTable = null;
                ReflectionClassInfoEx current = classInfo;
                while (current != null)
                    if (memberInfo.IsMappingClass(current.SourceInfo))
                    { 
                        if (current == classInfo) persistentTable = table; else persistentTable = current.TableEx; 
                        break; 
                    }
                    else
                        current = current.Base;
                // Обработка колонок свойства
                if (persistentTable != null)
                {
                    string mappingPath = memberInfo.SubMembers.Count == 0 ? memberInfo.MappingField : string.Empty;
                    ProcessMemberColumns(persistentTable, persistentTable == table, memberInfo, mappingPath, false, 
                        new List<XPMemberInfo>(), memberInfo.Name, propertiesColumns);
                }
            }
            // Таблица родительского класса 
            // (может быть только одна, пока отсутствует множественное наследование)
            ReflectionClassInfoEx parent = classInfo.Base;
            while (parent != null)
                if (!string.IsNullOrEmpty(parent.TableName) && parent.TableName != table.Name)
                {
                    table.ParentTable = parent.TableEx;
                    break;
                }
                else
                    parent = parent.Base;
        }

        // Собирает метаданные колонок свойства класса
        // table - таблица, к которой относятся метаданные, origin - таблица является исходной для обрабатываемых метаданных
        // mappingPath - собирает название колонки, isRef - отслеживает путь до ссылки, branch - собирает свойства колонки
        // propertyName - имя свойства/поля по ветке через точку, propertiesColumns - справочник свойств/полей и колонок
        private static void ProcessMemberColumns(DBTableEx table, bool isOrigin, XPMemberInfo memberInfo, string mappingPath, 
            bool isRef, List<XPMemberInfo> branch, string propertyName, ColumnDictionary propertiesColumns)
        {
            // Построение ветки свойств. Если есть ссылка, то до ссылки включительно
            if (!isRef) branch.Add(memberInfo);

            // Список колонок текущего свойства
            propertiesColumns.Add(propertyName, new TableColumns(table));
            List<string> propertyColumns = propertiesColumns.GetColumns(propertyName);

            // Ссылка
            if (memberInfo.ReferenceType != null)
            {
                XPMemberInfo keyProperty = memberInfo.ReferenceType.KeyProperty;
                if (keyProperty == null) return;
                string keyPropertyName = string.Concat(propertyName, ".", keyProperty.Name);
                ProcessMemberColumns(table, isOrigin, keyProperty, mappingPath, true, branch, keyPropertyName, propertiesColumns);
                propertyColumns.AddRange(propertiesColumns.GetColumns(keyPropertyName));
            }
            // Простое свойство
            else if (memberInfo.SubMembers.Count == 0)
            {
                propertyColumns.Add(mappingPath);
                if (isOrigin) AddColumnConstraints(table, memberInfo, mappingPath, branch);
            }
            // Сложное свойство
            else
            {
                foreach (XPMemberInfo mi in memberInfo.SubMembers)
                    if (mi.IsPersistent)
                    {
                        string subPropertyName = mi.Name;
                        ProcessMemberColumns(table, isOrigin, mi, mappingPath + mi.MappingField, isRef, new List<XPMemberInfo>(branch), subPropertyName, propertiesColumns);
                        propertyColumns.AddRange(propertiesColumns.GetColumns(subPropertyName));
                    }
            }
        }

        #endregion

        /// <summary>
        /// Добавить констрейнты колонки
        /// </summary>
        /// <param name="table">Таблица</param>
        /// <param name="mi">Простое свойство, соответствующее колонке</param>
        /// <param name="columnName">Название колонки</param>
        /// <param name="branch">Ветка всех свойств, в которых состоит колонка. Если есть ссылка, то до ссылки включительно.</param>
        private static void AddColumnConstraints(DBTableEx table, XPMemberInfo mi, string columnName, List<XPMemberInfo> branch)
        {
            XPMemberInfo cmi = branch.LastOrDefault() ?? mi;

            // Колонка таблицы
            DBColumn column = table.GetColumn(columnName);
            if (column == null) throw new PropertyMissingException(cmi.Owner.FullName, cmi.Name);

            // Анализ метаданных
            bool notNull = cmi.HasAttribute(typeof(NotNullAttribute));
            bool readOnly = cmi.HasAttribute(typeof(ReadOnlyAttribute));
            foreach (XPMemberInfo item in branch)
                if (item.IsStruct) notNull = notNull && item.HasAttribute(typeof(NotNullAttribute));

            // Добавление констрейнтов
            if (notNull) table.AddConstraint(new DBNotNullConstraint(column));
            if (readOnly) table.AddConstraint(new DBReadOnlyConstraint(column));
        }

        /// <summary>
        /// Обработка констрейнтов класса
        /// </summary>
        /// <param name="table">Таблица</param>
        /// <param name="classInfo">Метаданные класса</param>
        /// <param name="propertiesColumns">Справочник свойств/полей и колонок</param>
        private static void ProcessClass(DBTableEx table, ReflectionClassInfoEx classInfo, ColumnDictionary propertiesColumns)
        {            
            // Атрибуты констрейнтов таблицы
            Attribute[] attributes = classInfo.AllAttributes;
            foreach (Attribute attribute in attributes)
            {
                // Констрейнт последовательности периодов
                if (attribute is ConsistentPeriodAttribute)
                {
                    ConsistentPeriodAttribute attr = (ConsistentPeriodAttribute)attribute;
                    DBColumnCollection periodKey = GetPropertyColumns(table, classInfo, propertiesColumns, attr.PeriodKey, true),
                        dateIn = GetPropertyColumns(table, classInfo, propertiesColumns, attr.DateIn, true),
                        dateOut = GetPropertyColumns(table, classInfo, propertiesColumns, attr.DateOut, true);
                    const string dateError = "Property '{1}' of ConsistentPeriodAttribute of {0} is not convertible to DateTime";
                    if (dateIn.Count != 1 || dateIn[0].ColumnType != DBColumnType.DateTime)
                        throw new InvalidOperationException(string.Format(dateError, classInfo.FullName, "DateIn"));
                    if (dateOut.Count != 1 || dateOut[0].ColumnType != DBColumnType.DateTime)
                        throw new InvalidOperationException(string.Format(dateError, classInfo.FullName, "DateOut"));
                    // Непересекающаяся последовательность
                    table.AddConstraint(new DBConsistentPeriodConstraint(
                        periodKey, dateIn[0], dateOut[0], attr.IncludeIn, attr.IncludeOut));
                    // Непрерывная последовательность
                    if (attr.Continuous)
                    {
                        table.AddConstraint(new DBContinuousPeriodConstraint(
                            periodKey, dateIn[0], dateOut[0], attr.IncludeIn, attr.IncludeOut));
                    }
                    // Проверка отношения между датами (только для исходной таблицы)
                    if (table.GetColumn(dateIn[0].Name) != null && table.GetColumn(dateOut[0].Name) != null)
                    {
                        QueryOperand dateInOp = new QueryOperand(dateIn[0], null);
                        QueryOperand dateOutOp = new QueryOperand(dateOut[0], null);
                        CriteriaOperator criteria = new GroupOperator(GroupOperatorType.Or,
                            new BinaryOperator(dateInOp, dateOutOp, attr.IncludeIn && attr.IncludeOut ? BinaryOperatorType.LessOrEqual : BinaryOperatorType.Less),
                            new UnaryOperator(UnaryOperatorType.IsNull, dateInOp),
                            new UnaryOperator(UnaryOperatorType.IsNull, dateOutOp));
                        table.AddConstraint(new DBCriteriaConstraint(criteria));
                    }
                }
                // Констрейнт иерархии
                if (attribute is HierarchyAttribute)
                {
                    HierarchyAttribute attr = (HierarchyAttribute)attribute;
                    DBColumnCollection parent = GetPropertyColumns(table, classInfo, propertiesColumns, attr.Parent, false);
                    if (!table.CanReferences(parent))
                        throw new InvalidOperationException(string.Format(
                            "Property '{1}' of HierarchyAttribute of {0} cannot reference to primary key of class", classInfo.FullName, attr.Parent));
                    if (attr.NoCycle)
                        table.AddConstraint(new DBNoCycleHierarchyConstraint(parent));
                }
                // Констрейнт уникальности
                if (attribute is UniqueAttribute)
                {
                    UniqueAttribute attr = (UniqueAttribute)attribute;
                    DBColumnCollection columns = GetPropertyColumns(table, classInfo, propertiesColumns, attr.Fields, true);
                    if (columns.Count == 0)
                        throw new InvalidOperationException(string.Format("There are no fields in UniqueAttribute of {0}", classInfo.FullName));
                    table.AddConstraint(new DBUniqueConstraint(columns));
                }
                // Констрейнт критерия или запроса
                if (attribute is ConstraintAttribute)
                {
                    ConstraintAttribute attr = (ConstraintAttribute)attribute;
                    SelectStatement select = ConvertCriteria(classInfo, table.Name, attr.Criteria);
                    if (select.SubNodes.Count > 0)
                        table.AddConstraint(new DBSelectConstraint(select));
                    else
                        table.AddConstraint(new DBCriteriaConstraint(select.Condition, select.Alias));
                }
                // Констрейнт запроса безопасности данных
                if (attribute is SecurityAttribute)
                {
                    SecurityAttribute attr = (SecurityAttribute)attribute;
                    SelectStatement select = ConvertCriteria(classInfo, table.Name, attr.Criteria);
                    table.AddConstraint(new DBSecurityConstraint(select, attr.Operations));
                }
            }
        }

        // Возвращает колонки свойств и проверяет их наличие в таблице
        private static DBColumnCollection GetPropertyColumns(DBTableEx table, ReflectionClassInfoEx classInfo, 
            ColumnDictionary propertiesColumns, StringCollection propertiesNames, bool foreignColumns)
        {
            DBColumnCollection columns = new DBColumnCollection();
            foreach (string property in propertiesNames)
                columns.AddRange(GetPropertyColumns(table, classInfo, propertiesColumns, property, foreignColumns));
            return columns;
        }

        // Возвращает колонки свойства и проверяет их наличие в таблице
        private static DBColumnCollection GetPropertyColumns(DBTableEx table, ReflectionClassInfoEx classInfo, 
            ColumnDictionary propertiesColumns, string propertyName, bool foreignColumns)
        {
            DBColumnCollection columns = new DBColumnCollection();
            TableColumns tableColumns;
            if (!propertiesColumns.TryGetValue(propertyName, out tableColumns))
                throw new PropertyMissingException(classInfo.FullName, propertyName);
            if (!foreignColumns && tableColumns.Item1.Name != table.Name)
                throw new InvalidOperationException(string.Format("Property {0}.{1} is not part of table {2}", classInfo.FullName, propertyName, table));
            foreach (string columnName in tableColumns.Item2)
            {
                DBColumn column = tableColumns.Item1.GetColumn(columnName);
                if (column == null)
                    throw new InvalidOperationException(string.Format("Column {2} of property {0}.{1} is not part of table {3}", classInfo.FullName, propertyName, columnName, table));
                if (tableColumns.Item1.Name != table.Name)
                    column = new DBTableColumn(tableColumns.Item1, column);
                columns.Add(column);
            }
            return columns;
        }

        // Конвертация объектного критерия в запрос базы данных
        private static SelectStatement ConvertCriteria(ReflectionClassInfoEx classInfo, string tableName, CriteriaOperator criteria)
        {
            ExpandedCriteriaHolder criteriaExpanded = PersistentCriterionExpander.ExpandToLogical(
                DummyPersistentValuesSource.Instance, classInfo.SourceInfo, criteria, true);
            if (criteriaExpanded.RequiresPostProcessing)
                throw new InvalidOperationException(string.Format("Criteria {1} of {0} requires postprocessing: {2}", 
                    classInfo.FullName, criteria, criteriaExpanded.PostProcessingCause));
            criteria = criteriaExpanded.ExpandedCriteria;
            CriteriaOperatorCollection props = new CriteriaOperatorCollection();
            props.Add(ExpandedCriteriaHolder.True.ExpandedCriteria);
            SelectStatement select = ClientSelectSqlGenerator.GenerateSelect(
                classInfo.SourceInfo, criteria, props, null, null, null, null, 0);
            if (XPDictionaryInformer.TranslateTableName(select.TableName) != tableName)
                throw new InvalidOperationException(string.Format("Unexpected table name in constraint {1} of {0}: {2}",
                    classInfo.FullName, criteria, tableName));
            return select;
        }

        class DummyPersistentValuesSource : IPersistentValueExtractor
        {
            public static DummyPersistentValuesSource Instance = new DummyPersistentValuesSource();
            public object ExtractPersistentValue(object criterionValue) { return criterionValue; }
            public bool CaseSensitive { get { return true; } }
        }
    }
}
