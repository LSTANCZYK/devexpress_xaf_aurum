using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Courier
{
    internal class RabbitConfigurator : IConfigurator, ICloneable
    {
        public static RabbitConfigurator Default = new RabbitConfigurator() { Connection = "host=localhost;username=guest;password=guest", Product = "aurum.courier" };

        public string Connection
        {
            get;
            set;
        }

        public string Product
        {
            get;
            set;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
