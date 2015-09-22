using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;

namespace Aurum.Security
{
    /// <summary>
    /// Процессор запросов разрешений на операции в базе данных
    /// </summary>
    public class DatabasePermissionRequestProcessor : IPermissionRequestProcessor, ISecurityCriteriaProvider2
    {
        /// <summary>Ложное условие</summary>
        private const string falseCriteria = "1=0";

        private IPermissionDictionary permissions;
        private Dictionary<Type, DatabaseTypePermission> typePermissions;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="permissions">Справочник разрешений</param>
        public DatabasePermissionRequestProcessor(IPermissionDictionary permissions)
        {
            this.permissions = permissions;
        }

        private Type GetTypeToProcess(Type type)
        {
            ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(type);
            if (typeInfo != null && typeInfo.IsPersistent) return typeInfo.Type;
            return null;
        }

        private Type GetTypeToProcess(Type targetType, object targetObject)
        {
            Type type;
            if (targetObject != null)
            {
                type = targetObject.GetType();
                if (!targetType.IsAssignableFrom(type))
                    throw new InvalidCastException(ServerSecurityLogLocalizer.Active.GetLocalizedString(ServerSecurityLogMessagesId.IncompatibleTypes));
            }
            else type = targetType;
            return GetTypeToProcess(type);
        }

        private DatabaseTypePermission GetTypePermission(Type type)
        {
            // Инициализация разрешений
            if (typePermissions == null)
            {
                typePermissions = new Dictionary<Type, DatabaseTypePermission>();
                foreach (DatabaseTypePermission permission in permissions.GetPermissions<DatabaseTypePermission>())
                    if (!typePermissions.ContainsKey(permission.ObjectType))
                        typePermissions.Add(permission.ObjectType, permission);
                    else
                    {
                        DatabaseTypePermission o = typePermissions[permission.ObjectType];
                        typePermissions[permission.ObjectType] = new DatabaseTypePermission(permission.ObjectType,
                            permission.CanSelect || o.CanSelect,
                            permission.CanInsert || o.CanInsert,
                            permission.CanUpdate || o.CanUpdate,
                            permission.CanDelete || o.CanDelete);
                    }
            }
            DatabaseTypePermission result;
            return typePermissions.TryGetValue(type, out result) ? result : null;
        }

        /// <summary>Выполняет проверку разрешения на операцию в базе данных</summary>
        /// <param name="permissionRequest">Запрос разрешения на операцию</param>
        /// <returns>True, если операция разрешена, иначе false</returns>
        protected bool IsGrantedCore(ServerPermissionRequest permissionRequest)
        {
            if (permissionRequest.ObjectType == null)
                throw new InvalidOperationException(ServerSecurityLogLocalizer.Active.GetLocalizedString(ServerSecurityLogMessagesId.CannotObtainType));
            Type targetType = GetTypeToProcess(permissionRequest.ObjectType, permissionRequest.TargetObject);
            if (targetType == null) return true;
            DatabaseTypePermission permission = GetTypePermission(targetType);
            if (permission == null) return false;
            switch (permissionRequest.Operation)
            {
                case SecurityOperations.Navigate:
                case SecurityOperations.Read: return permission.CanSelect;
                case SecurityOperations.Create: return permission.CanInsert;
                case SecurityOperations.Write: return permission.CanUpdate;
                case SecurityOperations.Delete: return permission.CanDelete;
            }
            return false;
        }

        #region IPermissionRequestProcessor

        /// <contentfrom cref="IPermissionRequestProcessor.IsGranted"/>
        public bool IsGranted(IPermissionRequest permissionRequest)
        {
            return IsGrantedCore((ServerPermissionRequest)permissionRequest);
        }

        #endregion

        #region ISecurityCriteriaProvider2

        /// <contentfrom cref="ISecurityCriteriaProvider2.GetObjectCriteria"/>
        public IList<string> GetObjectCriteria(Type type) 
        {
            Type targetType = GetTypeToProcess(type);
            if (targetType == null) 
                return new string[0];
            DatabaseTypePermission permission = GetTypePermission(targetType);
            if (permission != null && permission.CanSelect) 
                return new string[0];
            return new string[1] { falseCriteria };
		}

        /// <contentfrom cref="ISecurityCriteriaProvider2.GetMemberCriteria"/>
        public IList<string> GetMemberCriteria(Type type, string memberName) 
        {
            Type targetType = GetTypeToProcess(type);
            if (targetType == null)
                return new string[0];
            DatabaseTypePermission permission = GetTypePermission(targetType);
            if (permission != null && permission.CanSelect)
            {
                ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(type);
			    IMemberInfo memberInfo = typeInfo.FindMember(memberName);
                if (memberInfo != null && memberInfo.IsAssociation)
                {
                    DatabaseTypePermission memberPermission = GetTypePermission(memberInfo.MemberType);
                    if (memberPermission == null || !memberPermission.CanSelect)
                        return new string[1] { falseCriteria };
                }
                return new string[0];
            }
            return new string[1] { falseCriteria };
		}
        
        #endregion
    }

    /// <summary>
    /// Разрешение на операции с объектами в базе данных
    /// </summary>
    public class DatabaseTypePermission : IOperationPermission
    {
        /// <summary>Название разрешений на операцию в базе данных</summary>
        public const string OperationName = "Privilege";

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="type">Тип объекта</param>
        /// <param name="select">Разрешение на выборку объектов</param>
        /// <param name="insert">Разрешение на добавление объекта</param>
        /// <param name="update">Разрешение на изменение объекта</param>
        /// <param name="delete">Разрешение на удаление объекта</param>
        public DatabaseTypePermission(Type type, bool select, bool insert, bool update, bool delete)
        {
            this.ObjectType = type;
            this.CanSelect = select;
            this.CanInsert = insert;
            this.CanUpdate = update;
            this.CanDelete = delete;
        }

        /// <summary>Тип объекта</summary>
        public Type ObjectType { get; private set; }

        /// <summary>Разрешение на выборку объектов</summary>
        public bool CanSelect { get; private set; }
        
        /// <summary>Разрешение на добавление объекта</summary>
        public bool CanInsert { get; private set; }
        
        /// <summary>Разрешение на изменение объекта</summary>
        public bool CanUpdate { get; private set; }
        
        /// <summary>Разрешение на удаление объекта</summary>
        public bool CanDelete { get; private set; }

        /// <contentfrom cref="IOperationPermission.Operation"/>
        string IOperationPermission.Operation
        {
            get { return OperationName; }
        }
    }
}
