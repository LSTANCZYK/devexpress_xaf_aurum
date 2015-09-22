using System.Threading.Tasks;
namespace Aurum.Operations
{
    /// <summary>
    /// Статус операции
    /// </summary>
    public enum OperationStatus
    {
        /// <summary>
        /// Создана
        /// </summary>
        Created,

        /// <summary>
        /// Выполняется
        /// </summary>
        Running,

        /// <summary>
        /// Отменена
        /// </summary>
        Canceled,

        /// <summary>
        /// Возникло исключение, в ходе выполнения
        /// </summary>
        Faulted,

        /// <summary>
        /// Выполнена
        /// </summary>
        RanToCompletion
    }

    /// <summary>
    /// Расширения статусов операций
    /// </summary>
    public static class OperationStatusExtensions
    {
        /// <summary>
        /// Получить статус задачи из статуса выполнения System.Threading.Tasks.TaskStatus
        /// </summary>
        /// <param name="status">Статус выполнения System.Threading.Tasks.TaskStatus</param>
        /// <returns>Соответствующий статус задачи</returns>
        public static OperationStatus ToOperationStatus(this TaskStatus status)
        {
            switch (status)
            {
                case TaskStatus.Canceled: return OperationStatus.Canceled;
                case TaskStatus.Faulted: return OperationStatus.Faulted;
                case TaskStatus.RanToCompletion: return OperationStatus.RanToCompletion;
                case TaskStatus.Running: return OperationStatus.Running;
            }
            return OperationStatus.Created;
        }
    }
}
