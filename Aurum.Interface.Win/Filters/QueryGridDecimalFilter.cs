using System;
using System.Collections.Generic;
using System.Text;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// Фильтр дробного числа
    /// </summary>
    public class QueryGridDecimalFilter : QueryGridNumberFilter<decimal>
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public QueryGridDecimalFilter()
            : base(typeof(decimal), 2)
        {
        }

        /// <summary>
        /// Преобразование текста в дробное
        /// </summary>
        /// <param name="s"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        protected override bool TryParse(string s, out decimal o)
        {
            return decimal.TryParse(s, out o);
        }

        /// <summary>
        /// Создание глубокой копии объекта фильтра.
        /// </summary>
        /// <returns> Глубокая копия объекта </returns>
        public override QueryGridFilterBase Clone()
        {
            if (this.GetType() != typeof(QueryGridDecimalFilter))
                throw new NotImplementedException("Не определен метод Clone в классе " + this.GetType().FullName);
            QueryGridDecimalFilter clone = new QueryGridDecimalFilter();
            InitClone(clone);
            return clone;
        }
    }
}
