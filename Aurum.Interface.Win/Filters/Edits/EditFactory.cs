using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// Фабрика редакторов данных
    /// </summary>
    public static class EditFactory
    {
        /// <summary>
        /// Создание редактора
        /// </summary>
        /// <param name="dataType">Тип данных</param>
        /// <param name="parameter">Параметр, для которого создается редактор</param>
        /// <returns>Редактор</returns>
        public static Control CreateEdit(Type dataType)
        {
            IMethodParameter editor = null;
            // Стандартный редактор
            if (dataType == typeof(DateTime))
            {
                editor = new DateTimeEdit();
            }
            if (dataType == typeof(decimal))
            {
                editor = new DecimalEdit();
            }
            if (dataType == typeof(int))
            {
                editor = new IntegerEdit();
            }
            return (Control)editor;
        }
    }
}
