using Aurum.Menu.Base.Model;
using Aurum.Menu.Security.Model;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using System;
using System.Diagnostics;

namespace Aurum.Menu.Model.NodeGenerators
{
    public class MenusNodesGenerator : ModelNodesGeneratorBase
    {
        protected override void GenerateNodesCore(ModelNode node)
        {
            if (!Debugger.IsAttached || !AurumMenuModule.GenerateDefaultMenus)
            {
                return;
            }
            IModelMenus modelMenus = (IModelMenus)node;
            IModelGroups groups = node.Root.Application.Aurum().AurumLists().Groups;
            foreach (IModelNavigationItem current in ((IModelApplicationNavigationItems)node.Root.Application).NavigationItems.Items)
            {
                if (modelMenus.GetNode(current.Id) == null)
                {
                    IModelMenu modelMenu = modelMenus.AddNode<IModelMenu>(current.Id);
                    modelMenu.Caption = current.Caption;
                    modelMenu.Group = groups["Admin"];
                    foreach (IModelNavigationItem current2 in current.Items)
                    {
                        if (current2.Visible)
                        {
                            IModelMenuViewItem modelMenuViewItem = modelMenu.AddNode<IModelMenuViewItem>(current2.Id);
                            modelMenuViewItem.View = current2.View;
                            modelMenuViewItem.ImageName = current2.ImageName;
                            modelMenuViewItem.ObjectKey = current2.ObjectKey;
                        }
                    }
                }
            }
        }
    }
}
