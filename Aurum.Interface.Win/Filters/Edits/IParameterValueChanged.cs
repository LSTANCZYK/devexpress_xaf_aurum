using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// Интерфейс с событием изменения значения параметра.
    /// </summary>
    public interface IParameterValueChanged
    {
        /// <summary>
        /// Событие изменения параметра. 
        /// </summary>
        event EventHandler ValueChanged;
    }
}
