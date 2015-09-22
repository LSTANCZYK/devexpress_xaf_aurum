using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;
using Aurum.Xpo;
using System.Collections.Concurrent;

namespace Aurum.Security
{
    /// <summary>
    /// Справочник договоров между собственниками данных
    /// </summary>
    public class ContractDictionary
    {
        /// <summary>Список договоров одного типа</summary>
        public class TypeList : ConcurrentDictionary<ContractType, List<Contract>> { }
        /// <summary>Список договоров одного собственника</summary>
        public class OwnerList : ConcurrentDictionary<Owner, List<Contract>> { }

        private IList<Contract> contracts;
        private IList<Owner> owners;
        private ConcurrentDictionary<Owner, ConcurrentDictionary<Type, List<string>>> filters = new ConcurrentDictionary<Owner, ConcurrentDictionary<Type, List<string>>>();

        /// <summary>
        /// Список всех загруженных договоров
        /// </summary>
        public IEnumerable<Contract> Contracts { get { return contracts; } }

        /// <summary>
        /// Список всех собственников данных
        /// </summary>
        public IEnumerable<Owner> Owners { get { return owners; } }

        /// <summary>
        /// Договора собственников, получивших права на доступ к данным, упорядоченные по типам договоров
        /// </summary>
        public ConcurrentDictionary<Owner, TypeList> GranteeContractsByType { get; private set; }

        /// <summary>
        /// Договора собственников, получивших права на доступ к данным, упорядоченные по собственникам, предоставившим права
        /// </summary>
        public ConcurrentDictionary<Owner, OwnerList> GranteeContractsByGrantor { get; private set; }

        /// <summary>
        /// Собственники публичных данных
        /// </summary>
        public List<Owner> PublicOwners { get; private set; }

        /// <summary>
        /// Конструктор без параметров
        /// </summary>
        protected ContractDictionary()
        {
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="objectSpace">Пространство объектов для получения договоров</param>
        /// <param name="currentOwner">Собственник данных текущего пользователя системы</param>
        /// <remarks>При создании загружаются договора собственника даных</remarks>
        public ContractDictionary(IObjectSpace objectSpace, Owner currentOwner)
        {
            Load(objectSpace, currentOwner);
        }

        /// <summary>
        /// Загрузка договоров собственника данных
        /// </summary>
        /// <param name="objectSpace">Пространство объектов для получения договоров</param>
        /// <param name="currentOwner">Собственник данных текущего пользователя системы</param>
        public void Load(IObjectSpace objectSpace, Owner currentOwner)
        {
            contracts = objectSpace.GetObjects<Contract>(Contract.Fields.Grantee == new OperandValue(currentOwner));
            owners = objectSpace.GetObjects<Owner>();
            Fill();
        }

        /// <summary>
        /// Загрузка договоров собственника данных
        /// </summary>
        /// <param name="objectSpace">Пространство объектов для получения договоров</param>
        /// <param name="date">Дата актуальности договоров</param>
        public void Load(IObjectSpace objectSpace, DateTime date)
        {
            contracts = objectSpace.GetObjects<Contract>(Contract.Fields.DateIn <= new OperandValue(date) & 
                (Contract.Fields.DateOut > new OperandValue(date) | new FunctionOperator(FunctionOperatorType.IsNull, Contract.Fields.DateOut)));
            owners = objectSpace.GetObjects<Owner>();
            Fill();
        }

        /// <summary>
        /// Заполнение справочников
        /// </summary>
        protected virtual void Fill()
        {
            GranteeContractsByType = new ConcurrentDictionary<Owner, TypeList>();
            GranteeContractsByGrantor = new ConcurrentDictionary<Owner, OwnerList>();
            foreach (Contract contract in contracts)
            {
                GranteeContractsByType.GetOrAdd(contract.Grantee, new TypeList()).GetOrAdd(contract.Type, new List<Contract>()).Add(contract);
                GranteeContractsByGrantor.GetOrAdd(contract.Grantee, new OwnerList()).GetOrAdd(contract.Grantor, new List<Contract>()).Add(contract);
            }
            PublicOwners = new List<Owner>();
            PublicOwners.AddRange(owners.Where(owner => owner.Public));
        }

        /// <summary>
        /// Возвращает флаг разрешения на операцию для собственника над объектом другого собственника
        /// </summary>
        /// <param name="objectType">Тип объекта, над которым предполагается операция</param>
        /// <param name="grantor">Собственник, которому принадлежит объект</param>
        /// <param name="grantee">Собственник, который запрашивает разрешение на операцию</param>
        /// <param name="operation">Операция, разрешение на которую запрашивается</param>
        /// <param name="date">Актуальная дата операции, на которую проверяются договора между собственниками данных</param>
        /// <returns>True, если операция для собственника разрешена, иначе false</returns>
        public virtual bool IsGranted(Type objectType, Owner grantor, Owner grantee, string operation, DateTime date)
        {
            OwnerList ownerList; List<Contract> list;
            if (objectType != null && grantor != null && grantee != null && !string.IsNullOrEmpty(operation))
            {
                // Собственные данные
                if (grantee == grantor)
                    return true;
                // Публичные данные
                if ((grantor.Rights & OwnerRights.Public) == OwnerRights.Public && operation == SecurityOperations.Read)
                    return true;
                // Права на чтение данных всех собственников
                if ((grantee.Rights & OwnerRights.Reader) == OwnerRights.Reader && operation == SecurityOperations.Read)
                    return true;
                // Права на чтение и запись данных всех собственников
                if ((grantee.Rights & OwnerRights.Administrator) == OwnerRights.Administrator)
                    return true;
                // Договора между собственниками данных
                if (GranteeContractsByGrantor.TryGetValue(grantee, out ownerList))
                    if (ownerList.TryGetValue(grantor, out list))
                        if (list.Any(contract => contract.IsActual(date) && contract.Type.IsGranted(objectType, operation)))
                            return true;
            }
            return false;
        }

        /// <summary>
        /// Возвращает условие безопасной выборки данных указанного типа
        /// </summary>
        /// <param name="objectType">Тип данных, реализующий <see cref="IOwnedObject"/>, для которого возвращается условие выборки</param>
        /// <param name="grantee">Собственник, который запрашивает условие выборки</param>
        /// <returns>Условие, ограничивающее выборку данных типа <b>objectType</b></returns>
        public virtual IList<string> GetObjectCriteria(Type objectType, Owner grantee)
        {
            if (grantee == null || objectType == null) 
                return new string[0];
            // Права на чтение данных всех собственников
            if ((grantee.Rights & (OwnerRights.Reader | OwnerRights.Administrator)) != 0)
                return new string[0];
            // Кеш фильтров
            List<string> result = filters
                .GetOrAdd(grantee, ow => new ConcurrentDictionary<Type, List<string>>())
                .GetOrAdd(objectType, t => new List<string>());
            if (!result.Any())
            {
                List<Owner> owners = new List<Owner>();
                // Сам собственник
                owners.Add(grantee);
                // Собственники публичных данных
                owners.AddRange(PublicOwners);
                // Договора между собственниками данных
                OwnerList ownerList;
                if (GranteeContractsByGrantor.TryGetValue(grantee, out ownerList))
                {
                    bool isContractDate = typeof(IContractObjectDate<OwnerContext>).IsAssignableFrom(objectType);
                    foreach (KeyValuePair<Owner, List<Contract>> grantorList in ownerList)
                        if (grantorList.Value.Any(contract => contract.Type.IsGranted(objectType, SecurityOperations.Read)))
                        {
                            if (!isContractDate)
                                owners.Add(grantorList.Key);
                            else
                            {
                                // Условие по датам договоров
                                List<string> dates = new List<string>();
                                foreach (Contract contract in grantorList.Value)
                                    if (contract.Type.IsGranted(objectType, SecurityOperations.Read))
                                    {
                                        string contractDates = string.Format("ContractDate >= #{0}#", contract.DateIn.ToString("mm/dd/yyyy"));
                                        if (contract.DateOut.HasValue)
                                            contractDates += string.Format("ContractDate < #{0}#", contract.DateOut.Value.ToString("mm/dd/yyyy"));
                                        dates.Add(contractDates);
                                    }
                                result.Add(string.Format("Owner = {0} and ({1})", grantorList.Key.Id, string.Join(" or ", dates)));
                            }
                        }
                }
                result.Add("Owner in (" + string.Join(", ", owners.Select(owner => owner.Id)) + ")");
            }
            return result;
        }
    }
}
