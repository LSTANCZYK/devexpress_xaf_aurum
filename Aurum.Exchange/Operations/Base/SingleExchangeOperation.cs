using Aurum.Operations;
using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Exchange
{
    public abstract class SingleExchangeOperation : ExchangeOperationBase, IOperation
    {
        private OperationInfo operationInfo;

        public OperationInfo OperationInfo
        {
            get
            {
                if (operationInfo == null)
                {
                    operationInfo = new OperationInfo { Name = Model != null ? Model.Name : OperationInfo.Empty.Name };
                }
                return operationInfo;
            }
        }

        public void Execute(OperationInterop interop)
        {
            ExecuteExchange(interop);
        }

        protected abstract void ExecuteExchange(OperationInterop interop);

        public SingleExchangeOperation(XafApplication app)
            : base(app)
        {
        }
    }
}
