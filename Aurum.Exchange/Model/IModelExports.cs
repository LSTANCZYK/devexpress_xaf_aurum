using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using System.ComponentModel;

namespace Aurum.Exchange
{
    /// <summary>
    /// Список моделей доступных экспортов
    /// </summary>
    [ImageName("Action_Export")]
    [Description("Экспорты всех модулей")]
    [ModelNodesGenerator(typeof(ModelExportsGenerator))]
    public interface IModelExports : IModelNode, IModelList<IModelExport>
    {
    }
}
