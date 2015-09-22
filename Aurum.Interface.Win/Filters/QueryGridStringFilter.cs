using System;
using System.Xml;
using System.Xml.XPath;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// Фильтр текстового значения
    /// </summary>
    public partial class QueryGridStringFilter : QueryGridStringFilterBase
    {
        private const string SEARCH_EXACT = "stringSearchExact";
        private const string SEARCH_NULLS = "stringSearchNulls";
        private const string SEARCH_SENSITIVE = "stringSearchSensitive";
        private const string SEARCH_TEXT = "stringSearchText";

        /// <summary>
        /// Конструктор
        /// </summary>
        public QueryGridStringFilter()
        {
            InitializeComponent();
            InternalValuesToExternalValues();
        }

        /// <summary>
        /// Редактируемый текст
        /// </summary>
        protected override string EditText
        {
            get { return Convert.ToString(textBoxEdit.Value); }
            set { textBoxEdit.Text = value; }
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
            if (this.GetType() != typeof(QueryGridStringFilter))
                throw new NotImplementedException("Не определен метод Clone в классе " + this.GetType().FullName);
            QueryGridStringFilter clone = new QueryGridStringFilter();
            InitClone(clone);
            return clone;
        }

        /// <summary>
        /// Событие установки поиска пустых значений
        /// </summary>
        /// <param name="value">Поиск пустых значений</param>
        protected override void OnSetNulls(bool? value)
        {
            textBoxEdit.Enabled = !value.HasValue;
        }

        // Фокус на поле ввода текста
        private void QueryGridStringFilter_Enter(object sender, EventArgs e)
        {
            textBoxEdit.Focus();
        }

        private void QueryGridStringFilter_Resize(object sender, EventArgs e)
        {
            const int textBoxMargin = 3;
            const int buttonMargin = 3;
            const int space = 1;
            int w = this.Size.Width - (textBoxMargin + buttonExact.Size.Width + buttonMargin + buttonCase.Size.Width + buttonMargin + buttonNull.Size.Width + space);
            textBoxEdit.Size = new System.Drawing.Size(w, textBoxEdit.Size.Height);
        }
    }
}