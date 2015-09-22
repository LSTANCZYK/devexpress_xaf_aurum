namespace Aurum.Reports.Controllers
{
    partial class ReportWizardItemController
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
            this.CreateItem = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
            this.DeleteItem = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.DownItem = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.UpItem = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // CreateItem
            // 
            this.CreateItem.Caption = "New";
            this.CreateItem.Category = "ObjectsCreation";
            this.CreateItem.ConfirmationMessage = null;
            this.CreateItem.Id = "ReportWizardItemCreate";
            this.CreateItem.ImageName = "MenuBar_New";
            this.CreateItem.ItemType = DevExpress.ExpressApp.Actions.SingleChoiceActionItemType.ItemIsOperation;
            this.CreateItem.Shortcut = "CtrlN";
            this.CreateItem.TargetObjectType = typeof(Aurum.Reports.ReportWizardItem);
            this.CreateItem.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.CreateItem.ToolTip = null;
            this.CreateItem.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.CreateItem.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.CreateItem_Execute);
            // 
            // DeleteItem
            // 
            this.DeleteItem.Caption = "Delete";
            this.DeleteItem.Category = "Edit";
            this.DeleteItem.ConfirmationMessage = "You are about to delete the selected record(s). Do you want to proceed?";
            this.DeleteItem.Id = "ReportWizardItemDelete";
            this.DeleteItem.ImageName = "MenuBar_Delete";
            this.DeleteItem.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireMultipleObjects;
            this.DeleteItem.Shortcut = "CtrlD";
            this.DeleteItem.TargetObjectType = typeof(Aurum.Reports.ReportWizardItem);
            this.DeleteItem.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.DeleteItem.ToolTip = null;
            this.DeleteItem.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.DeleteItem.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.DeleteItem_Execute);
            // 
            // DownItem
            // 
            this.DownItem.Caption = "Report Wizard Item Down";
            this.DownItem.Category = "Edit";
            this.DownItem.ConfirmationMessage = null;
            this.DownItem.Id = "ReportWizardItemDown";
            this.DownItem.ImageName = "Action_Navigation_Next_Object";
            this.DownItem.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireMultipleObjects;
            this.DownItem.TargetObjectType = typeof(Aurum.Reports.ReportWizardItem);
            this.DownItem.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.DownItem.ToolTip = null;
            this.DownItem.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.DownItem.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.DownItem_Execute);
            // 
            // UpItem
            // 
            this.UpItem.Caption = "Report Wizard Item Up";
            this.UpItem.Category = "Edit";
            this.UpItem.ConfirmationMessage = null;
            this.UpItem.Id = "ReportWizardItemUp";
            this.UpItem.ImageName = "Action_Navigation_Previous_Object";
            this.UpItem.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireMultipleObjects;
            this.UpItem.TargetObjectType = typeof(Aurum.Reports.ReportWizardItem);
            this.UpItem.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.UpItem.ToolTip = null;
            this.UpItem.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.UpItem.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.UpItem_Execute);
            // 
            // ReportWizardItemController
            // 
            this.TargetObjectType = typeof(Aurum.Reports.ReportWizardItem);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SingleChoiceAction CreateItem;
        private DevExpress.ExpressApp.Actions.SimpleAction DeleteItem;
        private DevExpress.ExpressApp.Actions.SimpleAction DownItem;
        private DevExpress.ExpressApp.Actions.SimpleAction UpItem;
    }
}
