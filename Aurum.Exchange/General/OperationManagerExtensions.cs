using Aurum.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Exchange
{
    /// <summary>
    /// Расширение для Aurum.Operations.OperationManager
    /// </summary>
    public static class OperationManagerExtensions
    {
        /// <summary>
        /// Запустить операцию обмена
        /// </summary>
        /// <param name="mgr">Менеджер операций</param>
        /// <param name="op">Операция обмена</param>
        /// <returns>Объект представления операции</returns>
        /// <exception cref="System.ArgumentNullException" />
        /// <exception cref="System.ArgumentException">Операция обмена не реализует ни Aurum.Operations.IOperation, ни Aurum.Operations.ICompositeOperation</exception>
        public static OperationObject Run(this OperationManager mgr, ExchangeOperation op)
        {
            if (mgr == null)
            {
                throw new ArgumentNullException("mgr");
            }
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            if (op is ICompositeOperation)
            {
                return OperationManager.Default.Run(op as ICompositeOperation);
            }
            else if (op is IOperation)
            {
                return OperationManager.Default.Run(op as IOperation);
            }
            else throw new ArgumentException("Operation cannot be run");
        }
    }
}
