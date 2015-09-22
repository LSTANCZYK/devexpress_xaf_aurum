using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Operations
{
    /// <summary>
    /// Объект представления операции
    /// </summary>
    [DomainComponent]
    public class OperationObject
    {
        private OperationStatus status = OperationStatus.Created;
        private OperationManager manager;
        private Exception exception;

        private List<LogItemObject> items = new List<LogItemObject>();
        private List<OperationObject> children = new List<OperationObject>();

        /// <summary>
        /// Родительская операция
        /// </summary>
        public OperationObject Parent { get; set; }

        /// <summary>
        /// Дочерние операции
        /// </summary>
        public List<OperationObject> Children { get { return children; } }

        /// <summary>
        /// Элементы журнала
        /// </summary>
        public List<LogItemObject> Items { get { return items; } }

        /// <summary>
        /// Исключение, возникшее в ходе работы операции
        /// </summary>
        public Exception Exception
        {
            get { return exception; }
            set
            {
                exception = value;

                if (exception == null)
                {
                    return;
                }

                var ex = (AggregateException)exception;
                var message = String.Join("; ", ex.InnerExceptions.Select(i => i.Message));

                var caption = CaptionHelper.GetLocalizedText("Captions", "ExceptionText", (object)message);

                var item = new LogItemObject
                {
                    Time = DateTime.Now,
                    Message = caption,
                    Index = manager.GetNewIndex(OperationManager.LOG_ITEM_COUNTER_ID),
                    Type = LogItemType.Error,
                    OperationId = OperationId
                };

                Items.Add(item);
                StatusText = caption;
            }
        }

        /// <summary>
        /// Статус выполнения операции.
        /// При установке данного свойства, также изменяется временные свойства текущего объекта,
        /// а также добавляется запись в журнал, о смене статуса
        /// </summary>
        public OperationStatus Status
        {
            get { return status; }
            set
            {
                status = value;

                switch (value)
                {
                    case OperationStatus.Created:
                        Added = DateTime.Now;
                        break;
                    case OperationStatus.Running:
                        Started = DateTime.Now;
                        break;
                    default:
                        Ended = DateTime.Now;
                        break;
                }

                var statusText  = CaptionHelper.GetLocalizedText("Enums\\Aurum.Operations.OperationStatus", value.ToString());
                var caption = CaptionHelper.GetLocalizedText("Captions", "StatusText", (object)statusText);

                var item = new LogItemObject
                {
                    Time = DateTime.Now,
                    Message = caption,
                    Index = manager.GetNewIndex(OperationManager.LOG_ITEM_COUNTER_ID),
                    Type = LogItemType.Status,
                    OperationId = OperationId
                };

                Items.Add(item);
                if (Parent != null)
                {
                    Parent.Items.Add(item);
                }
                StatusText = statusText;
            }
        }

        /// <summary>
        /// Номер операции
        /// </summary>
        public int OperationId { get; set; }

        /// <summary>
        /// Значение прогресса, от 0 до 100 включительно
        /// </summary>
        public int Progress { get; set; }

        /// <summary>
        /// Текстовый статус
        /// </summary>
        public string StatusText { get; set; }

        /// <summary>
        /// Время добавления на исполнение
        /// </summary>
        [ModelDefault("DisplayFormat", "{0:G}")]
        public DateTime Added { get; set; }

        /// <summary>
        /// Время запуска
        /// </summary>
        [ModelDefault("DisplayFormat", "{0:G}")]
        public DateTime Started { get; set; }

        /// <summary>
        /// Время окончания
        /// </summary>
        [ModelDefault("DisplayFormat", "{0:G}")]
        public DateTime Ended { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }

        public OperationObject()
        {
            manager = OperationManager.Default;
        }
    }
}
