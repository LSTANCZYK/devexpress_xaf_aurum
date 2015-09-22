using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Aurum.Xpo;

namespace Aurum.Security
{
    /// <summary>
    /// Роль пользователя в базе данных (объект базы данных)
    /// </summary>
    [Persistent, CustomPersistent(typeof(DatabaseRoleCustomPersistent)), ImageName("BO_Role")]
    public sealed class DatabaseRole : DatabaseObject, ISecurityRole, ISecuritySystemRole, IOperationPermissionProvider
    {
        /// <summary>Конструктор без параметров</summary>
        public DatabaseRole() { }

        /// <summary>Конструктор с указанной сессией</summary>
        /// <param name="session">Сессия для загрузки и сохранения объектов в базе данных</param>
        public DatabaseRole(Session session) : base(session) { }

        /// <summary>Родительские роли, связанные с текущей дочерней ролью</summary>
        [Association("Child"), Browsable(false)]
        public IList<DatabaseRoleRolesAssociation> ToChild
        {
            get { return GetList<DatabaseRoleRolesAssociation>("ToChild"); }
        }

        /// <summary>Дочерние роли, связанные с текущей родительской ролью</summary>
        [Association("Parent"), Browsable(false)]
        public IList<DatabaseRoleRolesAssociation> ToParent
        {
            get { return GetList<DatabaseRoleRolesAssociation>("ToParent"); }
        }

        /// <summary>Пользователи, связанные с текущей ролью</summary>
        [Association("Role"), Browsable(false)]
        public IList<DatabaseUserRolesAssociation> ToRole
        {
            get { return GetList<DatabaseUserRolesAssociation>("ToRole"); }
        }

        /// <summary>Роли, являющиеся дочерними по отношению к текущей</summary>
        /// <value>Роли, разрешения которых включены в текущую роль</value>
        [ManyToManyAlias("ToParent", "Child")]
        public IList<DatabaseRole> ChildRoles
        {
            get { return GetList<DatabaseRole>("ChildRoles"); }
        }

        /// <summary>Роли, являющиеся родительскими по отношению к текущей</summary>
        /// <value>Роли, которые включают в себя разрешения текущей роли</value>
        [ManyToManyAlias("ToChild", "Parent")]
        public IList<DatabaseRole> ParentRoles
        {
            get { return GetList<DatabaseRole>("ParentRoles"); }
        }

        /// <summary>Пользователи, которым назначена текущая роль</summary>
        [ManyToManyAlias("ToRole", "User")]
        public IList<DatabaseUser> Users
        {
            get { return GetList<DatabaseUser>("Users"); }
        }

        /// <summary>
        /// Привилегии роли в базе данных
        /// </summary>
        [Association, Aggregated]
        public IList<DatabasePrivilege> Privileges
        {
            get
            {
                LoadPrivileges();
                return GetList<DatabasePrivilege>("Privileges");
            }
        }

        #region Прямая и обратная конвертация привилегий на таблицы в базе данных в привилегии на типы данных

        // Возвращает привилегии на таблицы для указанной привилегии роли в базе данных
        private IEnumerable<SecurityObject> GetSecurityObjects(DatabasePrivilege privilege)
        {
            List<SecurityObject> result = new List<SecurityObject>();
            SecurityTableRights rights = SecurityTableRights.None;
            if (privilege.AllowSelect) rights |= SecurityTableRights.Select;
            if (privilege.AllowInsert) rights |= SecurityTableRights.Insert;
            if (privilege.AllowUpdate) rights |= SecurityTableRights.Update;
            if (privilege.AllowDelete) rights |= SecurityTableRights.Delete;
            if (rights == SecurityTableRights.None) return result;
            foreach (string table in XPDictionaryInformer.GetReferencedTables(privilege.TargetType))
                result.Add(new SecurityObject(table, rights));
            return result;
        }

        // Возвращает привилегии роли в базе данных для указанных привилегий на таблицы
        private IEnumerable<DatabasePrivilege> GetDatabasePrivileges(List<SecurityObject> tablePrivs)
        {
            List<DatabasePrivilege> result = new List<DatabasePrivilege>();
            List<Type> types = new List<Type>();
            foreach (SecurityObject tablePriv in tablePrivs)
                types.AddRange(XPDictionaryInformer.GetReferencedTypes(tablePriv.ObjectName));
            foreach (Type type in types.Distinct())
            {
                bool allowSelect = true, allowInsert = true, allowUpdate = true, allowDelete = true, exists = false;
                foreach (string table in XPDictionaryInformer.GetReferencedTables(type))
                {
                    allowSelect &= tablePrivs.Exists(so => so.ObjectName == table && (so.TableRights & SecurityTableRights.Select) == SecurityTableRights.Select);
                    allowInsert &= tablePrivs.Exists(so => so.ObjectName == table && (so.TableRights & SecurityTableRights.Insert) == SecurityTableRights.Insert);
                    allowUpdate &= tablePrivs.Exists(so => so.ObjectName == table && (so.TableRights & SecurityTableRights.Update) == SecurityTableRights.Update);
                    allowDelete &= tablePrivs.Exists(so => so.ObjectName == table && (so.TableRights & SecurityTableRights.Delete) == SecurityTableRights.Delete);
                    exists = true;
                }
                if (exists && (allowSelect || allowInsert || allowUpdate || allowDelete))
                    new DatabasePrivilege(Session, this, type, allowSelect, allowInsert, allowUpdate, allowDelete);
            }
            return result;
        }

        #endregion

        private bool isLoadedPrivileges = false;

        /// <summary>
        /// Загрузка привилегий роли
        /// </summary>
        private void LoadPrivileges()
        {
            if (!isLoadedPrivileges && Session != null && !string.IsNullOrEmpty(Session.ConnectionString))
            {
                SecurityResult queryChilds = AdminSecurity(new SecurityStatement(
                    AdminSecurityOperations.GetRolePrivileges, new SecurityObject(SecurityObjectTypes.Role, Id)));
                List<SecurityObject> tablePrivs = new List<SecurityObject>(
                    queryChilds.Objects.Where(so => so.ObjectType == SecurityObjectTypes.Table));
                GetDatabasePrivileges(tablePrivs);
                isLoadedPrivileges = true;
            }
        }

        /// <summary>
        /// Сохранение привилегий роли
        /// </summary>
        private void SavePrivileges()
        {
            // Оригинальная и измененная коллекции
            List<SecurityObject> original = new List<SecurityObject>();
            List<SecurityObject> modified = new List<SecurityObject>();
            SecurityObject thisObject = new SecurityObject(SecurityObjectTypes.Role, Id);
            SecurityResult result = AdminSecurity(new SecurityStatement(AdminSecurityOperations.GetRolePrivileges, thisObject));
            original.AddRange(result.Objects.Where(so => so.ObjectType == SecurityObjectTypes.Table));
            foreach (DatabasePrivilege privilege in Privileges)
                modified.AddRange(GetSecurityObjects(privilege));

            // Список команд
            List<SecurityStatement> statements = new List<SecurityStatement>();
            foreach (SecurityObject table in SecurityObject.TablesDiff(original, modified))
                statements.Add(new SecurityStatement(AdminSecurityOperations.RevokeFrom, table, thisObject));
            foreach (SecurityObject table in SecurityObject.TablesDiff(modified, original))
                statements.Add(new SecurityStatement(AdminSecurityOperations.GrantTo, table, thisObject));
            AdminSecurity(statements.ToArray());
        }

        /// <summary>Устанавливает флаг обновления привилегий</summary>
        internal void UpdatePrivileges()
        {
            OnChanged("Privileges");
        }

        /// <inheritdoc/>
        protected override void OnSaved()
        {
            base.OnSaved();
            if (Session != null && !string.IsNullOrEmpty(Session.ConnectionString))
                SavePrivileges();
        }

        #region ISecurityRole

        string ISecurityRole.Name
        {
            get { return Id; }
        }

        #endregion

        #region IOperationPermissionProvider

        IEnumerable<IOperationPermissionProvider> IOperationPermissionProvider.GetChildren()
        {
            return ChildRoles.Cast<IOperationPermissionProvider>();
        }

        IEnumerable<IOperationPermission> IOperationPermissionProvider.GetPermissions()
        {
            List<IOperationPermission> result = new List<IOperationPermission>();
            foreach (DatabasePrivilege privilege in Privileges)
                result.AddRange(privilege.GetPermissions());
            return result; ;
        }

        #endregion

        /// <summary>
        /// Текстовое представление
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Id;
        }
    }

    /// <summary>
    /// Ассоциация иерархии ролей в базе данных
    /// </summary>
    [Persistent, CustomPersistent(typeof(DatabaseRoleRolesAssociationCustomPersistent))]
    public class DatabaseRoleRolesAssociation : DatabaseObjectAssociation<DatabaseRole, DatabaseRole>
    {
        /// <summary>Родительская роль в иерархии</summary>
        [Association("Parent")]
        public DatabaseRole Parent
        {
            get { return parent; }
            set { SetPropertyValue("Parent", ref parent, value); }
        }

        /// <summary>Дочерняя роль в иерархии</summary>
        [Association("Child")]
        public DatabaseRole Child
        {
            get { return child; }
            set { SetPropertyValue("Child", ref child, value); }
        }

        /// <summary>Конструктор без параметров</summary>
        public DatabaseRoleRolesAssociation() { }

        /// <summary>Конструктор с указанной сессией</summary>
        /// <param name="session">Сессия для загрузки и сохранения объектов в базе данных</param>
        public DatabaseRoleRolesAssociation(Session session) : base(session) { }
    }

    /// <summary>
    /// Контроллер ролей пользователя в базе данных
    /// </summary>
    public class DatabaseRoleCustomPersistent : DatabaseObjectCustomPersistent
    {
        /// <summary>Конструктор</summary>
        public DatabaseRoleCustomPersistent() : base(SecurityObjectTypes.Role) { }
    }

    /// <summary>
    /// Контроллер ассоциации иерархии ролей в базе данных
    /// </summary>
    public class DatabaseRoleRolesAssociationCustomPersistent : DatabaseAssociationCustomPersistent
    {
        /// <summary>Конструктор</summary>
        public DatabaseRoleRolesAssociationCustomPersistent() : base(SecurityObjectTypes.Role, SecurityObjectTypes.Role) { }
    }
}
