using System;
using System.Text;
using System.Linq;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.ReportsV2;
using System.Reflection;
using Aurum.App.Module.Controllers;

namespace Aurum.App.Module
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppModuleBasetopic.
    public sealed partial class AppModule : ModuleBase
    {
        public AppModule()
        {
            InitializeComponent();
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
        {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);

            return new ModuleUpdater[] { updater, GetPredefinedReportsUpdater(objectSpace, versionFromDB) };
        }
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            // Manage various aspects of the application UI and behavior at the module level.
        }

        private PredefinedReportsUpdater GetPredefinedReportsUpdater(IObjectSpace objectSpace, Version versionFromDB)
        {
            PredefinedReportsUpdater predefinedReportsUpdater = new PredefinedReportsUpdater(Application, objectSpace, versionFromDB);

            MethodInfo methodAddPredefinedReportGeneric = typeof(PredefinedReportsUpdater).GetMethod("AddPredefinedReport", new Type[] { typeof(string), typeof(Type), typeof(Type), typeof(bool) });
            foreach (var report in ReportsWindowController.Reports)
            {
                MethodInfo methodAddPredefinedReport = methodAddPredefinedReportGeneric.MakeGenericMethod(new Type[] { report.Item1 });
                methodAddPredefinedReport.Invoke(predefinedReportsUpdater, new object[] { report.Item4, report.Item2, report.Item3, report.Item5 });
            }

            return predefinedReportsUpdater;
        }
    }
}
