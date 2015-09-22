using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Interface.Win.Editors
{
    [PropertyEditor(typeof(string), "AurumStringPropertyEditor", true)]
    public class AurumStringPropertyEditor : StringPropertyEditor
    {
        public AurumStringPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
		}

        private bool IsSimpleStringEdit()
        {
            if (MemberInfo == null) return true;
            return MemberInfo.MemberType != typeof(string) || (Model.RowCount <= 1);
        }
        private bool IsComboBoxStringEdit()
        {
            return IsSimpleStringEdit() && !string.IsNullOrEmpty(Model.PredefinedValues);
        }

        protected override object CreateControlCore()
        {
            BaseEdit result;
            if (IsComboBoxStringEdit())
            {
                result = new AurumPredefinedValuesStringEdit(Model.MaxLength, PredefinedValuesEditorHelper.CreatePredefinedValuesFromString(Model.PredefinedValues));
            }
            else if (IsSimpleStringEdit())
            {
                result = new StringEdit(Model.MaxLength);
            }
            else
            {
                result = new LargeStringEdit(Model.RowCount, Model.MaxLength);
            }
            return result;
        }
    }

    public class AurumRepositoryItemPredefinedValuesStringEdit : RepositoryItemComboBox
    {
        internal const string EditorName = "AurumPredefinedValuesStringEdit";
        internal static void Register()
        {
            if (!EditorRegistrationInfo.Default.Editors.Contains(EditorName))
            {
                EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(AurumPredefinedValuesStringEdit),
                    typeof(AurumRepositoryItemPredefinedValuesStringEdit),
                    typeof(ComboBoxViewInfo), new ButtonEditPainter(), true,
                    EditImageIndexes.ComboBoxEdit, typeof(DevExpress.Accessibility.PopupEditAccessible)));
            }
        }
        public override string EditorTypeName { get { return EditorName; } }
        static AurumRepositoryItemPredefinedValuesStringEdit()
        {
            AurumRepositoryItemPredefinedValuesStringEdit.Register();
        }
        public AurumRepositoryItemPredefinedValuesStringEdit()
        {
            Mask.MaskType = MaskType.None;
            Mask.UseMaskAsDisplayFormat = true;
        }
        public AurumRepositoryItemPredefinedValuesStringEdit(int maxLength, IEnumerable<string> predefinedValues)
            : this()
        {
            MaxLength = maxLength;
            CreatePredefinedListItems(predefinedValues);
        }
        public void CreatePredefinedListItems(IEnumerable<string> predefinedValues)
        {
            Items.Clear();
            foreach (string item in predefinedValues)
            {
                Items.Add(item);
            }
        }
        public void Init(string editMask, EditMaskType maskType)
        {
            if (!string.IsNullOrEmpty(editMask))
            {
                Mask.EditMask = editMask;
                switch (maskType)
                {
                    case EditMaskType.RegEx:
                        Mask.MaskType = MaskType.RegEx;
                        break;
                    default:
                        Mask.MaskType = MaskType.Simple;
                        break;
                }
            }
        }
    }

    public class AurumPredefinedValuesStringEdit : ComboBoxEdit
    {
        static AurumPredefinedValuesStringEdit()
        {
            AurumRepositoryItemPredefinedValuesStringEdit.Register();
        }

        public AurumPredefinedValuesStringEdit() { }
        public AurumPredefinedValuesStringEdit(int maxLength, IEnumerable<string> predefinedValues)
            : this()
        {
            ((AurumRepositoryItemPredefinedValuesStringEdit)Properties).MaxLength = maxLength;
            CreatePredefinedListItems(predefinedValues);
        }

        protected override void OnPopupClosed(PopupCloseMode closeMode)
        {
            base.OnPopupClosed(closeMode);
            DestroyPopupForm();
        }

        public void CreatePredefinedListItems(IEnumerable<string> predefinedValues)
        {
            ((AurumRepositoryItemPredefinedValuesStringEdit)Properties).CreatePredefinedListItems(predefinedValues);
        }
        public override string EditorTypeName { get { return AurumRepositoryItemPredefinedValuesStringEdit.EditorName; } }
    }
}
