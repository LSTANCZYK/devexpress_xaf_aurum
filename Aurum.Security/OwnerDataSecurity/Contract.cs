using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Aurum.Xpo;

namespace Aurum.Security
{
    /// <summary>
    /// Договор на доступ к данным между собственниками данных
    /// </summary>
    [ImageName("BO_Contract"), DefaultClassOptions]
    [ConsistentPeriod("Type;Grantor;Grantee", "DateIn", "DateOut")]
    public class Contract : XPObjectBase
    {
        private ContractType contractType;
        private Owner grantor;
        private Owner grantee;
        private DateTime dateIn;
        private DateTime? dateOut;

        /// <summary>Конструктор без параметров</summary>
        public Contract() { }

        /// <summary>Конструктор с указанной сессией</summary>
        /// <param name="session">Сессия для загрузки и сохранения объектов в базе данных</param>
        public Contract(Session session) : base(session) { }

        /// <summary>
        /// Тип договора, определяющий список разрешений на доступ к данным
        /// </summary>
        [RuleRequiredField, NotNull]
        public ContractType Type
        {
            get { return contractType; }
            set { SetPropertyValue("Type", ref contractType, value); }
        }

        /// <summary>
        /// Собственник данных, предоставляющий права на доступ к своим данным для собственника <b>Grantee</b>
        /// </summary>
        [RuleRequiredField, NotNull]
        public Owner Grantor
        {
            get { return grantor; }
            set { SetPropertyValue<Owner>("Grantor", ref grantor, value); }
        }

        /// <summary>
        /// Собственник данных, получающий права на доступ к данным собственника <b>Grantor</b>
        /// </summary>
        [RuleRequiredField, NotNull]
        public Owner Grantee
        {
            get { return grantee; }
            set { SetPropertyValue<Owner>("Grantee", ref grantee, value); }
        }

        /// <summary>
        /// Начало периода действия договора
        /// </summary>
        [RuleRequiredField, NotNull]
        public DateTime DateIn
        {
            get { return dateIn; }
            set { SetPropertyValue<DateTime>("DateIn", ref dateIn, value); }
        }

        /// <summary>
        /// Окончание периода действия договора
        /// </summary>
        public DateTime? DateOut
        {
            get { return dateOut; }
            set { SetPropertyValue<DateTime?>("DateOut", ref dateOut, value); }
        }

        /// <summary>
        /// Определяет, актуален ли договор на указанную дату
        /// </summary>
        /// <param name="date">Дата, на которую проверяется актуальность договора</param>
        /// <returns>True, если договор актуален на указанную дату, иначе false</returns>
        public bool IsActual(DateTime date)
        {
            return dateIn <= date.Date && (!dateOut.HasValue || date.Date < dateOut.Value);
        }

        /// <summary>Операнды свойств класса</summary>
        public new class Fields : XPObjectBase.FieldsClass
        {
            private Fields() { }
            /// <summary></summary>
            public static OperandProperty Type { get { return new OperandProperty("Type"); } }
            /// <summary></summary>
            public static OperandProperty Grantor { get { return new OperandProperty("Grantor"); } }
            /// <summary></summary>
            public static OperandProperty Grantee { get { return new OperandProperty("Grantee"); } }
            /// <summary></summary>
            public static OperandProperty DateIn { get { return new OperandProperty("DateIn"); } }
            /// <summary></summary>
            public static OperandProperty DateOut { get { return new OperandProperty("DateOut"); } }
        }
    }
}
