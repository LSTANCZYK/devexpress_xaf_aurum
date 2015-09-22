using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Courier
{
    /// <summary>
    /// Исключение
    /// </summary>
    public class AurumCourierException : Exception
    {
        public AurumCourierException() : base() { }
        public AurumCourierException(string message) : base(message) { }
        public AurumCourierException(string message, Exception innerException) : base(message, innerException) { }
    }
}
