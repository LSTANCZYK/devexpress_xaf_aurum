using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;

namespace Aurum.Operations.Win.Editors
{
    public class LabelPropertyEditor : WinPropertyEditor
    {
        public LabelPropertyEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info)
        {
            base.ControlBindingProperty = "Text";
        }
        protected override object CreateControlCore()
        {
            return new LabelControl
            {
                Appearance =
                {
                    Options =
                    {
                        UseTextOptions = true
                    },
                    TextOptions =
                    {
                        HAlignment = HorzAlignment.Near,
                        VAlignment = VertAlignment.Top,
                        WordWrap = WordWrap.Wrap
                    }
                },
                AutoSizeMode = LabelAutoSizeMode.None,
                BorderStyle = BorderStyles.NoBorder
            };
        }
    }
}
