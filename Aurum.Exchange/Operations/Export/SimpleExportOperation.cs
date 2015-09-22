using Aurum.Operations;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Aurum.Exchange
{
    /// <summary>
    /// Операция экспорта над типом TDataClass с обычным форматировщиком
    /// </summary>
    /// <typeparam name="TDataClass">Тип, над которым выполняется экспорт</typeparam>
    public abstract class ExportOperation<TDataClass> : SingleExchangeOperation
        where TDataClass : XPBaseObject
    {
        private List<Func<TDataClass, object>> fields;
        private OutputFormatter formatter;

        /// <summary>
        /// Установить или получить текущий форматировщик
        /// </summary>
        /// <exception cref="InvalidOperationException">Изменение форматировщика запрещено</exception>
        /// <exception cref="ArgumentNullException">Форматировщик не указан</exception>
        protected OutputFormatter Formatter
        {
            get { return formatter; }
            set
            {
                if (Locked)
                {
                    throw new InvalidOperationException("Изменять форматировщик запрещено в данный момент");
                }

                if (value == null)
                {
                    throw new ArgumentNullException("Formatter", "Форматировщик не указан");
                }

                formatter = value;
            }
        }

        /// <summary>
        /// Тип, над которым выполняется экспорт. Равен типу TDataClass
        /// </summary>
        protected override Type ObjectType { get { return typeof(TDataClass); } }

        /// <summary>
        /// Добавить экспортируемое поле в конец коллекции
        /// </summary>
        /// <param name="f">Лямбда-выражение значения поля</param>
        /// <returns>Индекс добавленного поля</returns>
        /// <exception cref="InvalidOperationException">Добавление полей запрещено</exception>
        protected int AddField(Func<TDataClass, object> f)
        {
            if (Locked)
            {
                throw new InvalidOperationException("Добавлять поля запрещено в данный момент");
            }
            fields.Add(f);
            return fields.Count - 1;
        }

        /// <summary>
        /// Добавить экспортируемое поле, в виде вычисляемой строки, в конец коллекции
        /// </summary>
        /// <param name="eval">Вычисляемая строка</param>
        /// <returns>Индекс добавленного поля</returns>
        /// <exception cref="ArgumentNullException">Пустая вычисляемая строка</exception>
        /// <exception cref="InvalidOperationException">Изменение полей запрещено</exception>
        protected int AddField(string eval)
        {
            if (Locked)
            {
                throw new InvalidOperationException("Добавлять поля запрещено в данный момент");
            }

            if (String.IsNullOrEmpty(eval))
            {
                throw new ArgumentNullException("eval", "Вычисляемое поле не может быть пустым");
            }

            fields.Add(x => x.Evaluate(eval));
            return fields.Count - 1;
        }

        /// <summary>
        /// Установить экспортируемое поле
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <param name="f">Лямбда-выражение значения поля</param>
        /// <exception cref="ArgumentOutOfRangeException">Неверный индекс</exception>
        /// <exception cref="InvalidOperationException">Изменение полей запрещено</exception>
        protected void SetField(int index, Func<TDataClass, object> f)
        {
            if (Locked)
            {
                throw new InvalidOperationException("Изменять поля запрещено в данный момент");
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "Индекс поля не может быть отрицательным числом");
            }

            fields[index] = f;
        }

        /// <summary>
        /// Удалить экспортируемое поле.
        /// Индекс этого поля теперь будет указывать на пустое поле, которое
        /// не будет экспортировано. Все остальные индексы останутся без изменений
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="ArgumentOutOfRangeException">Неверный индекс</exception>
        /// <exception cref="InvalidOperationException">Изменение полей запрещено</exception>
        protected void RemoveField(int index)
        {
            if (Locked)
            {
                throw new InvalidOperationException("Изменять поля запрещено в данный момент");
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "Индекс поля не может быть отрицательным числом");
            }

            fields[index] = null;
        }

        /// <summary>
        /// Получить лямбда-выражение указанного поля
        /// </summary>
        /// <param name="index">Индекс поля</param>
        /// <exception cref="ArgumentOutOfRangeException">Неверный индекс</exception>
        protected Func<TDataClass, object> GetField(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "Индекс поля не может быть отрицательным числом");
            }

            return fields[index];
        }

        protected virtual void OnAfterExport(IEnumerable<object> data, OperationInterop interop)
        {
        }

        protected virtual CriteriaOperator GetCriteria()
        {
            return null;
        }

        protected virtual SortProperty[] GetSorting()
        {
            return null;
        }

        /// <summary>
        /// Список коллекций объекта для предзагрузки
        /// </summary>
        public string[] Prefetch
        {
            get;
            set;
        }

        /// <summary>
        /// Выполнить операцию
        /// </summary>
        protected sealed override void ExecuteExchange(OperationInterop interop)
        {
            interop.SetStatusText("Инициализация");
            interop.ThrowIfCancellationRequested();

            RaiseBeforeExecuteEvent();

            if (Formatter == null)
            {
                throw new InvalidOperationException("Не установлен форматировщик");
            }

            interop.SetStatusText("Обработка параметров");
            
            interop.SetStatusText("Загрузка объектов");
            interop.ThrowIfCancellationRequested();
            interop.SetProgress(10);
            
            // Загрузка объектов
            IEnumerable<TDataClass> dataList = null;

            var criteria = GetCriteria();
            var sorting = GetSorting();

            if (sorting == null || sorting.Length == 0)
            {
                XPDictionary xpDictionary = DevExpress.ExpressApp.Xpo.XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary;
                var classInfo = xpDictionary.GetClassInfo(typeof(TDataClass));
                sorting = new SortProperty[] { new SortProperty(classInfo.KeyProperty.Name, DevExpress.Xpo.DB.SortingDirection.Ascending) };
            }
            dataList = new ChunkLoader<TDataClass>(Application, criteria, new SortingCollection(sorting), ChunkLoaderRowCount, ChunkLoaderGcCollectEnabled, Prefetch);

            IList<object[]> outDataList = new List<object[]>();
            var actualFieldCount = fields.Count(x => x != null); // Если поля зануленные, то они не считаются

            interop.SetStatusText("Обработка данных");
            interop.ThrowIfCancellationRequested();
            interop.SetProgress(50);

            // Вычисление свойств (акцессоров)
            foreach (var obj in dataList)
            {
                var objectData = new Object[actualFieldCount];
                
                for (int
                    i = 0,
                    actualIndex = 0;// Индекс только по незануленным полям
                    i < fields.Count; ++i)
                {
                    if (fields[i] == null)
                    {
                        continue;
                    }

                    objectData[actualIndex] = fields[i].Invoke(obj);
                    ++actualIndex;
                }

                outDataList.Add(objectData);
            }

            interop.SetStatusText("Выгрузка данных");
            interop.ThrowIfCancellationRequested();
            interop.SetProgress(70);

            // Запуск форматировщика
            Formatter.OnBeforeFormatting();
            Formatter.Format(outDataList, interop);
            Formatter.OnAfterFormatting();

            interop.SetStatusText("Постобработка");
            interop.SetProgress(99);
            RaiseAfterExecuteEvent();
            OnAfterExport(outDataList, interop);
            interop.SetProgress(100);
        }

        /// <summary>
        /// Создать новую операцию экспорта над типом TDataClass
        /// </summary>
        /// <param name="app">Объект приложения</param>
        /// <exception cref="System.ArgumentNullException" />
        public ExportOperation(XafApplication app)
            : base(app)
        {
            fields = new List<Func<TDataClass, object>>();
        }
    }

    /// <summary>
    /// Операция экспорта над типом TDataClass с обычным форматировщиком
    /// </summary>
    public abstract class ExportOperation<TDataClass, TParametersClass> : ExportOperation<TDataClass>,
        IWithParametersType<TParametersClass>
        where TDataClass : XPBaseObject
        where TParametersClass : ExchangeParameters
    {
        /// <summary>
        /// Объект параметров конкретно указанного типа TParametersClass
        /// </summary>
        public new TParametersClass ParametersObject
        {
            get { return (TParametersClass)base.ParametersObject; }
            set { base.ParametersObject = value; }
        }

        /// <summary>
        /// Создать новую операцию экспорта над типом TDataClass и параметрами типа TParametersClass
        /// </summary>
        /// <param name="app">Объект приложения</param>
        /// <exception cref="System.ArgumentNullException" />
        public ExportOperation(XafApplication app)
            : base(app)
        {
        }
    }
}
