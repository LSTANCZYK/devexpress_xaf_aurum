using Aurum.Menu.Base.Model;
using DevExpress.ExpressApp.Model;
using System;
using System.ComponentModel;

namespace Aurum.Menu.Model
{
	public interface IModelAurumMenus : IModelAurum, IModelNode
	{
		IModelMenuEditor MenuEditor
		{
			get;
		}
	}
}
