using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// �������� ������ �����
    /// </summary>
    public class IntegerEdit : DecimalEdit
    {
        /// <summary>
        /// �����������
        /// </summary>
        public IntegerEdit()
            : base()
        {
            Digits = 0;
        }

        /// <summary>
        /// ��������
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override object Value
        {
            get
            {
                decimal? value = (decimal?)base.Value;
                if (value == null)
                    return null;
                return Convert.ToInt32(value);
            }
            set
            {
                base.Value = value;
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int? IntValueNullable
        {
            get { return Value != null ? (int?)Value : (int?)null; }
            set { Value = value.HasValue ? (object)value.Value : (object)null; }
        }

        /// <summary>
        /// ��������
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int IntValue
        {
            get { return Value != null ? (int)Value : default(int); }
            set { Value = value; }
        }
    }
}
