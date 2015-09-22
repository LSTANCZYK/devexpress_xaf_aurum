using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Snap;
using DevExpress.XtraBars;
using DevExpress.Xpo;

namespace Aurum.Reports.Win.Editors
{
    /// <summary>
    /// Редактор отчетов Snap
    /// </summary>
    public partial class SnapReportEditor : UserControl
    {
        /// <summary>Конструктор</summary>
        public SnapReportEditor()
        {
            InitializeComponent();
        }

        /// <summary>Элемент управления отчетов Snap</summary>
        public SnapControl Control 
        {
            get { return snapControl; }
        }

        /// <summary>Редактируемый отчет Snap</summary>
        public SnapReportDocument Report
        {
            get { return new SnapReportDocument(snapControl.SnxBytes); }
            set { if (value != null) snapControl.SnxBytes = value.Content; }
        }

        /// <summary>Флаг разрешения редактирования отчета</summary>
        public bool AllowEdit
        {
            get { return !snapControl.ReadOnly; }
            set { snapControl.ReadOnly = !value; }
        }

        /// <summary>Событие, вызываемое при изменении отчета</summary>
        public event EventHandler EditValueChanged
        {
            add { snapControl.TextChanged += value; }
            remove { snapControl.TextChanged -= value; }
        }
    }
}
