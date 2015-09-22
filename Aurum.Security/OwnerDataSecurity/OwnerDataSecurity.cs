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
    /// Поставщик безопасности операций с данными между собственниками данных
    /// </summary>
    public class OwnerDataSecurityProvider : ContractDataSecurityProvider<OwnerDataSecurity, OwnerContext>
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="securityStrategy">Стратегия безопасности, предоставляющая информацию о текущем пользователе системы</param>
        public OwnerDataSecurityProvider(ISecurityStrategyBase securityStrategy) : base(securityStrategy) { }
    }

    /// <summary>
    /// Система безопасности операций с данными между собственниками данных
    /// </summary>
    public class OwnerDataSecurity : ContractDataSecurity<OwnerContext>
    {
        private Type IOwnedObjectType = typeof(IOwnedObject);

        /// <inheritdoc/>
        public override bool IsGranted(IContractObject<OwnerContext> contractObject, string operation, OwnerContext context)
        {
            if (contractObject is IOwnedObject)
            {
                DateTime date = contractObject is IContractObjectDate<OwnerContext> ? ((IContractObjectDate<OwnerContext>)contractObject).ContractDate : DateTime.Today;
                return context.ContractDictionary.IsGranted(contractObject.GetType(), ((IOwnedObject)contractObject).Owner, context.UserOwner, operation, date);
            }
            return true;
        }

        /// <inheritdoc/>
        public override bool IsGranted(IContractObject<OwnerContext> contractObject, string operation, string memberName, OwnerContext context)
        {
            return true;
        }

        /// <inheritdoc/>
        public override IList<string> GetObjectCriteria(Type type, OwnerContext context)
        {
            if (IOwnedObjectType.IsAssignableFrom(type))
                return context.ContractDictionary.GetObjectCriteria(type, context.UserOwner);
            return new string[0];
        }
    }

    /// <summary>
    /// Контекст безопасности операций с данными между собственниками данных
    /// </summary>
    public class OwnerContext : ContractContext
    {
        private Owner userOwner;
        private ContractDictionary contractDictionary;

        /// <summary>Собственник данных текущего пользователя</summary>
        public Owner UserOwner
        {
            get { return userOwner; }
        }

        /// <summary>Справочник договоров между собственниками данных</summary>
        public ContractDictionary ContractDictionary
        {
            get { return contractDictionary; }
            protected set { contractDictionary = value; }
        }

        /// <inheritdoc/>
        public override void Initialize(IObjectSpace objectSpace, object user)
        {
            Owner owner = ((IOwnedObject)user).Owner;
            
            this.userOwner = owner;
            if (this.contractDictionary == null)
                this.contractDictionary = new ContractDictionary(objectSpace, owner);
            Values["UserOwner"] = owner;
            Values["ContractDictionary"] = contractDictionary;
        }
    }
}
