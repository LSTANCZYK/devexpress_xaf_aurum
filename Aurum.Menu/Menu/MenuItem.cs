using Aurum.Menu.Model;
using DevExpress.ExpressApp.Actions;
using System;

namespace Aurum.Menu
{
    public abstract class MenuItem : MenuItemBase
    {
        public new string ToolTip
        {
            get;
            set;
        }

        protected MenuItem(IModelMenuItem info, SingleChoiceAction menuItemsAction, string prefixId = "")
            : base(info, menuItemsAction, prefixId)
        {
        }

        protected string RemoveAmpersand(string sourceString)
        {
            if (sourceString == null)
            {
                return null;
            }
            string text = sourceString;
            for (int i = text.IndexOf('&'); i >= 0; i = text.IndexOf('&'))
            {
                text = text.Remove(i, 1);
            }
            return text;
        }

        public virtual void Execute(SingleChoiceActionExecuteEventArgs args)
        {
        }
    }
}
