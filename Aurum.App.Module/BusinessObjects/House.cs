using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.App.Module.BusinessObjects
{
    public class House : XPObject
    {
        public House() : base() { }
        public House(Session session) : base(session) { }

        public string Street
        {
            get { return GetPropertyValue<string>("Street"); }
            set { SetPropertyValue<string>("Street", value); }
        }

        public string Num
        {
            get { return GetPropertyValue<string>("Num"); }
            set { SetPropertyValue<string>("Num", value); }
        }

        [Association("House-Flats")]
        public XPCollection<Flat> Flats
        {
            get { return GetCollection<Flat>("Flats"); }
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", Street, Num);
        }
    }
}
