namespace Aurum.Reports.Win.Controllers
{
    partial class SnapReportController
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
            this.SelectDataSourceAction = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // SelectDataSourceAction
            // 
            this.SelectDataSourceAction.AcceptButtonCaption = null;
            this.SelectDataSourceAction.CancelButtonCaption = null;
            this.SelectDataSourceAction.Caption = "Select Data Source";
            this.SelectDataSourceAction.Category = "Edit";
            this.SelectDataSourceAction.ConfirmationMessage = null;
            this.SelectDataSourceAction.Id = "SnapReportSelectDataSource";
            this.SelectDataSourceAction.TargetObjectType = typeof(Aurum.Reports.SnapReport);
            this.SelectDataSourceAction.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.SelectDataSourceAction.ToolTip = null;
            this.SelectDataSourceAction.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);
            this.SelectDataSourceAction.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.SelectDataSourceAction_CustomizePopupWindowParams);
            // 
            // SnapReportController
            // 
            this.TargetObjectType = typeof(Aurum.Reports.SnapReport);
            this.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction SelectDataSourceAction;


    }
}
