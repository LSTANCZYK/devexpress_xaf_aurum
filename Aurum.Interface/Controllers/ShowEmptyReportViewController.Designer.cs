namespace Aurum.Interface.Controllers
{
    partial class ShowEmptyReportViewController
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
            this.ShowEmptyReportAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // ShowEmptyReportAction
            // 
            this.ShowEmptyReportAction.Caption = "Ïףסעמי מעקוע";
            this.ShowEmptyReportAction.Category = "PopupActions";
            this.ShowEmptyReportAction.ConfirmationMessage = null;
            this.ShowEmptyReportAction.Id = "ShowEmptyReportAction";
            this.ShowEmptyReportAction.TargetObjectType = typeof(DevExpress.ExpressApp.ReportsV2.ReportContainer);
            this.ShowEmptyReportAction.TargetViewId = "";
            this.ShowEmptyReportAction.ToolTip = null;

        }

        #endregion

        public DevExpress.ExpressApp.Actions.SimpleAction ShowEmptyReportAction;

    }
}
