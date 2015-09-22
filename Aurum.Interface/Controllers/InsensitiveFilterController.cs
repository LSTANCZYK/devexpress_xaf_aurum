using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;

namespace Aurum.Interface.Controllers
{
    /// <summary>
    /// Контроллер фильтра, нечувствительного к регистру
    /// </summary>
    public class InsensitiveFilterController : FilterController
    {
        /// <inheritdoc/>
        protected override ISearchCriteriaBuilder CreateSearchCriteriaBuilder()
        {
            if (this.View != null)
            {
                if (this.View.Model.DataAccessMode == DevExpress.ExpressApp.CollectionSourceDataAccessMode.Client
                    || this.Application.Model.Options.DataAccessMode == DevExpress.ExpressApp.CollectionSourceDataAccessMode.Client)
                {
                    return new InsensitiveSearchCriteriaBuilder();
                }
            }
            return base.CreateSearchCriteriaBuilder();
        }
    }

    /// <summary>
    /// Конструктор условия поиска, нечувствительного к регистру
    /// </summary>
    public class InsensitiveSearchCriteriaBuilder : SearchCriteriaBuilder
    {
        /// <inheritdoc/>
        protected override CriteriaOperator CreateCriteriaOperator(IMemberInfo memberInfo, string valueToSearch)
        {
            Guard.ArgumentNotNull(memberInfo, "memberInfo");
            if (string.IsNullOrEmpty(valueToSearch))
            {
                return null;
            }
            string trimmedValueToSearch = valueToSearch.Trim();
            if (!string.IsNullOrEmpty(trimmedValueToSearch))
            {
                if (memberInfo.MemberType == typeof(string))
                {
                    return new FunctionOperator(FunctionOperatorType.Contains,
                        new FunctionOperator(FunctionOperatorType.Upper, new OperandProperty(memberInfo.Name)),
                        new FunctionOperator(FunctionOperatorType.Upper, new OperandValue(trimmedValueToSearch)));
                }
                else
                {
                    object convertedValue = null;
                    if (TryConvertStringTo(memberInfo.MemberType, trimmedValueToSearch, out convertedValue))
                    {
                        return new BinaryOperator(memberInfo.Name, convertedValue, BinaryOperatorType.Equal);
                    }
                }
                return null;
            }
            return null;
        }
    }
}
