using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;

namespace Aurum.Reports.Win.Editors
{
    /// <summary>
    /// Редактор свойства отчетов Snap
    /// </summary>
    [PropertyEditor(typeof(SnapReportDocument), "SnapReportPropertyEditor", true)]
    public class SnapReportPropertyEditor : WinPropertyEditor
    {
        private SnapReportEditor editor;

        /// <summary>Конструктор</summary>
        public SnapReportPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

        /// <summary>Контрол типа отчета Snap</summary>
        public SnapReportEditor SnapReportEdit
        {
            get { return (SnapReportEditor)Control; }
        }

        /// <inheritdoc/>
        protected override object CreateControlCore()
        {
            editor = new SnapReportEditor();
            ControlBindingProperty = "Report";
            editor.AllowEdit = AllowEdit;
            AllowEdit.ResultValueChanged += AllowEdit_ResultValueChanged;
            editor.EditValueChanged += Editor_EditValueChanged;
            return editor;
        }

        void Editor_EditValueChanged(object sender, EventArgs e)
        {
            if (!inReadValue && (Control.DataBindings.Count > 0))
            {
                OnControlValueChanged();
            }
        }

        void AllowEdit_ResultValueChanged(object sender, DevExpress.ExpressApp.Utils.BoolValueChangedEventArgs e)
        {
            if (editor != null) editor.AllowEdit = this.AllowEdit;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (editor != null && !editor.IsDisposed)
                {
                    try { editor.Dispose(); } catch { }
                }
                editor = null;
            }
            base.Dispose(disposing);
        }
    }
}
