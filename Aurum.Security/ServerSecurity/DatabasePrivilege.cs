using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Aurum.Xpo;

namespace Aurum.Security
{
    /// <summary>
    /// Привилегия роли в базе данных
    /// </summary>
    [Persistent, CustomPersistent(typeof(DatabasePrivilegeCustomPermission))]
    [OptimisticLocking(false), DeferredDeletion(false)]
    [ImageName("BO_Security_Permission_Type")]
    public sealed class DatabasePrivilege : XPBaseObject
    {
        private string key;
        private DatabaseRole role;
        private Type targetType;
        private bool allowSelect;
        private bool allowInsert;
        private bool allowUpdate;
        private bool allowDelete;

        /// <summary>
        /// Ключ
        /// </summary>
        [Persistent, Key(false), Browsable(false)]
        public string Key
        {
            get { return key ?? (role != null && targetType != null ? string.Concat(role.Id, ".", targetType.FullName) : string.Empty); }
            set { key = value; }
        }

        /// <summary>Конструктор без параметров</summary>
        public DatabasePrivilege() { }

        /// <summary>Конструктор с указанной сессией</summary>
        /// <param name="session">Сессия для загрузки и сохранения объектов в базе данных</param>
        public DatabasePrivilege(Session session) : base(session) { }

        /// <summary>Конструктор с указанной сессией, используемый для загрузки привилегий роли</summary>
        /// <param name="session">Сессия для загрузки и сохранения объектов в базе данных</param>
        /// <param name="role">Роль пользователя в базе данных</param>
        /// <param name="targetType">Тип объектов, к которым относится разрешение на операции</param>
        /// <param name="allowSelect">Разрешение на выборку данных</param>
        /// <param name="allowInsert">Разрешение на добавление данных</param>
        /// <param name="allowUpdate">Разрешение на изменение данных</param>
        /// <param name="allowDelete">Разрешение на удаление данных</param>
        /// <remarks>Конструктор используется для загрузки привилегий роли и не вызывает обновления соответствующего свойства</remarks>
        public DatabasePrivilege(Session session, DatabaseRole role, Type targetType,
            bool allowSelect, bool allowInsert, bool allowUpdate, bool allowDelete)
            : base(session)
        {
            this.role = role;
            this.targetType = targetType;
            this.allowSelect = allowSelect;
            this.allowInsert = allowInsert;
            this.allowUpdate = allowUpdate;
            this.allowDelete = allowDelete;
        }

        /// <summary>
        /// Роль пользователя в базе данных
        /// </summary>
        [Association]
        public DatabaseRole Role
        {
            get { return role; }
            set { DatabaseRole oldRole = role; SetPropertyValue("Role", ref role, value); UpdatePrivileges(role ?? oldRole); }
        }

        /// <summary>
        /// Тип объектов, к которым относится разрешение на операции
        /// </summary>
        [ValueConverter(typeof(TypeToStringConverter))]
        [Size(256)]
        [VisibleInDetailView(false), VisibleInListView(false)]
        [RuleRequiredField("DatabasePermission_TargetType_RuleRequiredField", DefaultContexts.Save)]
        [TypeConverter(typeof(DevExpress.ExpressApp.Security.Strategy.SecurityStrategyTargetTypeConverter))]
        public Type TargetType
        {
            get { return targetType; }
            set { SetPropertyValue<Type>("TargetType", ref targetType, value); UpdatePrivileges(role); }
        }

        /// <summary>Описание типа объектов, к которым относится разрешение на операции</summary>
        public string TypeCaption
        {
            get
            {
                if (TargetType != null)
                {
                    string classCaption = CaptionHelper.GetClassCaption(TargetType.FullName);
                    return string.IsNullOrEmpty(classCaption) ? TargetType.Name : classCaption;
                }
                return string.Empty;
            }
        }

        /// <summary>Разрешение на выборку данных</summary>
        public bool AllowSelect
        {
            get { return allowSelect; }
            set { SetPropertyValue("AllowSelect", ref allowSelect, value); UpdatePrivileges(role); }
        }

        /// <summary>Разрешение на добавление данных</summary>
        public bool AllowInsert
        {
            get { return allowInsert; }
            set { SetPropertyValue("AllowInsert", ref allowInsert, value); UpdatePrivileges(role); }
        }

        /// <summary>Разрешение на изменение данных</summary>
        public bool AllowUpdate
        {
            get { return allowUpdate; }
            set { SetPropertyValue("AllowUpdate", ref allowUpdate, value); UpdatePrivileges(role); }
        }

        /// <summary>Разрешение на удаление данных</summary>
        public bool AllowDelete
        {
            get { return allowDelete; }
            set { SetPropertyValue("AllowDelete", ref allowDelete, value); UpdatePrivileges(role); }
        }

        // Обновление привилегий роли в базе данных
        private void UpdatePrivileges(DatabaseRole role)
        {
            if (role != null) role.UpdatePrivileges();
        }

        /// <summary>
        /// Возвращает разрешения на операции текущей привилегии роли
        /// </summary>
        /// <returns>Разрешения на операции в базе данных</returns>
        public IEnumerable<IOperationPermission> GetPermissions()
        {
            List<IOperationPermission> result = new List<IOperationPermission>();
            if (DatabaseUser.PermissionMode == DatabasePermissionModes.Database)
                result.Add(new DatabaseTypePermission(TargetType, AllowSelect, AllowInsert, AllowUpdate, AllowDelete));
            else if (DatabaseUser.PermissionMode == DatabasePermissionModes.Client)
            {
                if (AllowSelect) result.Add(new TypeOperationPermission(TargetType, SecurityOperations.Navigate));
                if (AllowSelect) result.Add(new TypeOperationPermission(TargetType, SecurityOperations.Read));
                if (AllowInsert) result.Add(new TypeOperationPermission(TargetType, SecurityOperations.Create));
                if (AllowUpdate) result.Add(new TypeOperationPermission(TargetType, SecurityOperations.Write));
                if (AllowDelete) result.Add(new TypeOperationPermission(TargetType, SecurityOperations.Delete));
            }
            return result;
        }
    }

    /// <summary>
    /// Контроллер привилегии на таблицу в базе данных
    /// </summary>
    public class DatabasePrivilegeCustomPermission : DatabaseObjectCustomPersistent, ICustomPersistentQuery
    {
        /// <summary>Конструктор</summary>
        public DatabasePrivilegeCustomPermission() : base(SecurityObjectTypes.Table) { }

        /// <inheritdoc/>
        protected override SecurityResult Create(IDataStore dataStore, string objectName, Dictionary<string, object> parameters)
        {
            return new SecurityResult();
        }

        /// <inheritdoc/>
        protected override SecurityResult Drop(IDataStore dataStore, string objectName)
        {
            return new SecurityResult();
        }

        /// <contentfrom cref="ICustomPersistentQuery.Select" />
        public SelectStatementResult Select(IDataStore dataStore, SelectStatement statement)
        {
            return new SelectStatementResult(new SelectStatementResultRow[] { });
        }
    }
}
