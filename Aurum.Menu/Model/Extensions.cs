using Aurum.Menu.Base.Model;
using Aurum.Menu.Security.Model;
using DevExpress.ExpressApp.Model;
using System;

namespace Aurum.Menu.Model
{
	public static class Extensions
	{
		public static IModelAurumMenus Menus(this IModelAurum aurum)
		{
			return (IModelAurumMenus)aurum;
		}
	}
}
