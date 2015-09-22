using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using DevExpress.Data.Filtering;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// Фильтр даты
    /// </summary>
    public partial class QueryGridDateFilter : QueryGridFilterBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public QueryGridDateFilter()
        {
            InitializeComponent();
            value = new DateFilterValue();
            // вкладка 1
            comboBox1.Items.Add(Operators.Equal);
            comboBox1.Items.Add(Operators.NotEqual);
            comboBox1.Items.Add(Operators.Less);
            comboBox1.Items.Add(Operators.LessEqual);
            comboBox1.Items.Add(Operators.Greater);
            comboBox1.Items.Add(Operators.GreaterEqual);
            comboBox1.SelectedIndex = 0;
            // вкладка 2
            comboBox2.Items.Add(Operators.Less);
            comboBox2.Items.Add(Operators.LessEqual);
            comboBox2.SelectedIndex = 0;
            comboBox3.Items.Add(Operators.Less);
            comboBox3.Items.Add(Operators.LessEqual);
            comboBox3.SelectedIndex = 0;
            // вкладка 3
            comboBoxOperators2.Items.Add(Operators.Equal);
            comboBoxOperators2.Items.Add(Operators.NotEqual);
            comboBoxOperators2.Items.Add(Operators.Less);
            comboBoxOperators2.Items.Add(Operators.LessEqual);
            comboBoxOperators2.Items.Add(Operators.Greater);
            comboBoxOperators2.Items.Add(Operators.GreaterEqual);
            comboBoxOperators2.SelectedIndex = 0;
            comboBoxCondition.Items.Add("Сегодня");
            comboBoxCondition.Items.Add("Первое число");
            comboBoxCondition.Items.Add("Последнее число");
            comboBoxCondition.SelectedIndex = 0;
            // Вкладка "Точный фильтр"
            exactDateTime.CustomFormat = Formats.DateTimeFormat2;
            exactDateTime.IsNullable = false;
            operatorsComboBox.Items.Add(Operators.Equal);
            operatorsComboBox.Items.Add(Operators.NotEqual);
            operatorsComboBox.Items.Add(Operators.Less);
            operatorsComboBox.Items.Add(Operators.LessEqual);
            operatorsComboBox.Items.Add(Operators.Greater);
            operatorsComboBox.Items.Add(Operators.GreaterEqual);

            InternalValuesToExternalValues();
        }

        /// <summary>
        /// Значение
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new DateFilterValue Value
        {
            get { return (DateFilterValue)base.Value; }
            set { base.Value = value; }
        }

        /// <summary>
        /// Дата из поля ввода
        /// </summary>
        private DateTime? date1
        {
            get
            {
                if (!dateTimeEdit1.Checked)
                    return null;
                return (DateTime)dateTimeEdit1.Value;
            }
        }

        /// <summary>
        /// Дата из поля ввода
        /// </summary>
        private DateTime? date2
        {
            get
            {
                if (!dateTimeEdit2.Checked)
                    return null;
                return (DateTime)dateTimeEdit2.Value;
            }
        }

        /// <summary>
        /// Дата из поля ввода
        /// </summary>
        private DateTime? date3
        {
            get
            {
                if (!dateTimeEdit3.Checked)
                    return null;
                return (DateTime)dateTimeEdit3.Value;
            }
        }

        /// <summary>
        /// Описание фильтра
        /// </summary>
        public override string Description
        {
            get
            {
                if (Value.BCheckBoxNull.HasValue)
                    return Value.BCheckBoxNull.Value ? notNullValue : nullValue;
                if (Value.SelectedTab == 0)
                {                    
                    DateTime? obj = Value.DateTimeEdit1Value;
                    if (!obj.HasValue)
                        return null;
                    Operators op = (Operators)comboBox1.Items[Value.ComboBox1SelectedIndex];
                    return op.ToString() + " " + obj.Value.ToString(Formats.DateFormat);
                }
                else if (Value.SelectedTab == 1)
                {
                    DateTime? o1 = Value.DateTimeEdit2Value;
                    DateTime? o2 = Value.DateTimeEdit3Value;
                    if (o1.HasValue)
                    {
                        if (o2.HasValue)
                        {
                            Operators op1 = (Operators)comboBox2.Items[Value.ComboBox2SelectedIndex];
                            Operators op2 = (Operators)comboBox3.Items[Value.ComboBox3SelectedIndex];

                            return o1.Value.ToString(Formats.DateFormat) + " " + op1.ToString() + " X " +
                                op2.ToString() + " " + o2.Value.ToString(Formats.DateFormat);
                        }
                        else
                        {
                            // введено только первое значение
                            Operators op = Operators.Invert((Operators)comboBox2.Items[Value.ComboBox2SelectedIndex]);

                            return op.ToString() + " " + o1.Value.ToString(Formats.DateFormat);
                        }
                    }
                    else if (o2.HasValue)
                    {
                        // введено только второе значение
                        Operators op = (Operators)comboBox3.Items[Value.ComboBox3SelectedIndex];

                        return op.ToString() + " " + o2.Value.ToString(Formats.DateFormat);
                    }
                    else
                    {
                        // ничего не введено
                        return null;
                    }
                }
                else if (Value.SelectedTab == 2)
                {
                    DateTime? obj = null;
                    switch (Value.ComboBoxConditionSelectedIndex)
                    {
                        case 0:
                            // сегодня
                            DateTime today = DateTime.Today;
                            obj = today;
                            break;
                        case 1:
                            // первое число
                            DateTime first = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                            obj = first;
                            break;
                        case 2:
                            // последнее число
                            DateTime last = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                            last = last.AddMonths(1).AddDays(-1);
                            obj = last;
                            break;
                    }
                    if (!obj.HasValue)
                        return null;
                    if (Value.EditShiftYearValue.HasValue)
                        obj = obj.Value.AddYears(Value.EditShiftYearValue.Value);
                    if (Value.EditShiftMonthValue.HasValue)
                        obj = obj.Value.AddMonths(Value.EditShiftMonthValue.Value);
                    if (Value.EditShiftDayValue.HasValue)
                        obj = obj.Value.AddDays(Value.EditShiftDayValue.Value);
                    Operators op = (Operators)comboBoxOperators2.Items[Value.ComboBoxOperators2SelectedIndex];
                    return op.ToString() + " " + obj.Value.ToString(Formats.DateFormat);
                }
                else if (Value.SelectedTab == 3)
                {
                    DateTime? obj = Value.ExactDateTimeEditValue;
                    if (!obj.HasValue)
                        return null;
                    Operators op = (Operators)operatorsComboBox.Items[Value.ComboBox4SelectedIndex];
                    return op.ToString() + " " + obj.Value.ToString(Formats.DateTimeFormat2);
                }
                else return base.Description;
            }
        }

        /// <summary>
        /// Получение выражения запроса
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
            if (Value.SelectedTab == 0)
            {
                DateTime? obj = Value.DateTimeEdit1Value;
                if (!obj.HasValue)
                    return null;
                Operators op = (Operators)comboBox1.Items[Value.ComboBox1SelectedIndex];

                return BuildQueryExpression(op, fieldRef, obj.Value);
            }
            else if (Value.SelectedTab == 1)
            {
                DateTime? o1 = Value.DateTimeEdit2Value;
                DateTime? o2 = Value.DateTimeEdit3Value;
                if (o1.HasValue)
                {
                    if (o2.HasValue)
                    {
                        Operators op1 = Operators.Invert((Operators)comboBox2.Items[Value.ComboBox2SelectedIndex]);
                        Operators op2 = (Operators)comboBox3.Items[Value.ComboBox3SelectedIndex];

                        return new GroupOperator(GroupOperatorType.And,
                            BuildQueryExpression(op1, fieldRef, o1.Value),
                            BuildQueryExpression(op2, fieldRef, o2.Value));
                    }
                    else
                    {
                        // введено только первое значение
                        Operators op = Operators.Invert((Operators)comboBox2.Items[Value.ComboBox2SelectedIndex]);

                        return BuildQueryExpression(op, fieldRef, o1.Value);
                    }
                }
                else if (o2 != null)
                {
                    // введено только второе значение
                    Operators op = (Operators)comboBox3.Items[Value.ComboBox3SelectedIndex];

                    return BuildQueryExpression(op, fieldRef, o2.Value);
                }
                else
                {
                    // ничего не введено
                    return null;
                }
            }
            else if (Value.SelectedTab == 2)
            {
                DateTime? obj = null;
                switch (Value.ComboBoxConditionSelectedIndex)
                {
                    case 0:
                        // сегодня
                        DateTime today = DateTime.Today;
                        obj = today;
                        break;
                    case 1:
                        // первое число
                        DateTime first = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                        obj = first;
                        break;
                    case 2:
                        // последнее число
                        DateTime last = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                        last = last.AddMonths(1).AddDays(-1);
                        obj = last;
                        break;
                }
                if (!obj.HasValue)
                    return null;
                if (Value.EditShiftYearValue.HasValue)
                    obj = obj.Value.AddYears(Value.EditShiftYearValue.Value);
                if (Value.EditShiftMonthValue.HasValue)
                    obj = obj.Value.AddMonths(Value.EditShiftMonthValue.Value);
                if (Value.EditShiftDayValue.HasValue)
                    obj = obj.Value.AddDays(Value.EditShiftDayValue.Value);
                Operators op = (Operators)comboBoxOperators2.Items[Value.ComboBoxOperators2SelectedIndex];

                return BuildQueryExpression(op, fieldRef, obj.Value);
            }
            else if (Value.SelectedTab == 3)
            {
                DateTime? obj = Value.ExactDateTimeEditValue;
                if (!obj.HasValue)
                    return null;
                Operators op = (Operators)operatorsComboBox.Items[Value.ComboBox4SelectedIndex];

                return new BinaryOperator(fieldRef, new OperandValue(obj.Value), op.GetOperators()[0]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Построить выражение
        /// </summary>
        /// <param name="op">Оператор</param>
        /// <param name="fieldRef">Поле</param>
        /// <param name="o">Значение</param>
        /// <returns></returns>
        protected CriteriaOperator BuildQueryExpression(Operators op, OperandProperty fieldRef, DateTime o)
        {
            if (op == Operators.NotEqual)
            {
                return new BinaryOperator(new FunctionOperator(FunctionOperatorType.GetDate, fieldRef), new OperandValue(o), BinaryOperatorType.NotEqual);
            }
            else if (op.Binary)
            {
                return new GroupOperator(op.Or ? GroupOperatorType.Or : GroupOperatorType.And,
                    new BinaryOperator(new FunctionOperator(FunctionOperatorType.GetDate, fieldRef), new OperandValue(o), op.GetOperators()[0]),
                    new BinaryOperator(new FunctionOperator(FunctionOperatorType.GetDate, fieldRef), new OperandValue(o), op.GetOperators()[1])
                    );
            }
            else
            {
                return new BinaryOperator(new FunctionOperator(FunctionOperatorType.GetDate, fieldRef), new OperandValue(o), op.GetOperators()[0]);
            }
        }

        /// <summary>
        /// Запоминание введенных значений во внутренних переменных
        /// </summary>
        public override void ExternalValuesToInternalValues()
        {
            //Value.
            Value.SelectedTab = tabControl1.SelectedIndex;
            if (checkBoxNull.CheckState == CheckState.Indeterminate)
                Value.BCheckBoxNull = null;
            else
                Value.BCheckBoxNull = checkBoxNull.CheckState == CheckState.Checked;
            Value.ComboBox1SelectedIndex = comboBox1.SelectedIndex;
            Value.ComboBox2SelectedIndex = comboBox2.SelectedIndex;
            Value.ComboBox3SelectedIndex = comboBox3.SelectedIndex;
            Value.ComboBox4SelectedIndex = operatorsComboBox.SelectedIndex;
            Value.DateTimeEdit1Checked = dateTimeEdit1.Checked;
            Value.DateTimeEdit2Checked = dateTimeEdit2.Checked;
            Value.DateTimeEdit3Checked = dateTimeEdit3.Checked;
            Value.DateTimeEdit1Value = date1;
            Value.DateTimeEdit2Value = date2;
            Value.DateTimeEdit3Value = date3;
            Value.ExactDateTimeEditValue = exactDateTime.DateTimeValueNullable;
            Value.ComboBoxOperators2SelectedIndex = comboBoxOperators2.SelectedIndex;
            Value.ComboBoxConditionSelectedIndex = comboBoxCondition.SelectedIndex;
            Value.EditShiftYearValue = editShiftYear.IntValueNullable;
            Value.EditShiftMonthValue = editShiftMonth.IntValueNullable;
            Value.EditShiftDayValue = editShiftDay.IntValueNullable;
        }

        /// <summary>
        /// Сброс полей ввода и восстановление сохраненных значений
        /// </summary>
        public override void InternalValuesToExternalValues()
        {
            tabControl1.SelectedIndex = Value.SelectedTab;
            if (!Value.BCheckBoxNull.HasValue)
                checkBoxNull.CheckState = CheckState.Indeterminate;
            else
                checkBoxNull.CheckState = Value.BCheckBoxNull.Value ? CheckState.Checked : CheckState.Unchecked;
            comboBox1.SelectedIndex = Value.ComboBox1SelectedIndex;
            comboBox2.SelectedIndex = Value.ComboBox2SelectedIndex;
            comboBox3.SelectedIndex = Value.ComboBox3SelectedIndex;
            operatorsComboBox.SelectedIndex = Value.ComboBox4SelectedIndex;
            dateTimeEdit1.Checked = Value.DateTimeEdit1Checked;
            dateTimeEdit2.Checked = Value.DateTimeEdit2Checked;
            dateTimeEdit3.Checked = Value.DateTimeEdit3Checked;
            dateTimeEdit1.Value = Value.DateTimeEdit1Value;
            dateTimeEdit2.Value = Value.DateTimeEdit2Value;
            dateTimeEdit3.Value = Value.DateTimeEdit3Value;
            exactDateTime.IsNullable = true;
            exactDateTime.Value = Value.ExactDateTimeEditValue;
            comboBoxOperators2.SelectedIndex = Value.ComboBoxOperators2SelectedIndex;
            comboBoxCondition.SelectedIndex = Value.ComboBoxConditionSelectedIndex;
            editShiftYear.IntValueNullable = Value.EditShiftYearValue;
            editShiftMonth.IntValueNullable = Value.EditShiftMonthValue;
            editShiftDay.IntValueNullable = Value.EditShiftDayValue;
        }

        /// <summary>
        /// Очистка полей ввода
        /// </summary>
        public override void ClearExternalValues()
        {
            checkBoxNull.CheckState = CheckState.Indeterminate;
            dateTimeEdit1.Value = null;
            dateTimeEdit2.Value = null;
            dateTimeEdit3.Value = null;
            exactDateTime.Value = null;
            editShiftYear.IntValueNullable = null;
            editShiftMonth.IntValueNullable = null;
            editShiftDay.IntValueNullable = null;
        }

        /// <summary>
        /// Очистка сохраненных значений
        /// </summary>
        public override void ClearInternalValues()
        {
            Value.SelectedTab = 0;
            Value.BCheckBoxNull = null;
            Value.ComboBox1SelectedIndex = 0;
            Value.ComboBox2SelectedIndex = 0;
            Value.ComboBox3SelectedIndex = 0;
            Value.ComboBox4SelectedIndex = 0;
            Value.DateTimeEdit1Checked = false;
            Value.DateTimeEdit2Checked = false;
            Value.DateTimeEdit3Checked = false;
            Value.DateTimeEdit1Value = null;
            Value.DateTimeEdit2Value = null;
            Value.DateTimeEdit3Value = null;
            Value.ExactDateTimeEditValue = null;
            Value.ComboBoxOperators2SelectedIndex = 0;
            Value.ComboBoxConditionSelectedIndex = 0;
            Value.EditShiftYearValue = null;
            Value.EditShiftMonthValue = null;
            Value.EditShiftDayValue = null;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
                dateTimeEdit1.Focus();
            if (tabControl1.SelectedIndex == 1)
                dateTimeEdit2.Focus();
            if (tabControl1.SelectedIndex == 2)
                comboBoxCondition.Focus();
        }

        private void QueryGridDateFilter_Enter(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
                dateTimeEdit1.Focus();
            if (tabControl1.SelectedIndex == 1)
                dateTimeEdit2.Focus();
            if (tabControl1.SelectedIndex == 2)
                comboBoxCondition.Focus();
        }

        private void checkBoxNull_CheckStateChanged(object sender, EventArgs e)
        {
            dateTimeEdit1.Enabled = checkBoxNull.CheckState == CheckState.Indeterminate;
            dateTimeEdit2.Enabled = checkBoxNull.CheckState == CheckState.Indeterminate;
            dateTimeEdit3.Enabled = checkBoxNull.CheckState == CheckState.Indeterminate;
            comboBoxCondition.Enabled = checkBoxNull.CheckState == CheckState.Indeterminate;
            editShiftYear.Enabled = checkBoxNull.CheckState == CheckState.Indeterminate;
            editShiftMonth.Enabled = checkBoxNull.CheckState == CheckState.Indeterminate;
            editShiftDay.Enabled = checkBoxNull.CheckState == CheckState.Indeterminate;
        }
        
        /// <summary>
        /// Копия обьекта
        /// </summary>
        /// <returns></returns>
        public override QueryGridFilterBase Clone()
        {
            if (this.GetType() != typeof(QueryGridDateFilter))
                throw new NotImplementedException("Не определен метод Clone в классе " + this.GetType().FullName);
            QueryGridDateFilter copy = new QueryGridDateFilter();
            InitClone(copy);
            return copy;
        }
    }

    /// <summary>
    /// Значение фильтра даты
    /// </summary>
    public class DateFilterValue : FilterValue
    {
        /// <summary>
        /// Ключи аттрибутов в Xml описании, соответствующие данным фильтра.
        /// </summary>
        private const string FILTER_SELECTEDTAB_ATTR = "dateTimeFilterSelectedTab";
        private const string FILTER_SELECTED1_ATTR = "dateTimeFilterBox1SelectedIndex";
        private const string FILTER_SELECTED2_ATTR = "dateTimeFilterBox2SelectedIndex";
        private const string FILTER_SELECTED3_ATTR = "dateTimeFilterBox3SelectedIndex";
        private const string FILTER_SELECTED4_ATTR = "dateTimeFilterBox4SelectedIndex";
        private const string FILTER_EDIT1_ATTR = "dateTimeFilterEdit1Value";
        private const string FILTER_EDIT2_ATTR = "dateTimeFilterEdit2Value";
        private const string FILTER_EDIT3_ATTR = "dateTimeFilterEdit3Value";
        private const string FILTER_EDIT4_ATTR = "exactDateTimeValue";
        private const string FILTER_NULLS_ATTR = "dateTimeFilterBoxNull";
        
        private const string FILTER_EDIT1_CHECKED_ATTR = "dateTimeFilterEdit1Checked";
        private const string FILTER_EDIT2_CHECKED_ATTR = "dateTimeFilterEdit2Checked";
        private const string FILTER_EDIT3_CHECKED_ATTR = "dateTimeFilterEdit3Checked";

        private const string FILTER_SELECTED_OPERATORS2_ATTR = "comboBoxOperators2SelectedIndex";
        private const string FILTER_SELECTED_CONDITION_ATTR = "comboBoxConditionSelectedIndex";
        private const string FILTER_SHIFT_YEAR_ATTR = "editShiftYearValue";
        private const string FILTER_SHIFT_MONTH_ATTR = "editShiftMonthValue";
        private const string FILTER_SHIFT_DAY_ATTR = "editShiftDayValue";

        private int selectedTab;
        private int comboBox1SelectedIndex;
        private int comboBox2SelectedIndex;
        private int comboBox3SelectedIndex;
        private int comboBox4SelectedIndex;
        private bool dateTimeEdit1Checked;
        private bool dateTimeEdit2Checked;
        private bool dateTimeEdit3Checked;
        private DateTime? dateTimeEdit1Value;
        private DateTime? dateTimeEdit2Value;
        private DateTime? dateTimeEdit3Value;
        private DateTime? exactDateTimeValue;
        private bool? bCheckBoxNull;
        
        private int comboBoxOperators2SelectedIndex;
        private int comboBoxConditionSelectedIndex;
        private int? editShiftYearValue;
        private int? editShiftMonthValue;
        private int? editShiftDayValue;

        /// <summary>
        /// Значение чекбокса null
        /// </summary>
        public bool? BCheckBoxNull
        {
            get { return bCheckBoxNull; }
            set { bCheckBoxNull = value; }
        }

        /// <summary>
        /// Выбранный таб
        /// </summary>
        public int SelectedTab
        {
            get { return selectedTab; }
            set { selectedTab = value; }
        }

        /// <summary>
        /// Значение комбобокса 1
        /// </summary>
        public int ComboBox1SelectedIndex
        {
            get { return comboBox1SelectedIndex; }
            set { comboBox1SelectedIndex = value; }
        }

        /// <summary>
        /// Значение комбобокса 2
        /// </summary>
        public int ComboBox2SelectedIndex
        {
            get { return comboBox2SelectedIndex; }
            set { comboBox2SelectedIndex = value; }
        }

        /// <summary>
        /// Значение комбобокса 3
        /// </summary>
        public int ComboBox3SelectedIndex
        {
            get { return comboBox3SelectedIndex; }
            set { comboBox3SelectedIndex = value; }
        }

        /// <summary>
        /// Значение комбобокса 4
        /// </summary>
        public int ComboBox4SelectedIndex
        {
            get { return comboBox4SelectedIndex; }
            set { comboBox4SelectedIndex = value; }
        }

        /// <summary>
        /// Значение комбобокса comboBoxOperators2SelectedIndex
        /// </summary>
        public int ComboBoxOperators2SelectedIndex
        {
            get { return comboBoxOperators2SelectedIndex; }
            set { comboBoxOperators2SelectedIndex = value; }
        }

        /// <summary>
        /// Значение комбобокса comboBoxConditionSelectedIndex
        /// </summary>
        public int ComboBoxConditionSelectedIndex
        {
            get { return comboBoxConditionSelectedIndex; }
            set { comboBoxConditionSelectedIndex = value; }
        }

        /// <summary>
        /// Значение редактора даты
        /// </summary>
        public bool DateTimeEdit1Checked
        {
            get { return dateTimeEdit1Checked; }
            set { dateTimeEdit1Checked = value; }
        }

        /// <summary>
        /// Значение редактора даты
        /// </summary>
        public bool DateTimeEdit2Checked
        {
            get { return dateTimeEdit2Checked; }
            set { dateTimeEdit2Checked = value; }
        }

        /// <summary>
        /// Значение редактора даты
        /// </summary>
        public bool DateTimeEdit3Checked
        {
            get { return dateTimeEdit3Checked; }
            set { dateTimeEdit3Checked = value; }
        }

        /// <summary>
        /// Значение редактора даты
        /// </summary>
        public DateTime? DateTimeEdit1Value
        {
            get { return dateTimeEdit1Value; }
            set { dateTimeEdit1Value = value; }
        }

        /// <summary>
        /// Значение редактора даты
        /// </summary>
        public DateTime? DateTimeEdit2Value
        {
            get { return dateTimeEdit2Value; }
            set { dateTimeEdit2Value = value; }
        }

        /// <summary>
        /// Значение редактора даты
        /// </summary>
        public DateTime? DateTimeEdit3Value
        {
            get { return dateTimeEdit3Value; }
            set { dateTimeEdit3Value = value; }
        }

        /// <summary>
        /// Значение сдвига Год
        /// </summary>
        public int? EditShiftYearValue
        {
            get { return editShiftYearValue; }
            set { editShiftYearValue = value; }
        }

        /// <summary>
        /// Значение сдвига Месяц
        /// </summary>
        public int? EditShiftMonthValue
        {
            get { return editShiftMonthValue; }
            set { editShiftMonthValue = value; }
        }

        /// <summary>
        /// Значение сдвига День
        /// </summary>
        public int? EditShiftDayValue
        {
            get { return editShiftDayValue; }
            set { editShiftDayValue = value; }
        }

        /// <summary>
        /// Точная дата
        /// </summary>
        public DateTime? ExactDateTimeEditValue
        {
            get { return exactDateTimeValue; }
            set { exactDateTimeValue = value; }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public DateFilterValue()
        {
        }
    }
}
