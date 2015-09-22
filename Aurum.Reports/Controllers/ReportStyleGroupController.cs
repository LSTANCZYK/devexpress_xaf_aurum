using System;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Model.NodeGenerators;
using System.Drawing;

namespace Aurum.Reports.Controllers
{
    /// <summary>
    /// Контроллер групп стиля отчетов
    /// </summary>
    public partial class ReportStyleGroupController : ViewController
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public ReportStyleGroupController()
        {
            InitializeComponent();
            RegisterActions(components);
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }

        private void CountAction_Execute(object sender, ParametrizedActionExecuteEventArgs e)
        {
            ListView list = (ListView)View;
            int count = Convert.ToInt32(e.ParameterCurrentValue);
            int oldCount = list.CollectionSource.List.Count;
            BeginUpdate();
            if (count >= 0 && count > oldCount)
            {
                for (int i = 0; i < count - oldCount; i++)
                {
                    ReportStyleGroup group = new ReportStyleGroup();
                    group.CaptionFont = new Font("Times New Roman", 12, FontStyle.Bold);
                    group.Left = 0;
                    list.CollectionSource.Add(group);
                }
            }
            else if (count >= 0 && count < oldCount)
            {
                for (int i = 0; i < oldCount - count; i++)
                {
                    object group = list.CollectionSource.List[oldCount - i - 1];
                    list.CollectionSource.Remove(group);
                }
            }
            EndUpdate();
            list.CollectionSource.ResetCollection();
        }
    }
}
