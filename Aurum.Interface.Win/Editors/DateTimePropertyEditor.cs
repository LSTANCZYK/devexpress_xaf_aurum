using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Mask;
using DevExpress.Utils;
using System.Windows.Forms;
using DevExpress.ExpressApp.Editors;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;

namespace Aurum.Interface.Win.Editors
{
    [PropertyEditor(typeof(DateTime), EditorAliases.DateTimePropertyEditor)]
    public class DatePropertyEditor : DXPropertyEditor
    {
        private void editor_KeyDown(object sender, KeyEventArgs e)
        {
            if (Control.IsPopupOpen) return;
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                Control.ShowPopup();
            }
        }

        protected override object CreateControlCore()
        {
            DateTimeEdit editor = new DateTimeEdit();
            editor.KeyDown += new KeyEventHandler(editor_KeyDown);
            return editor;
        }

        protected override void SetupRepositoryItem(RepositoryItem item)
        {
            base.SetupRepositoryItem(item);
            RepositoryItemDateTimeEdit dateTimeEdit = (RepositoryItemDateTimeEdit)item;
            dateTimeEdit.Init(EditMask, DisplayFormat);
            dateTimeEdit.NullDate = AllowNull ? null : (object)DateTime.MinValue;
            dateTimeEdit.AllowNullInput = AllowNull ? DefaultBoolean.True : DefaultBoolean.Default;
        }

        public override bool CanFormatPropertyValue
        {
            get { return true; }
        }

        protected override RepositoryItem CreateRepositoryItem()
        {
            return new RepositoryItemDateTimeEdit();
        }

        public DatePropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
        
        public new DateEdit Control
        {
            get { return (DateEdit)base.Control; }
        }
    }

    public class RepositoryItemDateTimeEdit : RepositoryItemDateEdit
    {
        internal const string EditorName = "DateTimeEdit";
        internal static void Register()
        {
            if (!EditorRegistrationInfo.Default.Editors.Contains(EditorName))
            {
                EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(DateTimeEdit),
                    typeof(RepositoryItemDateTimeEdit), typeof(DateEditViewInfo),
                    new ButtonEditPainter(), true, EditImageIndexes.DateEdit, typeof(DevExpress.Accessibility.PopupEditAccessible)));
            }
        }
        static RepositoryItemDateTimeEdit()
        {
            RepositoryItemDateTimeEdit.Register();
        }
        private void RepositoryItemDateTimeEdit_ParseEditValue(object sender, DevExpress.XtraEditors.Controls.ConvertEditValueEventArgs e)
        {
            RepositoryItemDateTimeEdit repItem = null;
            if (sender is RepositoryItemDateTimeEdit)
            {
                repItem = (RepositoryItemDateTimeEdit)sender;
            }
            else if (sender is DateTimeEdit)
            {
                repItem = (RepositoryItemDateTimeEdit)((DateTimeEdit)sender).Properties;
            }
            if (repItem != null && e.Value == null)
            {
                e.Value = repItem.AllowNullInput == DefaultBoolean.True ? null : (object)DateTime.MinValue;
                e.Handled = true;
            }
        }
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    this.ParseEditValue -= new DevExpress.XtraEditors.Controls.ConvertEditValueEventHandler(RepositoryItemDateTimeEdit_ParseEditValue);
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
        public override string EditorTypeName { get { return EditorName; } }
        public RepositoryItemDateTimeEdit(string editMask, string displayFormat)
            : this()
        {
            Init(editMask, displayFormat);
        }
        public RepositoryItemDateTimeEdit()
        {
            NullText = "";
            Mask.MaskType = MaskType.DateTime;
            Mask.UseMaskAsDisplayFormat = true;
            DisplayFormat.FormatType = FormatType.DateTime;
            ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.DoubleClick;
            ParseEditValue += new DevExpress.XtraEditors.Controls.ConvertEditValueEventHandler(RepositoryItemDateTimeEdit_ParseEditValue);
        }
        public void Init(string editMask, string displayFormat)
        {
            if (!string.IsNullOrEmpty(editMask))
            {
                Mask.EditMask = editMask;
            }
            if (!string.IsNullOrEmpty(displayFormat))
            {
                Mask.UseMaskAsDisplayFormat = false;
                DisplayFormat.FormatString = displayFormat;
            }
        }
    }

    [System.ComponentModel.ToolboxItem(false)]
    public class DateTimeEdit : DateEdit
    {
        static DateTimeEdit()
        {
            RepositoryItemDateTimeEdit.Register();
        }
        
        public DateTimeEdit()
        {
        }

        protected override void OnPopupClosed(PopupCloseMode closeMode)
        {
            base.OnPopupClosed(closeMode);
            DestroyPopupForm();
        }
        
        public DateTimeEdit(string editMask, string displayFormat)
        {
            Height = WinPropertyEditor.TextControlHeight;
            ((RepositoryItemDateTimeEdit)this.Properties).Init(editMask, displayFormat);
        }
        public override string EditorTypeName { get { return RepositoryItemDateTimeEdit.EditorName; } }
    }
}
