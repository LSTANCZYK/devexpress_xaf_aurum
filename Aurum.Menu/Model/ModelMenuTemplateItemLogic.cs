using Aurum.Menu.Base.Model;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aurum.Menu.Model
{
	[DomainLogic(typeof(IModelMenuTemplateLink))]
	public static class ModelMenuTemplateItemLogic
	{
		public static ICollection<IModelMenuTemplate> Get_Templates(IModelMenuTemplateLink modelExtender)
		{
			CalculatedModelNodeList<IModelMenuTemplate> calculatedModelNodeList = new CalculatedModelNodeList<IModelMenuTemplate>();
			calculatedModelNodeList.AddRange(
				from a in modelExtender.Application.Aurum().Menus().MenuEditor.Templates
				select a);
			return calculatedModelNodeList;
		}
	}
}
