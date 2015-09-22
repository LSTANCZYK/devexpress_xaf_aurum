using DevExpress.ExpressApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Operations
{
    /// <summary>
    /// Информация об операции
    /// </summary>
    public class OperationInfo
    {
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// "Пустая" информация об операции
        /// </summary>
        public static OperationInfo Empty
        {
            get
            {
                return new OperationInfo
                {
                    Name = CaptionHelper.GetLocalizedText("Captions", "UnnamedOperation")
                };
            }
        }
    }
}
