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
    [DefaultProperty("Text2")]
    public class Flat : XPObject
    {
        public Flat() : base() { }
        public Flat(Session session) : base(session) { }

        [Association("House-Flats")]
        public House House
        {
            get { return GetPropertyValue<House>("House"); }
            set { SetPropertyValue<House>("House", value); }
        }

        public string Num
        {
            get { return GetPropertyValue<string>("Num"); }
            set { SetPropertyValue<string>("Num", value); }
        }

        public string Text
        {
            get { return House != null ? (House.Street + " д. " + House.Num + ", кв. " + Num) : String.Empty; }
        }

        [Association("Flat-Persons")]
        public XPCollection<Person> Persons
        {
            get { return GetCollection<Person>("Persons"); }
        }
        [Association("Flat-Lodgers")]
        public XPCollection<Lodger> Lodgers
        {
            get { return GetCollection<Lodger>("Lodgers"); }
        }
        /// <summary>
        /// Текстовое представление
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [PersistentAlias("Num")]
        public string Text2
        {
            get { return (string)EvaluateAlias("Text2"); }
        }

    }
}
