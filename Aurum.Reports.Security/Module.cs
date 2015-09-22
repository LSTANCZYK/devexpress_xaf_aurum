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

namespace Aurum.Reports.Security
{
    /// <summary>
    /// Модуль расширения отчетов собственников данных
    /// </summary>
    public sealed partial class AurumReportsSecurityModule : ModuleBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public AurumReportsSecurityModule()
        {
            InitializeComponent();
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
    }
}
