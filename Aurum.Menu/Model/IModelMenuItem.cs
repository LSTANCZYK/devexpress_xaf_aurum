using DevExpress.ExpressApp.Model;
using System;
using System.ComponentModel;
namespace Aurum.Menu.Model
{
    /// <summary>
    /// Элемент меню
    /// </summary>
	[DisplayProperty("Caption")]
    [ModelAbstractClass]
	public interface IModelMenuItem : IModelNode
	{
		[Description("Название")]
		string Caption
		{
			get;
			set;
		}
		[Category("Appearance")]
        [DefaultValue(false)]
        [Description("Группа")]
		bool BeginGroup
		{
			get;
			set;
		}
	}
}
