using DevExpress.ExpressApp.Model;

namespace Aurum.Exchange
{
    /// <summary>
    /// Расширение модели приложения моделью обмена
    /// </summary>
    public interface IModelExchanges : IModelNode
    {
        /// <summary>
        /// Список моделей доступных экспортов
        /// </summary>
        IModelExports Exports { get; }
    }
}
