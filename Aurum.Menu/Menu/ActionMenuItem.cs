using Aurum.Menu.Model;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using System;
using System.Linq;

namespace Aurum.Menu
{
    public class ActionMenuItem : MenuItem
    {
        private ActionBase action;
        public new IModelMenuActionItem Model
        {
            get
            {
                return (IModelMenuActionItem)base.Model;
            }
        }
        public ActionBase Action
        {
            get
            {
                try
                {
                    if (this.action == null)
                    {
                        IModelAction modelAction = this.Model.Action;
                        if (modelAction != null)
                        {
                            IModelController controller = modelAction.Controller;
                            Type controllerType = ReflectionHelper.FindType(controller.Name);
                            Controller controller2 = this.FindController(base.MenuItemsAction.Application.MainWindow, controllerType) ?? this.FindController(base.MenuItemsAction.Controller.Frame, controllerType);
                            if (controller2 != null)
                            {
                                this.action = controller2.Actions[this.Model.Action.Id];
                            }
                        }
                        else
                        {
                            Tracing.Tracer.LogError("Ошибка пункта Меню '{0}': не найден Action '{1}'", new object[]
							{
								this.Model.Caption,
								this.Model.Action.Id
							});
                        }
                    }
                }
                catch (Exception exception)
                {
                    Tracing.Tracer.LogError(exception);
                }
                return this.action;
            }
        }
        public bool HasRights
        {
            get
            {
                if (this.action == null)
                {
                    Tracing.Tracer.LogVerboseText("Для меню {0} не найден соответствующий Action", new object[]
					{
						base.Id
					});
                    return false;
                }
                return true;
                /*
                if (SecuritySystem.IsGranted(new ActionOperationPermissionRequest(this.Action.Id)))
                {
                    Tracing.Tracer.LogVerboseText("Доступ к действию {0} разрешен правом доступа AccessActoinPermission", new object[]
					{
						this.Action.Id
					});
                    return true;
                }
                else
                {
                    Tracing.Tracer.LogVerboseText("Доступ к действию {0} запрещен правом доступа AccessActoinPermission", new object[]
				    {
					    this.Action.Id
				    });
                    return false;
                }
                */
            }
        }
        public ActionMenuItem(IModelMenuActionItem info, SingleChoiceAction menuItemsAction, string prefixId = "")
            : base(info, menuItemsAction, prefixId)
        {
            base.Enabled.SetItemValue("Action not null", this.Action != null);
            base.MenuItemsAction.Disposing += new EventHandler(this.MenuItemsAction_Disposing);
            base.Caption = base.RemoveAmpersand(info.Caption);
            base.ImageName = info.ImageName;
            base.ToolTip = info.ToolTip;
            if (this.Action != null)
            {
                base.Enabled.SetItemValue("Action is enabled", this.Action.Enabled);
                if (this.Action.Controller is ViewController)
                {
                    base.Enabled.SetItemValue("Action is active", true);
                }
                else
                {
                    base.Enabled.SetItemValue("Action is active", this.Action.Active);
                }
                base.Enabled.SetItemValue("HasRights", this.HasRights);
                this.Action.Changed += new EventHandler<ActionChangedEventArgs>(this.Action_Changed);
                SingleChoiceAction singleChoiceAction = this.Action as SingleChoiceAction;
                if (singleChoiceAction != null)
                {
                    this.CreateItemsSingleChoiceAction(singleChoiceAction.Items, base.Items, base.Id);
                    singleChoiceAction.ItemsChanged += new EventHandler<ItemsChangedEventArgs>(this.singleChoiceAction_ItemsChanged);
                }
            }
        }
        private void singleChoiceAction_ItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            if ((
                from a in e.ChangedItemsInfo.Values
                where a == ChoiceActionItemChangesType.Items
                select a).Count<ChoiceActionItemChangesType>() > 0)
            {
                this.ItemChanged(this, ChoiceActionItemChangesType.Items);
            }
        }
        private void Action_Changed(object sender, ActionChangedEventArgs e)
        {
            ActionBase actionBase = (ActionBase)sender;
            switch (e.ChangedPropertyType)
            {
                case ActionChangedType.Enabled:
                    base.Enabled.SetItemValue("Action is enabled", actionBase.Enabled);
                    return;
                case ActionChangedType.Active:
                    base.Enabled.SetItemValue("Action is active", actionBase.Active);
                    return;
                default:
                    return;
            }
        }
        private void CreateItemsSingleChoiceAction(ChoiceActionItemCollection items, ChoiceActionItemCollection menuItems, string nodeId)
        {
            foreach (ChoiceActionItem current in
                from a in items
                where a.Active && a.Enabled
                select a)
            {
                string text = nodeId + "/" + current.Id;
                ChoiceActionItemMenuItem choiceActionItemMenuItem = new ChoiceActionItemMenuItem(current, base.MenuItemsAction, text);
                menuItems.Add(choiceActionItemMenuItem);
                SingleChoiceAction singleChoiceAction = this.Action as SingleChoiceAction;
                if (singleChoiceAction != null && singleChoiceAction.IsHierarchical())
                {
                    this.CreateItemsSingleChoiceAction(current.Items, choiceActionItemMenuItem.Items, text);
                }
            }
        }
        private void MenuItemsAction_Disposing(object sender, EventArgs e)
        {
            this.RemoveHandlers();
        }
        private void RemoveHandlers()
        {
            base.MenuItemsAction.Disposing -= new EventHandler(this.MenuItemsAction_Disposing);
            if (this.action != null)
            {
                this.action.Changed -= new EventHandler<ActionChangedEventArgs>(this.Action_Changed);
                SingleChoiceAction singleChoiceAction = this.action as SingleChoiceAction;
                if (singleChoiceAction != null)
                {
                    singleChoiceAction.ItemsChanged -= new EventHandler<ItemsChangedEventArgs>(this.singleChoiceAction_ItemsChanged);
                }
            }
        }
        public override void Execute(SingleChoiceActionExecuteEventArgs args)
        {
            bool controllerWasActive = this.Action.Active["Controller active"];
            bool handled = false;
            if (!controllerWasActive)
            {
                this.Action.Active["Controller active"] = true;
            }
            try
            {
                SimpleAction simpleAction = this.Action as SimpleAction;
                if (simpleAction != null)
                {
                    simpleAction.DoExecute();
                    return;
                }
                PopupWindowShowAction popupWindowShowAction = this.Action as PopupWindowShowAction;
                if (popupWindowShowAction != null)
                {
                    CustomizePopupWindowParamsEventArgs popupWindowParams = popupWindowShowAction.GetPopupWindowParams();
                    args.ShowViewParameters.Context = popupWindowParams.Context;
                    args.ShowViewParameters.Controllers.Add(popupWindowParams.DialogController);
                    args.ShowViewParameters.CreateAllControllers = true;
                    args.ShowViewParameters.CreatedView = popupWindowParams.View;
                    args.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                    popupWindowParams.View.Closed += (sender, e) =>
                    {
                        if (!controllerWasActive)
                            this.Action.Active["Controller active"] = false;
                    };
                    handled = true;
                }
                SingleChoiceAction singleChoiceAction = this.Action as SingleChoiceAction;
                if (singleChoiceAction != null)
                {
                    singleChoiceAction.DoExecute(singleChoiceAction.SelectedItem);
                }
            }
            finally
            {
                if (!controllerWasActive && !handled)
                    this.Action.Active["Controller active"] = false;
            }
        }

        void View_Closed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        private Controller FindController(Frame frame, Type controllerType)
        {
            if (frame == null)
            {
                return null;
            }
            if (controllerType == null)
            {
                return null;
            }
            if (frame.Controllers.ContainsKey(controllerType))
            {
                return frame.Controllers[controllerType];
            }
            ITypeInfo typeInfo2 = XafTypesInfo.Instance.FindTypeInfo(controllerType);
            XafTypesInfo.Instance.LoadTypes(controllerType.Assembly);
            return (
                from typeInfo in ReflectionHelper.FindTypeDescendants(typeInfo2)
                where frame.Controllers.ContainsKey(typeInfo.Type)
                select frame.Controllers[typeInfo.Type]).FirstOrDefault<Controller>();
        }
        public override void Dispose()
        {
            base.Dispose();
            this.RemoveHandlers();
        }
    }
}
