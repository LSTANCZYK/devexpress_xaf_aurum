using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using System.Xml.XPath;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Utils;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// ������ ������������
    /// </summary>
    public partial class QueryGridEnumFilter : QueryGridFilterBase
    {
        /// <summary>
        /// �����������
        /// </summary>
        public QueryGridEnumFilter()
        {
            InitializeComponent();
            value = new EnumFilterValue();
            dataTypeChanged += new EventHandler(QueryGridEnumColumnFilter_dataTypeChanged);
            InternalValuesToExternalValues();
        }

        /// <summary>
        /// ��������
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new EnumFilterValue Value
        {
            get { return (EnumFilterValue)base.Value; }
            set { base.Value = value; }
        }

        /// <summary>
        /// ��� ������ �������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryGridEnumColumnFilter_dataTypeChanged(object sender, EventArgs e)
        {
            if (Type == null)
            {
                return;
            }
            enumToCheckedListBox(Type, checkedListBox1);
        }

        /// <summary>
        /// ���������� ������ �������������
        /// </summary>
        /// <param name="type"></param>
        /// <param name="checkedListBox1"></param>
        private void enumToCheckedListBox(Type type, CheckedListBox checkedListBox1)
        {
            if (type == null) throw new ArgumentNullException("Type is null!");
            if (type.IsGenericType && type.GetGenericTypeDefinition().FullName == "System.Nullable`1" && type.GenericTypeArguments[0].IsEnum)
            {
                type = type.GenericTypeArguments[0];
            }
            if (!type.IsEnum) throw new ArgumentException("Type is not enum!");

            // ��������� �������� ������
            checkedListBox1.Items.Clear();
            
            // �������� �� ��������
            EnumDescriptor ed = new EnumDescriptor(type);
            foreach (var enumValue in Enum.GetValues(type))
            {
                checkedListBox1.Items.Add(ed.GetCaption(enumValue));
            }
            if (checkedListBox1.Items.Count > 0 && checkedListBox1.Items.Count < 3)
            {
                checkedListBox1.Height = 15 * checkedListBox1.Items.Count + 4;
                Height = checkedListBox1.Height;
            }

            Value.HandleEnumTypeChanged(type);
        }

        /// <summary>
        /// �������� �������� ����� ������� �������.
        /// </summary>
        /// <returns> �������� ����� ������� </returns>
        public override QueryGridFilterBase Clone()
        {
            if (this.GetType() != typeof(QueryGridEnumFilter))
                throw new NotImplementedException("�� ��������� ����� Clone � ������ " + this.GetType().FullName);
            QueryGridEnumFilter clone = new QueryGridEnumFilter();
            InitClone(clone);
            return clone;
        }

        /// <summary>
        /// �������� �������
        /// </summary>
        public override string Description
        {
            get
            {
                if (Value.BCheckBoxNull.HasValue)
                {
                    return Value.BCheckBoxNull.Value ? notNullValue : nullValue;
                }
                else
                {
                    if (Value.BCheckedListBox == null || Type == null)
                    {
                        return null;
                    }

                    EnumDescriptor ed = new EnumDescriptor(Type);
                    if (ed == null)
                    {
                        return null;
                    }

                    if (Enum.GetValues(Type).Length != 
                        Value.BCheckedListBox.IfNotNull(x => x.Length))
                    {
                        return null;
                    }

                    string description = null;

                    int i = 0;
                    // �������� �� ��������
                    foreach (var enumValue in Enum.GetValues(Type))
                    {
                        if (Value.BCheckedListBox[i])
                        {
                            if (description != null)
                            {
                                description = "<������������� �����>";
                                break;
                            }
                            description = ed.GetCaption(enumValue);
                        }
                        i++;
                    }

                    return description;
                }
            }
        }

        /// <summary>
        /// ��������� ��������� �������
        /// </summary>
        /// <param name="fieldRef"></param>
        /// <returns></returns>
        public override CriteriaOperator GetExpression()
        {
            var fieldRef = new OperandProperty(FilterColumn.Property);
            if (Value.BCheckBoxNull.HasValue)
            {
                if (!Value.BCheckBoxNull.Value)
                {
                    return new UnaryOperator(UnaryOperatorType.IsNull, new OperandProperty(FilterColumn.Property));
                }
                else
                {
                    return new UnaryOperator(UnaryOperatorType.Not, new UnaryOperator(UnaryOperatorType.IsNull, new OperandProperty(FilterColumn.Property)));
                }
            }
            else
            {
                if (Value.BCheckedListBox == null) return null;
                var type = Type;
                if (type.IsGenericType && type.GetGenericTypeDefinition().FullName == "System.Nullable`1" && type.GenericTypeArguments[0].IsEnum)
                {
                    type = type.GenericTypeArguments[0];
                }
                EnumDescriptor ed = new EnumDescriptor(type);
                List<string> items = new List<string>();
                for (int i = 0; i < Value.BCheckedListBox.Length; i++)
                {
                    if (Value.BCheckedListBox[i])
                    {
                        items.Add((string)checkedListBox1.Items[i]);
                    }
                }
                if (items.Count == 0) return null;

                int val = 0;
                bool flags = false;
                bool flag_none = false;

                // ����������� �� �������� �������
                Type enumType = type.UnderlyingSystemType;
                Object[] attrs = enumType.GetCustomAttributes(typeof(FlagsAttribute), false);
                if (attrs.Length > 0)
                {
                    flags = true;
                }

                // ������������ �� �������� "�������" � ������� �����
                if (flags)
                {
                    foreach (var enumValue in Enum.GetValues(type))
                    {
                        if ((int)enumValue == 0)
                        {
                            flag_none = true;
                            break;
                        }
                    }
                }

                CriteriaOperator[] values = null;

                if (flag_none)
                {
                    // ��� �������� "�������" ������������ ������� ������� ��������� ������������ ���� ����
                    values = new CriteriaOperator[1];
                    values[0] = new BinaryOperator(fieldRef, new OperandValue(0), BinaryOperatorType.Equal);
                }
                else
                {
                    if (!flags)
                    {
                        values = new CriteriaOperator[items.Count];
                    }
                    else
                    {
                        values = new CriteriaOperator[1];
                    }
                    for (int i = 0; i < items.Count; i++)
                    {
                        object enumVal = null;
                        foreach (var enumValue in Enum.GetValues(type))
                        {
                            if (ed.GetCaption(enumValue) == items[i])
                            {
                                enumVal = enumValue;
                                break;
                            }
                        }
                        if (flags)
                        {
                            val |= (int)enumVal;
                        }
                        else
                            values[i] = new BinaryOperator(fieldRef, new OperandValue(enumVal), BinaryOperatorType.Equal);
                    }
                    if (flags)
                    {
                        values[0] = new BinaryOperator(fieldRef, new OperandValue(val), BinaryOperatorType.Equal);
                    }
                }

                return GroupOperator.Or(values);
            }
        }

        /// <summary>
        /// ����������� ��������� �������� �� ���������� ����������
        /// </summary>
        public override void ExternalValuesToInternalValues()
        {
            if (checkBoxNull.CheckState == CheckState.Indeterminate)
                Value.BCheckBoxNull = null;
            else
                Value.BCheckBoxNull = checkBoxNull.CheckState == CheckState.Checked;
            
            if (Value.BCheckedListBox.IfNotNull(x => x.Length, 0) !=
                checkedListBox1.Items.Count)
            {
                for (int i = 0; i < Value.BCheckedListBox.Length; i++)
                {
                    Value.BCheckedListBox[i] = false;
                }
                return;
            }

            for (int i = 0; i < Value.BCheckedListBox.Length; i++)
            {
                Value.BCheckedListBox[i] = checkedListBox1.GetItemCheckState(i) == CheckState.Checked;
            }
        }

        /// <summary>
        /// ����� ����� ����� � �������������� ����������� ��������
        /// </summary>
        public override void InternalValuesToExternalValues()
        {
            if (!Value.BCheckBoxNull.HasValue)
                checkBoxNull.CheckState = CheckState.Indeterminate;
            else
                checkBoxNull.CheckState = Value.BCheckBoxNull.Value ? CheckState.Checked : CheckState.Unchecked;

            if (Value.BCheckedListBox.IfNotNull(x => x.Length, 0) != 
                checkedListBox1.Items.Count)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                }
                return;
            }

            if (Value.BCheckedListBox != null)
            {
                int i = 0;
                foreach (bool b in Value.BCheckedListBox)
                {
                    checkedListBox1.SetItemCheckState(i, b ? CheckState.Checked : CheckState.Unchecked);
                    i++;
                }
            }
        }

        /// <summary>
        /// ������� ����� �����
        /// </summary>
        public override void ClearExternalValues()
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
            }
            checkBoxNull.CheckState = CheckState.Indeterminate;
        }

        /// <summary>
        /// ������� ����������� ��������
        /// </summary>
        public override void ClearInternalValues()
        {
            Value.BCheckBoxNull = null;
            if (Value.BCheckedListBox != null)
                for (int i = 0; i < Value.BCheckedListBox.Length; i++)
                {
                    Value.BCheckedListBox[i] = false;
                }
        }

        private void checkBoxNull_CheckStateChanged(object sender, EventArgs e)
        {
            checkedListBox1.Enabled = checkBoxNull.CheckState != CheckState.Indeterminate ? false : true;
        }
    }

    /// <summary>
    /// �������� ������� ������������
    /// </summary>
    public class EnumFilterValue : FilterValue
    {
        private const string FILTER_NULLS_ATTR = "enumFilterBoxNull";
        private const string FILTER_CHECKED_LISTBOX = "enumFilterCheckedListBox";
        // ������� ��������
        private bool? bCheckBoxNull;
        // ������ ��������� ��������
        private bool[] bCheckedListBox;

        /// <summary>
        /// �����������
        /// </summary>
        public EnumFilterValue()
        {
            bCheckBoxNull = null;
            bCheckedListBox = null;
        }

        /// <summary>
        /// �������� �������� ����� �������.
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            EnumFilterValue clone = new EnumFilterValue();
            clone.BCheckBoxNull = BCheckBoxNull;
            if (BCheckedListBox != null)
            {
                bool[] arr = new bool[BCheckedListBox.Length];
                Array.Copy(bCheckedListBox, arr, bCheckedListBox.Length);
                clone.BCheckedListBox = arr;
            }
            else
            {
                clone.BCheckedListBox = null;
            }
            return clone;
        }

        /// <summary>
        /// ������� ��������
        /// </summary>
        public bool? BCheckBoxNull
        {
            get { return bCheckBoxNull; }
            set { bCheckBoxNull = value; }
        }

        /// <summary>
        /// ������ ��������� ��������
        /// </summary>
        public bool[] BCheckedListBox
        {
            get { return bCheckedListBox; }
            set { bCheckedListBox = value; }
        }

        internal void HandleEnumTypeChanged(Type enm)
        {
            if (bCheckedListBox == null)
            {
                bCheckedListBox = new bool[Enum.GetValues(enm).Length];
            }
        }
    }
}
