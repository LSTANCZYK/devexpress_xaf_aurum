using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml.XPath;
using System.Xml;
using DevExpress.Data.Filtering;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// ������ �������� ��������
    /// </summary>
    public partial class QueryGridBooleanFilter : QueryGridFilterBase
    {
        private const string BOOL_VALUE = "value";
        /// <summary>
        /// �����������
        /// </summary>
        public QueryGridBooleanFilter()
        {
            InitializeComponent();
            value = new BooleanFilterValue();
            comboBox1.SelectedIndex = 0;
        }

        /// <summary>
        /// ��������
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new BooleanFilterValue Value
        {
            get { return (BooleanFilterValue)base.Value; }
            set { base.Value = value; }
        }

        /// <summary>
        /// �������� �������
        /// </summary>
        public override string Description
        {
            get
            {
                if (Value.BoolState == BooleanFilterValue.boolStateTrue)
                {
                    return @"""��""";
                }
                else if (Value.BoolState == BooleanFilterValue.boolStateFalse)
                {
                    return @"""���""";
                }
                else if (Value.BoolState == BooleanFilterValue.boolStateUndetermined)
                {
                    return @"""(�� ����������)""";
                }
                else
                {
                    Debug.Assert(Value.BoolState == BooleanFilterValue.boolStateNone);
                    return null;
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
            if (Value.BoolState == BooleanFilterValue.boolStateTrue)
            {
                return new BinaryOperator(fieldRef, new OperandValue(true), BinaryOperatorType.Equal);
            }
            else if (Value.BoolState == BooleanFilterValue.boolStateFalse)
            {
                return new BinaryOperator(fieldRef, new OperandValue(false), BinaryOperatorType.Equal);
            }
            else if (Value.BoolState == BooleanFilterValue.boolStateUndetermined)
            {
                return new UnaryOperator(UnaryOperatorType.IsNull, fieldRef);
            }
            else
            {
                Debug.Assert(Value.BoolState == BooleanFilterValue.boolStateNone);
                return null;
            }
        }

        /// <summary>
        /// ����������� ��������� �������� �� ���������� ����������
        /// </summary>
        public override void ExternalValuesToInternalValues()
        {
            Debug.Assert(comboBox1.SelectedIndex >= BooleanFilterValue.boolStateNone && comboBox1.SelectedIndex <= BooleanFilterValue.boolStateUndetermined);
            Value.BoolState = comboBox1.SelectedIndex;
        }

        /// <summary>
        /// ����� ����� ����� � �������������� ����������� ��������
        /// </summary>
        public override void InternalValuesToExternalValues()
        {
            comboBox1.SelectedIndex = Value.BoolState;
            Debug.Assert(comboBox1.SelectedIndex >= BooleanFilterValue.boolStateNone && comboBox1.SelectedIndex <= BooleanFilterValue.boolStateUndetermined);
        }

        /// <summary>
        /// ������� ����� �����
        /// </summary>
        public override void ClearExternalValues()
        {
            comboBox1.SelectedIndex = BooleanFilterValue.boolStateNone;
        }

        /// <summary>
        /// ������� ����������� ��������
        /// </summary>
        public override void ClearInternalValues()
        {
            Value.BoolState = BooleanFilterValue.boolStateNone;
        }
        
        /// <summary>
        /// �������������� ���� ����� �������
        /// </summary>
        /// <param name="clone"></param>
        protected override void InitClone(QueryGridFilterBase clone)
        {
            //������������� ����� �����
            if (clone is QueryGridBooleanFilter)
            {
                (clone as QueryGridBooleanFilter).Value = this.Value;
            }
        }
        /// <summary>
        /// ������� ����� �������
        /// </summary>
        /// <returns></returns>
        public override QueryGridFilterBase Clone()
        {
            if (this.GetType() != typeof(QueryGridBooleanFilter))
                throw new NotImplementedException("�� ��������� ����� Clone � ������ " + this.GetType().FullName);
            QueryGridBooleanFilter clone = new QueryGridBooleanFilter();
            base.InitClone(clone);
            InitClone(clone);
            return clone;
        }
        
    }



    /// <summary>
    /// �������� �������� �������
    /// </summary>
    public class BooleanFilterValue : FilterValue
    {
        private int boolState;

        /// <summary>
        /// �����������
        /// </summary>
        public BooleanFilterValue()
        {
            boolState = boolStateNone;
        }

        /// <summary>
        /// ��������� ������
        /// </summary>
        public int BoolState
        {
            get
            {
                Debug.Assert(boolState >= boolStateNone && boolState <= boolStateUndetermined);
                return boolState;
            }
            set
            {
                Debug.Assert(value >= boolStateNone && value <= boolStateUndetermined);
                boolState = value;
            }
        }

        /// <summary>
        /// ������ �� ����
        /// </summary>
        public static readonly int boolStateNone = 0;

        /// <summary>
        /// ���� true
        /// </summary>
        public static readonly int boolStateTrue = 1;

        /// <summary>
        /// ���� false
        /// </summary>
        public static readonly int boolStateFalse = 2;

        /// <summary>
        /// ���� �������������� "������" ��������� (null)
        /// </summary>
        public static readonly int boolStateUndetermined = 3;
    }
}
