using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
namespace Aurum.Menu.Model
{
    /// <summary>
    /// Шаблон
    /// </summary>
	[DisplayProperty("Caption")]
    [ModelDisplayName("Шаблон")]
    [ModelPersistentName("Template")]
    [ImageName("Action_Copy")]
    public interface IModelMenuTemplate : IModelNode, IModelList<IModelMenuItem>, IList<IModelMenuItem>, ICollection<IModelMenuItem>, IEnumerable<IModelMenuItem>, IEnumerable
	{
		[ModelValueCalculator("Id")]
        [Localizable(true)]
		string Caption
		{
			get;
			set;
		}
		[Category("Appearance")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ImageGalleryModelEditorControl, DevExpress.ExpressApp.Win.v14.1", typeof(UITypeEditor))]
		string ImageName
		{
			get;
			set;
		}
		[Localizable(true)]
		string ToolTip
		{
			get;
			set;
		}
		[Browsable(false)]
		ICollection<IModelMenuItem> AllChildrenItems
		{
			get;
		}
		[DataSourceProperty("AllChildrenItems")]
        [Category("Behavior")]
		IModelMenuItem DefaultItem
		{
			get;
			set;
		}
		[Category("Behavior")]
		bool UseDefaultItem
		{
			get;
			set;
		}
	}
}
