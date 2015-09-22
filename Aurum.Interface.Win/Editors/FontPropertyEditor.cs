using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Editors;

namespace Aurum.Interface.Win.Editors
{
    /// <summary>
    /// Редактор свойства с типом шрифт
    /// </summary>
    [PropertyEditor(typeof(Font), "FontPropertyEditor", true)]
    public class FontPropertyEditor : WinPropertyEditor
    {
        private FontEdit edit;

        /// <inheritdoc/>
        protected override object CreateControlCore()
        {
            edit = new FontEdit();
            ControlBindingProperty = "EditValue";
            edit.AllowEdit = this.AllowEdit;
            this.AllowEdit.ResultValueChanged += AllowEdit_ResultValueChanged;
            return edit;
        }

        void AllowEdit_ResultValueChanged(object sender, BoolValueChangedEventArgs e)
        {
            Control.AllowEdit = this.AllowEdit;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (edit != null && !edit.IsDisposed)
                    try { edit.Dispose(); } catch { }
                edit = null;
            }
            base.Dispose(disposing);
        }

        public FontPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

        private void Editor_EditValueChanged(object sender, EventArgs e)
        {
            if (!inReadValue && Control.IsModified && (Control.DataBindings.Count > 0))
            {
                OnControlValueChanged();
            }
        }

        protected override void OnControlCreated()
        {
            Control.ValueChanged += new EventHandler(Editor_EditValueChanged);
            base.OnControlCreated();
        }

        public new FontEdit Control
        {
            get { return (FontEdit)base.Control; }
        }
    }
}
