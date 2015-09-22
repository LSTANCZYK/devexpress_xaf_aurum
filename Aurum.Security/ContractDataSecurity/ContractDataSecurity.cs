using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;

namespace Aurum.Security
{
    /// <summary>
    /// Система безопасности операций с данными, основанная на контрактах
    /// </summary>
    public abstract class ContractDataSecurity : ISelectDataSecurity
    {
        private IContractContext context;
        private Type IContractObjectType = typeof(IContractObject);

        /// <summary>
        /// Контекст безопасности, используемый при проверке разрешений на операции
        /// </summary>
        public IContractContext Context
        {
            get { return context; }
            internal set { context = value; }
        }

        /// <contentfrom cref="M:IRequestSecurity.IsGranted" />
        public IList<bool> IsGranted(IList<IPermissionRequest> permissionRequests)
        {
            Guard.ArgumentNotNull(permissionRequests, "permissionRequests");
            List<bool> result = new List<bool>(permissionRequests.Count);
            for (int i = 0; i < permissionRequests.Count; i++)
            {
                result.Add(IsGranted(permissionRequests[i]));
            }
            return result;
        }

        /// <contentfrom cref="M:IRequestSecurity.IsGranted" />
        public bool IsGranted(IPermissionRequest permissionRequest)
        {
            if (permissionRequest is ServerPermissionRequest)
            {
                ServerPermissionRequest request = (ServerPermissionRequest)permissionRequest;
                IContractObject contractObject = request.TargetObject as IContractObject;
                if (contractObject != null && context != null)
                {
                    if (!string.IsNullOrEmpty(request.MemberName) && (
                        request.Operation == SecurityOperations.Read ||
                        request.Operation == SecurityOperations.Write))
                        return IsGranted(contractObject, request.Operation, request.MemberName, context);
                    else
                        return IsGranted(contractObject, request.Operation, context);
                }
            }
            return true;
        }

        /// <contentfrom cref="ISecurityCriteriaProvider2.GetMemberCriteria" />
        public IList<string> GetMemberCriteria(Type type, string memberName)
        {
            return new string[0];
        }

        /// <contentfrom cref="ISecurityCriteriaProvider2.GetObjectCriteria" />
        /// <remarks>Условие выборки данных должно совпадать с проверкой доступа на чтение объектов</remarks>
        public IList<string> GetObjectCriteria(Type type)
        {
            if (IContractObjectType.IsAssignableFrom(type) && context != null)
                return GetObjectCriteria(type, context) ?? new string[0];
            return new string[0];
        }

        /// <summary>
        /// Возвращает флаг разрешения на операцию над объектом
        /// </summary>
        /// <param name="contractObject">Объект данных, над которым должна быть выполнена операция</param>
        /// <param name="operation">Операция, разрешение на которую запрашивается</param>
        /// <param name="context">Контекст безопасности операций с данными</param>
        /// <returns>True - если операция над объектом разрешена, иначе false</returns>
        /// <remarks>Базовые операции над объектом описаны в классе <see cref="T:SecurityOperations"/></remarks>
        public abstract bool IsGranted(IContractObject contractObject, string operation, IContractContext context);

        /// <summary>
        /// Возвращает флаг разрешения на операцию со свойством или полем объекта
        /// </summary>
        /// <param name="contractObject">Объект данных, над которым должна быть выполнена операция</param>
        /// <param name="operation">Операция, разрешение на которую запрашивается</param>
        /// <param name="memberName">Свойство или поле объекта, над которым должна быть выполнена операция</param>
        /// <param name="context">Контекст безопасности операций с данными</param>
        /// <returns>True - если операция над объектом разрешена, иначе false</returns>
        /// <remarks>Базовые операции над объектом описаны в классе <see cref="T:SecurityOperations"/></remarks>
        public abstract bool IsGranted(IContractObject contractObject, string operation, string memberName, IContractContext context);

        /// <summary>
        /// Возвращает условие безопасной выборки данных указанного типа
        /// </summary>
        /// <param name="type">Тип данных, для которых возвращается условие выборки</param>
        /// <param name="context">Контекст безопасности, в котором определяется условие</param>
        /// <returns>Условие, ограничивающее выборку данных типа <b>type</b></returns>
        public abstract IList<string> GetObjectCriteria(Type type, IContractContext context);
    }

    /// <summary>
    /// Система безопасности операций с данными с типизированным контекстом безопасности
    /// </summary>
    /// <typeparam name="ContextType">Тип контекста безопасности</typeparam>
    public abstract class ContractDataSecurity<ContextType> : ContractDataSecurity
        where ContextType : IContractContext
    {
        /// <inheritdoc/>
        public sealed override bool IsGranted(IContractObject contractObject, string operation, IContractContext context)
        {
            // Условие выборки данных должно совпадать с проверкой доступа на чтение объектов, поэтому реализация прав на чтение для отдельного объекта запрещена
            if (contractObject is IContractObjectImplementation<ContextType> && context is ContextType && operation != SecurityOperations.Read)
                return ((IContractObjectImplementation<ContextType>)contractObject).IsGranted(operation, (ContextType)context);
            if (contractObject is IContractObject<ContextType> && context is ContextType)
                return IsGranted((IContractObject<ContextType>)contractObject, operation, (ContextType)context);
            return true;
        }

        /// <inheritdoc/>
        public sealed override bool IsGranted(IContractObject contractObject, string operation, string memberName, IContractContext context)
        {
            if (contractObject is IContractObjectImplementation<ContextType> && context is ContextType)
                return ((IContractObjectImplementation<ContextType>)contractObject).IsGranted(operation, memberName, (ContextType)context);
            if (contractObject is IContractObject<ContextType> && context is ContextType)
                return IsGranted((IContractObject<ContextType>)contractObject, operation, memberName, (ContextType)context);
            return true;
        }

        /// <inheritdoc/>
        public sealed override IList<string> GetObjectCriteria(Type type, IContractContext context)
        {
            if (context is ContextType)
                return GetObjectCriteria(type, (ContextType)context);
            return new string[0];
        }

        /// <summary>
        /// Возвращает флаг разрешения на операцию над объектом
        /// </summary>
        /// <param name="contractObject">Объект данных, над которым должна быть выполнена операция</param>
        /// <param name="operation">Операция, разрешение на которую запрашивается</param>
        /// <param name="context">Контекст безопасности операций с данными</param>
        /// <returns>True - если операция над объектом разрешена, иначе false</returns>
        /// <remarks>Базовые операции над объектом описаны в классе <see cref="T:SecurityOperations"/></remarks>
        public abstract bool IsGranted(IContractObject<ContextType> contractObject, string operation, ContextType context);

        /// <summary>
        /// Возвращает флаг разрешения на операцию со свойством или полем объекта
        /// </summary>
        /// <param name="contractObject">Объект данных, над которым должна быть выполнена операция</param>
        /// <param name="operation">Операция, разрешение на которую запрашивается</param>
        /// <param name="memberName">Свойство или поле объекта, над которым должна быть выполнена операция</param>
        /// <param name="context">Контекст безопасности операций с данными</param>
        /// <returns>True - если операция над объектом разрешена, иначе false</returns>
        /// <remarks>Базовые операции над объектом описаны в классе <see cref="T:SecurityOperations"/></remarks>
        public abstract bool IsGranted(IContractObject<ContextType> contractObject, string operation, string memberName, ContextType context);

        /// <summary>
        /// Возвращает условие безопасной выборки данных указанного типа
        /// </summary>
        /// <param name="type">Тип данных, для которых возвращается условие выборки</param>
        /// <param name="context">Контекст безопасности, в котором определяется условие</param>
        /// <returns>Условие, ограничивающее выборку данных типа <b>type</b></returns>
        public abstract IList<string> GetObjectCriteria(Type type, ContextType context);
    }

    /// <summary>
    /// Поставщик безопасности операций с данными, основанной на контрактах
    /// </summary>
    public abstract class ContractDataSecurityProvider : ISelectDataSecurityProvider
    {
        private ISecurityStrategyBase securityStrategy;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="securityStrategy">Стратегия безопасности, предоставляющая информацию о текущем пользователе системы</param>
        public ContractDataSecurityProvider(ISecurityStrategyBase securityStrategy)
        {
            Guard.ArgumentNotNull(securityStrategy, "securityStrategy");
            this.securityStrategy = securityStrategy;
        }

        /// <summary>
        /// Возвращает новый контекст безопасности операций с данными
        /// </summary>
        /// <returns>Контекст безопасности операций с данными, в рамках которого выполняется проверка разрешений на доступ к операциям</returns>
        public abstract IContractContext CreateContractContext();

        /// <summary>
        /// Возвращает новую типизированную систему безопасности операций с данными с указанным контекстом безопасности
        /// </summary>
        /// <param name="context">Контекст безопасности</param>
        public abstract ISelectDataSecurity CreateContractSecurity(IContractContext context);

        /// <contentfrom cref="ISelectDataSecurityProvider.CreateSelectDataSecurity" />
        public ISelectDataSecurity CreateSelectDataSecurity()
        {
            if (securityStrategy.User == null || securityStrategy.LogonObjectSpace == null)
                return new EmptySelectDataSecurity();

            IObjectSpace objectSpace = securityStrategy.LogonObjectSpace;
            IContractContext context = CreateContractContext();
            context.Initialize(objectSpace, securityStrategy.User);
            return CreateContractSecurity(context);
        }
    }

    /// <summary>
    /// Поставщик безопасности операций с данными с типизированной системой и контекстом
    /// </summary>
    /// <typeparam name="SecurityType">Тип системы безопасности</typeparam>
    /// <typeparam name="ContextType">Тип контекста безопасности</typeparam>
    public abstract class ContractDataSecurityProvider<SecurityType, ContextType> : ContractDataSecurityProvider
        where SecurityType : ContractDataSecurity<ContextType>, new()
        where ContextType : IContractContext, new()
    {
        /// <summary>Конструктор</summary>
        /// <param name="securityStrategy">Стратегия безопасности, предоставляющая информацию о текущем пользователе системы</param>
        public ContractDataSecurityProvider(ISecurityStrategyBase securityStrategy) : base(securityStrategy) { }

        /// <inheritfrom/>
        public sealed override IContractContext CreateContractContext()
        {
            return CreateContractTypedContext();
        }

        /// <summary>
        /// Возвращает новый типизированный контекст безопасности операций с данными
        /// </summary>
        /// <returns>Контекст безопасности операций с данными, в рамках которого выполняется проверка разрешений на доступ к операциям</returns>
        public virtual ContextType CreateContractTypedContext()
        {
            return new ContextType();
        }

        /// <inheritdoc/>
        public sealed override ISelectDataSecurity CreateContractSecurity(IContractContext context)
        {
            return CreateContractTypedSecurity((ContextType)context);
        }

        /// <summary>
        /// Возвращает новую типизированную систему безопасности операций с данными с указанным контекстом безопасности
        /// </summary>
        /// <param name="context">Контекст безопасности</param>
        /// <returns>Система безопасности операций с данными с установленным контекстом безопасности <b>context</b></returns>
        public virtual SecurityType CreateContractTypedSecurity(ContextType context)
        {
            SecurityType security = new SecurityType();
            security.Context = context;
            return security;
        }
    }
}
