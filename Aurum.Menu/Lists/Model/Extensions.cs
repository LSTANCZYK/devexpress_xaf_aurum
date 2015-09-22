using Aurum.Menu.Base.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Menu.Security.Model
{
    public static class Extensions
    {
        public static IModelAurumLists AurumLists(this IModelAurum aurum)
        {
            return (IModelAurumLists)aurum;
        }
    }
}
