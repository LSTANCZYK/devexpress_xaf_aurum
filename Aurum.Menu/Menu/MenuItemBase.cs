using Aurum.Menu.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model.Core;
using System;

namespace Aurum.Menu
{
    public class MenuItemBase : ChoiceActionItem, IDisposable
    {
        public new IModelMenuItem Model
        {
            get;
            private set;
        }

        public SingleChoiceAction MenuItemsAction
        {
            get;
            private set;
        }

        protected MenuItemBase(IModelMenuItem info, SingleChoiceAction MenuItemsAction, string prefixId = "")
        {
            this.Model = info;
            ModelNode modelNode = info as ModelNode;
            if (modelNode != null)
            {
                base.Id = prefixId + "/" + modelNode.Id;
            }
            else
            {
                base.Id = prefixId;
            }
            this.MenuItemsAction = MenuItemsAction;
            if (info != null)
            {
                base.BeginGroup = info.BeginGroup;
            }
        }
        ~MenuItemBase()
        {
            this.Dispose();
        }
        public virtual void Dispose()
        {
        }
    }
}
