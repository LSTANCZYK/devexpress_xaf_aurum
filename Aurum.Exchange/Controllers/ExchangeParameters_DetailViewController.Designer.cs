namespace Aurum.Exchange.Controllers
{
    partial class ExchangeParameters_DetailViewController
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
            this.validateAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // validateAction
            // 
            this.validateAction.Caption = "Validate";
            this.validateAction.Category = "PopupActions";
            this.validateAction.ConfirmationMessage = null;
            this.validateAction.Id = "ValidateExchangeParametersAction";
            this.validateAction.ToolTip = null;
            this.validateAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.validateAction_Execute);
            // 
            // ExchangeParameters_DetailViewController
            // 
            this.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction validateAction;
    }
}
