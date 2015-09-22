using DevExpress.Data.Filtering;
using System;
using System.ComponentModel;
using System.Drawing;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// ������� ����� ������� ���������� ��������
    /// </summary>
    public partial class QueryGridStringFilterBase : QueryGridFilterBase
    {
        // ������ ����� �� ���������
        private static StringFilterExactSearch? defaultExactSearch = null;

        private bool? nulls = null;
        private bool caseSensitive = false;
        private StringFilterExactSearch exactSearch = StringFilterExactSearch.Free;

        /// <summary>
        /// ������������� ������ � ����������� ������� ������� �������� �������.
        /// ����������, ������� ���� ����-������, ������ ����������� ����� � ���������������� ��� ������ � ���� ������.
        /// </summary>
        /// <param name="clone"> ������ ��� ������������� </param>
        protected override void InitClone(QueryGridFilterBase clone)
        {
            if (clone == null)
            {
                return;
            }

            base.InitClone(clone);
            if (clone is QueryGridStringFilterBase)
            {
                QueryGridStringFilterBase stringBaseFilter = clone as QueryGridStringFilterBase;
                stringBaseFilter.nulls = nulls;
                stringBaseFilter.caseSensitive = caseSensitive;
                stringBaseFilter.exactSearch = exactSearch;
            }
        }

        /// <summary>
        /// �������� �������� ����� ������� �������. 
        /// ������ ��������� ������ ����������� �����, 
        /// � ������� ����� ������� ��������� ������������ ������ ������� 
        /// � �������� ������ ���� ������ �������.
        /// </summary>
        /// <returns> �������� ����� ������� </returns>
        public override QueryGridFilterBase Clone()
        {
            if (this.GetType() != typeof(QueryGridStringFilterBase))
                throw new NotImplementedException("�� ��������� ����� Clone � ������ " + this.GetType().FullName);
            QueryGridStringFilterBase clone = new QueryGridStringFilterBase();
            InitClone(clone);
            return clone;
        }

        /// <summary>
        /// ������ ����� �� ���������
        /// </summary>
        private static StringFilterExactSearch DefaultExactSearch
        {
            get
            {
                if (!defaultExactSearch.HasValue)
                {
                    defaultExactSearch = StringFilterExactSearch.Free;
                }
                return defaultExactSearch.Value;
            }
        }

        /// <summary>
        /// ������������� �����
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual string EditText
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// ����� ������� ��������
        /// </summary>
        private bool? Nulls
        {
            get
            {
                return nulls;
            }
            set
            {
                nulls = value;
                buttonNull.Image =
                    !nulls.HasValue ? Resources.Textfield : (
                    nulls.Value ? Resources.TextfieldAdd : Resources.TextfieldDelete);
                buttonNull.BackColor = !nulls.HasValue ? SystemColors.Control : SystemColors.ButtonHighlight;
                buttonCase.Enabled = !nulls.HasValue;
                buttonExact.Enabled = !nulls.HasValue;
                this.toolTip1.SetToolTip(this.buttonNull,
                    !nulls.HasValue ? "����� ������/�������� �������� ��������" : (
                    nulls.Value ? "����� ������ ��������" : "����� �������� ��������"));
                OnSetNulls(value);
            }
        }

        /// <summary>
        /// ����� � ������ ��������
        /// </summary>
        private bool CaseSensitive
        {
            get
            {
                return caseSensitive;
            }
            set
            {
                caseSensitive = value;
                buttonCase.BackColor = !caseSensitive ? SystemColors.Control : SystemColors.ButtonHighlight;
                this.toolTip1.SetToolTip(this.buttonCase,
                    caseSensitive ? "����� � ������ ��������" : "����� ��� ����� ��������");
            }
        }

        /// <summary>
        /// ������ �����
        /// </summary>
        private StringFilterExactSearch ExactSearch
        {
            get
            {
                return exactSearch;
            }
            set
            {
                exactSearch = value;
                buttonExact.Image =
                    exactSearch == StringFilterExactSearch.Exact ? Resources.TextfieldAbcSelected : (
                    exactSearch == StringFilterExactSearch.Begin ? Resources.TextfieldAbcStartsel :
                    Resources.TextfieldAbc);
                buttonExact.BackColor = exactSearch == StringFilterExactSearch.Free ?
                    SystemColors.Control : SystemColors.ButtonHighlight;
                this.toolTip1.SetToolTip(this.buttonExact,
                    exactSearch == StringFilterExactSearch.Exact ? "������ �����" : 
                    exactSearch == StringFilterExactSearch.Free ? "����� � ����� ����� ������" :
                                                                  "����� � ������ ������");
            }
        }

        /// <summary>
        /// �����������
        /// </summary>
        public QueryGridStringFilterBase()
        {
            InitializeComponent();
            buttonCase.Image = Resources.TextSmallCaps;
            value = new StringFilterValue();
            Value.ExactSearch = DefaultExactSearch;
        }

        /// <summary>
        /// ��������
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new StringFilterValue Value
        {
            get { return (StringFilterValue)base.Value; }
            set { base.Value = value; }
        }

        /// <summary>
        /// �������� �������������� �������
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Description
        {
            get
            {
                if (Value.Nulls.HasValue)
                    return Value.Nulls.Value ? nullValue : notNullValue;
                return Value.Text1;
            }
        }

        /// <summary>
        /// �������� ��������� �������
        /// </summary>
        /// <param name="fieldRef">������ �� ���� �������</param>
        /// <returns>��������� �������</returns>
        public override CriteriaOperator GetExpression()
        {
            if (Value.Nulls.HasValue)
            {
                if (Value.Nulls.Value)
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
                string filter = GetFilterText();
                if (string.IsNullOrEmpty(filter))
                    return null;
                else
                {
                    CriteriaOperator field = new OperandProperty(FilterColumn.Property);
                    if (!Value.CaseSensitive)
                    {
                        return new BinaryOperator(new FunctionOperator(FunctionOperatorType.Upper, field), new OperandValue(filter.ToUpper()), BinaryOperatorType.Like);
                    }
                    else
                    {
                        return new BinaryOperator(field, new OperandValue(filter), BinaryOperatorType.Like);
                    }
                }
            }
        }

        /// <summary>
        /// ������� ������ �������
        /// </summary>
        /// <param name="s">������������ ������</param>
        /// <returns>������ ��� �������</returns>
        protected string ParseFilterText(string s)
        {
            // ������ ������
            if (string.IsNullOrEmpty(s)) return s;

            s = s.Replace("*", "%");

            // ���� ������ �� �������� ����. �������
            if(s.IndexOf("%") < 0 && s.IndexOf("_") < 0)
            {
                if (s.Length > 2 && s.StartsWith("\"") && s.EndsWith("\"")) // ����� � �������� - ������ ������������
                {
                    return s.Substring(1, s.Length - 2);
                }

                // ����� ���������������������
                switch (Value.ExactSearch)
                {
                    case StringFilterExactSearch.Exact: return s;
                    case StringFilterExactSearch.Free: return "%" + s + "%";
                    case StringFilterExactSearch.Begin: return s + "%";
                }
            }
            
            return s;
        }

        /// <summary>
        /// �������� ����� �������
        /// </summary>
        /// <returns>����� �������</returns>
        protected virtual string GetFilterText()
        {
            return ParseFilterText(Value.Text1);
        }

        /// <summary>
        /// ����������� ��������� �������� �� ���������� ����������
        /// </summary>
        public override void ExternalValuesToInternalValues()
        {
            Value.Text1 = EditText;
            Value.Nulls = Nulls;
            Value.ExactSearch = ExactSearch;
            Value.CaseSensitive = CaseSensitive;
        }

        /// <summary>
        /// ����� ����� ����� � �������������� ����������� ��������
        /// </summary>
        public override void InternalValuesToExternalValues()
        {
            EditText = Value.Text1;
            Nulls = Value.Nulls;
            ExactSearch = Value.ExactSearch;
            CaseSensitive = Value.CaseSensitive;
        }

        /// <summary>
        /// ������� ����� �����
        /// </summary>
        public override void ClearExternalValues()
        {
            EditText = string.Empty;
            Nulls = null;
            ExactSearch = DefaultExactSearch;
            CaseSensitive = false;
        }

        /// <summary>
        /// ������� ����������� ��������
        /// </summary>
        public override void ClearInternalValues()
        {
            Value.Text1 = null;
            Value.Nulls = null;
            Value.CaseSensitive = false;
            Value.ExactSearch = StringFilterExactSearch.Free;
        }

        // ��������� ����� ������� ������
        private void OnExactClick(object sender, EventArgs e)
        {
            switch (ExactSearch)
            {
                case StringFilterExactSearch.Free: ExactSearch = StringFilterExactSearch.Exact; break;
                case StringFilterExactSearch.Exact: ExactSearch = StringFilterExactSearch.Begin; break;
                case StringFilterExactSearch.Begin: ExactSearch = StringFilterExactSearch.Free; break;
            }
        }

        // ��������� ����� ������ � ������ ��������
        private void OnCaseClick(object sender, EventArgs e)
        {
            CaseSensitive = !CaseSensitive;
        }

        // ��������� ����� ������ ������ ��������
        private void OnNullsClick(object sender, EventArgs e)
        {
            Nulls = !Nulls.HasValue ? true : (Nulls.Value ? false : (bool?)null);
        }

        /// <summary>
        /// ������� ��������� ������ ������ ��������
        /// </summary>
        /// <param name="value">����� ������ ��������</param>
        protected virtual void OnSetNulls(bool? value)
        {
        }
    }

    /// <summary>
    /// �������� ���������� �������
    /// </summary>
    public class StringFilterValue : FilterValue
    {
        private string text1;
        private bool? nulls;
        private bool caseSensitive;
        private StringFilterExactSearch exactSearch;

        /// <summary>
        /// �����������
        /// </summary>
        public StringFilterValue()
        {
            text1 = null;
            caseSensitive = false;
            nulls = null;
            exactSearch = StringFilterExactSearch.Free;
        }

        /// <summary>
        /// �����
        /// </summary>
        public string Text1
        {
            get { return text1; }
            set { text1 = value; }
        }

        /// <summary>
        /// ����� ������ ��������
        /// </summary>
        public bool? Nulls
        {
            get { return nulls; }
            set { nulls = value; }
        }

        /// <summary>
        /// ����� � ������ ��������
        /// </summary>
        public bool CaseSensitive
        {
            get { return caseSensitive; }
            set { caseSensitive = value; }
        }

        /// <summary>
        /// ����� ������� ��������
        /// </summary>
        public StringFilterExactSearch ExactSearch
        {
            get { return exactSearch; }
            set { exactSearch = value; }
        }
    }

    /// <summary>
    /// ���� ������ ������� �������� � ��������� �������
    /// </summary>
    public enum StringFilterExactSearch
    {
        /// <summary>
        /// ������ �����
        /// </summary>
        Exact,

        /// <summary>
        /// ������������ �����
        /// </summary>
        Free,

        /// <summary>
        /// ����� �� ������ ������
        /// </summary>
        Begin
    }
}