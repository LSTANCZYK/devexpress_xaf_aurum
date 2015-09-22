using System;
using System.Collections.Generic;
using System.Text;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// ������ �������� �����
    /// </summary>
    public class QueryGridDecimalFilter : QueryGridNumberFilter<decimal>
    {
        /// <summary>
        /// �����������
        /// </summary>
        public QueryGridDecimalFilter()
            : base(typeof(decimal), 2)
        {
        }

        /// <summary>
        /// �������������� ������ � �������
        /// </summary>
        /// <param name="s"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        protected override bool TryParse(string s, out decimal o)
        {
            return decimal.TryParse(s, out o);
        }

        /// <summary>
        /// �������� �������� ����� ������� �������.
        /// </summary>
        /// <returns> �������� ����� ������� </returns>
        public override QueryGridFilterBase Clone()
        {
            if (this.GetType() != typeof(QueryGridDecimalFilter))
                throw new NotImplementedException("�� ��������� ����� Clone � ������ " + this.GetType().FullName);
            QueryGridDecimalFilter clone = new QueryGridDecimalFilter();
            InitClone(clone);
            return clone;
        }
    }
}
