namespace Aurum.Help.Controllers
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
            this.describePopupWindowShowAction = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // describePopupWindowShowAction
            // 
            this.describePopupWindowShowAction.AcceptButtonCaption = null;
            this.describePopupWindowShowAction.CancelButtonCaption = null;
            this.describePopupWindowShowAction.Caption = "Describe";
            this.describePopupWindowShowAction.ConfirmationMessage = null;
            this.describePopupWindowShowAction.Id = "DescribeAction";
            this.describePopupWindowShowAction.Shortcut = "Alt+F1";
            this.describePopupWindowShowAction.ToolTip = null;
            this.describePopupWindowShowAction.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.helpPopupWindowShowAction1_CustomizePopupWindowParams);
            this.describePopupWindowShowAction.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.helpPopupWindowShowAction1_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction describePopupWindowShowAction;
    }
}
