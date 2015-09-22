using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Editors;
using DevExpress.XtraEditors.Repository;
using DevExpress.ExpressApp.Win.SystemModule;
using System.Drawing;
using DevExpress.Utils;

namespace Aurum.Interface.Win.Editors
{
    /// <summary>
    /// Editor for bool
    /// </summary>
    [PropertyEditor(typeof(bool), "BooleanPropertyEditor", true)]
    public class BooleanPropertyEditor : DXPropertyEditor, IHtmlFormattingSupport
    {
        protected override object CreateControlCore()
        {
            BooleanEdit control = new BooleanEdit(Model.Caption);
            if (Model.ModelMember.Type == typeof(bool?))
            {
                control.Properties.AllowGrayed = true;
            }
            return control;
        }

        protected override void OnControlCreated()
        {
            base.OnControlCreated();
            ApplyHtmlFormatting();
        }

        protected override RepositoryItem CreateRepositoryItem()
        {
            return new RepositoryItemBooleanEdit();
        }

        protected override void SetRepositoryItemReadOnly(RepositoryItem item, bool readOnly)
        {
            base.SetRepositoryItemReadOnly(item, readOnly);
            if (item is RepositoryItemBooleanEdit)
            {
                item.Appearance.BackColor = Color.Transparent;
                if (Model.ModelMember.Type == typeof(bool?))
                {
                    ((RepositoryItemBooleanEdit)item).AllowGrayed = true;
                }
            }
            if (Control != null)
            {
                Control.Enabled = !readOnly;
            }
        }

        public BooleanPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

        public override bool IsCaptionVisible { get { return false; } }

        public override string Caption
        {
            get
            {
                return base.Caption;
            }
            set
            {
                base.Caption = value;
                if (Control is BooleanEdit)
                {
                    Control.Text = value;
                }
            }
        }

        #region IHtmlFormattingSupport Members
        private bool HtmlFormattingEnabled;
        public void SetHtmlFormattingEnabled(bool htmlFormattingEnabled)
        {
            this.HtmlFormattingEnabled = htmlFormattingEnabled;
            if (this.Control != null)
            {
                ApplyHtmlFormatting();
            }
        }

        private void ApplyHtmlFormatting()
        {
            if (this.Control is BooleanEdit)
            {
                ((BooleanEdit)this.Control).Properties.AllowHtmlDraw = HtmlFormattingEnabled ? DefaultBoolean.True : DefaultBoolean.False;
            }
        }
        #endregion
    }
}
