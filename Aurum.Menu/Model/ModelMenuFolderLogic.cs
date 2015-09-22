using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using System;
using System.Collections.Generic;
namespace Aurum.Menu.Model
{
	[DomainLogic(typeof(IModelMenuFolder))]
	public static class ModelMenuFolderLogic
	{
		public static ICollection<IModelMenuItem> Get_AllChildrenItems(IModelMenuFolder modelExtender)
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
