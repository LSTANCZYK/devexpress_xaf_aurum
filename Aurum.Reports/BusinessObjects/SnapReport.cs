using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Aurum.Xpo;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace Aurum.Reports
{
    /// <summary>
    /// Документ, содержащий отчет Snap
    /// </summary>
    public class SnapReportDocument
    {
        private byte[] content;

        /// <summary>Конструктор</summary>
        /// <param name="content">Содержание отчета</param>
        public SnapReportDocument(byte[] content) { this.content = content; }

        /// <summary>Содержание отчета</summary>
        public byte[] Content { get { return content; } }
    }

    /// <summary>
    /// Отчет Snap
    /// </summary>
    [DefaultClassOptions, NavigationItem("Reports"), ImageName("BO_Report"), DefaultProperty("Name")]
    public class SnapReport : XPObjectBase
    {
        private string name;

        /// <summary>Конструктор</summary>
        public SnapReport() { }

        /// <summary>Конструктор c указанием сессии</summary>
        public SnapReport(Session session) : base(session) { }
        
        /// <summary>
        /// Название
        /// </summary>
        [NotNull]
        public string Name
        {
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }

        /// <summary>
        /// Отчет
        /// </summary>
        [Persistent, NotNull]
        [ValueConverter(typeof(SnapReportToBytesConverter))]
        [Delayed(true)]
        public SnapReportDocument Report
        {
            get { return GetDelayedPropertyValue<SnapReportDocument>("Report"); }
            set { SetDelayedPropertyValue<SnapReportDocument>("Report", value); }
        }
    }

    /// <summary>
    /// Конвертор документа с содержанием отчета Snap в Blob
    /// </summary>
    public class SnapReportToBytesConverter : ValueConverter
    {
        /// <inheritdoc/>
        public override Type StorageType
        {
            get { return typeof(byte[]); }
        }

        /// <inheritdoc/>
        public override object ConvertFromStorageType(object value)
        {
            if (value == null) return null;
            byte[] bytes = (byte[])value;
            return new SnapReportDocument(bytes);
        }

        /// <inheritdoc/>
        public override object ConvertToStorageType(object value)
        {
            return value is SnapReportDocument ? ((SnapReportDocument)value).Content : null;
        }
    }

    /// <summary>
    /// Параметры выбора источника данных для отчета Snap
    /// </summary>
    [DomainComponent]
    public class SnapReportDataSourceParameters
    {
        /// <summary>Тип бизнес-класса источника данных</summary>
        [TypeConverter(typeof(ReportDataTypeConverter))] // IsVisibleInReports
        [RuleRequiredField]
        public Type DataSource { get; set; }
    }
}
