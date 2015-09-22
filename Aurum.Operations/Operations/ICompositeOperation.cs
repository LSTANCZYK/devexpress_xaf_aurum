using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Operations
{
    /// <summary>
    /// Составная операция
    /// </summary>
    public interface ICompositeOperation : IDisposable
    {
        /// <summary>
        /// Данные об операции
        /// </summary>
        OperationInfo OperationInfo { get; }

        /// <summary>
        /// Операции
        /// </summary>
        IOperation[] GetOperations();

        /// <summary>
        /// Действие, выполняющееся перед началом выполнения цепочки
        /// </summary>
        /// <param name="interop"></param>
        void OnChainStart(OperationInterop interop);

        /// <summary>
        /// Действия, выполнеющееся после выполнения цепочки
        /// </summary>
        void OnChainEnded();

        /// <summary>
        /// Параллельная операция
        /// </summary>
        /// <returns></returns>
        bool IsParallel { get; set; }
    }
}
