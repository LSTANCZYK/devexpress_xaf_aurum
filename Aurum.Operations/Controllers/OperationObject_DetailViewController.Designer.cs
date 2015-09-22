namespace Aurum.Operations.Controllers
{
    partial class OperationObject_DetailViewController
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
            this.hideAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.cancelAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // hideAction
            // 
            this.hideAction.Caption = "Hide";
            this.hideAction.Category = "PopupActions";
            this.hideAction.ConfirmationMessage = null;
            this.hideAction.Id = "HideAction";
            this.hideAction.ToolTip = "Hide, leaving operation running";
            this.hideAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.hideAction_Execute);
            // 
            // cancelAction
            // 
            this.cancelAction.Caption = "Cancel";
            this.cancelAction.Category = "PopupActions";
            this.cancelAction.ConfirmationMessage = null;
            this.cancelAction.Id = "35b24070-4bb8-4240-835d-219ad1888ce7";
            this.cancelAction.TargetObjectsCriteria = "";
            this.cancelAction.ToolTip = "Sends cancel request to operation";
            this.cancelAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.cancelAction_Execute);
            // 
            // OperationObject_DetailViewController
            // 
            this.TargetObjectType = typeof(Aurum.Operations.OperationObject);
            this.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction hideAction;
        private DevExpress.ExpressApp.Actions.SimpleAction cancelAction;
    }
}
