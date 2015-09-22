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
using System.Windows.Forms;

namespace Aurum.Operations.Win.Controllers
{
    public partial class WinNotifierInit_WindowController : WindowController
    {
        public WinNotifierInit_WindowController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            Window.TemplateChanged += Window_TemplateChanged;
        }

        void Window_TemplateChanged(object sender, EventArgs e)
        {
            WinNotifier.Form = Window.Template as Form;
        }

        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
        }

        protected override void OnAfterConstruction()
        {
            base.OnAfterConstruction();
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            WinNotifier.Form = null;
        }
    }
}
