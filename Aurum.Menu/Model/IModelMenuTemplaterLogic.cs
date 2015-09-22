using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using System;
using System.Collections.Generic;
namespace Aurum.Menu.Model
{
    /// <summary>
    /// Логика шаблона
    /// </summary>
	[DomainLogic(typeof(IModelMenuTemplate))]
	public static class IModelMenuTemplaterLogic
	{
        /// <summary>
        /// Получение дочерних элементов
        /// </summary>
        /// <param name="modelExtender"></param>
        /// <returns></returns>
		public static ICollection<IModelMenuItem> Get_AllChildrenItems(IModelMenuTemplate modelExtender)
		{
			CalculatedModelNodeList<IModelMenuItem> calculatedModelNodeList = new CalculatedModelNodeList<IModelMenuItem>();
			IEnumerable<IModelMenuViewItem> nodes = modelExtender.GetNodes<IModelMenuViewItem>();
			IEnumerable<IModelMenuActionItem> nodes2 = modelExtender.GetNodes<IModelMenuActionItem>();
			calculatedModelNodeList.AddRange(nodes);
			calculatedModelNodeList.AddRange(nodes2);
			return calculatedModelNodeList;
		}
	}
}
