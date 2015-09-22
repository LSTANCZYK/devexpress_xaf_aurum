using System;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Aurum.Security.DatabaseUpdate
{
    /// <summary>
    /// ���������� ���� ������
    /// </summary>
    public class Updater : ModuleUpdater
    {
        /// <inheritdoc/>
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion)
        {
        }
        
        /// <inheritdoc/>
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();

            // �������� �������
            Owner systemOperator = ObjectSpace.GetObjectByKey<Owner>(1);
            if (systemOperator == null)
            {
                systemOperator = ObjectSpace.CreateObject<Owner>();
                systemOperator.Id = 1;
                systemOperator.Name = "�������� �������";
                systemOperator.Rights = OwnerRights.Public;
            }
        }

        /// <inheritdoc/>
        public override void UpdateDatabaseBeforeUpdateSchema()
        {
            base.UpdateDatabaseBeforeUpdateSchema();
        }
    }
}
