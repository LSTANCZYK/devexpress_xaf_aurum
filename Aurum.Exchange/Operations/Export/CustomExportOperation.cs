using Aurum.Operations;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Aurum.Exchange
{
    /// <summary>
    /// Операция экспорта над типом TDataClass, с типизированными форматировщиками
    /// </summary>
    /// <typeparam name="TDataClass">Тип, над которым выполняется экспорт</typeparam>
    public abstract class CustomExportOperation<TDataClass> : SingleExchangeOperation
    {
        private List<OutputFormatter<TDataClass>> formatters = new List<OutputFormatter<TDataClass>>();

        /// <summary>
        /// Тип, над которым выполняется экспорт. Равен типу TDataClass
        /// </summary>
        protected sealed override Type ObjectType { get { return typeof(TDataClass); } }

        /// <summary>
        /// Текущие форматировщики
        /// </summary>
        protected ReadOnlyCollection<OutputFormatter<TDataClass>> Formatters
        {
            get { return formatters.AsReadOnly(); }
        }

        /// <summary>
        /// Добавление форматировщика
        /// </summary>
        /// <param name="formatter"></param>
        protected void AddFormatter(OutputFormatter<TDataClass> formatter)
        {
            if (Locked)
            {
                throw new InvalidOperationException("Изменять форматировщик запрещено в данный момент");
            }
            if (formatter == null)
            {
                throw new ArgumentNullException("Formatter", "Форматировщик не указан");
            }
            formatters.Add(formatter);
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
        /// Загрузка данных
        /// </summary>
        /// <param name="interop">Объект связи</param>
        /// <returns>Коллекция данных</returns>
        protected virtual IEnumerable<TDataClass> LoadData(OperationInterop interop)
        {
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
            
            return dataList;
        }

        protected virtual void OnAfterExport(IEnumerable<TDataClass> data, OperationInterop interop)
        {
        }

        protected override sealed void ExecuteExchange(OperationInterop interop)
        {
            // >>>
            interop.SetStatusText("Инициализация");
            interop.ThrowIfCancellationRequested();
            // <<<

            RaiseBeforeExecuteEvent();

            // >>>
            interop.SetStatusText("Обработка параметров");
            // <<<

            // >>>
            interop.SetStatusText("Загрузка объектов");
            interop.ThrowIfCancellationRequested();
            interop.SetProgress(10);
            // <<<

            // Загрузка объектов
            IEnumerable<TDataClass> dataList = null;
            if (Formatters.Count > 0)
            {
                dataList = LoadData(interop);
            }

            // >>>
            interop.SetStatusText("Обработка данных");
            interop.ThrowIfCancellationRequested();
            interop.SetProgress(50);
            // <<<

            // Before
            foreach (var formatter in Formatters)
            {
                formatter.OnBeforeFormatting();
            }

            // Processing
            if (dataList != null)
            {
                List<TDataClass> list = new List<TDataClass>();

                foreach (var record in dataList)
                {
                    list.Add(record);

                    if (list.Count == ChunkLoaderRowCount)
                    {
                        foreach (var formatter in Formatters)
                        {
                            formatter.Format(list, interop);
                        }
                        list.Clear();
                    }
                }
                if (list.Count > 0)
                {
                    foreach (var formatter in Formatters)
                    {
                        formatter.Format(list, interop);
                    }
                    list.Clear();
                }
            }
            else
            {
                foreach (var formatter in Formatters)
                {
                    formatter.Format(null, interop);
                }
            }
            
            // After
            foreach (var formatter in Formatters)
            {
                formatter.OnAfterFormatting();
            }

            // >>>
            interop.SetStatusText("Постобработка");
            interop.SetProgress(99);
            // <<<

            OnAfterExport(dataList, interop);
            RaiseAfterExecuteEvent();
            interop.SetProgress(100);

            if (dataList != null)
            {
                if (dataList is IDisposable)
                {
                    ((IDisposable)dataList).Dispose();
                }
                dataList = null;
            }
        }

        public override void Dispose()
        {
            if (formatters != null && formatters.Count > 0)
            {
                foreach (var formatter in formatters)
                {
                    formatter.Dispose();
                }
                formatters.Clear();
                formatters = null;
            }
            base.Dispose();
        }

        /// <summary>
        /// Создать новую операцию экспорта над типом TDataClass
        /// </summary>
        /// <exception cref="ArgumentNullException" />
        public CustomExportOperation(XafApplication app)
            : base(app)
        {
        }
    }

    /// <summary>
    /// Операция экспорта над типом TDataClass и параметрами типа TParametersClass, с типизированным форматировщиком
    /// </summary>
    /// <typeparam name="TDataClass">Тип, над которым выполняется экспорт</typeparam>
    /// <typeparam name="TParametersClass">Тип параметров</typeparam>
    public abstract class CustomExportOperation<TDataClass, TParametersClass> : CustomExportOperation<TDataClass>,
        IWithParametersType<TParametersClass>
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
        public CustomExportOperation(XafApplication app)
            : base(app)
        {
        }
    }
}
