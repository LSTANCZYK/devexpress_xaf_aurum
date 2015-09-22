namespace Aurum.Reports.Controllers
{
    partial class ReportStyleGroupController
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
            this.CountAction = new DevExpress.ExpressApp.Actions.ParametrizedAction(this.components);
            // 
            // CountAction
            // 
            this.CountAction.Caption = "Count";
            this.CountAction.Category = "ObjectsCreation";
            this.CountAction.ConfirmationMessage = null;
            this.CountAction.Id = "ReportStyleGroupCountAction";
            this.CountAction.NullValuePrompt = null;
            this.CountAction.ShortCaption = null;
            this.CountAction.ToolTip = null;
            this.CountAction.ValueType = typeof(int);
            this.CountAction.Execute += new DevExpress.ExpressApp.Actions.ParametrizedActionExecuteEventHandler(this.CountAction_Execute);
            // 
            // ReportStyleGroupController
            // 
            this.TargetObjectType = typeof(Aurum.Reports.ReportStyleGroup);
            this.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.TypeOfView = typeof(DevExpress.ExpressApp.ListView);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.ParametrizedAction CountAction;
    }
}
