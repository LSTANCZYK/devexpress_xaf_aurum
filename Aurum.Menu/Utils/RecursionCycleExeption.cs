using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Menu.Utils
{
    [Serializable]
    public class RecursionCycleExeption : Exception
    {
        public IEnumerable CycleItems
        {
            get;
            private set;
        }
        public RecursionCycleExeption(IEnumerable cycleItems)
        {
            this.CycleItems = cycleItems;
        }
        public RecursionCycleExeption(IEnumerable cycleItems, SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.CycleItems = cycleItems;
        }
        public RecursionCycleExeption(IEnumerable cycleItems, string message)
            : base(message)
        {
            this.CycleItems = cycleItems;
        }
        public RecursionCycleExeption(IEnumerable cycleItems, string message, Exception innerException)
            : base(message, innerException)
        {
            this.CycleItems = cycleItems;
        }
    }
}
