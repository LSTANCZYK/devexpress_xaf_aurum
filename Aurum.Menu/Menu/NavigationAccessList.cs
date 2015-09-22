using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Menu
{
    public class NavigationAccessList
    {
        public IList<string> Items
        {
            get;
            set;
        }

        public NavigationAccessList()
        {
            this.Items = new List<string>();
        }
    }
}
