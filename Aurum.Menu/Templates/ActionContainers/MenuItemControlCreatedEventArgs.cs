using DevExpress.ExpressApp.Actions;
using System;

namespace Aurum.Menu.Templates.ActionContainers
{
    public class MenuItemControlCreatedEventArgs : EventArgs
    {
        public ChoiceActionItem MenuItem { get; private set; }

        public object Control { get; private set; }

        public MenuItemControlCreatedEventArgs(ChoiceActionItem menuItem, object control)
        {
            this.MenuItem = menuItem;
            this.Control = control;
        }
    }
}
