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
using DevExpress.ExpressApp.Model;
using Aurum.Interface.Model;

namespace Aurum.Interface.Controllers
{
    /// <summary>
    /// Контроллер nested ListView
    /// </summary>
    public partial class NestedListViewController : ViewController
    {
        LinkUnlinkController linkUnlinkController;
        
        public NestedListViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetViewType = ViewType.ListView;
            TargetViewNesting = Nesting.Nested;
        }
        protected override void OnActivated()
        {
            base.OnActivated();

            // разрешить Убирание для внутренних коллекций
            linkUnlinkController = Frame.GetController<LinkUnlinkController>();
            linkUnlinkController.Activated += new EventHandler(linkUnlinkController_Activated);
            View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
            RemoveViewAllowDeleteItem();
            
            // Действия: Создание, Удаление, Добавление, Убирание
            IModelListView modelListView = (IModelListView)View.Model;
            IModelNestedListViewActions modelListViewActions = modelListView as IModelNestedListViewActions;
            if (modelListViewActions != null)
            {
                Frame.GetController<DevExpress.ExpressApp.SystemModule.NewObjectViewController>().NewObjectAction
                    .Active.SetItemValue("PS_NestedListViewController", modelListViewActions.AllowCreate);
                Frame.GetController<DevExpress.ExpressApp.SystemModule.DeleteObjectsViewController>().DeleteAction
                    .Active.SetItemValue("PS_NestedListViewController", modelListViewActions.AllowDelete);
                Frame.GetController<DevExpress.ExpressApp.SystemModule.LinkUnlinkController>().LinkAction
                    .Active.SetItemValue("PS_NestedListViewController", modelListViewActions.AllowLink);
                Frame.GetController<DevExpress.ExpressApp.SystemModule.LinkUnlinkController>().UnlinkAction
                    .Active.SetItemValue("PS_NestedListViewController", modelListViewActions.AllowUnlink);
            }

            // Подтверждение Удаления, Убирания - убрать
            Frame.GetController<DevExpress.ExpressApp.SystemModule.DeleteObjectsViewController>().DeleteAction.ConfirmationMessage = null;
            Frame.GetController<DevExpress.ExpressApp.SystemModule.LinkUnlinkController>().UnlinkAction.ConfirmationMessage = null;
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            linkUnlinkController.Activated -= new EventHandler(linkUnlinkController_Activated);
            View.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void linkUnlinkController_Activated(Object sender, EventArgs e)
        {
            RemoveViewAllowDeleteItem();
        }

        private void View_CurrentObjectChanged(Object sender, EventArgs e)
        {
            RemoveViewAllowDeleteItem();
        }

        private void RemoveViewAllowDeleteItem()
        {
            linkUnlinkController.UnlinkAction.Active.RemoveItem("ViewAllowDelete");
        }
    }
}
