using System;
using System.Collections.Generic;
using System.Text;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// Направления, в которых может растягиваться контрол
    /// </summary>
    [Flags]
    public enum ChangeSizeDirection
    {
        /// <summary>
        /// Не может растягиваться
        /// </summary>
        None = 0, 
        /// <summary>
        /// Может растягиваться по вертикали
        /// </summary>
        Vertical = 1, 
        /// <summary>
        /// Может растягиваться по горизонтали
        /// </summary>
        Horizontal = 2, 
        /// <summary>
        /// Может растягиваться по вертикали и горизонтали
        /// </summary>
        Both = Vertical | Horizontal
    }
}
