using Aurum.Menu.Controllers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using System;

namespace Aurum.Menu.Win.Controllers
{
#pragma warning disable 618
    public class WinMenuController : MenuController
    {
        protected const string InitKey = "StartupWindow shown";
        private WinShowViewStrategyBase WinShowViewStrategy
        {
            get
            {
                if (base.Application == null)
                {
                    return null;
                }
                return base.Application.ShowViewStrategy as WinShowViewStrategyBase;
            }
        }
        protected override void OnWindowChanging(Window window)
        {
            base.OnWindowChanging(window);
            if (!window.IsMain)
            {
                return;
            }
            base.Active[InitKey] = false;
            if (this.WinShowViewStrategy != null)
            {
                this.WinShowViewStrategy.StartupWindowShown += new EventHandler(this.WinMenuController_StartupWindowShown);
            }
            base.Disposed += new EventHandler(this.WinMenuController_Disposed);
        }
        private void WinMenuController_Disposed(object sender, EventArgs e)
        {
            if (this.WinShowViewStrategy != null)
            {
                this.WinShowViewStrategy.StartupWindowShown -= new EventHandler(this.WinMenuController_StartupWindowShown);
            }
            base.Disposed -= new EventHandler(this.WinMenuController_Disposed);
        }
        private void WinMenuController_StartupWindowShown(object sender, EventArgs e)
        {
            base.Active[InitKey] = true;
            if (this.WinShowViewStrategy != null)
            {
                this.WinShowViewStrategy.StartupWindowShown -= new EventHandler(this.WinMenuController_StartupWindowShown);
            }
        }
        public override void RefreshItems()
        {
            if (((WinWindow)base.Window).IsClosing)
            {
                base.Active["Main Form is closing"] = false;
                return;
            }
            base.RefreshItems();
        }
    }
#pragma warning restore 618
}
