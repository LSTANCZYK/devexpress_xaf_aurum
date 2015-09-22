using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base.Security;
using System;
using System.ComponentModel;

namespace Aurum.Menu.Win.Controllers
{
    public class ShowNavigationPermissionItemController : ShowNavigationItemController
    {
        private IContainer components;
        public ShowNavigationPermissionItemController()
        {
            this.InitializeComponent();
            base.RegisterActions(this.components);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
        }
        protected override bool HasRights(ChoiceActionItem item, IModelViews views)
        {
            return true;

            /*
            if (NavigationPermission.AllGranted)
            {
                return base.HasRights(item, views);
            }
            return base.HasRights(item, views) && SecuritySystem.IsGranted(new NavigationPermission(this.GetIdPath(item)));
            */
        }
        private string GetIdPath(ChoiceActionItem сhoiceActionItem)
        {
            ModelNode modelNode = (ModelNode)сhoiceActionItem.Model;
            string text = modelNode.Id;
            ModelNode modelNode2 = modelNode;
            while (modelNode2 != null && modelNode2.Parent.Parent != null && modelNode2.Parent.Parent.Id != "NavigationItems")
            {
                text = string.Format("{0}/{1}", modelNode2.Parent.Parent.Id, text);
                modelNode2 = modelNode2.Parent.Parent;
            }
            return text;
        }
        private void ShowNavigationPermissionItemController_Activated(object sender, EventArgs e)
        {
            NavigationPermission.AllGranted = true;

            /*
            IUserWithRoles userWithRoles = SecuritySystem.CurrentUser as IUserWithRoles;
            if (userWithRoles == null)
            {
                return;
            }
            foreach (IRole current in userWithRoles.Roles)
            {
                if (!(current.Name != SecurityStrategy.AdministratorRoleName) || !(current.Name != "Администратор"))
                {
                    NavigationPermission.AllGranted = true;
                    break;
                }
                NavigationPermission.AllGranted = false;
            }
            */
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.components = new Container();
            base.Activated += new EventHandler(this.ShowNavigationPermissionItemController_Activated);
        }
    }
}
