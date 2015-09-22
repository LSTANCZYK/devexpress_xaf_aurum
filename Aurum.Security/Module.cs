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

namespace Aurum.Security
{
    /// <summary>
    /// ������ ������������ ���� ������
    /// </summary>
    public sealed partial class AurumSecurityModule : ModuleBase
    {
        static AurumSecurityModule()
        {
            ModelNodesGeneratorSettings.SetIdPrefix(typeof(Contract), "Aurum_Contract");
        }
        /// <inheritdoc/>
        public AurumSecurityModule()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
        {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new ModuleUpdater[] { updater };
        }

        /// <inheritdoc/>
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            // Manage various aspects of the application UI and behavior at the module level.
        }
    }
}
