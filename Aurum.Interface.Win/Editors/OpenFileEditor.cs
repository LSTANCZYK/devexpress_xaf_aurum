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
using System.Windows.Forms;

namespace Aurum.Interface.Win.Editors
{
    /// <summary>
    /// Диалог открытия файла
    /// </summary>
    [PropertyEditor(typeof(string), "OpenFileEditor", false)]
    public class OpenFileEditor : DXPropertyEditor
    {
        public OpenFileEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) { }
        private void buttonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            // TODO: расширение файлов, множественный выбор файлов
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ((ButtonEdit)sender).Text = dialog.FileName;
                }
            }
        }
        protected override object CreateControlCore()
        {
            return new ButtonEdit();
        }
        protected override RepositoryItem CreateRepositoryItem()
        {
            return new RepositoryItemButtonEdit();
        }
        protected override void SetupRepositoryItem(RepositoryItem item)
        {
            base.SetupRepositoryItem(item);
            ((RepositoryItemButtonEdit)item).ButtonClick += buttonEdit_ButtonClick;
        }
        protected override void SetRepositoryItemReadOnly(RepositoryItem item, bool readOnly)
        {
            base.SetRepositoryItemReadOnly(item, readOnly);
            ((RepositoryItemButtonEdit)item).Buttons[0].Enabled = !readOnly;
        }
    }
}
