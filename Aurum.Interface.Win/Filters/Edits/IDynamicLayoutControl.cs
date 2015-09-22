using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// »нтерфейс динамического размещени€
    /// </summary>
    public interface IDynamicLayoutControl
    {
        /// <summary>
        /// ѕолучение минимального и максимального размера контрола в зависимости от предложенной области
        /// </summary>
        /// <param name="proposal">–азмеры области, в которую может быть вписан контрол. ћинимум X,Y; ћаксимум Width, Height</param>
        /// <returns>ƒиапазон размеров контрола. X,Y - минимальные размеры, Width, Height - максимальные.</returns>
        Rectangle GetSizeRange(Rectangle proposal);

        /// <summary>
        /// Ќаправлени€, в которых может раст€гиватьс€ контрол
        /// </summary>
        /// <returns></returns>
        ChangeSizeDirection GetDirection();
    }
}
