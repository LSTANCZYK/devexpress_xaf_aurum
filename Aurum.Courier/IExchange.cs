using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Courier
{
    /// <summary>
    /// Обмен данными
    /// </summary>
    public interface IExchange
    {
        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="courier">Курьер</param>
        void Init(ICourier courier);
    }
}
