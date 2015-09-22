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
using System.Threading.Tasks;

namespace Aurum.Operations.Controllers
{
    /// <summary>
    /// Контроллер, содержащий действие для показа OperationObjects
    /// </summary>
    public partial class OperationWindowController : WindowController
    {
        public OperationWindowController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        private void showOperationsAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var newView = Application.CreateDetailView(Application.CreateObjectSpace(), new OperationObjects());
            e.ShowViewParameters.CreatedView = newView;
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.Context = TemplateContext.PopupWindow;
        }
    }
}
