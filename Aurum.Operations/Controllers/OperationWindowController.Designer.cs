namespace Aurum.Operations.Controllers
{
    partial class OperationWindowController
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
            this.showOperationsAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // showOperationsAction
            // 
            this.showOperationsAction.Caption = "Show Operations";
            this.showOperationsAction.Category = "View";
            this.showOperationsAction.ConfirmationMessage = null;
            this.showOperationsAction.Id = "ShowOperationsAction";
            this.showOperationsAction.ImageName = "Action_OrganizeDashboard";
            this.showOperationsAction.TargetObjectsCriteria = "";
            this.showOperationsAction.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.showOperationsAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.showOperationsAction.ToolTip = null;
            this.showOperationsAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.showOperationsAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.showOperationsAction_Execute);
            // 
            // OperationWindowController
            // 
            this.TargetWindowType = DevExpress.ExpressApp.WindowType.Main;

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction showOperationsAction;
    }
}
