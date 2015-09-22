using DevExpress.Data.Filtering;
using System;
using System.ComponentModel;
using System.Drawing;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// Базовый класс фильтра текстового значения
    /// </summary>
    public partial class QueryGridStringFilterBase : QueryGridFilterBase
    {
        // Точный поиск по умолчанию
        private static StringFilterExactSearch? defaultExactSearch = null;

        private bool? nulls = null;
        private bool caseSensitive = false;
        private StringFilterExactSearch exactSearch = StringFilterExactSearch.Free;

        /// <summary>
        /// Инициализация данных в клонируемом объекте данными текущего объекта.
        /// Наследники, имеющие свои член-данные, должны перегрузить метод и инициализировать эти данные в этом методе.
        /// </summary>
        /// <param name="clone"> Объект для инициализации </param>
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
        /// Создание глубокой копии объекта фильтра. 
        /// Каждый наследник обязан перегрузить метод, 
        /// в котором нужно создать экземпляр собственного класса фильтра 
        /// с глубокой копией всех данных фильтра.
        /// </summary>
        /// <returns> Глубокая копия объекта </returns>
        public override QueryGridFilterBase Clone()
        {
            if (this.GetType() != typeof(QueryGridStringFilterBase))
                throw new NotImplementedException("Не определен метод Clone в классе " + this.GetType().FullName);
            QueryGridStringFilterBase clone = new QueryGridStringFilterBase();
            InitClone(clone);
            return clone;
        }

        /// <summary>
        /// Точный поиск по умолчанию
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
        /// Редактируемый текст
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual string EditText
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// Поиск пустого значения
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
                    !nulls.HasValue ? "Поиск пустых/непустых значений выключен" : (
                    nulls.Value ? "Поиск пустых значений" : "Поиск непустых значений"));
                OnSetNulls(value);
            }
        }

        /// <summary>
        /// Поиск с учетом регистра
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
                    caseSensitive ? "Поиск с учетом регистра" : "Поиск без учета регистра");
            }
        }

        /// <summary>
        /// Точный поиск
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
                    exactSearch == StringFilterExactSearch.Exact ? "Точный поиск" : 
                    exactSearch == StringFilterExactSearch.Free ? "Поиск в любом месте строки" :
                                                                  "Поиск с начала строки");
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public QueryGridStringFilterBase()
        {
            InitializeComponent();
            buttonCase.Image = Resources.TextSmallCaps;
            value = new StringFilterValue();
            Value.ExactSearch = DefaultExactSearch;
        }

        /// <summary>
        /// Значение
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new StringFilterValue Value
        {
            get { return (StringFilterValue)base.Value; }
            set { base.Value = value; }
        }

        /// <summary>
        /// Описание установленного фильтра
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
        /// Получить выражение фильтра
        /// </summary>
        /// <param name="fieldRef">Ссылка на поле запроса</param>
        /// <returns>Выражение фильтра</returns>
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
        /// Парсинг строки фильтра
        /// </summary>
        /// <param name="s">Оригинальная строка</param>
        /// <returns>Строка для фильтра</returns>
        protected string ParseFilterText(string s)
        {
            // Пустой фильтр
            if (string.IsNullOrEmpty(s)) return s;

            s = s.Replace("*", "%");

            // Если строка не содержит спец. символы
            if(s.IndexOf("%") < 0 && s.IndexOf("_") < 0)
            {
                if (s.Length > 2 && s.StartsWith("\"") && s.EndsWith("\"")) // Текст в кавычках - полное соответствие
                {
                    return s.Substring(1, s.Length - 2);
                }

                // Поиск подпоследовательности
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
        /// Получить текст фильтра
        /// </summary>
        /// <returns>Текст фильтра</returns>
        protected virtual string GetFilterText()
        {
            return ParseFilterText(Value.Text1);
        }

        /// <summary>
        /// Запоминание введенных значений во внутренних переменных
        /// </summary>
        public override void ExternalValuesToInternalValues()
        {
            Value.Text1 = EditText;
            Value.Nulls = Nulls;
            Value.ExactSearch = ExactSearch;
            Value.CaseSensitive = CaseSensitive;
        }

        /// <summary>
        /// Сброс полей ввода и восстановление сохраненных значений
        /// </summary>
        public override void InternalValuesToExternalValues()
        {
            EditText = Value.Text1;
            Nulls = Value.Nulls;
            ExactSearch = Value.ExactSearch;
            CaseSensitive = Value.CaseSensitive;
        }

        /// <summary>
        /// Очистка полей ввода
        /// </summary>
        public override void ClearExternalValues()
        {
            EditText = string.Empty;
            Nulls = null;
            ExactSearch = DefaultExactSearch;
            CaseSensitive = false;
        }

        /// <summary>
        /// Очистка сохраненных значений
        /// </summary>
        public override void ClearInternalValues()
        {
            Value.Text1 = null;
            Value.Nulls = null;
            Value.CaseSensitive = false;
            Value.ExactSearch = StringFilterExactSearch.Free;
        }

        // Изменение опции точного поиска
        private void OnExactClick(object sender, EventArgs e)
        {
            switch (ExactSearch)
            {
                case StringFilterExactSearch.Free: ExactSearch = StringFilterExactSearch.Exact; break;
                case StringFilterExactSearch.Exact: ExactSearch = StringFilterExactSearch.Begin; break;
                case StringFilterExactSearch.Begin: ExactSearch = StringFilterExactSearch.Free; break;
            }
        }

        // Изменение опции поиска с учетом регистра
        private void OnCaseClick(object sender, EventArgs e)
        {
            CaseSensitive = !CaseSensitive;
        }

        // Изменение опции поиска пустых значений
        private void OnNullsClick(object sender, EventArgs e)
        {
            Nulls = !Nulls.HasValue ? true : (Nulls.Value ? false : (bool?)null);
        }

        /// <summary>
        /// Событие установки поиска пустых значений
        /// </summary>
        /// <param name="value">Поиск пустых значений</param>
        protected virtual void OnSetNulls(bool? value)
        {
        }
    }

    /// <summary>
    /// Значение строкового фильтра
    /// </summary>
    public class StringFilterValue : FilterValue
    {
        private string text1;
        private bool? nulls;
        private bool caseSensitive;
        private StringFilterExactSearch exactSearch;

        /// <summary>
        /// Конструктор
        /// </summary>
        public StringFilterValue()
        {
            text1 = null;
            caseSensitive = false;
            nulls = null;
            exactSearch = StringFilterExactSearch.Free;
        }

        /// <summary>
        /// Текст
        /// </summary>
        public string Text1
        {
            get { return text1; }
            set { text1 = value; }
        }

        /// <summary>
        /// Поиск пустых значений
        /// </summary>
        public bool? Nulls
        {
            get { return nulls; }
            set { nulls = value; }
        }

        /// <summary>
        /// Поиск с учетом регистра
        /// </summary>
        public bool CaseSensitive
        {
            get { return caseSensitive; }
            set { caseSensitive = value; }
        }

        /// <summary>
        /// Поиск точного значения
        /// </summary>
        public StringFilterExactSearch ExactSearch
        {
            get { return exactSearch; }
            set { exactSearch = value; }
        }
    }

    /// <summary>
    /// Типы поиска точного значения в строковом фильтре
    /// </summary>
    public enum StringFilterExactSearch
    {
        /// <summary>
        /// Точный поиск
        /// </summary>
        Exact,

        /// <summary>
        /// Произвольный поиск
        /// </summary>
        Free,

        /// <summary>
        /// Поиск по началу строки
        /// </summary>
        Begin
    }
}