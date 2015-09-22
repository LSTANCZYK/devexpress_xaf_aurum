using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using System;
using System.ComponentModel;
namespace Aurum.Menu.Model
{
	public interface IModelMenuEditor : IModelNode
	{
		IModelMenus Menus
		{
			get;
		}
		IModelMenuTemplates Templates
		{
			get;
		}
		[DataSourceProperty("Menus")]
        [Category("Behavior")]
        IModelMenu StartupMenuItem
		{
			get;
			set;
		}
	}
}
