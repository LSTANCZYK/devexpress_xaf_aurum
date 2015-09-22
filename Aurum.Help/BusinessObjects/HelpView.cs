using System;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;

namespace Aurum.Help
{
    /// <summary>
    /// ������� �������������
    /// </summary>
    [NavigationItem("Help")]
    [DefaultClassOptions]
    public class HelpView : XPObject
    {
        private string viewId;
        private string text;

        public HelpView() : base() { }
        public HelpView(Session session) : base(session) { }

        /// <summary>
        /// ������������� �������������
        /// </summary>
        public string ViewId
        {
            get { return viewId; }
            set { SetPropertyValue("ViewId", ref viewId, value); }
        }

        /// <summary>
        /// ����� (html)
        /// </summary>
        [Size(SizeAttribute.Unlimited)]
        public string Text
        {
            get { return text; }
            set { SetPropertyValue("Text", ref text, value); }
        }
    }

}