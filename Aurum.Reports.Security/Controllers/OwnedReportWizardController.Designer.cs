namespace Aurum.Reports.Controllers
{
    partial class OwnedReportWizardController
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
            this.OwnedReportWizard = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.PredefinedOwnedReportWizard = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.OwnedReportStyle = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // OwnedReportWizard
            // 
            this.OwnedReportWizard.AcceptButtonCaption = null;
            this.OwnedReportWizard.CancelButtonCaption = null;
            this.OwnedReportWizard.Caption = "Owned Report Wizard";
            this.OwnedReportWizard.Category = "Reports";
            this.OwnedReportWizard.ConfirmationMessage = null;
            this.OwnedReportWizard.Id = "OwnedReportWizard";
            this.OwnedReportWizard.TargetViewId = "ReportDataV2_ListView;OwnedReportData_ListView";
            this.OwnedReportWizard.ToolTip = null;
            this.OwnedReportWizard.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.ReportWizard_CustomizePopupWindowParams);
            // 
            // PredefinedOwnedReportWizard
            // 
            this.PredefinedOwnedReportWizard.AcceptButtonCaption = null;
            this.PredefinedOwnedReportWizard.CancelButtonCaption = null;
            this.PredefinedOwnedReportWizard.Caption = "Owned Report Wizard";
            this.PredefinedOwnedReportWizard.Category = "Reports";
            this.PredefinedOwnedReportWizard.ConfirmationMessage = null;
            this.PredefinedOwnedReportWizard.Id = "PredefinedOwnedReportWizard";
            this.PredefinedOwnedReportWizard.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.PredefinedOwnedReportWizard.TargetViewId = "PredefinedReportWizard_ListView";
            this.PredefinedOwnedReportWizard.ToolTip = null;
            this.PredefinedOwnedReportWizard.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.ReportWizard_CustomizePopupWindowParams);
            // 
            // OwnedReportStyle
            // 
            this.OwnedReportStyle.AcceptButtonCaption = null;
            this.OwnedReportStyle.CancelButtonCaption = null;
            this.OwnedReportStyle.Caption = "Style";
            this.OwnedReportStyle.Category = "PopupActions";
            this.OwnedReportStyle.ConfirmationMessage = null;
            this.OwnedReportStyle.Id = "OwnedReportStyle";
            this.OwnedReportStyle.ToolTip = null;
            this.OwnedReportStyle.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.OwnedReportStyle_CustomizePopupWindowParams);
            // 
            // OwnedReportWizardController
            // 
            this.TargetViewId = "ReportDataV2_ListView;PredefinedReportWizard_ListView;OwnedReportData_ListView";

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction OwnedReportWizard;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction PredefinedOwnedReportWizard;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction OwnedReportStyle;
    }
}
