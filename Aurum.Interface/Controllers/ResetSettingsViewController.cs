using System;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.XtraEditors;

namespace Aurum.Interface.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public class ResetSettingsViewController : ViewController
    {
        private const string EnabledKey = "DisabledForNewObjectInDetailView";
        EventHandler committedEventHandler = null;
        public SimpleAction ResetViewSettingsAction { get; private set; }

        public ResetSettingsViewController()
        {
            TargetViewNesting = Nesting.Root;
            ResetViewSettingsAction = new SimpleAction(this, "ResetViewSettings", DevExpress.Persistent.Base.PredefinedCategory.View);
            ResetViewSettingsAction.ImageName = "Attention";
            ResetViewSettingsAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.ResetViewSettingsAction_Execute);
        }
        private void ResetViewSettingsAction_Execute(object sender, DevExpress.ExpressApp.Actions.SimpleActionExecuteEventArgs e)
        {
            try
            {
                IModelView oldModel = View.Model;
                ViewShortcut oldViewShortcut = Frame.View.CreateShortcut();
                Frame.SetView(null);
                ((ModelNode)oldModel).Undo();
                Frame.SetView(Application.ProcessShortcut(oldViewShortcut), true, Frame);
            }
            catch
            {
                XtraMessageBox.Show("An error occurred when resetting the View's settings. Please contact your application administrator for a solution. We will try to close the current View after you press the OK button.",
                    ResetViewSettingsAction.Caption,
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error
                );
                View.Close();
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            if (View.ObjectSpace.IsNewObject(View.CurrentObject) && (View is DetailView))
            {
                ResetViewSettingsAction.Enabled[EnabledKey] = false;
                committedEventHandler = (s, e) =>
                {
                    View.ObjectSpace.Committed -= committedEventHandler;
                    ResetViewSettingsAction.Enabled[EnabledKey] = true;
                };
                View.ObjectSpace.Committed += committedEventHandler;
            }
        }

        protected override void OnDeactivated()
        {
            View.ObjectSpace.Committed -= committedEventHandler;
            base.OnDeactivated();
        }
    }
}
