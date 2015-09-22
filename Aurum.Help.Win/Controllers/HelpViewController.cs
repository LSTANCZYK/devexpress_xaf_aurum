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
using DevExpress.ExpressApp.HtmlPropertyEditor.Win;

namespace Aurum.Help.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class HelpViewController : ViewController
    {
        public HelpViewController()
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

        private void helpPopupWindowShowAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            HelpView hw = objectSpace.FindObject<HelpView>(CriteriaOperator.Parse("ViewId = ?", View.Id));
            HelpDocument help = objectSpace.CreateObject<HelpDocument>();
            help.Text = hw != null ? hw.Text : null;
            DetailView dv = Application.CreateDetailView(objectSpace, help, false);
            HtmlPropertyEditor htmlEditor = (HtmlPropertyEditor)dv.Items[0];
            htmlEditor.ControlCreated += htmlEditor_ControlCreated;
            e.View = dv;
        }

        void htmlEditor_ControlCreated(object sender, EventArgs e)
        {
            HtmlPropertyEditor htmlPropertyEditor = (HtmlPropertyEditor)sender;
            HtmlEditor htmlEditor = htmlPropertyEditor.Editor;
            WebBrowserEx webBrowserEx = (WebBrowserEx)htmlEditor.TabControl.TabPages[0].Controls[0];
            webBrowserEx.Parent = null;
            htmlEditor.Controls.Remove(htmlEditor.TabControl);
            htmlEditor.Controls.Add(webBrowserEx);

            // htmlEditor.Editor.TabControl.TabPages.RemoveAt(1);
        }
    }
}
