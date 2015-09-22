using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using Aurum.Xpo;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.Data.Filtering;

namespace Aurum.Security
{
    /// <summary>
    /// Пользователь в базе данных (объект базы данных)
    /// </summary>
    /// <remarks>Количество и порядок свойств должен быть фиксированным для запросов из БД: Id, IsActive, IsExpired, Locked, Created</remarks>
    [Persistent, CustomPersistent(typeof(DatabaseUserPersistentCustom)), ImageName("BO_User")]
    public sealed class DatabaseUser : DatabaseObject, IAuthenticationStandardUser, ISecurityUser, ISecurityUserWithRoles, IOperationPermissionProvider
    {
        /// <summary>
        /// Режим создания разрешений на операции в базе данных
        /// </summary>
        public static DatabasePermissionModes PermissionMode = DatabasePermissionModes.Client;

        private bool isActive = true;
        private bool isExpired;
        private DateTime? locked;
        private DateTime? created;

        /// <summary>Конструктор без параметров</summary>
        public DatabaseUser() { }

        /// <summary>Конструктор с указанной сессией</summary>
        /// <param name="session">Сессия для загрузки и сохранения объектов в базе данных</param>
        public DatabaseUser(Session session) : base(session) { }

        /// <summary>Определяет, может ли пользователь подключиться к базе данных</summary>
        /// <value>True, если пользователь может подключиться к базе данных, иначе false</value>
        public bool IsActive
        {
            get { return isActive; }
            set 
            {
                if (SetPropertyValue("IsActive", ref isActive, value) && !IsLoading)
                {
                    // Установка свойства даты блокировки для синхронизации интерфейса после сохранения пользователя,
                    // при новой загрузке/обновлении записи значение свойства будет получено из базы данных
                    Locked = value ? (DateTime?)null : DateTime.Today;
                }
            }
        }

        /// <summary>Определяет, требуется ли пользователю ввести новый пароль при подключении к базе данных</summary>
        /// <value>True, если пользователю требуется ввести новый пароль при подключении к базе данных, иначе false</value>
        public bool IsExpired
        {
            get { return isExpired; }
            set { SetPropertyValue("IsExpired", ref isExpired, value); }
        }

        /// <summary>Дата блокировки пользователя</summary>
        public DateTime? Locked
        {
            get { return locked; }
            set { SetPropertyValue("Locked", ref locked, value); }
        }

        /// <summary>Дата создания пользователя</summary>
        public DateTime? Created
        {
            get { return created; }
            set { SetPropertyValue("Created", ref created, value); }
        }

        /// <summary>Роли, связанные с текущим пользователем</summary>
        [Association("User"), Browsable(false)]
        public IList<DatabaseUserRolesAssociation> ToUser
        {
            get { return GetList<DatabaseUserRolesAssociation>("ToUser"); }
        }

        /// <summary>Пользователи, которым назначена текущая роль</summary>
        [ManyToManyAlias("ToUser", "Role")]
        public IList<DatabaseRole> Roles
        {
            get { return GetList<DatabaseRole>("Roles"); }
        }

        /// <summary>
        /// Возвращает разрешения на типы пользователя и роли в базе данных
        /// </summary>
        /// <returns>Разрешения на <c>DatabaseUser</c>, <c>DatabaseRole</c> и <c>DatabasePrivilege</c></returns>
        public IEnumerable<IOperationPermission> GetSystemTypePermissions()
        {
            List<IOperationPermission> result = new List<IOperationPermission>();
            Type userType = typeof(DatabaseUser);
            Type roleType = typeof(DatabaseRole);
            Type privType = typeof(DatabasePrivilege);

            bool readUserRoles = 
                CheckAccess(AdminSecurityOperations.GetTable, SecurityObjectTypes.User, SecurityObjectTypes.Role);
            bool writeUserRoles = 
                CheckAccess(AdminSecurityOperations.GrantTo, SecurityObjectTypes.Role, SecurityObjectTypes.User) &&
                CheckAccess(AdminSecurityOperations.RevokeFrom, SecurityObjectTypes.Role, SecurityObjectTypes.User);
            bool writeRolePrivs = 
                CheckAccess(AdminSecurityOperations.GrantTo, SecurityObjectTypes.Table, SecurityObjectTypes.Role) &&
                CheckAccess(AdminSecurityOperations.RevokeFrom, SecurityObjectTypes.Table, SecurityObjectTypes.Role);

            // Разрешения на тип пользователя
            result.Add(new TypeOperationPermission(userType, SecurityOperations.Navigate));
            if (CheckAccess(AdminSecurityOperations.Create, SecurityObjectTypes.User))
                result.Add(new TypeOperationPermission(userType, SecurityOperations.Create));
            if (CheckAccess(AdminSecurityOperations.Drop, SecurityObjectTypes.User))
                result.Add(new TypeOperationPermission(userType, SecurityOperations.Delete));
            result.Add(new MemberOperationPermission(userType, "Id;IsActive;IsExpired;Locked;Created", SecurityOperations.Read));
            result.Add(new MemberOperationPermission(userType, "Id", SecurityOperations.Write));
            if (CheckAccess(AdminSecurityOperations.SetUserInfo, SecurityObjectTypes.User, SecurityObjectTypes.UserInfo))
                result.Add(new MemberOperationPermission(userType, "IsActive;IsExpired", SecurityOperations.Write));
            if (readUserRoles) result.Add(new MemberOperationPermission(userType, "Roles", SecurityOperations.Read));
            if (writeUserRoles) result.Add(new MemberOperationPermission(userType, "Roles", SecurityOperations.Write));

            // Разрешения на тип роли
            result.Add(new TypeOperationPermission(roleType, SecurityOperations.Navigate));
            if (CheckAccess(AdminSecurityOperations.Create, SecurityObjectTypes.Role))
                result.Add(new TypeOperationPermission(roleType, SecurityOperations.Create));
            if (CheckAccess(AdminSecurityOperations.Drop, SecurityObjectTypes.Role))
                result.Add(new TypeOperationPermission(roleType, SecurityOperations.Delete));
            result.Add(new MemberOperationPermission(roleType, "Id", SecurityOperations.Read));
            result.Add(new MemberOperationPermission(roleType, "Id", SecurityOperations.Write));
            if (CheckAccess(AdminSecurityOperations.GetTable, SecurityObjectTypes.Role, SecurityObjectTypes.Role))
                result.Add(new MemberOperationPermission(roleType, "ChildRoles;ParentRoles", SecurityOperations.Read));
            if (CheckAccess(AdminSecurityOperations.GrantTo, SecurityObjectTypes.Role, SecurityObjectTypes.Role) &&
                CheckAccess(AdminSecurityOperations.RevokeFrom, SecurityObjectTypes.Role, SecurityObjectTypes.Role))
                result.Add(new MemberOperationPermission(roleType, "ChildRoles;ParentRoles", SecurityOperations.Write));
            if (readUserRoles) result.Add(new MemberOperationPermission(roleType, "Users", SecurityOperations.Read));
            if (writeUserRoles) result.Add(new MemberOperationPermission(roleType, "Users", SecurityOperations.Write));
            if (CheckAccess(AdminSecurityOperations.GetRolePrivileges, SecurityObjectTypes.Role))
                result.Add(new MemberOperationPermission(roleType, "Privileges", SecurityOperations.Read));
            if (writeRolePrivs)
                result.Add(new MemberOperationPermission(roleType, "Privileges", SecurityOperations.Write));

            // Разрешения на тип привилегии
            result.Add(new TypeOperationPermission(privType, SecurityOperations.Navigate));
            if (CheckAccess(AdminSecurityOperations.GetRolePrivileges, SecurityObjectTypes.Role))
                result.Add(new TypeOperationPermission(privType, SecurityOperations.Read));
            if (writeRolePrivs)
            {
                result.Add(new TypeOperationPermission(privType, SecurityOperations.Create));
                result.Add(new TypeOperationPermission(privType, SecurityOperations.Write));
                result.Add(new TypeOperationPermission(privType, SecurityOperations.Delete));
            }

            return result;
        }

        /// <summary>
        /// Возвращает разрешения на объекты в базе данных
        /// </summary>
        /// <returns>Разрешения на объекты в базе данных</returns>
        public IEnumerable<IOperationPermission> GetTypePermissions()
        {
            List<IOperationPermission> result = new List<IOperationPermission>();
            HashSet<string> allowSelects = new HashSet<string>(), allowInserts = new HashSet<string>();
            HashSet<string> allowUpdates = new HashSet<string>(), allowDeletes = new HashSet<string>();
            foreach (SecurityObject so in AdminSecurity(new SecurityStatement(AdminSecurityOperations.GetCurrentPrivileges, null)).Objects)
            {
                if ((so.TableRights & SecurityTableRights.Select) == SecurityTableRights.Select) allowSelects.Add(so.ObjectName);
                if ((so.TableRights & SecurityTableRights.Insert) == SecurityTableRights.Insert) allowInserts.Add(so.ObjectName);
                if ((so.TableRights & SecurityTableRights.Update) == SecurityTableRights.Update) allowUpdates.Add(so.ObjectName);
                if ((so.TableRights & SecurityTableRights.Delete) == SecurityTableRights.Delete) allowDeletes.Add(so.ObjectName);
            }
            foreach (XPClassInfo classInfo in Session.Dictionary.Classes)
            {
                Type type = classInfo.ClassType;
                bool exists = false, allowSelect = true, allowInsert = true, allowUpdate = true, allowDelete = true;
                foreach (string table in XPDictionaryInformer.GetReferencedTables(type))
                {
                    allowSelect &= allowSelects.Contains(table);
                    allowInsert &= allowInserts.Contains(table);
                    allowUpdate &= allowUpdates.Contains(table);
                    allowDelete &= allowDeletes.Contains(table);
                    exists = true;
                }
                if (classInfo.IsPersistent && !exists)
                {
                    allowSelect = false; allowInsert = false; allowUpdate = false; allowDelete = false;
                }
                if (PermissionMode == DatabasePermissionModes.Database)
                {
                    if (exists) result.Add(new DatabaseTypePermission(type, allowSelect, allowInsert, allowUpdate, allowDelete));
                }
                else if (PermissionMode == DatabasePermissionModes.Client)
                {
                    if (allowSelect) result.Add(new TypeOperationPermission(type, SecurityOperations.Navigate));
                    if (allowSelect) result.Add(new TypeOperationPermission(type, SecurityOperations.Read));
                    if (allowUpdate) result.Add(new TypeOperationPermission(type, SecurityOperations.Write));
                    if (allowInsert) result.Add(new TypeOperationPermission(type, SecurityOperations.Create));
                    if (allowDelete) result.Add(new TypeOperationPermission(type, SecurityOperations.Delete));
                }
            }
            return result;
        }

        #region IAuthenticationStandardUser

        // UserName должен быть обычным свойством, иначе XafApplication.Logon() вылетает с исключением
        // DevExpress.Data.Filtering.Exceptions.InvalidPropertyPathException
        /// <contentfrom cref="IAuthenticationStandardUser.UserName" />
        /// <contentfrom cref="ISecurityUser.UserName" />
        [Browsable(false)]
        public string UserName
        {
            get { return Id; }
        }

        /// <contentfrom cref="IAuthenticationStandardUser.ChangePasswordOnFirstLogon" />
        [NonPersistent]
        bool IAuthenticationStandardUser.ChangePasswordOnFirstLogon
        {
            get { return IsExpired; }
            set { IsExpired = value; }
        }

        /// <contentfrom cref="IAuthenticationStandardUser.ComparePassword" />
        bool IAuthenticationStandardUser.ComparePassword(string password)
        {
            return true;
        }

        /// <contentfrom cref="IAuthenticationStandardUser.SetPassword" />
        void IAuthenticationStandardUser.SetPassword(string password)
        {
        }

        #endregion

        #region ISecurityUser

        bool ISecurityUser.IsActive
        {
            get { return IsActive; }
        }

        #endregion

        #region ISecurityUserWithRoles

        /// <contentfrom cref="ISecurityUserWithRoles.Roles" />
        IList<ISecurityRole> ISecurityUserWithRoles.Roles
        {
            get { return new List<ISecurityRole>(Roles.Cast<ISecurityRole>()); }
        }

        #endregion

        #region IOperationPermissionProvider

        IEnumerable<IOperationPermissionProvider> IOperationPermissionProvider.GetChildren()
        {
            // Если доступны привилегии на объекты для текущего пользователя, то в ролях нет необходимости
            if (CheckAccess(AdminSecurityOperations.GetCurrentPrivileges, SecurityObjectTypes.User))
                return new IOperationPermissionProvider[0];
            return Roles.Cast<IOperationPermissionProvider>();
        }

        IEnumerable<IOperationPermission> IOperationPermissionProvider.GetPermissions()
        {
            List<IOperationPermission> result = new List<IOperationPermission>();
            result.AddRange(GetSystemTypePermissions());
            result.AddRange(GetTypePermissions());
            return result;
        }

        #endregion

        /// <summary>
        /// Текстовое представление
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return UserName;
        }
    }

    /// <summary>
    /// Ассоциация пользователей и ролей в базе данных
    /// </summary>
    [Persistent, CustomPersistent(typeof(DatabaseRoleUsersAssociationCustomPersistent))]
    public class DatabaseUserRolesAssociation : DatabaseObjectAssociation<DatabaseUser, DatabaseRole>
    {
        /// <summary>Пользователь, которому назначена роль</summary>
        [Association("User")]
        public DatabaseUser User
        {
            get { return parent; }
            set { SetPropertyValue("User", ref parent, value); }
        }

        /// <summary>Роль, которая назначена пользователю</summary>
        [Association("Role")]
        public DatabaseRole Role
        {
            get { return child; }
            set { SetPropertyValue("Role", ref child, value); }
        }

        /// <summary>Конструктор без параметров</summary>
        public DatabaseUserRolesAssociation() { }

        /// <summary>Конструктор с указанной сессией</summary>
        /// <param name="session">Сессия для загрузки и сохранения объектов в базе данных</param>
        public DatabaseUserRolesAssociation(Session session) : base(session) { }
    }

    /// <summary>
    /// Контроллер пользователей в базе данных
    /// </summary>
    public class DatabaseUserPersistentCustom : DatabaseObjectCustomPersistent
    {
        private DBTable table;

        /// <summary>Конструктор</summary>
        public DatabaseUserPersistentCustom() : base(SecurityObjectTypes.User) { }

        /// <inheritdoc/>
        protected override SecurityResult Create(IDataStore dataStore, string objectName, Dictionary<string, object> parameters)
        {
            SecurityResult result = base.Create(dataStore, objectName, parameters);
            Alter(dataStore, objectName, parameters);
            return result;
        }

        /// <inheritdoc/>
        protected override SecurityResult Alter(IDataStore dataStore, string objectName, Dictionary<string, object> parameters)
        {
            SecurityUserInfo userInfo = new SecurityUserInfo();
            SecurityUserInfo currInfo = GetUserInfo(dataStore, objectName);
            if (parameters.ContainsKey("IsActive")) userInfo.IsActive = (bool?)parameters["IsActive"];
            if (parameters.ContainsKey("IsExpired")) userInfo.IsExpired = (bool?)parameters["IsExpired"];
            if (userInfo.IsActive == currInfo.IsActive) userInfo.IsActive = null;
            if (userInfo.IsExpired == currInfo.IsExpired) userInfo.IsExpired = null;
            return AdminSecurity(dataStore, new SecurityStatement(AdminSecurityOperations.SetUserInfo,
                new SecurityObject(SecurityObjectTypes.User, objectName), userInfo));
        }

        /// <summary>Низкоуровневое получение данных об указанном пользователе</summary>
        /// <param name="dataStore">Хранилище данных</param>
        /// <param name="objectName">Имя пользователя</param>
        /// <returns>Информация о пользователе с именем <paramref name="objectName"/></returns>
        private SecurityUserInfo GetUserInfo(IDataStore dataStore, string objectName)
        {
            if (table == null) table = XPDictionaryInformer.FindOriginalTable(typeof(DatabaseUser));
            string alias = "N0";
            SelectStatement select = new SelectStatement(table, alias);
            select.Operands.Add(new QueryOperand("IsActive", alias));
            select.Operands.Add(new QueryOperand("IsExpired", alias));
            select.Condition = new BinaryOperator(new QueryOperand("Id", alias), new OperandValue(objectName), BinaryOperatorType.Equal);
            SelectedData data = dataStore.SelectData(select);
            if (data.ResultSet.Length == 0 || data.ResultSet[0].Rows.Length == 0) return null;
            SelectStatementResultRow row = data.ResultSet[0].Rows[0];
            bool? isActive = row.Values[0] != null ? Convert.ToInt32(row.Values[0]) == 1 : (bool?)null;
            bool? isExpired = row.Values[1] != null ? Convert.ToInt32(row.Values[1]) == 1 : (bool?)null;
            return new SecurityUserInfo(isActive, isExpired);
        }
    }

    /// <summary>
    /// Контроллер ассоциации пользователей и ролей в базе данных
    /// </summary>
    public class DatabaseRoleUsersAssociationCustomPersistent : DatabaseAssociationCustomPersistent
    {
        /// <summary>Конструктор</summary>
        public DatabaseRoleUsersAssociationCustomPersistent() : base(SecurityObjectTypes.User, SecurityObjectTypes.Role) { }
    }

    /// <summary>
    /// Режимы создания разрешений на операции в базе данных
    /// </summary>
    public enum DatabasePermissionModes
    {
        /// <summary>
        /// Клиентский режим, создаются разрешения типа 
        /// <see cref="DevExpress.ExpressApp.Security.TypeOperationPermission"/>.
        /// Достаточно использовать стандартную систему безопасности со стандартным процессором серверных запросов разрешений
        /// <see cref="DevExpress.ExpressApp.Security.ServerPermissionRequestProcessor"/>.
        /// Этот режим используется по умолчанию.
        /// </summary>
        Client = 0,

        /// <summary>
        /// Режим базы данных, создаются разрешения типа
        /// <see cref="Aurum.Security.DatabaseTypePermission"/>.
        /// Необходимо переопределить процессор серверных запросов разрешений на 
        /// <see cref="Aurum.Security.DatabasePermissionRequestProcessor"/>.
        /// </summary>
        Database = 1
    }
}
