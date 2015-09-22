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
using Aurum.Reports.ModelUpdate;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Validation;

namespace Aurum.Reports
{
    /// <summary>
    /// Модуль расширения отчетов
    /// </summary>
    public sealed partial class AurumReportsModule : ModuleBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public AurumReportsModule()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            updaters.Add(new ReportWizardListViewUpdater());
            base.AddGeneratorUpdaters(updaters);
        }

        /// <inheritdoc/>
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
        {
            return ModuleUpdater.EmptyModuleUpdaters;
        }

        /// <inheritdoc/>
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
        }

        /// <inheritdoc/>
        public override void Setup(ApplicationModulesManager moduleManager)
        {
            base.Setup(moduleManager);
            ValidationRulesRegistrator.RegisterRule(moduleManager, typeof(ReportWizardValidationRule), typeof(IRuleBaseProperties));
            ValidationRulesRegistrator.RegisterRule(moduleManager, typeof(PredefinedReportWizardSaveRule), typeof(IRuleBaseProperties));
        }
    }
}
