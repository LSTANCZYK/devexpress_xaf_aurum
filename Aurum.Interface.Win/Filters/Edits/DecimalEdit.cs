using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Aurum.Interface.Win.Filters
{
	/// <summary>
	/// �������� �������� �����
	/// </summary>
	/// <ToDo priority="low">�������� � ����������� ������������ 
	/// ����� ������������ �������� ���������� (��������� ���������):
	/// 1. ��� ��������� ��������, ����������� ��� �������� ������������
	/// 2. ��� ��������� �������, �������� �� ������������ ��������, �������� ������������ ��� ����� ��������
	/// 3. ��� �������������� �������� ����� ����������� ��������, ���� ��������� �������� ������������, 
	///    �� ����������� ���������� ���������� ��������, �� �������� �� ��������
	/// 4. ��� ������ �� ���������, ���� �������� ��������� ������������ ����, �������� �������� 
	///    � ������������ � ���������� ���������
	/// 5. ��� �������� ������� ������������ ����������� ������ (������������ MS)
	/// </ToDo>
	/// <ToDo priority="low">���������� ���������, �������� � ������</ToDo>
	/// <ToDo priority="low">ErrorProvider ��� ����������� ������ ������������� �������� ������������</ToDo>
	public class DecimalEdit : TextBox, IMethodParameter
	{
        // ������ ����������� ������� �����
        private static NumberFormatInfo numberFormat = null;

        /// <summary>
        /// ������ ����������� ������� �����
        /// </summary>
        public static NumberFormatInfo NumberFormat
        {
            get
            {
                if (numberFormat == null)
                {
                    numberFormat = new NumberFormatInfo();
                    numberFormat.NumberDecimalSeparator = ".";
                    numberFormat.NumberGroupSeparator = " ";
                }
                return numberFormat;
            }
        }

		// ��������
		private decimal? value = null;
		private decimal? minValue = null;
		private decimal? maxValue = null;

		// ����������� ������� ��������
		private bool isNullable = true;

		// ����� ��������������
		private bool editing = false;
		private bool editingSuccess = true;

		// ������
		private string textFormat = String.Empty;
		private NumberFormatInfo displayFormat;
		private NumberFormatInfo editFormat;

		// ����������� ������������� ��������
		private static string emptyText = "<���>";
		private static Color grayedTextColor = SystemColors.GrayText;
		private static Color textColor = SystemColors.WindowText;

		// ��������� ��� ������� ��������
		private string hint = null;

		// �����
		private bool _focused = false;

		/// <summary>
		/// ������ ������ �� ���������
		/// </summary>
		protected virtual string DefaultTextFormat
		{
			get { return String.Empty; }
		}

		/// <summary>
		/// �������� �� ���������
		/// </summary>
		private decimal? DefaultValue
		{
			get
			{
				if (isNullable)
					return null;
				else if (minValue.HasValue)
					return minValue;
				else if (maxValue.HasValue)
					return maxValue;
				else
					return 0;
			}
		}

		/// <summary>
		/// ���������� ������ ����� �������
		/// </summary>
		[Browsable(true), Category("Appearance"), Description("���������� ������ ����� �������")]
		[DefaultValue(2)]
		public int Digits
		{
			get
			{
				return displayFormat.NumberDecimalDigits;
			}
			set
			{
				displayFormat.NumberDecimalDigits = value;
				editFormat.NumberDecimalDigits = value;
				SetText();
			}
		}

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

					// ������������� ��������
					if (!isNullable)
						SetValueChecked(DefaultValue);
				}
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
		/// ������������ ��������
		/// </summary>
		[Browsable(true), Category("Behavior"), DefaultValue(null), Description("������������ ��������")]
		public decimal? MaxValue
		{
			get
			{
				return maxValue;
			}
			set
			{
				if (value.HasValue && minValue.HasValue && value.Value < minValue.Value)
					throw new ArgumentException("MaxValue must be not less then MinValue", "value");

				maxValue = value;

				// ������������� ��������
				if (maxValue.HasValue && this.value.HasValue && this.value.Value > maxValue.Value)
					SetValueChecked(maxValue);
			}
		}

		/// <summary>
		/// ����������� ��������
		/// </summary>
		[Browsable(true), Category("Behavior"), DefaultValue(null), Description("����������� ��������")]
		public decimal? MinValue
		{
			get
			{
				return minValue;
			}
			set
			{
				if (value.HasValue && maxValue.HasValue && value.Value > maxValue.Value)
					throw new ArgumentException("MinValue must be not greater then MaxValue", "value");

				minValue = value;

				// ������������� ��������
				if (minValue.HasValue && this.value.HasValue && this.value.Value < minValue.Value)
					SetValueChecked(minValue);
			}
		}

		/// <summary>
		/// ��������������� ���������
		/// </summary>
		/// <remarks>�������� ����� �� ����������� � ������������� �����</remarks>
		[Browsable(false)]
		public override bool Multiline
		{
			get { return base.Multiline; }
			set { }
		}

		/// <summary>
		/// �����
		/// </summary>
		/// <remarks>�������� �� ������� �� ���������, � ������� �� �����������</remarks>
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}

		/// <summary>
		/// ������ ������
		/// </summary>
		[Browsable(true), Category("Appearance"), Description("������ ������")]
		public string TextFormat
		{
			get
			{
				return textFormat;
			}
			set
			{
				textFormat = value;
				SetText();
			}
		}

		/// <summary>
		/// ��������
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object Value
		{
			get
			{
				if (isNullable)
				{
					return value;
				}
				else
				{
					return value != null ? value : DefaultValue;
				}
			}
			set
			{
				if (value == null || string.IsNullOrEmpty(value.ToString()))
				{
					if (isNullable)
						SetValue(null, true);
					else
						throw new ArgumentNullException("Value", "����� �� ����� ���� null");
				}
				else
				{
					SetValue(Convert.ToDecimal(value), true);
				}
			}
		}

		/// <summary>
		/// ��������
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual decimal? DecimalValueNullable
		{
			get { return value; }
			set { Value = (object)value; }
		}

		/// <summary>
		/// ��������
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual decimal DecimalValue
		{
			get { return value.HasValue ? value.Value : 0; }
			set { Value = (object)value; }
		}

		/// <summary>
		/// ������� ��������� ��������
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("�������, ����������� ��� ��������� ��������")]
		public event EventHandler ValueChanged;


		/// <summary>
		/// �����������
		/// </summary>
		public DecimalEdit()
		{
			// ������ ����������� �����
			displayFormat = new NumberFormatInfo();
			displayFormat.NumberDecimalSeparator = NumberFormat.NumberDecimalSeparator;
			displayFormat.NumberGroupSeparator = NumberFormat.NumberGroupSeparator;

			// ������ �������������� �����
			editFormat = new NumberFormatInfo();
			editFormat.NumberDecimalSeparator = NumberFormat.NumberDecimalSeparator;
			editFormat.NumberGroupSeparator = "";

			// ������ ������
			textFormat = DefaultTextFormat;

			SetText();
		}

		/// <summary>
		/// ��������� ������
		/// </summary>
		/// <param name="e">���������</param>
		protected override void OnEnter(EventArgs e)
		{
			// ����� ��������������
			editing = true;

			// ���� ����������� ����� ���������� ���������, �� ���������� ������ ������������
			if (!(CausesValidation && !editingSuccess))
				SetText();

			base.OnEnter(e);
		}

		/// <summary>
		/// ������� �������, ��������� ��������� �����
		/// </summary>
		/// <param name="e">���������</param>
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);

			// ������� ����������
			if (e.Handled)
				return;

			// ������ ������
			if (!Char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',' && e.KeyChar != '-' && e.KeyChar != '\b')
			{
				e.Handled = true;
				return;
			}

			// ���������� �����������
			if (e.KeyChar == ',')
				e.KeyChar = '.';

			// ���������� �����������
			if (e.KeyChar == '.' && base.Text.Contains("."))
			{
				e.Handled = true;
				return;
			}

			// ���������� �����������
			if (e.KeyChar == '.' && Digits == 0)
			{
				e.Handled = true;
				return;
			}

			// �����
			if (e.KeyChar == '-' && base.Text.Contains("-"))
			{
				e.Handled = true;
				return;
			}

			// �����
			if (e.KeyChar == '-' && minValue.HasValue && minValue.Value >= 0)
			{
				e.Handled = true;
				return;
			}

			// ���������� ������ ����� ������� (���� �����)
			int decimalSep = base.Text.IndexOf('.');
			if (Char.IsDigit(e.KeyChar) &&
				decimalSep >= 0 &&
				SelectionStart > decimalSep &&
				TextLength - decimalSep - 1 >= editFormat.NumberDecimalDigits)
			{
				e.Handled = true;
				return;
			}

			// ���������� ������ ����� ������� (���� ����������� �����������)
			if (e.KeyChar == '.' &&
				TextLength - SelectionStart > editFormat.NumberDecimalDigits)
			{
				e.Handled = true;
				return;
			}
		}

		/// <summary>
		/// ������ ������
		/// </summary>
		/// <param name="e">���������</param>
		protected override void OnLeave(EventArgs e)
		{
			editing = false;

			// ���� ��������� �����������, �� �������������� ����������� ��������� ���������� ���������
			if (!CausesValidation)
			{
				SetText();
				editingSuccess = true;
			}

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
				decimal d;
				if (String.IsNullOrEmpty(base.Text))
					SetValue(null, false);
				else if (decimal.TryParse(base.Text, NumberStyles.Number, editFormat, out d))
					SetValue(d, false);
			}

			base.OnTextChanged(e);
		}

		/// <summary>
		/// ��������� ��������
		/// </summary>
		/// <param name="e">���������</param>
		protected override void OnValidating(CancelEventArgs e)
		{
			base.OnValidating(e);

			// �������� ������������
			if (!editingSuccess)
			{
				//e.Cancel = true;
				value = DefaultValue;
			}
		}

		/// <summary>
		/// �������� ��������� ��������
		/// </summary>
		/// <param name="e">���������</param>
		protected override void OnValidated(EventArgs e)
		{
			base.OnValidated(e);

			// ��������� ������
			SetText();
		}

		// ��������� ������ �������� �������� �������� � ������ ��������������
		private void SetText()
		{
			string text = null;
			base.ForeColor = textColor;
			if (value.HasValue)
			{
				text = value.Value.ToString("N", editing ? editFormat : displayFormat);
				if (!editing && !String.IsNullOrEmpty(textFormat))
					text = String.Format(textFormat, text);
			}
			else
			{
				// ����� �������������� � ������ ��������
				if (!editing && isNullable)
				{
					if (!string.IsNullOrEmpty(hint))
						text = hint;
					else
						text = emptyText;
					base.ForeColor = grayedTextColor;
				}
			}
			base.Text = text;
		}

		// ��������� �������� ��� ��������� Value (external) ��� �������������� ������������ (internal)
		private void SetValue(decimal? newValue, bool external)
		{
			editingSuccess = false;
			// ��������� ��������
			try
			{
				if (!isNullable && !newValue.HasValue)
					throw new Exception("������� ��������");
				if (newValue.HasValue && minValue.HasValue && newValue.Value < minValue.Value)
					throw new Exception("�������� �� ������ ���� ������ " + minValue.ToString());
				if (newValue.HasValue && maxValue.HasValue && newValue.Value > maxValue.Value)
					throw new Exception("�������� �� ������ ���� ������ " + maxValue.ToString());
				editingSuccess = true;
			}
			// ��������� ���������� ��� ������������ ��������
			catch (Exception)
			{
				if (external)
					throw;
				else
					return;
			}

			// ��������� ������ ��������
			if (newValue != this.value)
			{
				this.value = newValue;

				// ��������� ������
				if (external)
					SetText();

				// ����� ������� ��������� ��������
				OnValueChanged(new EventArgs());
				if (ValueChanged != null)
				{
					ValueChanged(this, new EventArgs());
				}
			}
		}

		/// <summary>
		/// �������� ��������
		/// </summary>
		/// <param name="eventargs"></param>
		protected virtual void OnValueChanged(EventArgs eventargs)
		{
		}

		// ��������� �������� ��� ��������� �������, ��������� � ������������� ��������
		private void SetValueChecked(decimal? newValue)
		{
			if (this.value != newValue)
			{
				this.value = newValue;

				// ��������� ������
				SetText();

				// ����� ������� ��������� ��������
				OnValueChanged(new EventArgs());
				if (ValueChanged != null)
				{
					ValueChanged(this, new EventArgs());
				}
			}
		}

		// ������������ TextFormat
		private bool ShouldSerializeTextFormat()
		{
			return textFormat != DefaultTextFormat;
		}

		/// <summary>
		/// ��������� ������
		/// </summary>
		/// <param name="e"></param>
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			if (MouseButtons == MouseButtons.None)
			{
				SelectAll();
				_focused = true;
			}
		}

		/// <summary>
		/// ������ ������
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			_focused = false;
		}

		/// <summary>
		/// ������� ���������� ������ ����
		/// </summary>
		/// <param name="mevent"></param>
		protected override void OnMouseUp(MouseEventArgs mevent)
		{
			base.OnMouseUp(mevent);
			if (!_focused && this.SelectionLength == 0)
			{
				_focused = true;
				SelectAll();
			}
		}
	}
}
