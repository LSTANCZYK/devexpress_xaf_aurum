using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.App.Module.BusinessObjects
{
    [DefaultProperty("Text")]
    public class Lodger : XPObject
    {
        public Lodger() : base() { }
        public Lodger(Session session) : base(session) { }

        [Association("Flat-Lodgers")]
        public Flat Flat
        {
            get { return GetPropertyValue<Flat>("Flat"); }
            set { SetPropertyValue<Flat>("Flat", value); }
        }

        public string FIO
        {
            get { return GetPropertyValue<string>("FIO"); }
            set { SetPropertyValue<string>("FIO", value); }
        }
        /// <summary>
        /// Текстовое представление
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [PersistentAlias("FIO")]
        public string Text
        {
            get { return (string)EvaluateAlias("Text"); }
        }

    }
}
