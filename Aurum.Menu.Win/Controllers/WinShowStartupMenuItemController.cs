using Aurum.Menu.Controllers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.SystemModule;
using System;

namespace Aurum.Menu.Win.Controllers
{
#pragma warning disable 618
    public class WinShowStartupMenuItemController : WindowController
    {
        protected const string InitKey = "Used WinShowStartupMenuItemController";
        private MenuController menuController;
        private bool isDelayedShowStartup;
        public WinShowStartupMenuItemController()
        {
            base.TargetWindowType = WindowType.Main;
        }
        protected override void OnWindowChanging(Window window)
        {
            base.OnWindowChanging(window);
            if (!window.IsMain)
            {
                return;
            }
            this.menuController = base.Frame.GetController<MenuController>();
            if (this.menuController != null)
            {
                this.menuController.Activated += new EventHandler(this.controller_Activated);
                this.menuController.Deactivated += new EventHandler(this.controller_Deactivated);
            }
        }
        private void controller_Activated(object sender, EventArgs e)
        {
            if (this.isDelayedShowStartup)
            {
                this.DoShowStartup();
            }
            this.SetActiveWinStartup(false);
        }
        private void controller_Deactivated(object sender, EventArgs e)
        {
            this.SetActiveWinStartup(true);
        }
        private void SetActiveWinStartup(bool flag)
        {
            WinShowStartupNavigationItemController controller = base.Frame.GetController<WinShowStartupNavigationItemController>();
            if (controller != null)
            {
                controller.Active[InitKey] = flag;
            }
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            ((WinShowViewStrategyBase)base.Application.ShowViewStrategy).StartupWindowShown += new EventHandler(this.WinShowNavigationItemController_StartupWindowShown);
        }
        protected override void OnDeactivated()
        {
            ((WinShowViewStrategyBase)base.Application.ShowViewStrategy).StartupWindowShown -= new EventHandler(this.WinShowNavigationItemController_StartupWindowShown);
            base.OnDeactivated();
        }
        private void WinShowNavigationItemController_StartupWindowShown(object sender, EventArgs e)
        {
            ((WinShowViewStrategyBase)base.Application.ShowViewStrategy).StartupWindowShown -= new EventHandler(this.WinShowNavigationItemController_StartupWindowShown);
            this.isDelayedShowStartup = false;
            if (this.menuController.Active)
            {
                this.DoShowStartup();
                return;
            }
            this.isDelayedShowStartup = true;
        }
        private void DoShowStartup()
        {
            MenuItem startupMenuItem = this.menuController.GetStartupMenuItem();
            if (startupMenuItem != null)
            {
                this.menuController.MenuAction.DoExecute(startupMenuItem);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.menuController != null)
                {
                    this.menuController.Activated -= new EventHandler(this.controller_Activated);
                    this.menuController.Deactivated -= new EventHandler(this.controller_Deactivated);
                }
                this.menuController = null;
            }
            base.Dispose(disposing);
        }
    }
#pragma warning restore 618
}
