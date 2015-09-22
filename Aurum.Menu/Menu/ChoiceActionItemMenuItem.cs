using DevExpress.ExpressApp.Actions;
using System;

namespace Aurum.Menu
{
    public class ChoiceActionItemMenuItem : MenuItem
    {
        public SingleChoiceAction Owner
        {
            get
            {
                ChoiceActionItem parentItem = base.ParentItem;
                while (parentItem != null && !(parentItem is ActionMenuItem))
                {
                    parentItem = parentItem.ParentItem;
                }
                if (parentItem == null)
                {
                    return null;
                }
                return ((ActionMenuItem)parentItem).Action as SingleChoiceAction;
            }
        }
        public ChoiceActionItem ChoiceActionItem
        {
            get;
            set;
        }
        public ChoiceActionItemMenuItem(ChoiceActionItem choiceActionItem, SingleChoiceAction menuItemsAction, string prefixId = "")
            : base(null, menuItemsAction, prefixId)
        {
            base.Caption = base.RemoveAmpersand(choiceActionItem.Caption);
            base.ImageName = choiceActionItem.ImageName;
            base.BeginGroup = choiceActionItem.BeginGroup;
            this.ChoiceActionItem = choiceActionItem;
            base.MenuItemsAction.Disposing += new EventHandler(this.MenuItemsAction_Disposing);
            this.ChoiceActionItem.Active.Changed += new EventHandler<EventArgs>(this.Active_Changed);
            this.ChoiceActionItem.Enabled.Changed += new EventHandler<EventArgs>(this.Enabled_Changed);
        }
        private void MenuItemsAction_Disposing(object sender, EventArgs e)
        {
            this.RemoveHandlers();
        }
        private void RemoveHandlers()
        {
            base.MenuItemsAction.Disposing -= new EventHandler(this.MenuItemsAction_Disposing);
            if (this.ChoiceActionItem != null)
            {
                this.ChoiceActionItem.Active.Changed -= new EventHandler<EventArgs>(this.Active_Changed);
                this.ChoiceActionItem.Enabled.Changed -= new EventHandler<EventArgs>(this.Enabled_Changed);
            }
        }
        private void Enabled_Changed(object sender, EventArgs e)
        {
            base.Enabled.SetItemValue("ChoiceActionItem is enabled", this.ChoiceActionItem.Enabled);
        }
        private void Active_Changed(object sender, EventArgs e)
        {
            base.Active.SetItemValue("ChoiceActionItem is active", this.ChoiceActionItem.Active);
        }
        public override void Execute(SingleChoiceActionExecuteEventArgs args)
        {
            if (this.Owner == null)
            {
                throw new InvalidOperationException("SingleChoiceAction not found");
            }
            this.Owner.DoExecute(this.ChoiceActionItem);
        }
        public override void Dispose()
        {
            base.Dispose();
            this.RemoveHandlers();
        }
    }
}
