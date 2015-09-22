using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.Security;

namespace Aurum.Menu
{
    [NonPersistent]
    public class NavigationPermission : PermissionBase
    {
        private string idPath;

        public NavigationPermission()
        {
            this.AccessInfo = new NavigationAccessList();
        }

        public NavigationPermission(string idPathChoiceActionItem) : this()
        {
            this.idPath = idPathChoiceActionItem;
        }

        public static bool AllGranted
        {
            get;
            set;
        }

        [NonPersistent]
        public NavigationAccessList AccessInfo
        {
            get;
            set;
        }

        public override IPermission Copy()
        {
            return new NavigationPermission();
        }

        public override bool IsSubsetOf(IPermission target)
        {
            if (NavigationPermission.AllGranted)
            {
                return true;
            }
            NavigationPermission navigationPermission = target as NavigationPermission;
            return navigationPermission.AccessInfo.Items.Contains(this.idPath);
        }

        public override IPermission Intersect(IPermission target)
        {
            NavigationPermission navigationPermission = target as NavigationPermission;
            if (navigationPermission == null)
            {
                throw new ArgumentException(string.Format("Incorrect permission is passed: '{0}' instead of '{1}'", target.GetType(), base.GetType()));
            }
            NavigationAccessList navigationAccessList = new NavigationAccessList();
            NavigationPermission navigationPermission2 = new NavigationPermission();
            foreach (string current in this.AccessInfo.Items)
            {
                if (navigationPermission.AccessInfo.Items.Contains(current))
                {
                    navigationAccessList.Items.Add(current);
                }
            }
            navigationPermission2.AccessInfo = navigationAccessList;
            return navigationPermission2;
        }

        public override IPermission Union(IPermission target)
        {
            NavigationPermission navigationPermission = new NavigationPermission();
            NavigationPermission navigationPermission2 = target as NavigationPermission;
            if (navigationPermission2 == null)
            {
                throw new ArgumentException(string.Format("Incorrect permission is passed: '{0}' instead of '{1}'", target.GetType(), base.GetType()));
            }
            NavigationAccessList navigationAccessList = new NavigationAccessList();
            foreach (string current in this.AccessInfo.Items)
            {
                if (!navigationAccessList.Items.Contains(current))
                {
                    navigationAccessList.Items.Add(current);
                }
            }
            foreach (string current2 in navigationPermission2.AccessInfo.Items)
            {
                if (!navigationAccessList.Items.Contains(current2))
                {
                    navigationAccessList.Items.Add(current2);
                }
            }
            navigationPermission.AccessInfo = navigationAccessList;
            return navigationPermission;
        }

        public override SecurityElement ToXml()
        {
            SecurityElement securityElement = base.ToXml();
            if (this.AccessInfo != null)
            {
                foreach (string current in this.AccessInfo.Items)
                {
                    SecurityElement securityElement2 = new SecurityElement("Node");
                    securityElement2.AddAttribute("NodeID", current);
                    securityElement.AddChild(securityElement2);
                }
            }
            return securityElement;
        }

        public override void FromXml(SecurityElement e)
        {
            this.AccessInfo = new NavigationAccessList();
            if (e.Children != null)
            {
                foreach (SecurityElement securityElement in e.Children)
                {
                    if (securityElement.Tag == "Node" && securityElement.Attributes.ContainsKey("NodeID"))
                    {
                        this.AccessInfo.Items.Add(securityElement.Attributes["NodeID"].ToString());
                    }
                }
            }
        }
    }
}
