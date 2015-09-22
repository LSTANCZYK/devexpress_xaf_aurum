using Aurum.Menu.Security.Model;
using Aurum.Menu.Utils;
using Aurum.Menu.Base.Model;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aurum.Menu.Model
{
	[DomainLogic(typeof(IModelMenu))]
	public static class ModelMenuLogic
	{
		public static ICollection<IModelGroup> Get_Groups(IModelMenu modelExtender)
		{
			CalculatedModelNodeList<IModelGroup> calculatedModelNodeList = new CalculatedModelNodeList<IModelGroup>();
			calculatedModelNodeList.AddRange(
				from a in modelExtender.Application.Aurum().AurumLists().Groups
				select a);
			return calculatedModelNodeList;
		}

		public static ICollection<IModelMenuItem> Get_AllItems(IModelMenu modelExtender)
		{
			CalculatedModelNodeList<IModelMenuItem> calculatedModelNodeList = new CalculatedModelNodeList<IModelMenuItem>();
			IList<IModelMenuItem> collection = RecursiveHelper.ToList<IModelMenuItem>(modelExtender, delegate(IModelMenuItem a)
			{
				if (a is IModelMenu)
				{
					return a as IModelMenu;
				}
				if (a is IModelMenuTemplateLink)
				{
					return (a as IModelMenuTemplateLink).Template;
				}
				if (a is IModelMenuFolder)
				{
					return a as IModelMenuFolder;
				}
				return null;
			}, (IModelMenuItem a) => a is IModelMenuViewItem || a is IModelMenuActionItem);
			calculatedModelNodeList.AddRange(collection);
			return calculatedModelNodeList;
		}
	}
}
