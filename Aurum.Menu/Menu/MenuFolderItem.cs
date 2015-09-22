using Aurum.Menu.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Menu
{
    public class MenuFolderItem : MenuItemBase
    {
        public MenuFolderItem(IModelMenuFolder info, string prefixId = "")
            : base(info, null, prefixId)
        {
            base.Caption = info.Caption;
            base.ImageName = info.ImageName;
        }

        public MenuFolderItem(IModelMenuTemplateLink info, string prefixId = "")
            : base(info, null, prefixId)
        {
            base.BeginGroup = info.BeginGroup;
            base.Caption = info.Caption;
            if (info.Template != null)
            {
                base.ImageName = info.Template.ImageName;
            }
        }
    }
}
