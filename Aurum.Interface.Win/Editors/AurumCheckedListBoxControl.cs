using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Interface.Win.Editors
{
    public class AurumCheckedListBoxControl : CheckedListBoxControl
    {
        /// <summary>
        ///  Условие отображения текстового представления элемента
        /// </summary>
        public string ItemTextCriteriaString { get; set; }
        public CriteriaOperator ItemTextCriteria { get; set; }
        
        /// <summary>
        /// Текстовое представление элемента, удовлетворяющему условию отображения
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override string GetItemText(int index)
        {
            //if (!string.IsNullOrEmpty(ItemTextCriteriaString))
            if(!ReferenceEquals(ItemTextCriteria, null))
            {
                var obj = base.GetItem(index);
                XPBaseObject xpObject = obj as XPBaseObject;
                if (xpObject != null)
                {
                    //var result = xpObject.Evaluate(ItemTextCriteriaString);
                    var result = xpObject.Evaluate(ItemTextCriteria);
                    string resultString = Convert.ToString(result);
                    return resultString;
                }
            }
            return base.GetItemText(index);
        }
    }
}
