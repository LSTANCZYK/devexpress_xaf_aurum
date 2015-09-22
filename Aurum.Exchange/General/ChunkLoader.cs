using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Exchange
{
    /// <summary>
    /// Загрузчик объектов из БД. Загружает частями в указанных количествах.
    /// От данного класса нельзя отнаследоваться
    /// </summary>
    /// <typeparam name="T">Тип хранимого объекта</typeparam>
    /// <remarks>Данный класс, к сожалению, (пока) не предоставляет средства для решения проблемы SELECT N+1</remarks>
    public sealed class ChunkLoader<T> : IEnumerable<T>, IDisposable
    {
        private XafApplication application;
        private SortingCollection sorting;
        private CriteriaOperator criteria;
        private int fixedSize;
        private bool enableGC;
        private string[] prefetch;

        private Session lastSession;
        private IObjectSpace lastObjectSpace;
        private ICollection lastSource;

        private ICollection Repopulate(Session session, int skip)
        {
            if (fixedSize == 0 && skip > 0)
                return new List<T>();
            return session.GetObjects(session.GetClassInfo<T>(), criteria, sorting, skip, fixedSize, false, true);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            int count = 0;

            while (true)
            {
                var objectSpace = application.CreateObjectSpace();
                var session = (objectSpace as XPObjectSpace).Session;
                var source = Repopulate(session, count);
                
                // prefetch
                if (prefetch != null)
                {
                    session.PreFetch(session.GetClassInfo<T>(), source, prefetch);
                }
                
                bool ret = false;
                foreach (var item in source.Cast<T>())
                {
                    ret = true;
                    ++count;
                    yield return item;
                }

                if (source.Count > 0)
                {
                    if (lastSource != null)
                    {
                        lastSource = null;
                    }
                    if (lastSession != null)
                    {
                        lastSession.Dispose();
                        lastSession = null;
                    }
                    if (lastObjectSpace != null)
                    {
                        lastObjectSpace.Dispose();
                        lastObjectSpace = null;
                    }
                    if (enableGC)
                    {
                        GC.Collect();
                    }

                    lastSource = source;
                    lastSession = session;
                    lastObjectSpace = objectSpace;
                }

                if (!ret)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Создать загрузчик
        /// </summary>
        /// <param name="application">Приложение</param>
        /// <param name="criteria">Критерий</param>
        /// <param name="sorting">Сортировка. Обязательно наличие хоть одного поля</param>
        /// <param name="fixedSize">Количество объектов, загружаемых в один проход</param>
        /// <param name="enableGC">Запускать сборку мусора при новой выборке</param>
        /// <param name="prefetch">Список коллекций для предзагрузки</param>
        public ChunkLoader(XafApplication application, CriteriaOperator criteria, SortingCollection sorting, int fixedSize, bool enableGC, string[] prefetch)
        {
            if (application == null)
            {
                throw new ArgumentNullException("application");
            }
            if (fixedSize < 0)
            {
                throw new ArgumentException("fixed size cannot be less than zero");
            }
            if (sorting == null || sorting.Count == 0)
            {
                throw new ArgumentException("sorting must be defined");
            }

            this.application = application;
            this.fixedSize = fixedSize;
            this.sorting = sorting;
            this.criteria = criteria;
            this.enableGC = enableGC;
            this.prefetch = prefetch;
        }

        public void Dispose()
        {
            if (lastSource != null)
            {
                lastSource = null;
            }
            if (lastSession != null)
            {
                lastSession.Dispose();
                lastSession = null;
            }
            if (lastObjectSpace != null)
            {
                lastObjectSpace.Dispose();
                lastObjectSpace = null;
            }
            if (enableGC)
            {
                GC.Collect();
            }
        }
    }
}
