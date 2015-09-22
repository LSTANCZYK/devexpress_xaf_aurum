namespace Aurum.Help.Win.Controllers
{
    partial class HelpViewController
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
            this.helpPopupWindowShowAction = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // helpPopupWindowShowAction
            // 
            this.helpPopupWindowShowAction.AcceptButtonCaption = null;
            this.helpPopupWindowShowAction.CancelButtonCaption = null;
            this.helpPopupWindowShowAction.Caption = "Справка";
            this.helpPopupWindowShowAction.ConfirmationMessage = null;
            this.helpPopupWindowShowAction.Id = "HelpWin";
            this.helpPopupWindowShowAction.ImageName = "Action_AboutInfo";
            this.helpPopupWindowShowAction.Shortcut = "F1";
            this.helpPopupWindowShowAction.ToolTip = null;
            this.helpPopupWindowShowAction.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.helpPopupWindowShowAction_CustomizePopupWindowParams);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction helpPopupWindowShowAction;
    }
}
