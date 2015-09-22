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
    /// Editor for unsigned long
    /// </summary>
    [PropertyEditor(typeof(ulong), "ULongPropertyEditor", true)]
    public class ULongPropertyEditor : IntegerPropertyEditor
    {
        protected override void SetupRepositoryItem(RepositoryItem item)
        {
            base.SetupRepositoryItem(item);
            RepositoryItemIntegerEdit intItem = (RepositoryItemIntegerEdit)item;
            if ((intItem.MaxValue == 0) || (intItem.MaxValue > ulong.MaxValue))
            {
                intItem.MaxValue = ulong.MaxValue;
            }
            if ((intItem.MinValue == 0) || (intItem.MinValue < ulong.MinValue))
            {
                intItem.MinValue = ulong.MinValue;
            }
        }

        public ULongPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
    }
}
