using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Help
{
    /// <summary>
    /// Документ-справка, показываемый пользователю
    /// </summary>
    [NonPersistent]
    public class HelpDocument : XPCustomObject
    {
        public HelpDocument() : base() { }
        public HelpDocument(Session session) : base(session) { }

        /// <summary>
        /// Текст
        /// </summary>
        [Size(SizeAttribute.Unlimited)]
        public string Text
        {
            get { return GetPropertyValue<string>("Text"); }
            set { SetPropertyValue<string>("Text", value); }
        }
    }
}
