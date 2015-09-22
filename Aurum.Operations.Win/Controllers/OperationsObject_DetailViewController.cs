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
    public class OperationsObject_DetailViewController : AutoRefreshViewController
    {
        public OperationsObject_DetailViewController()
            : base()
        {
            TargetViewType = ViewType.DetailView;
            TargetObjectType = typeof(OperationObjects);
            Interval = 1000d;
        }
    }
}
