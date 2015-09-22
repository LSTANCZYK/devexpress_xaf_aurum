using Aurum.Interface.Win.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// Фабрика фильров
    /// </summary>
    public class QueryGridFilterFactory
    {
        /// <summary>
        /// Создание компонента фильтра
        /// </summary>
        /// <param name="field">Поле запроса</param>
        /// <param name="dataType">Тип данных</param>
        /// <returns>Фильтр</returns>
        public static QueryGridFilterBase CreateFilter(FilterColumn column)
        {
            Type type = column.Type;
            if (type == typeof(string))
            {
                return new QueryGridStringFilter();
            }
            if (type == typeof(bool) || type == typeof(bool?))
            {
                return new QueryGridBooleanFilter();
            }
            if (type == typeof(int) || type == typeof(int?))
            {
                return new QueryGridIntegerFilter();
            }
            if (type == typeof(decimal) || type == typeof(decimal?))
            {
                return new QueryGridDecimalFilter();
            }
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return new QueryGridDateFilter();
            }
            if (type.IsEnum || (type.IsGenericType && type.GetGenericTypeDefinition().FullName == "System.Nullable`1" && type.GenericTypeArguments[0].IsEnum))
            {
                return new QueryGridEnumFilter();
            }
            
            return null;
        }

        public static FilterValue CreateEmptyValue(FilterColumn column)
        {
            Type type = column.Type;
            if (type == typeof(string))
            {
                return new StringFilterValue();
            }
            if (type == typeof(bool) || type == typeof(bool?))
            {
                return new BooleanFilterValue();
            }
            if (type == typeof(int) || type == typeof(int?))
            {
                return new NumberFilterValue() { DataType = typeof(int) };
            }
            if (type == typeof(decimal) || type == typeof(decimal?))
            {
                return new NumberFilterValue() { DataType = typeof(decimal) };
            }
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return new DateFilterValue();
            }
            if (type.IsEnum || (type.IsGenericType && type.GetGenericTypeDefinition().FullName == "System.Nullable`1" && type.GenericTypeArguments[0].IsEnum))
            {
                var val = new EnumFilterValue();
                Type t = type;
                if (type.IsGenericType && type.GetGenericTypeDefinition().FullName == "System.Nullable`1" && type.GenericTypeArguments[0].IsEnum)
                {
                    t = type.GenericTypeArguments[0];
                }
                val.HandleEnumTypeChanged(t);
                return val;
            }

            return null;
        }
    }
}
