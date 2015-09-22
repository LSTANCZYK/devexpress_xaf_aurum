using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Operations
{
    /// <summary>
    /// Простая операция
    /// </summary>
    public class SimpleOperation : IOperation, IDisposable
    {
        private readonly Action<OperationInterop> action;

        /// <summary>
        /// Выполнить операцию
        /// </summary>
        /// <param name="interop">Объект взаимодействия</param>
        public void Execute(OperationInterop interop)
        {
            action.Invoke(interop);
        }

        /// <summary>
        /// Объект информации
        /// </summary>
        public OperationInfo OperationInfo { get; private set; }

        #region IDisposable & Co
        public virtual void Dispose(bool disposing)
        {
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }
        #endregion

        /// <summary>
        /// Создать простую операцию из существующего метода
        /// </summary>
        /// <param name="action">Метод</param>
        /// <param name="info">Объект информации</param>
        /// <exception cref="System.ArgumentNullException" />
        public SimpleOperation(Action<OperationInterop> action, OperationInfo info)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            this.action = action;
            this.OperationInfo = info;
        }
    }
}
