using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Operations
{
    public class DummyNotifier : INotifier
    {
        public void Notify(string caption, string message, Action action) { }
    }
}
