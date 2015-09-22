namespace Aurum.App.Win
{
    partial class AppWindowsFormsApplication
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
            this.module1 = new DevExpress.ExpressApp.SystemModule.SystemModule();
            this.module2 = new DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule();
            this.module3 = new Aurum.App.Module.AppModule();
            this.reportsModuleV21 = new DevExpress.ExpressApp.ReportsV2.ReportsModuleV2();
            this.aurumOperationsModule1 = new Aurum.Operations.AurumOperationsModule();
            this.aurumOperationsWinModule1 = new Aurum.Operations.Win.AurumOperationsWinModule();
            this.aurumMenuModule1 = new Aurum.Menu.AurumMenuModule();
            this.aurumMenuWinModule1 = new Aurum.Menu.Win.AurumMenuWinModule();
            this.aurumInterfaceModule1 = new Aurum.Interface.AurumInterfaceModule();
            this.aurumInterfaceWinModule1 = new Aurum.Interface.Win.AurumInterfaceWinModule();
            this.aurumExchangeModule1 = new Aurum.Exchange.AurumExchangeModule();
            this.conditionalAppearanceModule1 = new DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule();
            this.reportsWindowsFormsModuleV21 = new DevExpress.ExpressApp.ReportsV2.Win.ReportsWindowsFormsModuleV2();
            this.aurumHelpModule1 = new Aurum.Help.AurumHelpModule();
            this.aurumHelpWinModule1 = new Aurum.Help.Win.AurumHelpWinModule();
            this.htmlPropertyEditorWindowsFormsModule1 = new DevExpress.ExpressApp.HtmlPropertyEditor.Win.HtmlPropertyEditorWindowsFormsModule();
            this.validationModule1 = new DevExpress.ExpressApp.Validation.ValidationModule();
            this.aurumExchangeWinModule1 = new Aurum.Exchange.Win.AurumExchangeWinModule();
            this.aurumOperationsModule2 = new Aurum.Operations.AurumOperationsModule();
            this.aurumOperationsWinModule2 = new Aurum.Operations.Win.AurumOperationsWinModule();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // reportsModuleV21
            // 
            this.reportsModuleV21.EnableInplaceReports = true;
            this.reportsModuleV21.ReportDataType = typeof(DevExpress.Persistent.BaseImpl.ReportDataV2);
            // 
            // validationModule1
            // 
            this.validationModule1.AllowValidationDetailsAccess = true;
            this.validationModule1.IgnoreWarningAndInformationRules = false;
            // 
            // AppWindowsFormsApplication
            // 
            this.ApplicationName = "Aurum.App";
            this.Modules.Add(this.module1);
            this.Modules.Add(this.module2);
            this.Modules.Add(this.reportsModuleV21);
            this.Modules.Add(this.conditionalAppearanceModule1);
            this.Modules.Add(this.aurumOperationsModule1);
            this.Modules.Add(this.aurumMenuModule1);
            this.Modules.Add(this.aurumInterfaceModule1);
            this.Modules.Add(this.aurumOperationsWinModule1);
            this.Modules.Add(this.validationModule1);
            this.Modules.Add(this.aurumExchangeModule1);
            this.Modules.Add(this.aurumHelpModule1);
            this.Modules.Add(this.module3);
            this.Modules.Add(this.aurumMenuWinModule1);
            this.Modules.Add(this.aurumInterfaceWinModule1);
            this.Modules.Add(this.reportsWindowsFormsModuleV21);
            this.Modules.Add(this.htmlPropertyEditorWindowsFormsModule1);
            this.Modules.Add(this.aurumHelpWinModule1);
            this.Modules.Add(this.aurumExchangeWinModule1);
            this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.AppWindowsFormsApplication_DatabaseVersionMismatch);
            this.CreateCustomTemplate += new System.EventHandler<DevExpress.ExpressApp.CreateCustomTemplateEventArgs>(this.AppWindowsFormsApplication_CreateCustomTemplate);
            this.CustomizeLanguagesList += new System.EventHandler<DevExpress.ExpressApp.CustomizeLanguagesListEventArgs>(this.AppWindowsFormsApplication_CustomizeLanguagesList);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.ExpressApp.SystemModule.SystemModule module1;
        private DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule module2;
        private Aurum.App.Module.AppModule module3;
        private DevExpress.ExpressApp.ReportsV2.ReportsModuleV2 reportsModuleV21;
        private Operations.AurumOperationsModule aurumOperationsModule1;
        private Operations.Win.AurumOperationsWinModule aurumOperationsWinModule1;
        private Menu.AurumMenuModule aurumMenuModule1;
        private Menu.Win.AurumMenuWinModule aurumMenuWinModule1;
        private Interface.AurumInterfaceModule aurumInterfaceModule1;
        private Interface.Win.AurumInterfaceWinModule aurumInterfaceWinModule1;
        private Exchange.AurumExchangeModule aurumExchangeModule1;
        private DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule conditionalAppearanceModule1;
        private DevExpress.ExpressApp.ReportsV2.Win.ReportsWindowsFormsModuleV2 reportsWindowsFormsModuleV21;
        private Help.AurumHelpModule aurumHelpModule1;
        private Help.Win.AurumHelpWinModule aurumHelpWinModule1;
        private DevExpress.ExpressApp.HtmlPropertyEditor.Win.HtmlPropertyEditorWindowsFormsModule htmlPropertyEditorWindowsFormsModule1;
        private DevExpress.ExpressApp.Validation.ValidationModule validationModule1;
        private Aurum.Exchange.Win.AurumExchangeWinModule aurumExchangeWinModule1;
        private Operations.AurumOperationsModule aurumOperationsModule2;
        private Operations.Win.AurumOperationsWinModule aurumOperationsWinModule2;
    }
}
