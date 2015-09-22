using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors;
using System;
using System.Drawing;

namespace Aurum.Operations.Win.Editors
{
    [PropertyEditor(typeof(OperationStatus), false)]
    public class AurumOperationStatusEditor : PropertyEditor
    {
        public AurumOperationStatusEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info)
        {
        }
        protected override object CreateControlCore()
        {
            MarqueeProgressBarControl marqueeProgressBarControl = new MarqueeProgressBarControl
            {
                MaximumSize = new Size(0, 18),
                MinimumSize = new Size(0, 18)
            };
            marqueeProgressBarControl.Properties.Stopped = true;
            return marqueeProgressBarControl;
        }
        protected override void ReadValueCore()
        {
            (base.Control as MarqueeProgressBarControl).Properties.Stopped = ((OperationStatus)base.PropertyValue != OperationStatus.Running);
        }
        protected override object GetControlValueCore()
        {
            return 0;
        }
    }
}
