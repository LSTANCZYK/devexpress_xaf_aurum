using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.App.Module.BusinessObjects
{
    [DefaultProperty("Lastname")]
    public class Person : XPObject
    {
        private string lastname;
        private string firstname;
        private DateTime birthdate;
        private string comments;
        private decimal? sum;
        private string predefined;
        private int blood;
        private bool recipient;
        private PersonSex? sex;

        public Person() : base() { }
        public Person(Session session) : base(session) { }

        public string Lastname
        {
            get { return lastname; }
            set { SetPropertyValue("Lastname", ref lastname, value); }
        }
        public string Firstname
        {
            get { return firstname; }
            set { SetPropertyValue("Firstname", ref firstname, value); }
        }
        public DateTime Birthdate
        {
            get { return birthdate; }
            set { SetPropertyValue("Birthdate", ref birthdate, value); }
        }

        [Size(SizeAttribute.Unlimited)]
        public string Comments
        {
            get { return comments; }
            set { SetPropertyValue("Comments", ref comments, value); }
        }

        public decimal? Sum
        {
            get { return sum; }
            set { SetPropertyValue("Sum", ref sum, value); }
        }

        [ModelDefault("PredefinedValues", "Predefined Value 1;Predefined Value 2;Predefined Value 3;Predefined Value 4")]
        public string Predefined
        {
            get { return predefined; }
            set { SetPropertyValue("Predefined", ref predefined, value); }
        }

        [Association("Flat-Persons")]
        public Flat Flat
        {
            get { return GetPropertyValue<Flat>("Flat"); }
            set { SetPropertyValue<Flat>("Flat", value); }
        }

        public int Blood
        {
            get { return blood; }
            set { SetPropertyValue("Blood", ref blood, value); }
        }

        public bool Recipient
        {
            get { return recipient; }
            set { SetPropertyValue("Recipient", ref recipient, value); }
        }

        public PersonSex? Sex
        {
            get { return sex; }
            set { SetPropertyValue("Sex", ref sex, value); }
        }
    }

    public enum PersonSex
    {
        Male,
        Female
    }
}
