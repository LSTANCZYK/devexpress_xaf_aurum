namespace Aurum.Reports.Controllers
{
    partial class ReportWizardController
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ReportWizard = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.PredefinedReportWizard = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.ReportStyle = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // ReportWizard
            // 
            this.ReportWizard.AcceptButtonCaption = null;
            this.ReportWizard.CancelButtonCaption = null;
            this.ReportWizard.Caption = "Report Wizard";
            this.ReportWizard.Category = "Reports";
            this.ReportWizard.ConfirmationMessage = null;
            this.ReportWizard.Id = "ReportWizard";
            this.ReportWizard.TargetViewId = "ReportDataV2_ListView";
            this.ReportWizard.ToolTip = null;
            this.ReportWizard.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.ReportWizard_CustomizePopupWindowParams);
            // 
            // PredefinedReportWizard
            // 
            this.PredefinedReportWizard.AcceptButtonCaption = null;
            this.PredefinedReportWizard.CancelButtonCaption = null;
            this.PredefinedReportWizard.Caption = "Report Wizard";
            this.PredefinedReportWizard.Category = "Reports";
            this.PredefinedReportWizard.ConfirmationMessage = null;
            this.PredefinedReportWizard.Id = "PredefinedReportWizard";
            this.PredefinedReportWizard.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.PredefinedReportWizard.TargetViewId = "PredefinedReportWizard_ListView";
            this.PredefinedReportWizard.ToolTip = null;
            this.PredefinedReportWizard.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.ReportWizard_CustomizePopupWindowParams);
            // 
            // ReportStyle
            // 
            this.ReportStyle.AcceptButtonCaption = null;
            this.ReportStyle.CancelButtonCaption = null;
            this.ReportStyle.Caption = "Style";
            this.ReportStyle.Category = "PopupActions";
            this.ReportStyle.ConfirmationMessage = null;
            this.ReportStyle.Id = "ReportStyle";
            this.ReportStyle.TargetViewId = "";
            this.ReportStyle.ToolTip = null;
            this.ReportStyle.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.ReportWizardStyle_CustomizePopupWindowParams);
            // 
            // ReportWizardController
            // 
            this.TargetViewId = "ReportDataV2_ListView;PredefinedReportWizard_ListView";

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction ReportWizard;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction PredefinedReportWizard;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction ReportStyle;
    }
}
