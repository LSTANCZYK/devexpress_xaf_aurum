using System;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Model.NodeGenerators;
using Aurum.Menu.Model;
using Aurum.Menu.Utils;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Aurum.Menu.Base.Model;
using Aurum.Menu.Security.Model;

namespace Aurum.Menu.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppWindowControllertopic.
    public partial class MenuController : WindowController, IModelExtender
    {
        private ShowNavigationItemController showNavigationItemController;

        public event EventHandler<EventArgs> ItemsInitialized;
        public event EventHandler<CreateCustomMenuItemEventArgs> CreateCustomMenuItem;
        public event EventHandler<MenuItemCreatedEventArgs> MenuItemCreated;

        public static Func<string, bool> HasGroup;

        protected ShowNavigationItemController ShowNavigationItemController
        {
            get
            {
                if (this.showNavigationItemController == null)
                {
                    this.showNavigationItemController = base.Frame.GetController<ShowNavigationItemController>();
                }
                return this.showNavigationItemController;
            }
        }
        
        public bool IsRefreshingItems
        {
            get;
            private set;
        }
        
        public IModelMenuEditor ModelMenuEditor
        {
            get
            {
                return base.Application.Model.Aurum().Menus().MenuEditor;
            }
        }

        public SingleChoiceAction MenuAction
        {
            get
            {
                return this.MenuItemsAction;
            }
        }

        public MenuController()
        {
            InitializeComponent();
            RegisterActions(components);
            // Target required Windows (via the TargetXXX properties) and create their Actions.
            base.Active["Template is ISupportMenus"] = false;
        }

        protected override void OnWindowChanging(Window window)
        {
            base.OnWindowChanging(window);
            if (!window.IsMain)
            {
                return;
            }
            base.Frame.Disposing += new EventHandler(this.Frame_Disposing);
            base.Frame.TemplateChanged += new EventHandler(this.Frame_TemplateChanged);
        }

        protected override void OnDeactivated()
        {
            this.ShowNavigationItemController.Active.RemoveItem("Using MenuController");
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
        
        private void Frame_TemplateChanged(object sender, EventArgs e)
        {
            if (base.Frame.Template == null)
            {
                return;
            }
            bool flag = base.Frame.Template is ISupportMenus;
            this.ShowNavigationItemController.Active["Using MenuController"] = !flag;
            base.Active["Template is ISupportMenus"] = flag;
            if (flag)
            {
                ISupportMenus supportMenus = (ISupportMenus)base.Frame.Template;
                IModelMenuEditor menuEditor = base.Application.Model.Aurum().Menus().MenuEditor;
                if (!string.IsNullOrEmpty(menuEditor.Menus.Caption))
                {
                    supportMenus.SetMenusCaption(menuEditor.Menus.Caption);
                }
            }
        }

        private void Frame_Disposing(object sender, EventArgs e)
        {
            this.ShowNavigationItemController.Active["Frame is disposed"] = false;
            base.Active["Frame is disposed"] = false;
        }

        private static bool CheckMenuGroup(IModelMenu menuNode)
        {
            if (menuNode.Group == null)
            {
                return true;
            }

            if (HasGroup != null)
            {
                return HasGroup(menuNode.Group.Id);
            }

            return false;
        }

        public virtual void RefreshItems()
        {
            if (this.IsRefreshingItems)
            {
                return;
            }
            this.IsRefreshingItems = true;
            try
            {
                this.MenuItemsAction.BeginUpdate();
                try
                {
                    this.ClearItems(this.MenuItemsAction.Items);
                    IModelMenuEditor menuEditor = base.Application.Model.Aurum().Menus().MenuEditor;
                    foreach (IModelMenu current in menuEditor.Menus)
                    {
                        if (MenuController.CheckMenuGroup(current))
                        {
                            MenuMenuItem menuMenuItem = new MenuMenuItem(current, null);
                            this.FillMenu(current, menuMenuItem);
                            this.MenuItemsAction.Items.Add(menuMenuItem);
                        }
                    }
                }
                finally
                {
                    this.MenuItemsAction.EndUpdate();
                }
                this.OnItemsInitialized();
            }
            finally
            {
                this.IsRefreshingItems = false;
            }
        }

        protected virtual void OnItemsInitialized()
        {
            if (this.ItemsInitialized != null)
            {
                this.ItemsInitialized(this, EventArgs.Empty);
            }
        }

        private void ClearItems(ChoiceActionItemCollection items)
        {
            if (items == null)
            {
                return;
            }
            ChoiceActionItem[] array = items.ToArray<ChoiceActionItem>();
            for (int i = 0; i < array.Length; i++)
            {
                ChoiceActionItem choiceActionItem = array[i];
                this.ClearItems(choiceActionItem.Items);
                if (choiceActionItem is IDisposable)
                {
                    (choiceActionItem as IDisposable).Dispose();
                }
            }
            items.Clear();
        }

        private void MenuItemsController_Activated(object sender, EventArgs e)
        {
            this.MenuItemsAction.ItemsChanged -= new EventHandler<ItemsChangedEventArgs>(this.MenuItemsAction_ItemsChanged);
            this.MenuItemsAction.ItemsChanged += new EventHandler<ItemsChangedEventArgs>(this.MenuItemsAction_ItemsChanged);
            this.RefreshItems();
        }

        private void MenuItemsAction_ItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            if (((SingleChoiceAction)sender).SelectedItem == null)
            {
                return;
            }
            if (this.IsRefreshingItems)
            {
                return;
            }
            if (e.ChangedItemsInfo.Values.Any((ChoiceActionItemChangesType a) => a == ChoiceActionItemChangesType.Enabled || a == ChoiceActionItemChangesType.Active || a == ChoiceActionItemChangesType.Items))
            {
                this.RefreshItems();
            }
        }

        private void FillMenu(IEnumerable<IModelMenuItem> node, ChoiceActionItem root)
        {
            string text = (root != null) ? root.Id : ((ModelNode)node).Id;
            List<ChoiceActionItem> list = new List<ChoiceActionItem>();
            foreach (IModelMenuItem current in node)
            {
                MenuItemBase menuItemBase = this.OnCreateCustomMenuItem(current, this.MenuItemsAction, text);
                if (menuItemBase == null)
                {
                    if (current is IModelMenuActionItem)
                    {
                        menuItemBase = new ActionMenuItem(current as IModelMenuActionItem, this.MenuItemsAction, text);
                    }
                    else if (current is IModelMenuViewItem)
                    {
                        menuItemBase = new ViewMenuItem(current as IModelMenuViewItem, this.MenuItemsAction, text);
                    }
                    else if (current is IModelMenuFolder)
                    {
                        IModelMenuFolder modelMenuFolder = current as IModelMenuFolder;
                        menuItemBase = new MenuFolderItem(modelMenuFolder, text);
                        this.FillMenu(modelMenuFolder, menuItemBase);
                    }
                    else if (current is IModelMenuTemplateLink)
                    {
                        IModelMenuTemplateLink modelMenuTemplateLink = current as IModelMenuTemplateLink;
                        menuItemBase = new MenuFolderItem(modelMenuTemplateLink, text);
                        this.FillMenu(modelMenuTemplateLink.Template, menuItemBase);
                    }
                }
                if (menuItemBase != null)
                {
                    this.OnMenuItemCreated(menuItemBase);
                    list.Add(menuItemBase);
                }
            }
            root.Items.AddRange(list);
        }

        private void OnMenuItemCreated(MenuItemBase menuItem)
        {
            if (this.MenuItemCreated == null)
            {
                return;
            }
            this.MenuItemCreated(this, new MenuItemCreatedEventArgs(menuItem));
        }

        private MenuItemBase OnCreateCustomMenuItem(IModelMenuItem modelMenuItem, SingleChoiceAction menuItemsAction, string nodeId)
        {
            if (this.CreateCustomMenuItem != null)
            {
                CreateCustomMenuItemEventArgs createCustomMenuItemEventArgs = new CreateCustomMenuItemEventArgs(modelMenuItem, menuItemsAction, nodeId);
                this.CreateCustomMenuItem(this, createCustomMenuItemEventArgs);
                return createCustomMenuItemEventArgs.MenuItemInstance;
            }
            return null;
        }

        private void menuItemAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            MenuItem menuItem = e.SelectedChoiceActionItem as MenuItem;
            if (menuItem != null)
            {
                menuItem.Execute(e);
            }
        }

        private void MenuController_Deactivating(object sender, EventArgs e)
        {
            this.MenuItemsAction.ItemsChanged -= new EventHandler<ItemsChangedEventArgs>(this.MenuItemsAction_ItemsChanged);
        }

        public virtual MenuItem GetStartupMenuItem()
        {
            if (!base.Active)
            {
                return null;
            }
            if (base.Application != null && base.Application.Model != null)
            {
                IModelMenu startupMenuItem = base.Application.Model.Aurum().Menus().MenuEditor.StartupMenuItem;
                if (startupMenuItem != null)
                {
                    IModelMenuItem item = startupMenuItem.StartupItem;
                    if (item != null)
                    {
                        ChoiceActionItem choiceActionItem = this.MenuItemsAction.Items.RecursiveFind((ChoiceActionItem a) => a.Items, delegate(ChoiceActionItem a)
                        {
                            MenuItem menuItem = a as MenuItem;
                            return menuItem != null && menuItem.Model == item;
                        });
                        if (choiceActionItem != null && choiceActionItem.Enabled && choiceActionItem.Active)
                        {
                            return choiceActionItem as MenuItem;
                        }
                        choiceActionItem = this.MenuItemsAction.Items.RecursiveFind((ChoiceActionItem a) => a.Items, (ChoiceActionItem a) => a.Active && a.Enabled && a is ViewMenuItem);
                        if (choiceActionItem != null)
                        {
                            return choiceActionItem as MenuItem;
                        }
                    }
                }
            }
            return null;
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelAurum, IModelAurumMenus>();
        }
    }

    public class CreateCustomMenuItemEventArgs : EventArgs
    {
        public IModelMenuItem Model
        {
            get;
            private set;
        }

        public SingleChoiceAction MenuItemsAction
        {
            get;
            private set;
        }

        public string NodeId
        {
            get;
            private set;
        }

        public MenuItemBase MenuItemInstance
        {
            get;
            set;
        }

        public CreateCustomMenuItemEventArgs(IModelMenuItem modelMenuItem, SingleChoiceAction menuItemsAction, string nodeId)
        {
            this.Model = modelMenuItem;
            this.MenuItemsAction = menuItemsAction;
            this.NodeId = nodeId;
        }
    }

    public class MenuItemCreatedEventArgs : EventArgs
    {
        public MenuItemBase MenuItem
        {
            get;
            set;
        }

        public MenuItemCreatedEventArgs(MenuItemBase menuItem)
        {
            this.MenuItem = menuItem;
        }
    }
}
