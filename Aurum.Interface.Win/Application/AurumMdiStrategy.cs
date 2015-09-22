using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Interface.Win.Mdi
{
    /// <summary>
    /// MdiStrategy
    /// </summary>
    public class AurumMdiStrategy : MdiShowViewStrategy
    {
        public AurumMdiStrategy(XafApplication application, MdiMode mdiMode)
			: base(application, mdiMode) {
		}

        public AurumMdiStrategy(XafApplication application)
			: base(application) {
		}

        // открывать новые окна в новых табах
        protected override bool IsNewWindowForced(DevExpress.ExpressApp.ShowViewParameters parameters, DevExpress.ExpressApp.ShowViewSource showViewSource)
        {
            return true;
        }
    }
}
