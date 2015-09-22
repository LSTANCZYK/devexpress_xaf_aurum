using System;
using System.Collections.Generic;
using ComponentModel = System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Aurum.Xpo;

namespace Aurum.Security
{
    /// <summary>
    /// Тип договора на доступ к данным между собственниками данных
    /// </summary>
    public class ContractType : XPObjectBase<string>
    {
        /// <summary>Конструктор без параметров</summary>
        public ContractType() { }

        /// <summary>Конструктор с указанной сессией</summary>
        /// <param name="session">Сессия для загрузки и сохранения объектов в базе данных</param>
        public ContractType(Session session) : base(session) { }

        /// <summary>
        /// Код, уникально идентифицирующий данный тип договоров
        /// </summary>
        [PersistentAlias("Id"), RuleRequiredField, ReadOnly, Size(30)]
        public string Code
        {
            get { return Id; }
            set { Id = value; }
        }

        /// <summary>Разрешения на доступ к данным для договоров данного типа</summary>
        [Association, Aggregated]
        public XPCollection<ContractPermission> ObjectPermissions
        {
            get { return GetCollection<ContractPermission>("ObjectPermissions"); }
        }

        /// <summary>
        /// Определяет, есть ли в данном типе договоров разрешение на указанную операцию над объектами указанного типа
        /// </summary>
        /// <param name="objectType">Тип объектов</param>
        /// <param name="operation">Операция над объектами</param>
        /// <returns>True, если операция над объектами указанного типа разрешена, иначе false</returns>
        public bool IsGranted(Type objectType, string operation)
        {
            return ObjectPermissions.Any(permission => permission.IsGranted(objectType, operation));
        }

        /// <summary>
        /// Текстовое представление
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Code;
        }
    }

    /// <summary>
    /// Разрешение на операции с данными по договору между собственниками данных
    /// </summary>
    [ImageName("BO_Security_Permission_Type")]
    public class ContractPermission : XPObjectBase
    {
        ContractType contractType;
        Type targetType;
        bool allowCreate;
        bool allowRead;
        bool allowWrite;
        bool allowDelete;
        bool allowTransfer;

        /// <summary>Конструктор без параметров</summary>
        public ContractPermission() { }

        /// <summary>Конструктор с указанной сессией</summary>
        /// <param name="session">Сессия для загрузки и сохранения объектов в базе данных</param>
        public ContractPermission(Session session) : base(session) { }

        /// <summary>
        /// Тип договора между собственниками данных, к которому относится данное разрешение
        /// </summary>
        [Association]
        [VisibleInListView(false), VisibleInDetailView(false)]
        public ContractType ContractType
        {
            get { return contractType; }
            set { SetPropertyValue("ContractType", ref contractType, value); }
        }

        /// <summary>
        /// Тип объектов, к которым относится разрешение на операции
        /// </summary>
        [ValueConverter(typeof(TypeToStringConverter))]
        [Size(1024)]
        [VisibleInDetailView(false), VisibleInListView(false)]
        [RuleRequiredField("ContractPermission_TargetType_RuleRequiredField", DefaultContexts.Save)]
        [ComponentModel.TypeConverter(typeof(SecurityStrategyTargetTypeConverter))]
        public Type TargetType
        {
            get { return targetType; }
            set { SetPropertyValue<Type>("TargetType", ref targetType, value); }
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

        /// <summary>Разрешение на создание данных</summary>
        public bool AllowCreate
        {
            get { return allowCreate; }
            set { SetPropertyValue<bool>("AllowCreate", ref allowCreate, value); }
        }

        /// <summary>Разрешение на чтение данных</summary>
        public bool AllowRead
        {
            get { return allowRead; }
            set { SetPropertyValue<bool>("AllowRead", ref allowRead, value); }
        }

        /// <summary>Разрешение на запись данных</summary>
        public bool AllowWrite
        {
            get { return allowWrite; }
            set { SetPropertyValue<bool>("AllowWrite", ref allowWrite, value); }
        }

        /// <summary>Разрешение на удаление данных</summary>
        public bool AllowDelete
        {
            get { return allowDelete; }
            set { SetPropertyValue<bool>("AllowDelete", ref allowDelete, value); }
        }

        /// <summary>
        /// Разрешение на передачу данных
        /// </summary>
        public bool AllowTransfer
        {
            get { return allowTransfer; }
            set { SetPropertyValue("AllowTransfer", ref allowTransfer, value); }
        }

        /// <summary>
        /// Определяет, дает ли право данное разрешение на указанную операцию над объектами указанного типа
        /// </summary>
        /// <param name="objectType">Тип объектов</param>
        /// <param name="operation">Операция над объектами</param>
        /// <returns>True, если разрешение дает право на операцию <b>operation</b> над объектами типа <b>T</b>, иначе false</returns>
        public bool IsGranted(Type objectType, string operation)
        {
            return objectType.IsAssignableFrom(targetType) && (
                (operation == SecurityOperations.Create && allowCreate) ||
                (operation == SecurityOperations.Read && allowRead) ||
                (operation == SecurityOperations.Write && allowWrite) ||
                (operation == SecurityOperations.Delete && allowDelete) ||
                (operation == ExtendedSecurityOperations.Transfer && allowTransfer));
        }
    }

    /// <summary>
    /// Расширенные операции, ограниченный системой безопасности
    /// </summary>
    public static class ExtendedSecurityOperations
    {
        /// <summary>Операция передачи данных</summary>
        public const string Transfer = "Transfer";
    }
}
