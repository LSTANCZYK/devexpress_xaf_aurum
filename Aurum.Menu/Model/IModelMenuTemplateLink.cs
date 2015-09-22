using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace Aurum.Menu.Model
{
    /// <summary>
    /// ������ �� ������
    /// </summary>
	[DisplayProperty("Caption")]
    [ModelDisplayName("������ �� ������")]
    [ModelPersistentName("TemplateLink")]
    [ImageName("Action_Copy")]
    public interface IModelMenuTemplateLink : IModelMenuItem, IModelNode
	{
		[ModelValueCalculator("Template", "Caption")]
        [Localizable(true)]
		new string Caption
		{
			get;
			set;
		}
		[ModelValueCalculator("Template", "ToolTip")]
        [Localizable(true)]
		string ToolTip
		{
			get;
			set;
		}
		[Browsable(false)]
		ICollection<IModelMenuTemplate> Templates
		{
			get;
		}
		[Required]
        [DataSourceProperty("Templates")]
        [Category("Data")]
        IModelMenuTemplate Template
		{
			get;
			set;
		}
	}
}
