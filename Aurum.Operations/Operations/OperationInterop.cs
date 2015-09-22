using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aurum.Operations
{
    /// <summary>
    /// Посредник между операцией и менеджером операции.
    /// </summary>
    /// <remarks>Потокобезопасные модифицирует объекты OperationObject в общем DataSet с помощью OperationManager</remarks>
    public class OperationInterop
    {
        // Токен отмены
        private CancellationToken cancelToken;
        // Операция
        private OperationObject operation;
        // Объект состояния
        private InteropState state;

        /// <summary>
        /// Была ли запрошена отмена.
        /// Данное свойство доступно только для чтения
        /// </summary>
        public bool IsCancellationRequested
        {
            get { return cancelToken.IsCancellationRequested; }
        }

        /// <summary>
        /// Получить объект состояния.
        /// Если текущая операция выполняется в группе, то данный объект является общим для всей группы.
        /// Данное свойство доступно только для чтения
        /// </summary>
        public InteropState State
        {
            get { return state; }
        }

        /// <summary>
        /// Получить новый индекс записи журнала
        /// </summary>
        /// <returns>Новый индекс записи журнала</returns>
        private int GetNewLogItemIndex()
        {
            return OperationManager.Default.GetNewIndex(OperationManager.LOG_ITEM_COUNTER_ID);
        }

        /// <summary>
        /// Вызвать корректное исключение System.OperationCanceledException, если была запрошена отмена
        /// </summary>
        /// <exception cref="System.OperationCanceledException" />
        /// <exception cref="System.ObjectDisposedException" />
        public void ThrowIfCancellationRequested()
        {
            cancelToken.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Добавить запись в журнал операции
        /// </summary>
        /// <param name="type">Тип</param>
        /// <param name="format">Формат строки</param>
        /// <param name="args">Аргументы строки</param>
        /// <exception cref="System.ArgumentNullException" />
        public void WriteToLog(LogItemType type, string format, params object[] args)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }

            var now = DateTime.Now;

            var item = new LogItemObject
            {
                Time = DateTime.Now,
                Message = String.Format(format, args),
                Index = GetNewLogItemIndex(),
                Type = type,
                OperationId = operation.OperationId
            };

            operation.Items.Add(item);

            if (operation.Parent != null)
            {
                operation.Parent.Items.Add(item);
            }
        }

        /// <summary>
        /// Добавить запись в журнал операции с типом "Информация"
        /// </summary>
        /// <param name="format">Формат строки</param>
        /// <param name="obj">Аргументы строки</param>
        /// <exception cref="System.ArgumentNullException" />
        public void WriteToLog(string format, params object[] obj)
        {
            WriteToLog(LogItemType.Info, format, obj);
        }

        /// <summary>
        /// Установить текстовый статус
        /// </summary>
        /// <param name="text">Текст статуса</param>
        /// <exception cref="System.ArgumentNullException" />
        public void SetStatusText(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            operation.StatusText = text;
            WriteToLog(LogItemType.Status, CaptionHelper.GetLocalizedText("Captions", "StatusText", (object)text));
        }

        /// <summary>
        /// Установить значение прогресса
        /// </summary>
        /// <param name="newProgress">Новое значение прогресса</param>
        /// <exception cref="System.ArgumentOutOfRangeException" />
        public void SetProgress(int newProgress)
        {
            if (newProgress < 0)
            {
                throw new ArgumentOutOfRangeException("newProgress", "Значение прогресса не может быть меньше нуля");
            }

            operation.Progress = newProgress;
        }

        /// <summary>
        /// Увеличить прогресс на указанное значение
        /// </summary>
        public void IncrementProgress(int value = 1)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("newProgress", "Увеличение не может быть меньше нуля");
            }

            if (value == 0)
            {
                return;
            }

            operation.Progress = value;
        }

        /// <summary>
        /// Создать объект-посредник
        /// </summary>
        /// <param name="operationObject">Объект операции, принадлежащая указанному Object Space</param>
        /// <param name="cancelToken">Токен отмены задачи</param>
        /// <param name="state">Объект состояния</param>
        /// <exception cref="System.ArgumentNullException" />
        public OperationInterop(OperationObject operationObject, CancellationToken cancelToken, InteropState state = null)
        {
            if (operationObject == null)
            {
                throw new ArgumentNullException("operationObject");
            }
            this.cancelToken = cancelToken;
            this.operation = operationObject;
            this.state = state;
        }
    }

    /// <summary>
    /// Объект состояния
    /// </summary>
    public class InteropState
    {
        /// <summary>
        /// Значение состояния.
        /// Данное свойство доступно для чтения и записи
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Получить значение состояния, явно приведенное к указанному типу
        /// </summary>
        /// <typeparam name="T">Приведенный тип</typeparam>
        /// <returns>Значение, приведенное к указанному типу</returns>
        /// <exception cref="System.InvalidCastException" />
        public T GetValue<T>()
        {
            return (T)Value;
        }
    }
}
