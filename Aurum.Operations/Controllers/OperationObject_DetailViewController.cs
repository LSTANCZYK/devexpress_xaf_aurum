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

namespace Aurum.Operations.Controllers
{
    /// <summary>
    /// Контроллер, содержащий действия для детального представления операции
    /// </summary>
    public partial class OperationObject_DetailViewController : ViewController
    {
        public OperationObject_DetailViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            OperationManager.Default.Watch((OperationObject)View.CurrentObject);
        }

        protected override void OnDeactivated()
        {
            OperationManager.Default.Unwatch((OperationObject)View.CurrentObject);
            base.OnDeactivated();
        }

        private void hideAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (View.CanClose())
            {
                View.Close();
            }
        }

        private void cancelAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            OperationManager.Default.Cancel((e.CurrentObject as OperationObject).OperationId);
        }
    }
}
