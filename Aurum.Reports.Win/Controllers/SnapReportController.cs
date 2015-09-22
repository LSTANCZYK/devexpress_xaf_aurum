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
using Aurum.Reports.Win.Editors;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;

namespace Aurum.Reports.Win.Controllers
{
    /// <summary>
    /// Контроллер отчета Snap
    /// </summary>
    public partial class SnapReportController : ViewController
    {
        /// <summary>Конструктор</summary>
        public SnapReportController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        private void SelectDataSourceAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            e.View = Application.CreateDetailView(ObjectSpaceInMemory.CreateNew(), new SnapReportDataSourceParameters());
            e.DialogController.AcceptAction.Execute += AcceptAction_Execute;
        }

        private void AcceptAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            SnapReportDataSourceParameters parameters = e.CurrentObject as SnapReportDataSourceParameters;
            SnapReportPropertyEditor editor = ((DetailView)View).GetItems<SnapReportPropertyEditor>().FirstOrDefault();
            if (parameters != null && parameters.DataSource != null && editor != null && editor.SnapReportEdit != null)
            {
                Type type = parameters.DataSource;
                XPCollection dataSource = new XPCollection(((XPObjectSpace)Application.CreateObjectSpace(type)).Session, type);
                editor.SnapReportEdit.Control.Document.BeginUpdateDataSource();
                editor.SnapReportEdit.Control.Document.DataSources.Add(type.Name, dataSource);
                editor.SnapReportEdit.Control.Document.EndUpdateDataSource();
            }
        }
    }
}
