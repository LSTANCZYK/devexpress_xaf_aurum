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
    public class OperationObject_DetailViewController_Win : AutoRefreshViewController
    {
        public OperationObject_DetailViewController_Win()
            : base()
        {
            TargetViewType = ViewType.DetailView;
            TargetObjectType = typeof(OperationObject);
            Interval = 1000d;
        }
    }
}
