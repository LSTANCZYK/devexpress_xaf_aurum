using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Operations
{
    /// <summary>
    /// Операция
    /// </summary>
    public interface IOperation : IDisposable
    {
        /// <summary>
        /// Выполнить операцию
        /// </summary>
        /// <param name="interop">Посредник</param>
        void Execute(OperationInterop interop);

        /// <summary>
        /// Получить данные об операции
        /// </summary>
        OperationInfo OperationInfo { get; }
    }
}
