using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Courier
{
    /// <summary>
    /// Конфигурация
    /// </summary>
    public interface IConfigurator
    {
        /// <summary>
        /// Строка соединения
        /// </summary>
        string Connection { get; set; }

        /// <summary>
        /// Продукт
        /// </summary>
        string Product { get; set; }
    }
}
