using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aurum.Xpo;
using System.Collections.Concurrent;

namespace Aurum.Security
{
    /// <summary>
    /// Справочник договоров передачи данных между собственниками данных 
    /// </summary>
    public class TransferDictionary : ContractDictionary
    {
        /// <summary>
        /// Экземпляр справочника договоров передачи данных между собственниками данных
        /// </summary>
        public static readonly TransferDictionary Default = new TransferDictionary();

        private class TypeOwners : ConcurrentDictionary<Type, List<Owner>> { }
        private bool loaded = false;
        private ConcurrentDictionary<Owner, TypeOwners> grantorTypeReceivers;
        private ConcurrentDictionary<Owner, TypeOwners> granteeTypeSenders;
        
        /// <summary>Конструктор без параметров</summary>
        public TransferDictionary() { }
        
        /// <summary>Конструктор с указанным поставщиком пространства объектов</summary>
        /// <param name="provider">Поставщик пространства объектов</param>
        public TransferDictionary(IObjectSpaceProvider provider) { this.Provider = provider; }

        /// <summary>
        /// Поставщик пространства объектов
        /// </summary>
        public IObjectSpaceProvider Provider { get; set; }

        /// <summary>
        /// Дата актуальности договоров
        /// </summary>
        public DateTime ActualDate { get; protected set; }

        /// <summary>Актуализация договоров на текущую дату</summary>
        private void Actualize()
        {
            if (!loaded || ActualDate < DateTime.Today)
            {
                if (Provider == null)
                    throw new InvalidOperationException("Provider of transfer dictionary is not initialized");
                ActualDate = DateTime.Today;
                using (IObjectSpace objectSpace = Provider.CreateObjectSpace())
                {
                    Load(objectSpace, ActualDate);
                }
                loaded = true;
            }
        }

        /// <inheritdoc/>
        protected override void Fill()
        {
            grantorTypeReceivers = new ConcurrentDictionary<Owner, TypeOwners>();
            granteeTypeSenders = new ConcurrentDictionary<Owner, TypeOwners>();
            foreach (Contract contract in Contracts)
            {
                TypeOwners receivers = null, senders = null;
                foreach (ContractPermission permission in contract.Type.ObjectPermissions)
                    if (permission.AllowTransfer)
                    {
                        // Справочник получателей
                        if (receivers == null)
                            receivers = grantorTypeReceivers.GetOrAdd(contract.Grantor, new TypeOwners());
                        List<Owner> owners = receivers.GetOrAdd(permission.TargetType, new List<Owner>());
                        if (!owners.Contains(contract.Grantee)) 
                            owners.Add(contract.Grantee);
                        // Справочник отправителей
                        if (senders == null)
                            senders = granteeTypeSenders.GetOrAdd(contract.Grantee, new TypeOwners());
                        owners = senders.GetOrAdd(permission.TargetType, new List<Owner>());
                        if (!owners.Contains(contract.Grantor))
                            owners.Add(contract.Grantor);
                    }
            }
        }

        /// <summary>
        /// Возвращает список собственников, получающих данные по договорам от указанного собственника
        /// </summary>
        /// <param name="objectType">Тип передаваемых данных</param>
        /// <param name="grantor">Собственник, отправляющий данные</param>
        /// <returns>Список собственников, получающих по договорам c <paramref name="grantor"/> на текущую дату данные 
        /// типа <paramref name="objectType"/>, включая самого <paramref name="grantor"/></returns>
        public virtual IEnumerable<Owner> GetGrantees(Type objectType, Owner grantor)
        {
            List<Owner> result = new List<Owner>();
            if (objectType != null && grantor != null)
            {
                Actualize();
                result.Add(grantor);
                TypeOwners typeOwners; List<Owner> owners;
                if (grantorTypeReceivers.TryGetValue(grantor, out typeOwners))
                    if (typeOwners.TryGetValue(objectType, out owners))
                        result.AddRange(owners);
            }
            return result;
        }

        /// <summary>
        /// Возвращает список собственников, отправляющих данные по договорам указанному собственнику
        /// </summary>
        /// <param name="objectType">Тип передаваемых данных</param>
        /// <param name="grantee">Собственник, получающий данные</param>
        /// <returns>Список собственников, отправляющих по договорам c <paramref name="grantee"/> на текущую дату данные 
        /// типа <paramref name="objectType"/>, включая самого <paramref name="grantee"/></returns>
        public virtual IEnumerable<Owner> GetGrantors(Type objectType, Owner grantee)
        {
            List<Owner> result = new List<Owner>();
            if (objectType != null && grantee != null)
            {
                Actualize();
                result.Add(grantee);
                TypeOwners typeOwners; List<Owner> owners;
                if (granteeTypeSenders.TryGetValue(grantee, out typeOwners))
                    if (typeOwners.TryGetValue(objectType, out owners))
                        result.AddRange(owners);
            }
            return result;
        }
    }
}
