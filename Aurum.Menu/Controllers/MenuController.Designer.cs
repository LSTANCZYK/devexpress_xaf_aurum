namespace Aurum.Menu.Controllers
{
    partial class MenuController
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
            this.MenuItemsAction = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
            // 
            // MenuItemsAction
            // 
            this.MenuItemsAction.Caption = "Menus";
            this.MenuItemsAction.Category = "Menus";
            this.MenuItemsAction.ConfirmationMessage = null;
            this.MenuItemsAction.EmptyItemsBehavior = DevExpress.ExpressApp.Actions.EmptyItemsBehavior.Disable;
            this.MenuItemsAction.Id = "MenuItemsAction";
            this.MenuItemsAction.ItemType = DevExpress.ExpressApp.Actions.SingleChoiceActionItemType.ItemIsOperation;
            this.MenuItemsAction.Shortcut = null;
            this.MenuItemsAction.Tag = null;
            this.MenuItemsAction.TargetObjectsCriteria = null;
            this.MenuItemsAction.TargetViewId = null;
            this.MenuItemsAction.ToolTip = null;
            this.MenuItemsAction.TypeOfView = null;
            this.MenuItemsAction.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.menuItemAction_Execute);
            // 
            // MenuController
            // 
            this.TargetWindowType = DevExpress.ExpressApp.WindowType.Main;
            this.Activated += new System.EventHandler(this.MenuItemsController_Activated);
            this.Deactivated += new System.EventHandler(this.MenuController_Deactivating);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SingleChoiceAction MenuItemsAction;
    }
}
