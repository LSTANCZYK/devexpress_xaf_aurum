namespace Aurum.Interface.Win.Controllers
{
    partial class FilterWindowViewController
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
            this.filterWindowShowAction = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.clearFilterAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // filterWindowShowAction
            // 
            this.filterWindowShowAction.AcceptButtonCaption = null;
            this.filterWindowShowAction.ActionMeaning = DevExpress.ExpressApp.Actions.ActionMeaning.Accept;
            this.filterWindowShowAction.CancelButtonCaption = null;
            this.filterWindowShowAction.Caption = "Фильтр";
            this.filterWindowShowAction.ConfirmationMessage = null;
            this.filterWindowShowAction.Id = "FilterWindowShowAction";
            this.filterWindowShowAction.ImageName = "Action_Filter";
            this.filterWindowShowAction.Shortcut = "Control+F";
            this.filterWindowShowAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.filterWindowShowAction.ToolTip = null;
            this.filterWindowShowAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.filterWindowShowAction.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.filterWindowShowAction_CustomizePopupWindowParams);
            this.filterWindowShowAction.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.filterWindowShowAction_Execute);
            // 
            // clearFilterAction
            // 
            this.clearFilterAction.Caption = "Очистить фильтр";
            this.clearFilterAction.ConfirmationMessage = null;
            this.clearFilterAction.Id = "clearFilterAction";
            this.clearFilterAction.ImageName = "Action_Deny";
            this.clearFilterAction.Shortcut = "Control+Shift+F";
            this.clearFilterAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.clearFilterAction.ToolTip = null;
            this.clearFilterAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.clearFilterAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.clearFilterAction_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction filterWindowShowAction;
        private DevExpress.ExpressApp.Actions.SimpleAction clearFilterAction;
    }
}
