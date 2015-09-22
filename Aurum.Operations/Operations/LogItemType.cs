using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Operations
{
    /// <summary>
    /// Типы сообщений журнала операции
    /// </summary>
    public enum LogItemType
    {
        /// <summary>
        /// Информация
        /// </summary>
        Info = 0,

        /// <summary>
        /// Смена статуса
        /// </summary>
        Status = 1,

        /// <summary>
        /// Предупреждение
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Ошибка
        /// </summary>
        Error = 3
    }
}
