using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml.XPath;
using System.Xml;
using System.Linq;
using System.Collections;
using DevExpress.Data.Filtering;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// Фильтр чисел
    /// </summary>
    public partial class QueryGridNumberFilter<T> : QueryGridFilterBase
    {
        private readonly int constraintDigits;
        private readonly Type dataType;

        /// <summary>
        /// Конструктор
        /// </summary>
        public QueryGridNumberFilter(Type dt, int constraintDigits)
        {
            this.constraintDigits = constraintDigits;
            this.dataType = dt;

            InitializeComponent();
            value = new NumberFilterValue();
            comboBox1.Items.Add(Operators.Equal);
            comboBox1.Items.Add(Operators.NotEqual);
            comboBox1.Items.Add(Operators.Less);
            comboBox1.Items.Add(Operators.LessEqual);
            comboBox1.Items.Add(Operators.Greater);
            comboBox1.Items.Add(Operators.GreaterEqual);
            comboBox1.SelectedIndex = 0;
            comboBox2.Items.Add(Operators.Less);
            comboBox2.Items.Add(Operators.LessEqual);
            comboBox2.SelectedIndex = 0;
            comboBox3.Items.Add(Operators.Less);
            comboBox3.Items.Add(Operators.LessEqual);
            comboBox3.SelectedIndex = 0;
            Value.DataType = dt;
            
            comboBox4.Items.Add(Operators.In);

            arrayEdit1.ElementType = dt;

            setConstraintDigits(constraintDigits);
            InternalValuesToExternalValues();
        }

        /// <summary>
        /// Значение
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new NumberFilterValue Value
        {
            get { return (NumberFilterValue)base.Value; }
            set { base.Value = value; }
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
                    string text = Value.DecimalEdit1Value;
                    if (string.IsNullOrEmpty(text))
                        return null;
                    T obj;
                    if (!TryParse(text, out obj))
                    {
                        return errorMessage;
                    }
                    Operators op = (Operators)comboBox1.Items[Value.ComboBox1SelectedIndex];
                    return op.ToString() + " " + obj;
                }
                else if (Value.SelectedTab == 1)
                {
                    string text1 = Value.DecimalEdit2Value;
                    string text2 = Value.DecimalEdit3Value;
                    if (!string.IsNullOrEmpty(text1))
                    {
                        if (!string.IsNullOrEmpty(text2))
                        {
                            T o1;
                            if (!TryParse(text1, out o1))
                            {
                                return errorMessage;
                            }
                            T o2;
                            if (!TryParse(text2, out o2))
                            {
                                return errorMessage;
                            }
                            Operators op1 = (Operators)comboBox2.Items[Value.ComboBox2SelectedIndex];
                            Operators op2 = (Operators)comboBox3.Items[Value.ComboBox3SelectedIndex];

                            return o1 + " " + op1.ToString() + " X " + op2.ToString() + " " + o2;
                        }
                        else
                        {
                            // введено только первое значение
                            T o;
                            if (!TryParse(text1, out o))
                            {
                                return errorMessage;
                            }
                            Operators op = Operators.Invert((Operators)comboBox2.Items[Value.ComboBox2SelectedIndex]);

                            return op.ToString() + " " + o;
                        }
                    }
                    else if (!string.IsNullOrEmpty(text2))
                    {
                        // введено только второе значение
                        T o;
                        if (!TryParse(text2, out o))
                        {
                            return errorMessage;
                        }
                        Operators op = (Operators)comboBox3.Items[Value.ComboBox3SelectedIndex];

                        return op.ToString() + " " + o;
                    }
                    else
                    {
                        // ничего не введено
                        return null;
                    }
                }
                else if (Value.SelectedTab == 2)
                {
                    string text = null;
                    if (Value.ArrayEdit1Value != null)
                        text = string.Join(",", Value.ArrayEdit1Value);
                    else
                        return "";

                    Operators op4 = (Operators)comboBox4.Items[Value.ComboBox4SelectedIndex];
                    return op4.ToString() + "(" + text + ")";

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
                string text = Value.DecimalEdit1Value;
                if (string.IsNullOrEmpty(text))
                    return null;
                T obj;
                if (!TryParse(text, out obj))
                {
                    return null;
                }
                Operators op = (Operators)comboBox1.Items[Value.ComboBox1SelectedIndex];

                return BuildQueryExpression(op, fieldRef, obj);
            }
            else if (Value.SelectedTab == 1)
            {
                string text1 = Value.DecimalEdit2Value;
                string text2 = Value.DecimalEdit3Value;
                if (!string.IsNullOrEmpty(text1))
                {
                    if (!string.IsNullOrEmpty(text2))
                    {
                        T o1;
                        if (!TryParse(text1, out o1))
                        {
                            return null;
                        }
                        T o2;
                        if (!TryParse(text2, out o2))
                        {
                            return null;
                        }
                        Operators op1 = Operators.Invert((Operators)comboBox2.Items[Value.ComboBox2SelectedIndex]);
                        Operators op2 = (Operators)comboBox3.Items[Value.ComboBox3SelectedIndex];

                        return new GroupOperator(GroupOperatorType.And,
                            BuildQueryExpression(op1, fieldRef, o1),
                            BuildQueryExpression(op2, fieldRef, o2));
                    }
                    else
                    {
                        // введено только первое значение
                        T o;
                        if (!TryParse(text1, out o))
                        {
                            return null;
                        }
                        Operators op = Operators.Invert((Operators)comboBox2.Items[Value.ComboBox2SelectedIndex]);

                        return BuildQueryExpression(op, fieldRef, o);
                    }
                }
                else if (!string.IsNullOrEmpty(text2))
                {
                    // введено только второе значение
                    T o;
                    if (!TryParse(text2, out o))
                    {
                        return null;
                    }
                    Operators op = (Operators)comboBox3.Items[Value.ComboBox3SelectedIndex];

                    return BuildQueryExpression(op, fieldRef, o);
                }
                else
                {
                    // ничего не введено
                    return null;
                }
            }
            else
            {
                if (Value.SelectedTab == 2)
                {
                    string text = null;

                    if (Value.ArrayEdit1Value != null) text = string.Join(";", Value.ArrayEdit1Value);

                    if (string.IsNullOrEmpty(text))
                        return null;

                    IEnumerable obj = null;

                    if (dataType == typeof(int))
                    {
                        obj = Value.ArrayEdit1Value.Select(x => int.Parse(x.Replace(".", ",")));
                    }
                    if (dataType == typeof(decimal))
                    {
                        obj = Value.ArrayEdit1Value.Select(x => Convert.ToDecimal(x.Replace(".", ",")));
                    }

                    // Operators op = (Operators)comboBox4.Items[Value.ComboBox4SelectedIndex];

                    if (dataType == typeof(int))
                    {
                        arrayEdit1.Value = Value.ArrayEdit1Value.Select(x => int.Parse(x.Replace(".", ","))).ToArray();
                    }
                    if (dataType == typeof(decimal))
                    {
                        arrayEdit1.Value = Value.ArrayEdit1Value.Select(x => Convert.ToDecimal(x.Replace(".", ","))).ToArray();
                    }

                    return new InOperator(fieldRef.PropertyName, obj);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Построить выражение
        /// </summary>
        /// <param name="op">Оператор</param>
        /// <param name="fieldRef">Поле запроса</param>
        /// <param name="o">Значение</param>
        /// <returns></returns>
        protected CriteriaOperator BuildQueryExpression(Operators op, OperandProperty fieldRef, T o)
        {
            if (op == Operators.NotEqual)
            {
                return new BinaryOperator(fieldRef, new OperandValue(o), BinaryOperatorType.NotEqual);
            }
            else if (op.Binary)
            {
                return new GroupOperator(op.Or ? GroupOperatorType.Or : GroupOperatorType.And,
                    new BinaryOperator(fieldRef, new OperandValue(o), op.GetOperators()[0]),
                    new BinaryOperator(fieldRef, new OperandValue(o), op.GetOperators()[1])
                    );
            }
            else
            {
                return new BinaryOperator(fieldRef, new OperandValue(o), op.GetOperators()[0]);
            }
        }

        /// <summary>
        /// Преобразование из строки в T
        /// </summary>
        /// <param name="s">Строка</param>
        /// <param name="o">Объект типа T</param>
        /// <returns>Объект</returns>
        protected virtual bool TryParse(string s, out T o)
        {
            o = default(T);
            return false;
        }

        /// <summary>
        /// Установка ограничения по количеству цифр после запятой
        /// </summary>
        /// <param name="i"></param>
        private void setConstraintDigits(int i)
        {
            decimalEdit1.Digits = i;
            decimalEdit2.Digits = i;
            decimalEdit3.Digits = i;
        }

        /// <summary>
        /// Запоминание введенных значений во внутренних переменных
        /// </summary>
        public override void ExternalValuesToInternalValues()
        {
            if (checkBoxNull.CheckState == CheckState.Indeterminate)
                Value.BCheckBoxNull = null;
            else
                Value.BCheckBoxNull = checkBoxNull.CheckState == CheckState.Checked;
            Value.SelectedTab = tabControl1.SelectedIndex;
            Value.ComboBox1SelectedIndex = comboBox1.SelectedIndex;
            Value.ComboBox2SelectedIndex = comboBox2.SelectedIndex;
            Value.ComboBox3SelectedIndex = comboBox3.SelectedIndex;
            Value.DecimalEdit1Value = Convert.ToString(decimalEdit1.Value);
            Value.DecimalEdit2Value = Convert.ToString(decimalEdit2.Value);
            Value.DecimalEdit3Value = Convert.ToString(decimalEdit3.Value);

            Value.ArrayEdit1Value = arrayEdit1.Value == null ? null : ((Array)arrayEdit1.Value).OfType<object>().Select(o => o.ToString().Replace(",",".")).ToArray();
        }

        /// <summary>
        /// Сброс полей ввода и восстановление сохраненных значений
        /// </summary>
        public override void InternalValuesToExternalValues()
        {
            if (!Value.BCheckBoxNull.HasValue)
                checkBoxNull.CheckState = CheckState.Indeterminate;
            else
                checkBoxNull.CheckState = Value.BCheckBoxNull.Value ? CheckState.Checked : CheckState.Unchecked;
            tabControl1.SelectedIndex = Value.SelectedTab;
            comboBox1.SelectedIndex = Value.ComboBox1SelectedIndex;
            comboBox2.SelectedIndex = Value.ComboBox2SelectedIndex;
            comboBox3.SelectedIndex = Value.ComboBox3SelectedIndex;
            comboBox4.SelectedIndex = Value.ComboBox4SelectedIndex;
            decimalEdit1.Value = Value.DecimalEdit1Value;
            decimalEdit2.Value = Value.DecimalEdit2Value;
            decimalEdit3.Value = Value.DecimalEdit3Value;

            if (dataType == typeof(decimal))
            {
                arrayEdit1.Value = (Value.ArrayEdit1Value == null || (Value.ArrayEdit1Value.Count() == 1 && Value.ArrayEdit1Value[0] == "")) ? null : Value.ArrayEdit1Value.IfNotNull(x => x).Select(x => Convert.ToDecimal(x.Replace(".", ","))).ToArray();
            }
            if (dataType == typeof(int))
            {
                arrayEdit1.Value = (Value.ArrayEdit1Value == null || (Value.ArrayEdit1Value.Count() == 1 && Value.ArrayEdit1Value[0] == "")) ? null : Value.ArrayEdit1Value.IfNotNull(x => x).Select(x => int.Parse(x.Replace(".", ","))).ToArray();
            }
        }

        /// <summary>
        /// Очистка полей ввода
        /// </summary>
        public override void ClearExternalValues()
        {
            decimalEdit1.Value = null;
            decimalEdit2.Value = null;
            decimalEdit3.Value = null;
            arrayEdit1.Value = null;
            checkBoxNull.CheckState = CheckState.Indeterminate;
        }

        /// <summary>
        /// Очистка сохраненных значений
        /// </summary>
        public override void ClearInternalValues()
        {
            Value.BCheckBoxNull = null;
            Value.SelectedTab = 0;
            Value.ComboBox1SelectedIndex = 0;
            Value.ComboBox2SelectedIndex = 0;
            Value.ComboBox3SelectedIndex = 0;
            Value.ComboBox4SelectedIndex = 0;
            Value.DecimalEdit1Value = null;
            Value.DecimalEdit2Value = null;
            Value.DecimalEdit3Value = null;
            Value.ArrayEdit1Value = null;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
                decimalEdit1.Focus();
            if (tabControl1.SelectedIndex == 1)
                decimalEdit2.Focus();
            if (tabControl1.SelectedIndex == 2)
                arrayEdit1.Focus();
        }

        private void QueryGridNumberFilter_Enter(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
                decimalEdit1.Focus();
            if (tabControl1.SelectedIndex == 1)
                decimalEdit2.Focus();
            if (tabControl1.SelectedIndex == 2)
                arrayEdit1.Focus();
        }

        private void checkBoxNull_CheckStateChanged(object sender, EventArgs e)
        {
            decimalEdit1.Enabled = checkBoxNull.CheckState == CheckState.Indeterminate;
            decimalEdit2.Enabled = checkBoxNull.CheckState == CheckState.Indeterminate;
            decimalEdit3.Enabled = checkBoxNull.CheckState == CheckState.Indeterminate;
            arrayEdit1.Enabled = checkBoxNull.CheckState == CheckState.Indeterminate;
        }
    }

    /// <summary>
    /// Значение фильтра чисел
    /// </summary>
    public class NumberFilterValue : FilterValue
    {
        /// <summary>
        /// Ключи аттрибутов в Xml описании, соответствующие данным фильтра.
        /// </summary>
        private const string FILTER_DATATYPE_ATTR = "numberFilterDataType";
        private const string FILTER_SELECTEDTAB_ATTR = "numberFilterSelectedTab";
        private const string FILTER_SELECTED1_ATTR = "numberFilterBox1SelectedIndex";
        private const string FILTER_SELECTED2_ATTR = "numberFilterBox2SelectedIndex";
        private const string FILTER_SELECTED3_ATTR = "numberFilterBox3SelectedIndex";
        private const string FILTER_EDIT1_ATTR = "numberFilterEdit1Value";
        private const string FILTER_EDIT2_ATTR = "numberFilterEdit2Value";
        private const string FILTER_EDIT3_ATTR = "numberFilterEdit3Value";
        private const string FILTER_NULLS_ATTR = "numberFilterBoxNull";


        private Type dataType;
        private int selectedTab;
        private int comboBox1SelectedIndex;
        private int comboBox2SelectedIndex;
        private int comboBox3SelectedIndex;
        private int comboBox4SelectedIndex;

        private string decimalEdit1Value;
        private string decimalEdit2Value;
        private string decimalEdit3Value;
        private string[] arrayEdit1Value;

        private bool? bCheckBoxNull;

        /// <summary>
        /// Состояние чекбокса пустого значения
        /// </summary>
        public bool? BCheckBoxNull
        {
            get { return bCheckBoxNull; }
            set { bCheckBoxNull = value; }
        }

        /// <summary>
        /// Тип данных
        /// </summary>
        public Type DataType
        {
            get { return dataType; }
            set { dataType = value; }
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
        /// значение выпадающего списка 1
        /// </summary>
        public int ComboBox1SelectedIndex
        {
            get { return comboBox1SelectedIndex; }
            set { comboBox1SelectedIndex = value; }
        }

        /// <summary>
        /// значение выпадающего списка 2
        /// </summary>
        public int ComboBox2SelectedIndex
        {
            get { return comboBox2SelectedIndex; }
            set { comboBox2SelectedIndex = value; }
        }

        /// <summary>
        /// значение выпадающего списка 3
        /// </summary>
        public int ComboBox3SelectedIndex
        {
            get { return comboBox3SelectedIndex; }
            set { comboBox3SelectedIndex = value; }
        }

        /// <summary>
        /// значение выпадающего списка 3
        /// </summary>
        public int ComboBox4SelectedIndex
        {
            get { return comboBox4SelectedIndex; }
            set { comboBox4SelectedIndex = value; }
        }

        /// <summary>
        /// Значение редактора 1
        /// </summary>
        public string DecimalEdit1Value
        {
            get { return decimalEdit1Value; }
            set { decimalEdit1Value = string.IsNullOrEmpty(value) ? null : value; }
        }

        /// <summary>
        /// Значение редактора 2
        /// </summary>
        public string DecimalEdit2Value
        {
            get { return decimalEdit2Value; }
            set { decimalEdit2Value = string.IsNullOrEmpty(value) ? null : value; }
        }

        /// <summary>
        /// Значение редактора 3
        /// </summary>
        public string DecimalEdit3Value
        {
            get { return decimalEdit3Value; }
            set { decimalEdit3Value = string.IsNullOrEmpty(value) ? null : value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] ArrayEdit1Value
        {
            get { return arrayEdit1Value; }
            set { arrayEdit1Value = value; }
        }


        /// <summary>
        /// Конструктор
        /// </summary>
        public NumberFilterValue()
        {
        }
    }
}
