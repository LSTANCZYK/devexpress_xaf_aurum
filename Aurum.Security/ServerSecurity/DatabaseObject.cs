using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Security;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using Aurum.Xpo;
using DevExpress.ExpressApp.ConditionalAppearance;

namespace Aurum.Security
{
    /// <summary>
    /// Объект безопасности в базе данных
    /// </summary>
    [NonPersistent, OptimisticLocking(false), DeferredDeletion(false)]
    public abstract class DatabaseObject : XPBaseObject
    {
        /// <summary>
        /// Идентификатор объекта
        /// </summary>
        [Persistent, Key(false), Size(30)]
        public string Id { get; set; }

        /// <summary>Конструктор без параметров</summary>
        public DatabaseObject() { }

        /// <summary>Конструктор с указанной сессией</summary>
        /// <param name="session">Сессия для загрузки и сохранения объектов в базе данных</param>
        public DatabaseObject(Session session) : base(session) { }

        /// <summary>Указывает, является ли объект уже созданным или новым</summary>
        /// <returns>True, если объект уже создан. False, если объект новый</returns>
        /// <remarks>Определяет доступ к редактированию свойства <see cref="Id"/></remarks>
        [Appearance("DisableId", AppearanceItemType = "ViewItem", Context = "DetailView", Enabled = false, TargetItems = "Id")]
        protected bool IsCreated()
        {
            return !Session.IsNewObject(this);
        }

        /// <summary>
        /// Выполнение команд администрирования безопасности
        /// </summary>
        /// <param name="statements">Команды администрирования безопасности</param>
        /// <returns>Объекты безопасности, полученные в результате выполнения команд</returns>
        /// <exception cref="AdminSecurityNotSupportedException">
        /// Вызывается, когда администрирование безопасности не поддерживается</exception>
        protected SecurityResult AdminSecurity(params SecurityStatement[] statements)
        {
            IDataLayerSecurity security = Session.DataLayer as IDataLayerSecurity;
            if (security == null) throw new AdminSecurityNotSupportedException();
            return security.AdminSecurity(statements);
        }

        /// <summary>
        /// Проверка доступа к командам администрирования безопасности
        /// </summary>
        /// <param name="statements">Команды администрирования безопасности</param>
        /// <returns>Флаги доступа к перечисленным командам: true, если команда поддерживается и есть право на ее выполнение, иначе false</returns>
        /// <exception cref="AdminSecurityNotSupportedException">Вызывается, когда администрирование безопасности не поддерживается</exception>
        protected bool[] CheckAccess(params SecurityStatement[] statements)
        {
            IDataLayerSecurity security = Session.DataLayer as IDataLayerSecurity;
            if (security == null || !security.CanAdminSecurity)
                throw new AdminSecurityNotSupportedException();
            return security.CheckAccess(statements);
        }

        /// <summary>
        /// Проверка доступа к командам администрирования безопасности указанного типа
        /// </summary>
        /// <param name="operation">Тип команды администрирования безопасности</param>
        /// <param name="left">Тип левого операнда операции</param>
        /// <returns>True, если команда указанного типа поддерживается и есть право на ее выполнение в контексте текущего пользователя, иначе false</returns>
        /// <exception cref="AdminSecurityNotSupportedException">Вызывается, когда администрирование безопасности не поддерживается</exception>
        protected bool CheckAccess(AdminSecurityOperations operation, SecurityObjectTypes left)
        {
            return CheckAccess(new SecurityStatement(operation, new SecurityObject(left, null)))[0];
        }

        /// <summary>
        /// Проверка доступа к командам администрирования безопасности указанного типа
        /// </summary>
        /// <param name="operation">Тип команды администрирования безопасности</param>
        /// <param name="left">Тип левого операнда операции</param>
        /// <param name="right">Тип правого операнда операции</param>
        /// <returns>True, если команда указанного типа поддерживается и есть право на ее выполнение в контексте текущего пользователя, иначе false</returns>
        /// <exception cref="AdminSecurityNotSupportedException">Вызывается, когда администрирование безопасности не поддерживается</exception>
        protected bool CheckAccess(AdminSecurityOperations operation, SecurityObjectTypes left, SecurityObjectTypes right)
        {
            return CheckAccess(new SecurityStatement(operation, new SecurityObject(left, null), new SecurityObject(right, null)))[0];
        }
    }

    /// <summary>
    /// Ассоциация иерархии ролей в базе данных
    /// </summary>
    [NonPersistent, OptimisticLocking(false), DeferredDeletion(false)]
    public abstract class DatabaseObjectAssociation<ParentType, ChildType> : XPBaseObject
        where ParentType : DatabaseObject
        where ChildType: DatabaseObject
    {
        private string key;

        /// <summary>Родительский объект ассоциации</summary>
        protected ParentType parent;
        /// <summary>Дочерний объект ассоциации</summary>
        protected ChildType child;

        /// <summary>Ключ иерархии</summary>
        [Key(false)]
        public string Key
        {
            get { return key ?? (parent != null && child != null ? string.Concat(parent.Id, ".", child.Id) : string.Empty); }
            set { key = value; }
        }

        /// <summary>Конструктор без параметров</summary>
        public DatabaseObjectAssociation() { }

        /// <summary>Конструктор с указанной сессией</summary>
        /// <param name="session">Сессия для загрузки и сохранения объектов в базе данных</param>
        public DatabaseObjectAssociation(Session session) : base(session) { }
    }

    /// <summary>
    /// Контроллер объектов безопасности в базе данных
    /// </summary>
    public abstract class DatabaseObjectCustomPersistent : ICustomPersistent
    {
        private DBTableEx table;
        private SecurityObjectTypes objectType;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="objectType">Тип объекта безопасности</param>
        public DatabaseObjectCustomPersistent(SecurityObjectTypes objectType)
        {
            this.objectType = objectType;
        }

        /// <summary>
        /// Выполнение команд администрирования безопасности
        /// </summary>
        /// <param name="dataStore">Объект, выполняющий операции с данными</param>
        /// <param name="statements">Команды администрирования безопасности</param>
        /// <returns>Объекты безопасности, полученные в результате выполнения команд</returns>
        /// <exception cref="AdminSecurityNotSupportedException">
        /// Вызывается, когда администрирование безопасности не поддерживается</exception>
        protected SecurityResult AdminSecurity(IDataStore dataStore, params SecurityStatement[] statements)
        {
            ISqlDataStoreSecurity security = dataStore as ISqlDataStoreSecurity;
            if (security == null) throw new AdminSecurityNotSupportedException();
            return security.AdminSecurity(statements);
        }

        /// <summary>
        /// Выполняет команду получения названия таблицы объектов безопасности
        /// </summary>
        /// <param name="dataStore">Объект, выполняющий операции с данными</param>
        /// <returns>Результат выполнения команды</returns>
        /// <remarks>Результат команды содержит название таблицы, непосредственно используемое 
        /// в базе данных <c>dataStore</c> для получения объектов безопасности.</remarks>
        protected virtual SecurityResult GetObjectTable(IDataStore dataStore)
        {
            return AdminSecurity(dataStore, new SecurityStatement(
                AdminSecurityOperations.GetTable, new SecurityObject(objectType, null)));
        }

        /// <summary>
        /// Выполняет команду создания объекта безопасности
        /// </summary>
        /// <param name="dataStore">Объект, выполняющий операции с данными</param>
        /// <param name="objectName">Название объекта</param>
        /// <param name="parameters">Параметры создаваемого объекта</param>
        /// <returns>Результат выполнения команды</returns>
        protected virtual SecurityResult Create(IDataStore dataStore, string objectName, Dictionary<string, object> parameters)
        {
            return AdminSecurity(dataStore, new SecurityStatement(
                AdminSecurityOperations.Create, new SecurityObject(objectType, objectName)));
        }

        /// <summary>
        /// Выполняет команду удаления объекта безопасности
        /// </summary>
        /// <param name="dataStore">Объект, выполняющий операции с данными</param>
        /// <param name="objectName">Название объекта</param>
        /// <returns>Результат выполнения команды</returns>
        protected virtual SecurityResult Drop(IDataStore dataStore, string objectName)
        {
            return AdminSecurity(dataStore, new SecurityStatement(
                AdminSecurityOperations.Drop, new SecurityObject(objectType, objectName)));
        }

        /// <summary>
        /// Выполняет команду изменения объекта безопасности
        /// </summary>
        /// <param name="dataStore">Объект, выполняющий операции с данными</param>
        /// <param name="objectName">Название объекта</param>
        /// <param name="parameters">Параметры обновления</param>
        /// <returns>Результат выполнения команды</returns>
        protected virtual SecurityResult Alter(IDataStore dataStore, string objectName, Dictionary<string, object> parameters)
        {
            return new SecurityResult();
        }

        /// <summary>
        /// Возвращает коллекцию названий объектов для указанного выражения модификации данных
        /// </summary>
        /// <param name="dataStore">Объект, выполняющий операции с данными</param>
        /// <param name="statement">Выражение модификации данных</param>
        /// <returns>Коллекция названий объектов</returns>
        protected StringCollection GetObjectNames(IDataStore dataStore, BaseStatement statement)
        {
            // Условие одного объекта
            StringCollection objectNames = new StringCollection();
            if (statement.Condition is BinaryOperator)
            {
                BinaryOperator binary = (BinaryOperator)statement.Condition;
                if (binary.OperatorType == BinaryOperatorType.Equal && binary.RightOperand is OperandValue)
                    objectNames.Add((string)((OperandValue)binary.RightOperand).Value);
            }
            // Условие списка
            else if (statement.Condition is InOperator)
            {
                InOperator inOp = (InOperator)statement.Condition;
                foreach (OperandValue value in inOp.Operands)
                    objectNames.Add((string)value.Value);
            }
            // Произвольный запрос
            else
            {
                SelectStatement select = new SelectStatement();
                select.TableName = statement.TableName;
                select.Condition = statement.Condition;
                select.Operands.Add(new QueryOperand("Id", statement.Alias));
                select.Alias = statement.Alias;
                SelectStatementResult result = dataStore.SelectData(select).ResultSet[0];
                foreach (SelectStatementResultRow row in result.Rows)
                    objectNames.Add((string)row.Values[0]);
            }
            return objectNames;
        }

        /// <summary>
        /// Возвращает коллекцию параметров указанного выражения модификации данных
        /// </summary>
        /// <param name="statement">Выражение модификации данных</param>
        /// <returns>Коллекция параметров и их значений</returns>
        protected Dictionary<string, object> GetParameters(ModificationStatement statement)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            for (int i = 0; i < statement.Operands.Count; i++) 
                if (statement.Operands[i] is QueryOperand)
                {
                    string columnName = ((QueryOperand)statement.Operands[i]).ColumnName;
                    parameters[columnName] = statement.Parameters[i].Value;
                }
            return parameters;
        }

        #region ICustomPersistent

        /// <contentfrom cref="ICustomPersistent.GetTable" />
        DBTableEx ICustomPersistent.GetTable(IDataStore dataStore)
        {
            if (table == null)
            {
                SecurityResult getTable = GetObjectTable(dataStore);
                if (getTable.Objects.Length > 0)
                    table = XPDictionaryInformer.Schema.GetTable(getTable.Objects[0].ObjectName);
            }
            return table;
        }

        /// <contentfrom cref="ICustomPersistent.Insert" />
        ParameterValue ICustomPersistent.Insert(IDataStore dataStore, InsertStatement statement)
        {
            string objectName = (string)statement.Parameters[0].Value;
            var parameters = GetParameters(statement);
            Create(dataStore, objectName, parameters);
            ParameterValue result = new ParameterValue();
            result.Value = objectName;
            return result;
        }

        /// <contentfrom cref="ICustomPersistent.Update" />
        void ICustomPersistent.Update(IDataStore dataStore, UpdateStatement statement)
        {
            var parameters = GetParameters(statement);
            foreach (string objectName in GetObjectNames(dataStore, statement))
                Alter(dataStore, objectName, parameters);
        }

        /// <contentfrom cref="ICustomPersistent.Delete" />
        void ICustomPersistent.Delete(IDataStore dataStore, DeleteStatement statement)
        {
            foreach (string objectName in GetObjectNames(dataStore, statement)) 
                Drop(dataStore, objectName);
        }

        #endregion
    }

    /// <summary>
    /// Контроллер ассоциации объектов безопасности в базе данных
    /// </summary>
    public class DatabaseAssociationCustomPersistent : DatabaseObjectCustomPersistent, ICustomPersistent
    {
        SecurityObjectTypes parentType;
        SecurityObjectTypes childType;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="parentType">Тип родительского объекта безопасности в ассоциации</param>
        /// <param name="childType">Тип дочернего объекта безопасности в ассоциации</param>
        public DatabaseAssociationCustomPersistent(SecurityObjectTypes parentType, SecurityObjectTypes childType)
            : base(parentType)
        {
            this.parentType = parentType;
            this.childType = childType;
        }

        /// <inheritdoc/>
        protected override SecurityResult GetObjectTable(IDataStore dataStore)
        {
            return AdminSecurity(dataStore, new SecurityStatement(AdminSecurityOperations.GetTable, 
                new SecurityObject(parentType, null), new SecurityObject(childType, null)));
        }

        /// <summary>
        /// Выполняет команду создания ассоциации между двумя объектами безопасности
        /// </summary>
        /// <param name="dataStore">Объект, выполняющий операции с данными</param>
        /// <param name="parentName">Название родительского объекта безопасности в ассоциации</param>
        /// <param name="childName">Название дочернего объекта безопасности в ассоциации</param>
        /// <returns>Результат выполнения команды</returns>
        /// <remarks>По умолчанию выполняет назначение доступа к objectName1 для objectName2</remarks>
        protected virtual SecurityResult Create(IDataStore dataStore, string parentName, string childName)
        {
            return AdminSecurity(dataStore, new SecurityStatement(AdminSecurityOperations.GrantTo,
                new SecurityObject(childType, childName), new SecurityObject(parentType, parentName)));
        }

        /// <inheritdoc/>
        protected override SecurityResult Drop(IDataStore dataStore, string objectName)
        {
            int delimiter = objectName.IndexOf('.');
            string parentName = objectName.Substring(0, delimiter);
            string childName = objectName.Substring(delimiter + 1);
            return AdminSecurity(dataStore, new SecurityStatement(AdminSecurityOperations.RevokeFrom,
                new SecurityObject(childType, childName), new SecurityObject(parentType, parentName)));
        }

        #region ICustomPersistent

        /// <contentfrom cref="ICustomPersistent.Insert" />
        ParameterValue ICustomPersistent.Insert(IDataStore dataStore, InsertStatement statement)
        {
            string parentName = (string)statement.Parameters[1].Value;
            string childName = (string)statement.Parameters[2].Value;
            Create(dataStore, parentName, childName);
            ParameterValue result = new ParameterValue();
            result.Value = string.Concat(parentName, '.', childName);
            return result;
        }

        #endregion
    }
}
