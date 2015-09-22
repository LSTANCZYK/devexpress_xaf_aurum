using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// Интерфейс параметра метода
    /// </summary>
    public interface IMethodParameter
    {
        /// <summary>
        /// Редактируемое значение
        /// </summary>
        object Value
        {
            get;
            set;
        }

        /// <summary>
        /// Возможность пустого значения
        /// </summary>
        bool IsNullable
        {
            get;
            set;
        }
    }
}
