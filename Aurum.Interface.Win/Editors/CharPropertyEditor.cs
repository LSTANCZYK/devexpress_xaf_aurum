using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Mask;

namespace Aurum.Interface.Win.Editors
{
    /// <summary>
    /// Editor for char
    /// </summary>
    [PropertyEditor(typeof(char), "CharPropertyEditor", true)]
    public class CharPropertyEditor : DXPropertyEditor
    {
        protected override void SetupRepositoryItem(RepositoryItem item)
        {
            base.SetupRepositoryItem(item);
            if (item is RepositoryItemCharEdit)
            {
                ((RepositoryItemCharEdit)item).Init(EditMask, EditMaskType);
            }
        }

        protected override object CreateControlCore()
        {
            if (Model.ModelMember.Type == typeof(char?))
                return new CharEdit(true);
            else
                return new CharEdit(false);
        }

        protected override RepositoryItem CreateRepositoryItem()
        {
            return new RepositoryItemCharEdit();
        }

        public CharPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model)
        {
        }

        public new TextEdit Control
        {
            get { return (TextEdit)base.Control; }
        }

        public override bool CanFormatPropertyValue
        {
            get { return true; }
        }
    }

    public class RepositoryItemCharEdit : RepositoryItemTextEdit
    {
        internal const string EditorName = "CharEdit";

        internal static void Register()
        {
            if (!EditorRegistrationInfo.Default.Editors.Contains(EditorName))
            {
                EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(CharEdit),
                    typeof(RepositoryItemCharEdit),
                    typeof(TextEditViewInfo), new TextEditPainter(), true, EditImageIndexes.TextEdit,
                    typeof(DevExpress.Accessibility.TextEditAccessible)));
            }
        }

        static RepositoryItemCharEdit()
        {
            RepositoryItemCharEdit.Register();
        }

        public override string EditorTypeName
        {
            get { return EditorName; }
        }

        public RepositoryItemCharEdit()
        {
            Mask.MaskType = MaskType.None;
            if (Mask.MaskType != MaskType.RegEx)
            {
                Mask.UseMaskAsDisplayFormat = true;
            }
            MaxLength = 1;
        }

        public void Init(string editMask, EditMaskType maskType)
        {
            if (!string.IsNullOrEmpty(editMask))
            {
                Mask.EditMask = editMask;
                switch (maskType)
                {
                    case EditMaskType.RegEx:
                        Mask.UseMaskAsDisplayFormat = false;
                        Mask.MaskType = MaskType.RegEx;
                        break;
                    default:
                        Mask.MaskType = MaskType.Simple;
                        break;
                }
            }
        }
    }

    [System.ComponentModel.ToolboxItem(false)]
    public class CharEdit : TextEdit
    {
        static CharEdit()
        {
            RepositoryItemCharEdit.Register();
        }

        private bool nullable;
        public CharEdit(bool nullable)
        {
            this.nullable = nullable;
            ((RepositoryItemCharEdit)Properties).MaxLength = 1;
        }

        public override string EditorTypeName { get { return RepositoryItemCharEdit.EditorName; } }

        public override object EditValue
        {
            get
            {
                var s = Convert.ToString(base.EditValue);
                if (!string.IsNullOrEmpty(s))
                    return s[0];
                else
                {
                    return nullable ? (char?)null : default(char);
                }
            }
            set
            {
                base.EditValue = value;
            }
        }
    }
}
