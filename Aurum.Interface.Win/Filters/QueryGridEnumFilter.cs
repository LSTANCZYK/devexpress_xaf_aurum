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
    /// Фильтр перечислений
    /// </summary>
    public partial class QueryGridEnumFilter : QueryGridFilterBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public QueryGridEnumFilter()
        {
            InitializeComponent();
            value = new EnumFilterValue();
            dataTypeChanged += new EventHandler(QueryGridEnumColumnFilter_dataTypeChanged);
            InternalValuesToExternalValues();
        }

        /// <summary>
        /// Значение
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new EnumFilterValue Value
        {
            get { return (EnumFilterValue)base.Value; }
            set { base.Value = value; }
        }

        /// <summary>
        /// Тип данных изменен
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
        /// Заполнение списка перечислением
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

            // Установка значений списка
            checkedListBox1.Items.Clear();
            
            // Значения из описания
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
        /// Создание глубокой копии объекта фильтра.
        /// </summary>
        /// <returns> Глубокая копия объекта </returns>
        public override QueryGridFilterBase Clone()
        {
            if (this.GetType() != typeof(QueryGridEnumFilter))
                throw new NotImplementedException("Не определен метод Clone в классе " + this.GetType().FullName);
            QueryGridEnumFilter clone = new QueryGridEnumFilter();
            InitClone(clone);
            return clone;
        }

        /// <summary>
        /// Описание фильтра
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
                    // Значения из описания
                    foreach (var enumValue in Enum.GetValues(Type))
                    {
                        if (Value.BCheckedListBox[i])
                        {
                            if (description != null)
                            {
                                description = "<Множественный выбор>";
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

                // Определение на флаговый атрибут
                Type enumType = type.UnderlyingSystemType;
                Object[] attrs = enumType.GetCustomAttributes(typeof(FlagsAttribute), false);
                if (attrs.Length > 0)
                {
                    flags = true;
                }

                // Используется ли значение "никакой" в фильтре флага
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
                    // Для значения "никакой" используется обычное условие равенства фильтруемого поля нулю
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
        /// Запоминание введенных значений во внутренних переменных
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
        /// Сброс полей ввода и восстановление сохраненных значений
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
        /// Очистка полей ввода
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
        /// Очистка сохраненных значений
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
    /// Значение фильтра перечислений
    /// </summary>
    public class EnumFilterValue : FilterValue
    {
        private const string FILTER_NULLS_ATTR = "enumFilterBoxNull";
        private const string FILTER_CHECKED_LISTBOX = "enumFilterCheckedListBox";
        // Нулевое значение
        private bool? bCheckBoxNull;
        // Массив выбранных значений
        private bool[] bCheckedListBox;

        /// <summary>
        /// Конструктор
        /// </summary>
        public EnumFilterValue()
        {
            bCheckBoxNull = null;
            bCheckedListBox = null;
        }

        /// <summary>
        /// Создание глубокой копии фильтра.
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
        /// Нулевое значение
        /// </summary>
        public bool? BCheckBoxNull
        {
            get { return bCheckBoxNull; }
            set { bCheckBoxNull = value; }
        }

        /// <summary>
        /// Массив выбранных значений
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
