namespace Aurum.Operations.Controllers
{
    partial class OperationObject_ViewController
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
            this.cancelAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.cancelAllAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // cancelAction
            // 
            this.cancelAction.Caption = "Cancel";
            this.cancelAction.Category = "Edit";
            this.cancelAction.ConfirmationMessage = null;
            this.cancelAction.Id = "CancelOperationAction";
            this.cancelAction.ToolTip = null;
            this.cancelAction.TypeOfView = typeof(DevExpress.ExpressApp.View);
            this.cancelAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.cancelAction_Execute);
            // 
            // cancelAllAction
            // 
            this.cancelAllAction.Caption = "Cancel all";
            this.cancelAllAction.Category = "Edit";
            this.cancelAllAction.ConfirmationMessage = null;
            this.cancelAllAction.Id = "CancelAllOperationAction";
            this.cancelAllAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.cancelAllAction.ToolTip = null;
            this.cancelAllAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.cancelAllAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.cancelAllAction_Execute);
            // 
            // OperationObjectViewController
            // 
            this.TargetObjectType = typeof(Aurum.Operations.OperationObject);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction cancelAction;
        private DevExpress.ExpressApp.Actions.SimpleAction cancelAllAction;
    }
}
