using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// Редактор строки/текста
    /// </summary>
    /// <ToDo priority="low">При отображении формы значение отображется не полностью, только правый кусочек.
    /// Возможно связано с размером контрола в момент присваивания значения.
    /// bator: это дефолтное поведение контрола, происходит при получении фокуса (не через mouse event)</ToDo>
    public class TextBoxEdit : TextBox, IMethodParameter, IParameterValueChanged
    {
        // Отображение обязательного значения
        private static string emptyText = "<нет>";
        private static string warnText = "Введите значение";
        private static Color grayedTextColor = SystemColors.GrayText;
        private static Color textColor = SystemColors.WindowText;

        // Значение
        private bool editing = false;
        private string value = string.Empty;

        // Возможность пустого значения
        private bool isNullable = true;

        // Подсказка для пустого значения
        private string hint = null;

        /// <summary>
        /// Возможность пустого значения
        /// </summary>
        [Browsable(true), Category("Appearance"), Description("Возможность пустого значения")]
        [DefaultValue(true)]
        public bool IsNullable
        {
            get
            {
                return isNullable;
            }
            set
            {
                if (value != isNullable)
                {
                    isNullable = value;
                }
                SetText();
            }
        }

        /// <summary>
        /// Подсказка для пустого значения
        /// </summary>
        [Browsable(true), Description("Текст для пустого значения")]
        [DefaultValue(null)]
        public string Hint
        {
            get { return hint; }
            set
            {
                hint = value;
                SetText();
            }
        }

        /// <summary>
        /// Значение в текстовом формате
        /// </summary>
        [Browsable(true), Category("Appearance"), Description("Значение в текстовом формате")]
        //[DefaultValue("")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new string Text
        {
            get
            {
                return value;
            }
            set
            {
                Value = value;
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public TextBoxEdit()
            : base()
        {
            SetText();
            KeyUp += TextBoxEdit_KeyUp;
        }

        void TextBoxEdit_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                var f = new Form
                {
                    StartPosition = FormStartPosition.CenterParent,
                    Text = "Редактор текста",
                    Width = 360,
                    Height = 240,
                    FormBorderStyle = FormBorderStyle.FixedDialog
                };
                var t = new TextBox
                {
                    Multiline = true,
                    Dock = DockStyle.Fill,
                    Text = this.Text.Replace("\n", "\r\n")
                };
                f.Controls.Add(t);
                f.ShowDialog();
                Text = t.Text.Replace("\r", String.Empty);
            }
        }

        /// <summary>
        /// Значение
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Value
        {
            get
            {
                return string.IsNullOrEmpty(value) && isNullable ? null : value;
            }
            set
            {
                string text = value != null ? Convert.ToString(value) : string.Empty;
                if (text != this.value)
                {
                    this.value = text;
                    SetText();
                    SelectedText = null;

                    // Вызов события изменения значения
                    if (ValueChanged != null)
                    {
                        ValueChanged(this, new EventArgs());
                    }
                }
            }
        }

        /// <summary>
        /// Получение фокуса
        /// </summary>
        /// <param name="e">Параметры</param>
        protected override void OnEnter(EventArgs e)
        {
            editing = true;
            SetText();

            base.OnEnter(e);
        }

        /// <summary>
        /// Потеря фокуса
        /// </summary>
        /// <param name="e">Параметры</param>
        protected override void OnLeave(EventArgs e)
        {
            editing = false;
            SetText();

            base.OnLeave(e);
        }

        /// <summary>
        /// Изменение текста
        /// </summary>
        /// <param name="e">Параметры</param>
        protected override void OnTextChanged(EventArgs e)
        {
            // Изменение значения
            if (editing)
            {
                value = base.Text;

                // Вызов события изменения значения
                if (ValueChanged != null)
                {
                    ValueChanged(this, new EventArgs());
                }
            }

            base.OnTextChanged(e);
        }

        // Установка текста согласно текущего значения и режима редактирования
        private void SetText()
        {
            if (editing)
            {
                base.Text = value;
                base.ForeColor = textColor;
            }
            else
            {
                if (String.IsNullOrEmpty(value))
                {
                    // Показ предупреждения о пустом значении
                    if (!string.IsNullOrEmpty(hint))
                        base.Text = hint;
                    else
                        base.Text = isNullable ? emptyText : warnText;
                    base.ForeColor = grayedTextColor;
                }
                else
                {
                    base.Text = value;
                    base.ForeColor = textColor;
                }
            }
        }

        #region IParameterValueChanged
        /// <summary>
        /// Событие изменения значения
        /// </summary>		
        [Browsable(true), Category("Property Changed"), Description("Событие, возникающее при изменении значения")]
        public event EventHandler ValueChanged;
        #endregion
    }
}
