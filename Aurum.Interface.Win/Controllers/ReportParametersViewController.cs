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
using DevExpress.ExpressApp.ReportsV2;
using System.Windows.Forms;
using DevExpress.ExpressApp.Win;

namespace Aurum.Interface.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class ReportParametersViewController : ViewController
    {
        public ReportParametersViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
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
        protected override void OnViewChanged()
        {
            base.OnViewChanged();
            if (View != null && View.ObjectTypeInfo != null && View.ObjectTypeInfo.Type != null && View.ObjectTypeInfo.Type.IsSubclassOf(typeof(ReportParametersObjectBase)))
            {
                View.ControlsCreated += View_ControlsCreated;
            }
        }

        void View_ControlsCreated(object sender, EventArgs e)
        {
            var view = (DevExpress.ExpressApp.DetailView)sender;
            Control control = (Control)view.Control;
            control.ParentChanged += control_ParentChanged;
        }

        void control_ParentChanged(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            var form = control.FindForm();
            if (form != null)
            {
                WinApplication app = (WinApplication)Application;
                WinWindow mainWindow = (WinWindow)app.MainWindow;
                form.Owner = mainWindow.Form;
                mainWindow.Disposing += (sender2, e2) =>
                    {
                        if (form != null && !form.IsDisposed)
                        {
                            form.Close();
                        }
                    };
            }
        }
    }
}
