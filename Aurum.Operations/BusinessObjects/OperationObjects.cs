using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Operations
{
    /// <summary>
    /// Domain объект для отображения списка операций
    /// </summary>
    [DomainComponent]
    public class OperationObjects
    {
        /// <summary>
        /// Список операций
        /// </summary>
        public List<OperationObject> Objects
        {
            get { return OperationManager.Default.Operations; }
        }
    }
}
