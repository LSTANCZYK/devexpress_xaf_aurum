using Aurum.Menu.Model;
using DevExpress.ExpressApp.Actions;
using System;

namespace Aurum.Menu
{
    public class MenuMenuItem : ChoiceActionItem, IDisposable
    {
        public new IModelMenu Model
        {
            get;
            private set;
        }
        public SingleChoiceAction MenuItemsAction
        {
            get;
            private set;
        }
        public MenuMenuItem(IModelMenu info, SingleChoiceAction menuItemsAction)
        {
            this.Model = info;
            base.Id = info.Caption;
            base.Caption = info.MenuCaption;
            base.ImageName = info.ImageName;
            this.MenuItemsAction = menuItemsAction;
        }

        public void Dispose()
        {
        }
    }
}
