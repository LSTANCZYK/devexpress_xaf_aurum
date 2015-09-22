using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Роль пользователя в базе данных
    /// </summary>
    [ImageName("BO_Role")]
    public class ServerRole : XPBaseObject, ISecurityRole, ISecuritySystemRole, IOperationPermissionProvider
    {
        private DatabaseRole id;

        /// <summary>
        /// Объект безопасности базы данных, определяющий роль пользователя
        /// </summary>
        [Persistent, Key(false)]
        public DatabaseRole Id
        {
            get { return id; }
            set { SetPropertyValue("Id", ref id, value); }
        }

        /// <summary>Название роли</summary>
        [NotNull, RuleRequiredField]
        public string Name { get; set; }

        /// <summary>Конструктор без параметров</summary>
        public ServerRole() { }

        /// <summary>Конструктор с указанной сессией</summary>
        /// <param name="session">Сессия для загрузки и сохранения объектов в базе данных</param>
        public ServerRole(Session session) : base(session) { }

        /// <summary>Указывает, является ли объект уже созданным или новым</summary>
        /// <returns>True, если объект уже создан. False, если объект новый</returns>
        /// <remarks>Определяет доступ к редактированию свойства <see cref="Id"/></remarks>
        [Appearance("DisableId", AppearanceItemType = "ViewItem", Context = "DetailView", Enabled = false, TargetItems = "Id")]
        protected bool IsCreated()
        {
            return !Session.IsNewObject(this);
        }

        #region ISecurityRole

        string ISecurityRole.Name
        {
            get { return Id != null ? Id.Id : string.Empty; }
        }

        #endregion

        #region IOperationPermissionProvider

        /// <contentfrom cref="IOperationPermissionProvider.GetChildren" />
        public IEnumerable<IOperationPermissionProvider> GetChildren()
        {
            return Id != null ? ((IOperationPermissionProvider)Id).GetChildren() : new IOperationPermissionProvider[0];
        }

        /// <contentfrom cref="IOperationPermissionProvider.GetPermissions" />
        public IEnumerable<IOperationPermission> GetPermissions()
        {
            return Id != null ? ((IOperationPermissionProvider)Id).GetPermissions() : new IOperationPermission[0];
        }

        #endregion
    }
}
