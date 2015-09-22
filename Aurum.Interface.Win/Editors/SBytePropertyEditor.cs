using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Repository;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Editors;

namespace Aurum.Interface.Win.Editors
{
    /// <summary>
    /// Editor for signed byte
    /// </summary>
    [PropertyEditor(typeof(sbyte), "SBytePropertyEditor", true)]
    public class SBytePropertyEditor : IntegerPropertyEditor
    {
        protected override void SetupRepositoryItem(RepositoryItem item)
        {
            base.SetupRepositoryItem(item);
            RepositoryItemIntegerEdit intItem = (RepositoryItemIntegerEdit)item;
            if ((intItem.MaxValue == 0) || (intItem.MaxValue > sbyte.MaxValue))
            {
                intItem.MaxValue = sbyte.MaxValue;
            }
            if ((intItem.MinValue == 0) || (intItem.MinValue < sbyte.MinValue))
            {
                intItem.MinValue = sbyte.MinValue;
            }
        }

        public SBytePropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
    }
}
