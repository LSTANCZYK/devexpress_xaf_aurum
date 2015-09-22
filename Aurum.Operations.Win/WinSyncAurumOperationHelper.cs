using Aurum.Operations.Win.Controllers;
using DevExpress.ExpressApp;
using System;

namespace Aurum.Operations.Win
{
    public class WinSyncAurumOperationHelper : AurumOperationHelper
    {
        public WinSyncAurumOperationHelper(XafApplication application, Guid operationObjectId)
            : base(application, operationObjectId)
        {
        }
        public WinSyncAurumOperationHelper(AurumOperation operation)
            : base(operation)
        {
        }
        protected override void InitShowViewParametersCore(ShowViewParameters parameters, string detailViewId, bool autoCloseWindow)
        {
            base.InitShowViewParametersCore(parameters, detailViewId, autoCloseWindow);
            parameters.Controllers.Add(new WinSyncAurumOperationDialogController
            {
                OperationManager = base.OperationManager,
                AutoCloseWindow = autoCloseWindow,
                Operation = base.Operation
            });
        }
    }
}
