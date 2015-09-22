namespace Aurum.App.Module.Controllers
{
    partial class ExchangeWindowController
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
            this.exportChoiceAction = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
            // 
            // exportChoiceAction
            // 
            this.exportChoiceAction.Caption = "Exports";
            this.exportChoiceAction.Category = "Export";
            this.exportChoiceAction.ConfirmationMessage = null;
            this.exportChoiceAction.Id = "ExportsSingleChoiceAction";
            this.exportChoiceAction.ImageName = "Action_Open";
            this.exportChoiceAction.ItemType = DevExpress.ExpressApp.Actions.SingleChoiceActionItemType.ItemIsOperation;
            this.exportChoiceAction.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.exportChoiceAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.exportChoiceAction.ToolTip = null;
            this.exportChoiceAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            // 
            // ExchangeWindowController
            // 
            this.TargetWindowType = DevExpress.ExpressApp.WindowType.Main;

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SingleChoiceAction exportChoiceAction;
    }
}
