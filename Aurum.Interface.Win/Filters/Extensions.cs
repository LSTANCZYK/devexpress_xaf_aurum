using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// Методы расширения
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Метод расширения для форматирования строки.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatMe(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>
        /// Метод расширения для словаря. Можно получить значение по ключу, 
        /// либо значение по-умолчанию, если ключ отсутствует
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TValue GetValueOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue defaultValue = default(TValue))
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }


        static GregorianCalendar _gc = new GregorianCalendar();

        /// <summary>
        /// Номер недели в месяце.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetWeekOfMonth(this DateTime time)
        {
            DateTime first = new DateTime(time.Year, time.Month, 1);
            return time.GetWeekOfYear() - first.GetWeekOfYear() + 1;
        }

        static int GetWeekOfYear(this DateTime time)
        {
            return _gc.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }

        /// <summary>
        /// Количество месяцев между двумя датами
        /// </summary>
        /// <param name="later">Дата позже</param>
        /// <param name="earlier">Дата раньше</param>
        /// <returns>Целое количество месяцев независимо от дней в датах</returns>
        public static int MonthsBetween(this DateTime later, DateTime earlier)
        {
            return (later.Year - earlier.Year) * 12 + (later.Month - earlier.Month);
        }

        /// <summary>
        /// Начало месяца
        /// </summary>
        /// <param name="date">Дата</param>
        /// <returns>Первый день месяца указанной даты</returns>
        public static DateTime MonthStart(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// Конец месяца
        /// </summary>
        /// <param name="date">Дата</param>
        /// <returns>Первый день месяца указанной даты</returns>
        public static DateTime MonthEnd(this DateTime date)
        {
            return date.MonthStart().AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// Усечь дату до определенной единицы измерения
        /// </summary>
        /// <param name="date">Дата</param>
        /// <param name="truncTo">Тип единицы, до которой нужно усечь дату</param>
        /// <returns>Усеченная дата</returns>
        public static DateTime Trunc(this DateTime date, DateTimeTruncationTypes truncTo)
        {
            switch (truncTo)
            {
                case DateTimeTruncationTypes.Day:
                    return new DateTime(date.Year, date.Month, date.Day);
                case DateTimeTruncationTypes.Month:
                    return new DateTime(date.Year, date.Month, 1);
                default:
                    return new DateTime(date.Year, 1, 1);
            }
        }

        /// <summary>
        /// Проверка на null
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }



        /// <summary>
        /// Безопасный вызов выражений над объектом
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="f0"></param>
        public static void IfNotNull<T>(this T t, Action<T> f0)
        {
            if (t != null)
            {
                f0(t);
            }
        }

        /// <summary>
        /// Безопасный вызов выражений над объектом
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="t"></param>
        /// <param name="f0"></param>
        /// <returns></returns>
        public static R IfNotNull<T, R>(this T t, Func<T, R> f0)
        {
            return t.IfNotNull(f0, x => x, x => x, x => x);
        }

        /// <summary>
        /// Безопасный вызов выражений над объектом
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="t"></param>
        /// <param name="f0"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static R IfNotNull<T, R>(this T t, Func<T, R> f0, R def)
        {
            return IfNotNull(t, f0, x => x, x => x, x => x, def);
        }

        /// <summary>
        /// Безопасный вызов выражений над объектом
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="t"></param>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public static S IfNotNull<T, R, S>(this T t, Func<T, R> f0, Func<R, S> f1)
        {
            return t.IfNotNull(f0, f1, x => x, x => x);
        }

        /// <summary>
        /// Безопасный вызов выражений над объектом
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="t"></param>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static S IfNotNull<T, R, S>(this T t, Func<T, R> f0, Func<R, S> f1, S def)
        {
            return IfNotNull(t, f0, f1, x => x, x => x, def);
        }

        /// <summary>
        /// Безопасный вызов выражений над объектом
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="t"></param>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static U IfNotNull<T, R, S, U>(this T t, Func<T, R> f0, Func<R, S> f1, Func<S, U> f2)
        {
            return IfNotNull(t, f0, f1, f2, x => x);
        }

        /// <summary>
        /// Безопасный вызов выражений над объектом
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="t"></param>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static U IfNotNull<T, R, S, U>(this T t, Func<T, R> f0, Func<R, S> f1, Func<S, U> f2, U def)
        {
            return IfNotNull(t, f0, f1, f2, x => x, def);
        }

        /// <summary>
        /// Безопасный вызов выражений над объектом
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="t"></param>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="f3"></param>
        /// <returns></returns>
        public static V IfNotNull<T, R, S, U, V>(this T t, Func<T, R> f0, Func<R, S> f1, Func<S, U> f2, Func<U, V> f3)
        {
            return IfNotNull(t, f0, f1, f2, f3, default(V));
        }

        /// <summary>
        /// Безопасный вызов выражений над объектом
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="t"></param>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="f3"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static V IfNotNull<T, R, S, U, V>(this T t, Func<T, R> f0, Func<R, S> f1, Func<S, U> f2, Func<U, V> f3, V def)
        {
            if (t == null)
            {
                return def;
            }

            var r0 = f0(t);
            if (r0 == null)
            {
                return def;
            }

            var r1 = f1(r0);
            if (r1 == null)
            {
                return def;
            }

            var r2 = f2(r1);
            if (r2 == null)
            {
                return def;
            }

            return f3(r2);
        }

        /// <summary>
        /// Мемоизация
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Func<T, R> Memoize<T, R>(this Func<T, R> f)
        {
            var cache = new Dictionary<T, R>();
            return x =>
            {
                R v;
                if (cache.TryGetValue(x, out v))
                    return v;
                else
                {
                    v = f(x);
                    cache.Add(x, v);
                    return v;
                }
            };
        }

        /// <summary>
        /// Безопасная работа с контролом
        /// </summary>
        /// <param name="control">Контрол</param>
        /// <param name="action">Действие</param>
        public static void InvokeIfRequired<T>(this T control, Action<T> action) where T : Control
        {
            if (control == null)
                throw new ArgumentNullException("control", "control is null.");
            if (action == null)
                throw new ArgumentNullException("action", "action is null.");
            if (control.InvokeRequired)
            {
                control.Invoke(action, control);
            }
            else
            {
                action.DynamicInvoke(control);
            }
        }

        /// <summary>
        /// Объединение множеств с включением всех элементов
        /// </summary>
        /// <typeparam name="TSource">Тип элементов</typeparam>
        /// <param name="first">Первое множество</param>
        /// <param name="second">Второе множество</param>
        /// <returns>Результирующее множество</returns>
        public static IEnumerable<TSource> UnionAll<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            List<TSource> result = new List<TSource>();
            if (first != null) result.AddRange(first);
            if (second != null) result.AddRange(second);
            return result;
        }
    }

    /// <summary>
    /// Типы усечения дат
    /// </summary>
    public enum DateTimeTruncationTypes
    {
        /// <summary>
        /// Усечение до дня
        /// </summary>
        Day,

        /// <summary>
        /// Усечение до месяца
        /// </summary>
        /// 
        Month,
        /// <summary>
        /// Усечение до года
        /// </summary>
        Year
    }
}
