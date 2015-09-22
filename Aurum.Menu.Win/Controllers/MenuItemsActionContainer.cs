using Aurum.Menu.Controllers;
using Aurum.Menu.Model;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.XtraEditors;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;

namespace Aurum.Menu.Win
{
    public class MenuItemsActionContainer : XtraUserControl, IActionContainer, ISupportUpdate, IDisposable
    {
        private string containerId = "Menus";
        private SingleChoiceAction action;
        private MenuController menuController;
        private object control;
#pragma warning disable 649
        private IContainer components;
#pragma warning restore 649
        string IActionContainer.ContainerId
        {
            get
            {
                return this.containerId;
            }
            set
            {
                this.containerId = value;
            }
        }
        ReadOnlyCollection<ActionBase> IActionContainer.Actions
        {
            get
            {
                return new ReadOnlyCollection<ActionBase>(new ActionBase[]
				{
					this.action
				});
            }
        }
        public MenuItemsActionContainer()
        {
            this.InitializeComponent();
        }
        void IActionContainer.Register(ActionBase action)
        {
            SingleChoiceAction singleChoiceAction = action as SingleChoiceAction;
            if (singleChoiceAction != null && singleChoiceAction.Controller is MenuController)
            {
                this.action = singleChoiceAction;
                this.menuController = (singleChoiceAction.Controller as MenuController);
                this.menuController.ItemsInitialized += new EventHandler<EventArgs>(this.menuController_ItemsInitialized);
            }
        }
        public void BeginUpdate()
        {
        }
        public void EndUpdate()
        {
        }
        private void menuController_ItemsInitialized(object sender, EventArgs e)
        {
            IModelMenuEditor modelMenuEditor = null;
            if (this.menuController != null && this.menuController.ModelMenuEditor != null)
            {
                modelMenuEditor = this.menuController.ModelMenuEditor;
            }
            IModelMenu startupMenuItem = null;
            if (modelMenuEditor != null)
            {
                startupMenuItem = modelMenuEditor.StartupMenuItem;
            }
            if (this.control != null)
            {
                if (this.control is TreeListMenusControl)
                {
                    (this.control as TreeListMenusControl).InitializeItems(this.action, startupMenuItem);
                }
            }
            else
            {
                if (modelMenuEditor != null)
                {
                    TreeListMenusControl treeListMenusControl = new TreeListMenusControl
                    {
                        Dock = DockStyle.Fill,
                        Name = "treeListItemsActionContainer"
                    };
                    treeListMenusControl.InitializeItems(this.action, startupMenuItem);
                    this.control = treeListMenusControl;
                    base.Controls.Add(treeListMenusControl);
                    return;
                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (this.menuController != null)
            {
                this.menuController.ItemsInitialized -= new EventHandler<EventArgs>(this.menuController_ItemsInitialized);
                this.menuController = null;
            }
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            base.SuspendLayout();
            base.Name = "MenuItemsActionContainer";
            base.ResumeLayout(false);
        }
    }
}
