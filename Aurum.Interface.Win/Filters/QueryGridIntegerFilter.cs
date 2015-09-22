using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;


namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// Фильтр целых чисел
    /// </summary>
    public class QueryGridIntegerFilter : QueryGridNumberFilter<int>
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public QueryGridIntegerFilter()
            : base(typeof(int), 0)
        {
        }

        /// <summary>
        /// Преобразование текста в целое
        /// </summary>
        /// <param name="s"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        protected override bool TryParse(string s, out int o)
        {
            return int.TryParse(s, out o);
        }

        /// <summary>
        /// Создание глубокой копии объекта фильтра.
        /// </summary>
        /// <returns> Глубокая копия объекта </returns>
        public override QueryGridFilterBase Clone()
        {
            if (this.GetType() != typeof(QueryGridIntegerFilter))
                throw new NotImplementedException("Не определен метод Clone в классе " + this.GetType().FullName);
            QueryGridIntegerFilter clone = new QueryGridIntegerFilter();
            InitClone(clone);
            return clone;
        }
    }
}
