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
    public partial class OperationObject_ViewController : ViewController
    {
        public OperationObject_ViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        private void cancelAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var x = (OperationObject)e.CurrentObject;
            try
            {
                OperationManager.Default.Cancel(x);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex);
            }
        }

        private void cancelAllAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var ops = ObjectSpace.GetObjects<OperationObject>();
            foreach (var op in ops)
            {
                try
                {
                    OperationManager.Default.Cancel(op);
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException(ex);
                }
            }
        }
    }
}
