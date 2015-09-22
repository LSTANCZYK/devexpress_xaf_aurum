using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Aurum.Menu.Model
{
	[DomainLogic(typeof(IModelMenuActionItem))]
	public static class ModelMenuActionItemLogic
	{
		public static ICollection<IModelAction> Get_MenuActions(IModelMenuActionItem modelExtender)
		{
			CalculatedModelNodeList<IModelAction> calculatedModelNodeList = new CalculatedModelNodeList<IModelAction>();
			calculatedModelNodeList.AddRange(
				from a in modelExtender.Application.ActionDesign.Actions.Where(delegate(IModelAction a)
				{
					IModelWindowController modelWindowController = a.Controller as IModelWindowController;
                    IModelViewController modelViewController = a.Controller as IModelViewController;
                    if (modelWindowController != null)
					    return modelWindowController.TargetWindowType == WindowType.Any || modelWindowController.TargetWindowType == WindowType.Main;
                    if (modelViewController != null)
                        return true;
                    return false;
				})
				orderby a.Id
				select a);
			return calculatedModelNodeList;
		}
	}
}
