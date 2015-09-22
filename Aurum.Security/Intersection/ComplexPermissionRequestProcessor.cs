using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;

namespace Aurum.Security
{
    /// <summary>
    /// Сложный процессор запросов разрешений на операцию 
    /// </summary>
    /// <remarks>Выполняет пересечение результатов дочерних процессоров (операция AND)</remarks>
    public class ComplexPermissionRequestProcessor : IPermissionRequestProcessor, ISecurityCriteriaProvider2
    {
        private IPermissionRequestProcessor[] processors;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="processors">Процессоры запросов разрешений, результаты которых будут логически перемножаться</param>
        public ComplexPermissionRequestProcessor(params IPermissionRequestProcessor[] processors)
        {
            Guard.ArgumentNotNull(processors, "processors");
            this.processors = processors;
        }

        /// <contentfrom cref="IPermissionRequestProcessor.IsGranted" />
        public bool IsGranted(IPermissionRequest permissionRequest)
        {
            foreach (IPermissionRequestProcessor processor in processors)
                if (!processor.IsGranted(permissionRequest)) return false;
            return true;
        }

        /// <contentfrom cref="ISecurityCriteriaProvider2.GetMemberCriteria"/>
        public IList<string> GetMemberCriteria(Type type, string memberName)
        {
            List<string> result = new List<string>();
            foreach (IPermissionRequestProcessor processor in processors)
                if (processor is ISecurityCriteriaProvider2) 
                    result.AddRange(((ISecurityCriteriaProvider2)processor).GetMemberCriteria(type, memberName));
            return result;
        }

        /// <contentfrom cref="ISecurityCriteriaProvider2.GetObjectCriteria"/>
        public IList<string> GetObjectCriteria(Type type)
        {
            List<string> result = new List<string>();
            foreach (IPermissionRequestProcessor processor in processors)
                if (processor is ISecurityCriteriaProvider2)
                    result.AddRange(((ISecurityCriteriaProvider2)processor).GetObjectCriteria(type));
            return result;
        }
    }

    /// <summary>
    /// Процессор запросов разрешений на основе систем безопасности операций с данными
    /// </summary>
    /// <example>Пример использования процессора для отображения ограничений в интерфейсе
    /// <code><![CDATA[
    ///    private void securityStrategyComplex1_CustomizeRequestProcessors(object sender, CustomizeRequestProcessorsEventArgs e)
    ///    {
    ///        e.Processors[typeof(ClientPermissionRequest)] = new ComplexPermissionRequestProcessor(
    ///            e.Processors[typeof(ClientPermissionRequest)],
    ///            new SecurityPermissionRequestProcessor(Security.LogonObjectSpace,
    ///                new OwnerDataSecurityProvider(Security), new FilialDataSecurityProvider(Security)));
    ///    }
    /// ]]></code></example>
    public class SecurityPermissionRequestProcessor : IPermissionRequestProcessor
    {
        private IObjectSpace nonSecuredObjectSpace;
        private ISelectDataSecurityProvider[] providers;
        private ISelectDataSecurity[] securities = null;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="nonSecuredObjectSpace">Пространство объектов, не ограниченное безопасностью</param>
        /// <param name="providers">Поставщики безопасности операций с данными</param>
        public SecurityPermissionRequestProcessor(IObjectSpace nonSecuredObjectSpace, params ISelectDataSecurityProvider[] providers)
        {
            Guard.ArgumentNotNull(nonSecuredObjectSpace, "nonSecuredObjectSpace");
            Guard.ArgumentNotNull(providers, "providers");
            this.nonSecuredObjectSpace = nonSecuredObjectSpace;
            this.providers = providers;
        }

        /// <contentfrom cref="IPermissionRequestProcessor.IsGranted" />
        public bool IsGranted(IPermissionRequest permissionRequest)
        {
            // Инициализация систем безопасности
            if (securities == null)
            {
                securities = providers.Select(
                    provider => provider.CreateSelectDataSecurity()).ToArray();
            }
            // Преобразование клиентского запроса
            if (permissionRequest is ClientPermissionRequest)
            {
                ClientPermissionRequest clientPermissionRequest = (ClientPermissionRequest)permissionRequest;
                object targetObject = !string.IsNullOrEmpty(clientPermissionRequest.TargetObjectHandle) ? 
                    nonSecuredObjectSpace.GetObjectByHandle(clientPermissionRequest.TargetObjectHandle) : null;
                permissionRequest = new ServerPermissionRequest(clientPermissionRequest.ObjectType, targetObject,
                        clientPermissionRequest.MemberName, clientPermissionRequest.Operation, new SecurityExpressionEvaluator(nonSecuredObjectSpace));
            }
            // Проверка серверного запроса
            if (permissionRequest is ServerPermissionRequest)
            {
                foreach (ISelectDataSecurity security in securities)
                    if (!security.IsGranted(permissionRequest)) return false;
            }
            return true;
        }
    }

    // Калькулятор выражений безопасности (используется в качестве заглушки)
    class SecurityExpressionEvaluator : ISecurityExpressionEvaluator
    {
        private IObjectSpace objectSpace;
        private Dictionary<string, bool> fitCash = new Dictionary<string, bool>();
        
        public SecurityExpressionEvaluator(IObjectSpace objectSpace)
        {
            this.objectSpace = objectSpace;
        }
        
        public bool Fit(string criteriaString, object targetObject)
        {
            string key = targetObject.GetHashCode().ToString() + criteriaString;
            bool result;
            if (!fitCash.TryGetValue(key, out result))
            {
                ExpressionEvaluator evaluator = objectSpace.GetExpressionEvaluator(targetObject.GetType(), objectSpace.ParseCriteria(criteriaString));
                result = evaluator.Fit(targetObject);
                fitCash[key] = result;
            }
            return result;
        }
    }
}
