using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aurum.Security;
using Aurum.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Aurum.Reports.Security
{
    /// <summary>
    /// Отчет Snap собственника данных
    /// </summary>
    [DefaultClassOptions, NavigationItem("Reports"), ImageName("BO_Report"), DefaultProperty("Name")]
    public class OwnedSnapReport : XPOwnedObject
    {
        private string name;

        /// <summary>Конструктор</summary>
        public OwnedSnapReport() { }

        /// <summary>Конструктор c указанием сессии</summary>
        public OwnedSnapReport(Session session) : base(session) { }

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
}
