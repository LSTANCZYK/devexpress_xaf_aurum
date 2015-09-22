using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Security;
using Aurum.App.Module.BusinessObjects;
//using DevExpress.ExpressApp.Reports;
//using DevExpress.ExpressApp.PivotChart;
//using DevExpress.ExpressApp.Security.Strategy;
//using Aurum.App.Module.BusinessObjects;

namespace Aurum.App.Module.DatabaseUpdate
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppUpdatingModuleUpdatertopic
    public class Updater : ModuleUpdater
    {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion)
        {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            Master master = ObjectSpace.FindObject<Master>(new BinaryOperator("MasterName", "TestMaster"));
            if (master == null)
            {
                master = ObjectSpace.CreateObject<Master>();
                master.MasterName = "TestMaster";
                CreateDetail("1");
                CreateDetail("2");
                master.Details.Add(CreateDetail("3"));
                CreateDetail("4");
                CreateDetail(null);
                master.Save();
            }
        }
        private Detail CreateDetail(string name)
        {
            Detail detail = ObjectSpace.CreateObject<Detail>();
            detail.DetailName = name;
            detail.Save();
            return detail;
        }
        public override void UpdateDatabaseBeforeUpdateSchema()
        {
            base.UpdateDatabaseBeforeUpdateSchema();
            //if(CurrentDBVersion < new Version("1.1.0.0") && CurrentDBVersion > new Version("0.0.0.0")) {
            //    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName");
            //}
        }
    }
}
