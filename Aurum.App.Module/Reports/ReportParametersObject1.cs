using System;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.ReportsV2;

namespace Aurum.App.Module.Reports
{
    [DomainComponent]
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/CustomDocument2778.
    public class ReportParametersObject1 : ReportParametersObjectBase
    {
        [XafDisplayName("Фамилия")]
        public string Lastname { get; set; }

        public ReportParametersObject1(IObjectSpaceCreator osc) : base(osc) { }

        protected override DevExpress.ExpressApp.IObjectSpace CreateObjectSpace()
        {
            return objectSpaceCreator.CreateObjectSpace(typeof(Person));
        }

        public override CriteriaOperator GetCriteria()
        {
            if (string.IsNullOrEmpty(Lastname))
                throw new Exception("Введите фамилию");
            CriteriaOperator criteria = CriteriaOperator.Parse("Upper([Lastname]) like ?", "%" + Lastname.ToUpper() + "%");
            return criteria;
        }

        public override SortProperty[] GetSorting()
        {
            SortProperty[] sorting = { };
            return sorting;
        }
    }
}
