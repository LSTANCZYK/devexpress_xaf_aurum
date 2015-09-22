using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Aurum.Xpo;
using DevExpress.ExpressApp.ConditionalAppearance;

namespace Aurum.Security
{
    /// <summary>
    /// Пользователь в базе данных
    /// </summary>
    /// <remarks>Разрешение IsAdministratorPermission не используется, так как с ним нельзя выполнить 
    /// пересечение разрешений в процессорах (см. <see cref="ComplexPermissionRequestProcessor"/>).
    /// Для кастомизации процессоров разрешений используйте событие стратегии безопасности 
    /// <see cref="DevExpress.ExpressApp.Security.SecurityStrategy.CustomizeRequestProcessors"/></remarks>
    /// <example>Пример реализации входа в базу данных в приложении Xaf<code>
    /// private void MyXafApplicationWindowsFormsApplication_LoggingOn(object sender, LogonEventArgs e)
    /// {
    ///     AuthenticationStandardLogonParameters logonParameters = (AuthenticationStandardLogonParameters)e.LogonParameters;
    ///     string connectionString = provider.ConnectionString;
    ///     if (connectionString.Contains(";user id"))
    ///         connectionString = connectionString.Remove(connectionString.IndexOf(";user id"));
    ///     connectionString += string.Format(";user id={0};password={1}", logon.UserName, logon.Password);
    ///     ObjectSpaceProvider.ConnectionString = connectionString;
    /// }
    /// </code></example>
    /// <example>Пример замены процессора разрешений<code>
    /// private void SecurityStrategyComplex_CustomizeRequestProcessors(object sender, CustomizeRequestProcessorsEventArgs e)
    /// {
    ///     ServerPermissionRequestProcessor processor = (ServerPermissionRequestProcessor)e.Processors[typeof(ServerPermissionRequest)];
    ///     e.Processors[typeof(ServerPermissionRequest)] = new AdminPermissionRequestProcessor(processor, e.Permissions);
    /// }
    /// </code></example>
    [ImageName("BO_User")]
    public class ServerUser : XPBaseObject, ISecurityUserWithRoles, ISecurityUser, IAuthenticationStandardUser, IOperationPermissionProvider
    {
        private DatabaseUser id;

        /// <summary>
        /// Объект безопасности базы данных, определяющий пользователя
        /// </summary>
        [Persistent, Key(false)]
        public DatabaseUser Id
        {
            get { return id; }
            set { SetPropertyValue("Id", ref id, value); }
        }

        /// <summary>Фамилия пользователя</summary>
        [NotNull, RuleRequiredField]
        public string LastName { get; set; }

        /// <summary>Имя пользователя</summary>
        public string FirstName { get; set; }

        /// <summary>Отчество пользователя</summary>
        public string MiddleName { get; set; }

        /// <summary>Группы, в которые входит пользователь</summary>
        [Association("Group-Users")]
        public XPCollection<UserGroup> Groups
        {
            get { return GetCollection<UserGroup>("Groups"); }
        }

        /// <summary>Конструктор без параметров</summary>
        public ServerUser() { }

        /// <summary>Конструктор с указанной сессией</summary>
        /// <param name="session">Сессия для загрузки и сохранения объектов в базе данных</param>
        public ServerUser(Session session) : base(session) { }

        /// <summary>
        /// Текстовое представление
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [PersistentAlias("Concat(Id, ' (', Id, ' ', LastName, ' ', FirstName, ' ', MiddleName, ')')")]
        public string Text
        {
            get { return (string)EvaluateAlias("Text"); }
        }

        /// <summary>
        /// Текстовое представление
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0} ({1} {2} {3})", Id, LastName, FirstName, MiddleName);
        }

        /// <summary>Указывает, является ли объект уже созданным или новым</summary>
        /// <returns>True, если объект уже создан. False, если объект новый</returns>
        /// <remarks>Определяет доступ к редактированию свойства <see cref="Id"/></remarks>
        [Appearance("DisableId", AppearanceItemType = "ViewItem", Context = "DetailView", Enabled = false, TargetItems = "Id")]
        protected bool IsCreated()
        {
            return !Session.IsNewObject(this);
        }

        #region IAuthenticationStandardUser

        /// <contentfrom cref="IAuthenticationStandardUser.ChangePasswordOnFirstLogon" />
        [NonPersistent]
        public bool ChangePasswordOnFirstLogon
        {
            get { return Id != null ? ((IAuthenticationStandardUser)Id).ChangePasswordOnFirstLogon : false; }
            set { if (Id != null) ((IAuthenticationStandardUser)Id).ChangePasswordOnFirstLogon = value; }
        }

        /// <contentfrom cref="IAuthenticationStandardUser.ComparePassword" />
        public bool ComparePassword(string password)
        {
            return Id != null ? ((IAuthenticationStandardUser)Id).ComparePassword(password) : true;
        }

        /// <contentfrom cref="IAuthenticationStandardUser.SetPassword" />
        public void SetPassword(string password)
        {
            if (Id != null) ((IAuthenticationStandardUser)Id).SetPassword(password);
        }

         /// <contentfrom cref="IAuthenticationStandardUser.UserName" />
        /// <contentfrom cref="ISecurityUser.UserName" />
        [Browsable(false)]
        public string UserName 
        { 
            get { return Id != null ? Id.UserName : string.Empty; }
        }

       #endregion

        #region ISecurityUser

        bool ISecurityUser.IsActive
        {
            get { return Id != null ? ((ISecurityUser)Id).IsActive : true; }
        }

        #endregion

        #region ISecurityUserWithRoles

        /// <contentfrom cref="ISecurityUserWithRoles.Roles" />
        IList<ISecurityRole> ISecurityUserWithRoles.Roles
        {
            get { return Id != null ? ((ISecurityUserWithRoles)Id).Roles : new List<ISecurityRole>(); }
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
