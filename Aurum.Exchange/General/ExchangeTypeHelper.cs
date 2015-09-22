using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Exchange
{
    /// <summary>
    /// Вспомогательные методы поиска типов обмена
    /// </summary>
    internal static class ExchangeTypeHelper
    {
        private class TypeEqualityComparer : IEqualityComparer<Type>
        {
            public bool Equals(Type x, Type y)
            {
                return y.FullName == x.FullName;
            }

            public int GetHashCode(Type obj)
            {
                return obj.GetHashCode();
            }
        }

        /// <summary>
        /// Безопасно загрузить типы из сборки
        /// </summary>
        /// <param name="assembly">Сборка</param>
        /// <returns>Коллекция типов, загруженных из сборки. Если загрузка типов не удалась, возвращается пустая коллекция</returns>
        /// <remarks>При обработке редакторами (например Редактор модели XAF) некоторые сборки могут давать исключение TypeLoadException из-за неполной загрузки</remarks>
        private static IEnumerable<Type> TryLoadTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            } catch { }
            return new Type[0];
        }

        /// <summary>
        /// Отфильтровать случайные дубли
        /// </summary>
        /// <param name="types">Коллекция типов</param>
        /// <remarks>При работе из дизайнера студия иногда подрубает сборки по >= 2 раза</remarks>
        private static IEnumerable<Type> FilterOccasionalDoubles(IEnumerable<Type> types)
        {
            return types.Distinct(new TypeEqualityComparer());
        }

        /// <summary>
        /// Получить все типы из всех сборок
        /// </summary>
        /// <returns>Коллекция всех типов из всех сборок</returns>
        private static IEnumerable<Type> GetAllTypes()
        {
            return FilterOccasionalDoubles(
                AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(t => TryLoadTypes(t)));
        }

        /// <summary>
        /// Получить из коллекции типов только подходящие классы обмена
        /// </summary>
        /// <param name="types">Коллекция типов</param>
        private static IEnumerable<Type> FilterExchanges(IEnumerable<Type> types)
        {
            return types
                .Where(t =>
                    t.IsPublic &&
                    !t.IsAbstract &&
                    t.IsClass &&
                    typeof(ExchangeOperation).IsAssignableFrom(t));
        }

        /// <summary>
        /// Получить все классы обмена
        /// </summary>
        /// <returns>Коллекция типов классов обмена</returns>
        public static IEnumerable<Type> FindAllExchanges()
        {
            return FilterExchanges(GetAllTypes());
        }
    }
}
