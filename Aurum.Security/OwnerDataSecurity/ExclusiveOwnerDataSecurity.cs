using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;

namespace Aurum.Security
{
    /// <summary>
    /// Поставщик безопасности операций с данными эксклюзивного собственника данных
    /// </summary>
    public class ExclusiveOwnerDataSecurityProvider : OwnerDataSecurityProvider
    {
        /// <inheritfrom/>
        public override OwnerContext CreateContractTypedContext()
        {
            return new ExclusiveOwnerConext();
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="securityStrategy">Стратегия безопасности, предоставляющая пространство объектов</param>
        public ExclusiveOwnerDataSecurityProvider(ISecurityStrategyBase securityStrategy) : base(securityStrategy) { }
    }

    /// <summary>
    /// Эксклюзивный собственник данных
    /// </summary>
    class ExclusiveOwner : IOwnedObject
    {
        public Owner Owner { get; set; }
    }

    /// <summary>
    /// Контекст безопасности эксклюзивного собственника данных
    /// </summary>
    class ExclusiveOwnerConext : OwnerContext
    {
        /// <inheritdoc />
        public override void Initialize(IObjectSpace objectSpace, object user)
        {
            ExclusiveOwner owner = new ExclusiveOwner();
            ContractDictionary = new ExclusiveOwnerContractDictionary();
            base.Initialize(objectSpace, owner);
        }
    }

    /// <summary>
    /// Справочник договоров эксклюзивного собственника данных
    /// </summary>
    /// <remarks>Для эксклюзивного собственника данных договора не загружаются, а проверка разрешений на операции всегда возвращает True</remarks>
    class ExclusiveOwnerContractDictionary : ContractDictionary
    {
        /// <inherifdoc/>
        public override bool IsGranted(Type objectType, Owner grantor, Owner grantee, string operation, DateTime date)
        {
            return true;
        }

        /// <inherifdoc/>
        public override IList<string> GetObjectCriteria(Type objectType, Owner grantee)
        {
            return new string[0];
        }
    }
}
