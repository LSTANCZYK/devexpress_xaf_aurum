using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aurum.Interface.Win.Editors
{
    /// <summary>
    /// Редактор шрифта
    /// </summary>
    /// <remarks>Редактирует значения основных атрибутов шрифта: семейство, размер, стиль (подчеркнутый, курсив, жирный)</remarks>
    public partial class FontEdit : UserControl
    {
        private event EventHandler valueChanged;
        private bool isModified;
        private bool setValue;

        /// <summary>Конструктор редактора</summary>
        public FontEdit()
        {
            InitializeComponent();
        }

        /// <summary>Редактируемое значение шрифта</summary>
        public Font EditValue
        {
            get
            {
                string fontFamily = editFontFamily.EditValue as string;
                if (string.IsNullOrEmpty(fontFamily)) return null;
                float size;
                if (!float.TryParse(editSize.Text, out size)) size = 10;
                size = Math.Min(Math.Max(size, 4), 40);
                FontStyle style = FontStyle.Regular;
                if (editUnderline.Checked) style |= FontStyle.Underline;
                if (editItalic.Checked) style |= FontStyle.Italic;
                if (editBold.Checked) style |= FontStyle.Bold;
                return new Font(fontFamily, size, style);
            }
            set
            {
                setValue = true;
                if (value == null)
                {
                    editFontFamily.EditValue = string.Empty;
                    editSize.Text = string.Empty;
                    editUnderline.Checked = false;
                    editItalic.Checked = false;
                    editBold.Checked = false;
                }
                else
                {
                    editFontFamily.EditValue = value.FontFamily.Name;
                    editSize.Text = value.Size.ToString();
                    editUnderline.Checked = value.Underline;
                    editItalic.Checked = value.Italic;
                    editBold.Checked = value.Bold;
                }
                setValue = false;
                isModified = false;
            }
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            if (!setValue)
            {
                isModified = true;
                if (valueChanged != null)
                    valueChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>Событие изменения значения</summary>
        public event EventHandler ValueChanged
        {
            add { valueChanged += value; }
            remove { valueChanged -= value; }
        }

        /// <summary>Указывает, было ли изменено значение пользователем после последней установки</summary>
        public bool IsModified
        {
            get { return isModified; }
        }

        /// <summary>Указывает, доступно ли значение для редактирования пользователем</summary>
        public bool AllowEdit
        {
            set
            {
                editFontFamily.ReadOnly = !value;
                editSize.ReadOnly = !value;
                editUnderline.Enabled = value;
                editItalic.Enabled = value;
                editBold.Enabled = value;
            }
        }
    }
}
