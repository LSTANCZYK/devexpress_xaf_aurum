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
using DevExpress.Persistent.BaseImpl;
using Aurum.Reports.Security;

namespace Aurum.Reports.Controllers
{
    /// <summary>
    /// Контроллер мастера отчетов
    /// </summary>
    public partial class OwnedReportWizardController : ViewController
    {
        /// <summary>Конструктор</summary>
        public OwnedReportWizardController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        private ReportWizard wizard;
        private ReportStyle style;
        private IObjectSpace objectSpace;

        // Форма мастера отчетов
        private void ReportWizard_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            PredefinedReportWizard predefined = View.CurrentObject as PredefinedReportWizard;
            wizard = predefined != null ? predefined.Main : new ReportWizard();
            style = null;
            objectSpace = Application.CreateObjectSpace();
            e.View = Application.CreateDetailView(objectSpace, wizard);
            e.DialogController.AcceptAction.Execute += AcceptAction_Execute;
            e.DialogController.Actions.Add(OwnedReportStyle);
        }

        // Завершение мастера
        private void AcceptAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            Validator.RuleSet.Validate(objectSpace, wizard, ContextIdentifier.Save);
            OwnedReportData reportData = objectSpace.CreateObject<OwnedReportData>();
            wizard.Generate(Application.TypesInfo, reportData, style);
            objectSpace.CommitChanges(); 
        }

        // Стиль отчета
        private void OwnedReportStyle_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            if (style == null) style = new ReportStyle();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            e.View = Application.CreateDetailView(objectSpace, style);
            e.DialogController.CancelAction.Active["StyleCancel"] = false;
        }
    }
}
