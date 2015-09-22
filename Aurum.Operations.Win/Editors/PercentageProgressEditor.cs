using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Repository;
using System;
using System.Drawing;

namespace Aurum.Operations.Win.Editors
{
    [PropertyEditor(typeof(int), false)]
    public class PercentageProgressEditor : WinPropertyEditor, IInplaceEditSupport
    {
        public PercentageProgressEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info)
        {
            base.ControlBindingProperty = "Position";
        }

        protected override object CreateControlCore()
        {
            PercentageProgressBarControl percentageProgressBarControl = new PercentageProgressBarControl
            {
                MaximumSize = new Size(0, 18),
                MinimumSize = new Size(0, 18)
            };
            percentageProgressBarControl.Properties.Minimum = 0;
            percentageProgressBarControl.Properties.Maximum = 100;
            percentageProgressBarControl.Properties.ShowTitle = true;
            percentageProgressBarControl.Properties.PercentView = true;
            return percentageProgressBarControl;
        }

        RepositoryItem IInplaceEditSupport.CreateRepositoryItem()
        {
            return new RepositoryItemPercentageProgress();
        }
    }
}
