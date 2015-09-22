using Aurum.Operations;
using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Exchange
{
    /// <summary>
    /// Операция импорта
    /// </summary>
    public abstract class CustomImportOperation : SingleExchangeOperation
    {
        private List<InputFormatter> formatters = new List<InputFormatter>();

        /// <summary>
        /// Тип, над которым выполняется импорт
        /// </summary>
        protected override Type ObjectType
        {
            get { return null; }
        }

        /// <summary>
        /// Текущие форматировщики
        /// </summary>
        protected ReadOnlyCollection<InputFormatter> Formatters
        {
            get { return formatters.AsReadOnly(); }
        }

        /// <summary>
        /// Добавление форматировщика
        /// </summary>
        /// <param name="formatter"></param>
        protected void AddFormatter(InputFormatter formatter)
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

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <exception cref="ArgumentNullException" />
        public CustomImportOperation(XafApplication app)
            : base(app)
        {
        }

        protected virtual IObjectSpace GetObjectSpace()
        {
            return Application.CreateObjectSpace();
        }

        protected virtual void CloseObjectSpace(IObjectSpace objectSpace, bool success)
        {
            if (objectSpace != null && !objectSpace.IsDisposed)
            {
                if (success)
                {
                    objectSpace.CommitChanges();
                }
                else
                {
                    objectSpace.Rollback();
                }
                objectSpace.Dispose();
            }
        }

        protected override void ExecuteExchange(OperationInterop interop)
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
            interop.SetStatusText("Чтение файлов");
            interop.ThrowIfCancellationRequested();
            interop.SetProgress(10);
            // <<<

            // Чтение файлов
            foreach (var formatter in Formatters)
            {
                formatter.ReadFiles(interop);
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
            foreach (var formatter in Formatters)
            {
                interop.ThrowIfCancellationRequested();
                var objectSpace = GetObjectSpace();
                bool success = false;
                try
                {
                    formatter.ProcessData(objectSpace, interop);
                    success = true;
                }
                finally
                {
                    CloseObjectSpace(objectSpace, success);
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

            RaiseAfterExecuteEvent();
            interop.SetProgress(100);
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
    }

    /// <summary>
    /// Операция импорта с параметрами типа TParametersClass
    /// </summary>
    /// <typeparam name="TParametersClass">Тип параметров</typeparam>
    public abstract class CustomImportOperation<TParametersClass> : CustomImportOperation,
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
        /// Конструктор
        /// </summary>
        /// <param name="app">Объект приложения</param>
        /// <exception cref="System.ArgumentNullException" />
        public CustomImportOperation(XafApplication app)
            : base(app)
        {
        }
    }
}
