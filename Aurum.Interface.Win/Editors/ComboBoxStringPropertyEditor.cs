using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.XtraEditors.Controls;

namespace Aurum.Interface.Win.Editors
{
    /// <summary>
    /// Editor for string by ComboBox
    /// </summary>
    [PropertyEditor(typeof(string), "ComboBoxStringPropertyEditor", false)]
    public class ComboBoxStringPropertyEditor : DXPropertyEditor
    {
        protected override object CreateControlCore()
        {
            var control = new ComboBoxStringEdit() { };
            this.ClearButton = new EditorButton(ButtonPredefines.Delete);
            this.ClearButton.ToolTip = "Очистить поле";
            control.Properties.Buttons.Add(this.ClearButton);
            control.Properties.ButtonClick += new ButtonPressedEventHandler(this.OnClearButtonClick);
            return control;
        }

        protected override RepositoryItem CreateRepositoryItem()
        {
            return new RepositoryItemComboBox();
        }

        protected override void SetupRepositoryItem(
            DevExpress.XtraEditors.Repository.RepositoryItem item)
        {
            if (!string.IsNullOrEmpty(Model.DataSourceProperty))
            {
                IMemberInfo propWithEnum = MemberInfo.Owner.FindMember(Model.DataSourceProperty);
                if (propWithEnum != null)
                {
                    var list = propWithEnum.GetValue(this.CurrentObject) as IEnumerable<string>;
                    if (list != null)
                    {
                        foreach (var l in list)
                        {
                            ((RepositoryItemComboBox)item).Items.Add(l);
                        }
                    }
                }
            }
        }

        protected EditorButton ClearButton
        {
            get;
            private set;
        }
        private void OnClearButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == ClearButton)
            {
                this.Control.Text = null;
            }
        }

        public ComboBoxStringPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
    }

    public class ComboBoxStringEdit : ComboBoxEdit
    {
        protected override void OnPopupClosed(PopupCloseMode closeMode)
        {
            base.OnPopupClosed(closeMode);
            DestroyPopupForm();
        }
    }
}
