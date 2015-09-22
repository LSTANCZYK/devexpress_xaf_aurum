using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;


namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// ������ ����� �����
    /// </summary>
    public class QueryGridIntegerFilter : QueryGridNumberFilter<int>
    {
        /// <summary>
        /// �����������
        /// </summary>
        public QueryGridIntegerFilter()
            : base(typeof(int), 0)
        {
        }

        /// <summary>
        /// �������������� ������ � �����
        /// </summary>
        /// <param name="s"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        protected override bool TryParse(string s, out int o)
        {
            return int.TryParse(s, out o);
        }

        /// <summary>
        /// �������� �������� ����� ������� �������.
        /// </summary>
        /// <returns> �������� ����� ������� </returns>
        public override QueryGridFilterBase Clone()
        {
            if (this.GetType() != typeof(QueryGridIntegerFilter))
                throw new NotImplementedException("�� ��������� ����� Clone � ������ " + this.GetType().FullName);
            QueryGridIntegerFilter clone = new QueryGridIntegerFilter();
            InitClone(clone);
            return clone;
        }
    }
}
