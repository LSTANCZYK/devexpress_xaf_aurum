using DevExpress.ExpressApp.Actions;
namespace Aurum.Operations.Controllers
{
    partial class AutoRefreshViewController
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
            this.RefreshTimeoutAction = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
            // 
            // RefreshTimeoutAction
            // 
            this.RefreshTimeoutAction = new SingleChoiceAction(this.components);
            this.RefreshTimeoutAction.Caption = "RefreshTimeout";
            this.RefreshTimeoutAction.Category = "View";
            this.RefreshTimeoutAction.Id = "RefreshTimeout";
            this.RefreshTimeoutAction.Execute += new SingleChoiceActionExecuteEventHandler(this.repeatTimeAction_Execute);
            // 
            // AutoRefreshViewController
            // 
            this.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SingleChoiceAction RefreshTimeoutAction;
    }
}
