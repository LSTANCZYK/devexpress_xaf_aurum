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
    /// Фильтр булевого значения
    /// </summary>
    public partial class QueryGridBooleanFilter : QueryGridFilterBase
    {
        private const string BOOL_VALUE = "value";
        /// <summary>
        /// Конструктор
        /// </summary>
        public QueryGridBooleanFilter()
        {
            InitializeComponent();
            value = new BooleanFilterValue();
            comboBox1.SelectedIndex = 0;
        }

        /// <summary>
        /// Значение
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new BooleanFilterValue Value
        {
            get { return (BooleanFilterValue)base.Value; }
            set { base.Value = value; }
        }

        /// <summary>
        /// Описание фильтра
        /// </summary>
        public override string Description
        {
            get
            {
                if (Value.BoolState == BooleanFilterValue.boolStateTrue)
                {
                    return @"""Да""";
                }
                else if (Value.BoolState == BooleanFilterValue.boolStateFalse)
                {
                    return @"""Нет""";
                }
                else if (Value.BoolState == BooleanFilterValue.boolStateUndetermined)
                {
                    return @"""(Не определено)""";
                }
                else
                {
                    Debug.Assert(Value.BoolState == BooleanFilterValue.boolStateNone);
                    return null;
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
        /// Запоминание введенных значений во внутренних переменных
        /// </summary>
        public override void ExternalValuesToInternalValues()
        {
            Debug.Assert(comboBox1.SelectedIndex >= BooleanFilterValue.boolStateNone && comboBox1.SelectedIndex <= BooleanFilterValue.boolStateUndetermined);
            Value.BoolState = comboBox1.SelectedIndex;
        }

        /// <summary>
        /// Сброс полей ввода и восстановление сохраненных значений
        /// </summary>
        public override void InternalValuesToExternalValues()
        {
            comboBox1.SelectedIndex = Value.BoolState;
            Debug.Assert(comboBox1.SelectedIndex >= BooleanFilterValue.boolStateNone && comboBox1.SelectedIndex <= BooleanFilterValue.boolStateUndetermined);
        }

        /// <summary>
        /// Очистка полей ввода
        /// </summary>
        public override void ClearExternalValues()
        {
            comboBox1.SelectedIndex = BooleanFilterValue.boolStateNone;
        }

        /// <summary>
        /// Очистка сохраненных значений
        /// </summary>
        public override void ClearInternalValues()
        {
            Value.BoolState = BooleanFilterValue.boolStateNone;
        }
        
        /// <summary>
        /// Инициализируем поля копии объекта
        /// </summary>
        /// <param name="clone"></param>
        protected override void InitClone(QueryGridFilterBase clone)
        {
            //инициализация полей клона
            if (clone is QueryGridBooleanFilter)
            {
                (clone as QueryGridBooleanFilter).Value = this.Value;
            }
        }
        /// <summary>
        /// Создает копию обьекта
        /// </summary>
        /// <returns></returns>
        public override QueryGridFilterBase Clone()
        {
            if (this.GetType() != typeof(QueryGridBooleanFilter))
                throw new NotImplementedException("Не определен метод Clone в классе " + this.GetType().FullName);
            QueryGridBooleanFilter clone = new QueryGridBooleanFilter();
            base.InitClone(clone);
            InitClone(clone);
            return clone;
        }
        
    }



    /// <summary>
    /// Значение булевого фильтра
    /// </summary>
    public class BooleanFilterValue : FilterValue
    {
        private int boolState;

        /// <summary>
        /// Конструктор
        /// </summary>
        public BooleanFilterValue()
        {
            boolState = boolStateNone;
        }

        /// <summary>
        /// Состояние поиска
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
        /// Ничего не ищем
        /// </summary>
        public static readonly int boolStateNone = 0;

        /// <summary>
        /// Ищем true
        /// </summary>
        public static readonly int boolStateTrue = 1;

        /// <summary>
        /// Ищем false
        /// </summary>
        public static readonly int boolStateFalse = 2;

        /// <summary>
        /// Ищем неопределенное "третье" состояние (null)
        /// </summary>
        public static readonly int boolStateUndetermined = 3;
    }
}
