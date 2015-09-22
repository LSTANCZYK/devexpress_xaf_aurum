using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace Aurum.Interface.Win.Filters
{
	/// <summary>
	/// �������� ����/�������
	/// </summary>
	public class DateTimeEdit : DateTimePicker, IMethodParameter, IParameterValueChanged
	{
        /// <summary>
        /// ������ ������������� ���� (dd.MM.yyyy)
        /// </summary>
        const string DateFormat = "dd.MM.yyyy";

		// ����������� ������� ��������
		private bool isNullable = true;

		// ������������� ������
		private string customFormat = null;

		/// <summary>
		/// ������
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string CustomFormat
		{
			get { return customFormat; }
			set
			{
				customFormat = value;
				if (value != null)
				{
					base.CustomFormat = value;
				}
				else
				{
					base.CustomFormat = DateFormat;
				}
			}
		}

		/// <summary>
		/// �����������
		/// </summary>
		public DateTimeEdit()
			: base()
		{
			ShowCheckBox = isNullable;
			Checked = false;
			Format = DateTimePickerFormat.Custom;
			//ShowUpDown = true;
			base.CustomFormat = DateFormat;

            ValueChanged += new EventHandler(DateTimeEdit_ValueChanged);
		}

        private void DateTimeEdit_ValueChanged(object sender, EventArgs e)
        {
            raiseValueChanged();
        }

        private void raiseValueChanged()
        {
            lock (valueChangedLock)
            {
                if (valueChanged != null)
                {
                    valueChanged(this, new EventArgs());
                }
            }
        }

		/// <summary>
		/// WndProc
		/// </summary>
		/// <param name="m">Message</param>
		protected override void WndProc(ref Message m)
		{
			try
			{
				base.WndProc(ref m);
			}
			catch (ArgumentOutOfRangeException)
			{
				// workaround
				base.Value = new DateTime(base.Value.Year, base.Value.Month, 1, base.Value.Hour, base.Value.Minute, base.Value.Second);
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
				isNullable = value;
				ShowCheckBox = isNullable;
				if (isNullable)
				{
					Checked = false;
				}
				else
				{
					Checked = true;
				}
			}
		}

		/// <summary>
		/// ��������
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object Value
		{
			get
			{
				if (isNullable && !Checked)
				{
					return null;
				}
				if (customFormat == null)
					return base.Value.Date;
				else
				{
					string s = base.Value.ToString(customFormat);
					return DateTime.ParseExact(s, customFormat, null);
				}
			}
			set
			{
				if (value == null)
				{
					if (!isNullable)
						throw new ArgumentNullException("DateTime", "���� �� ����� ���� null");
					else
						Checked = false;
				}
				else
				{
					Checked = true;
                    DateTime temp = Convert.ToDateTime(value);
                    if (temp < base.MinDate)
                        temp = base.MinDate;
                    if (base.MaxDate < temp)
                        temp = base.MaxDate;
					base.Value = temp;
				}
			}
		}

        /// <summary>
        /// ������� ��������� ���������
        /// </summary>
        private event EventHandler valueChanged;
        private object valueChangedLock = new object();

        #region IParameterValueChanged
        /// <summary>
        /// ������� ��������� ��������
        /// </summary>
        [Browsable(true), Category("Property Changed"),
        Description("�������, ����������� ��� ��������� ��������")]
        event EventHandler IParameterValueChanged.ValueChanged
        {
            add
            {
                lock (valueChangedLock)
                {
                    valueChanged += value;
                }
            }
            remove
            {
                lock (valueChangedLock)
                {
                    valueChanged -= value;
                }
            }
        }
        #endregion

		/// <summary>
		/// ��������
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DateTime? DateTimeValueNullable
		{
			get { return Value != null ? (DateTime?)Value : (DateTime?)null; }
			set { Value = value.HasValue ? (object)value.Value : (object)null; }
		}

		/// <summary>
		/// ��������
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DateTime DateTimeValue
		{
			get { return Value != null ? (DateTime)Value : default(DateTime); }
			set { Value = value; }
		}
	}
}
