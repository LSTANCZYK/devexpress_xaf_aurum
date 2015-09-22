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
using DevExpress.ExpressApp.Security;

namespace Aurum.Interface.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class AurumModificationsController : ViewController
    {
        public AurumModificationsController()
        {
            InitializeComponent();
            RegisterActions(components);
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetViewType = ViewType.DetailView;
            TargetViewNesting = Nesting.Root;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            if (View is ObjectView)
            {
                var modificationsController = Frame.GetController<ModificationsController>();
                if (modificationsController != null)
                {
                    bool securityCanEdit;
                    if (!(SecuritySystem.Instance is IRequestSecurity))
                    {
                        securityCanEdit = true;
                    }
                    else
                    {
                        securityCanEdit = DataManipulationRight.CanEdit(((ObjectView)View).ObjectTypeInfo.Type, null, View.CurrentObject, LinkToListViewController.FindCollectionSource(Frame), View.ObjectSpace);
                    }
                    modificationsController.SaveAction.Enabled["AurumSecurity"] = securityCanEdit;
                    modificationsController.SaveAndCloseAction.Enabled["AurumSecurity"] = securityCanEdit;
                    modificationsController.SaveAndNewAction.Enabled["AurumSecurity"] = securityCanEdit;

                    if (View is DetailView && !securityCanEdit)
                    {
                        ((DetailView)View).ViewEditMode = ViewEditMode.View;
                    }
                }
            }
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
