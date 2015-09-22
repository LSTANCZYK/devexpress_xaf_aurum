using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// Форматы отображения данных
    /// </summary>
    public static class Formats
    {
        // Формат отображения дробных чисел
        private static NumberFormatInfo numberFormat = null;

        /// <summary>
        /// Формат отображения дробных чисел
        /// </summary>
        public static NumberFormatInfo NumberFormat
        {
            get
            {
                if (numberFormat == null)
                {
                    numberFormat = new NumberFormatInfo();
                    numberFormat.NumberDecimalSeparator = ".";
                    numberFormat.NumberGroupSeparator = " ";
                }
                return numberFormat;
            }
        }

        /// <summary>
        /// Формат отображения денег
        /// </summary>
        public static string MoneyFormat = "0.00";

        /// <summary>
        /// Название денежной единицы
        /// </summary>
        public static string Currency = "руб.";

        /// <summary>
        /// Формат представления даты (dd.MM.yyyy)
        /// </summary>
        public const string DateFormat = "dd.MM.yyyy";

        /// <summary>
        /// Формат представления даты (MM.yyyy)
        /// </summary>
        public const string DateFormatMonth = "MM.yyyy";

        /// <summary>
        /// Формат представления даты и времени (dd.MM.yyyy HH:mm)
        /// </summary>
        public const string DateTimeFormat = "dd.MM.yyyy HH:mm";

        /// <summary>
        /// Формат представления даты и времени (до секунд) (dd.MM.yyyy HH:mm:ss)
        /// </summary>
        public const string DateTimeFormat2 = "dd.MM.yyyy HH:mm:ss";
    }
}
