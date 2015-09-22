using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// �������� ������/������
    /// </summary>
    /// <ToDo priority="low">��� ����������� ����� �������� ����������� �� ���������, ������ ������ �������.
    /// �������� ������� � �������� �������� � ������ ������������ ��������.
    /// bator: ��� ��������� ��������� ��������, ���������� ��� ��������� ������ (�� ����� mouse event)</ToDo>
    public class TextBoxEdit : TextBox, IMethodParameter, IParameterValueChanged
    {
        // ����������� ������������� ��������
        private static string emptyText = "<���>";
        private static string warnText = "������� ��������";
        private static Color grayedTextColor = SystemColors.GrayText;
        private static Color textColor = SystemColors.WindowText;

        // ��������
        private bool editing = false;
        private string value = string.Empty;

        // ����������� ������� ��������
        private bool isNullable = true;

        // ��������� ��� ������� ��������
        private string hint = null;

        /// <summary>
        /// ����������� ������� ��������
        /// </summary>
        [Browsable(true), Category("Appearance"), Description("����������� ������� ��������")]
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
        /// ��������� ��� ������� ��������
        /// </summary>
        [Browsable(true), Description("����� ��� ������� ��������")]
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
        /// �������� � ��������� �������
        /// </summary>
        [Browsable(true), Category("Appearance"), Description("�������� � ��������� �������")]
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
        /// �����������
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
                    Text = "�������� ������",
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
        /// ��������
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

                    // ����� ������� ��������� ��������
                    if (ValueChanged != null)
                    {
                        ValueChanged(this, new EventArgs());
                    }
                }
            }
        }

        /// <summary>
        /// ��������� ������
        /// </summary>
        /// <param name="e">���������</param>
        protected override void OnEnter(EventArgs e)
        {
            editing = true;
            SetText();

            base.OnEnter(e);
        }

        /// <summary>
        /// ������ ������
        /// </summary>
        /// <param name="e">���������</param>
        protected override void OnLeave(EventArgs e)
        {
            editing = false;
            SetText();

            base.OnLeave(e);
        }

        /// <summary>
        /// ��������� ������
        /// </summary>
        /// <param name="e">���������</param>
        protected override void OnTextChanged(EventArgs e)
        {
            // ��������� ��������
            if (editing)
            {
                value = base.Text;

                // ����� ������� ��������� ��������
                if (ValueChanged != null)
                {
                    ValueChanged(this, new EventArgs());
                }
            }

            base.OnTextChanged(e);
        }

        // ��������� ������ �������� �������� �������� � ������ ��������������
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
                    // ����� �������������� � ������ ��������
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
        /// ������� ��������� ��������
        /// </summary>		
        [Browsable(true), Category("Property Changed"), Description("�������, ����������� ��� ��������� ��������")]
        public event EventHandler ValueChanged;
        #endregion
    }
}
