namespace Aurum.Reports.Controllers
{
    partial class ReportWizardBlockController
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
            this.MoveUp = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.MoveDown = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // MoveUp
            // 
            this.MoveUp.Caption = "Up";
            this.MoveUp.Category = "Edit";
            this.MoveUp.ConfirmationMessage = null;
            this.MoveUp.Id = "ReportWizardBlockUp";
            this.MoveUp.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.MoveUp.TargetObjectType = typeof(Aurum.Reports.ReportWizardBlock);
            this.MoveUp.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.MoveUp.ToolTip = null;
            this.MoveUp.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.MoveUp.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.MoveUp_Execute);
            // 
            // MoveDown
            // 
            this.MoveDown.Caption = "Down";
            this.MoveDown.Category = "Edit";
            this.MoveDown.ConfirmationMessage = null;
            this.MoveDown.Id = "ReportWizardBlockDown";
            this.MoveDown.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.MoveDown.TargetObjectType = typeof(Aurum.Reports.ReportWizardBlock);
            this.MoveDown.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.MoveDown.ToolTip = null;
            this.MoveDown.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.MoveDown.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.MoveDown_Execute);
            // 
            // ReportWizardBlockController
            // 
            this.TargetObjectType = typeof(Aurum.Reports.ReportWizardBlock);
            this.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.TypeOfView = typeof(DevExpress.ExpressApp.ListView);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction MoveUp;
        private DevExpress.ExpressApp.Actions.SimpleAction MoveDown;
    }
}
