using DevExpress.ExpressApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Menu.Base.Model
{
    public static class Extensions
    {
        public static IModelAurum Aurum(this IModelApplication application)
        {
            return ((IModelApplicationAurum)application).Aurum;
        }
    }
}
