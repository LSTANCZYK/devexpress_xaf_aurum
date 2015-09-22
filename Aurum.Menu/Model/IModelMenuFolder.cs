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
    /// Папка
    /// </summary>
	[DisplayProperty("Caption")]
    [ModelDisplayName("Папка")]
    [ModelPersistentName("Folder")]
    [Description("Папка")]
    [ImageName("BO_Folder")]
	public interface IModelMenuFolder : IModelMenuItem, IModelNode, IModelList<IModelMenuItem>, IList<IModelMenuItem>, ICollection<IModelMenuItem>, IEnumerable<IModelMenuItem>, IEnumerable
	{
		[Category("Appearance")]
        [Description("Изображение")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ImageGalleryModelEditorControl, DevExpress.ExpressApp.Win.v14.1", typeof(UITypeEditor))]
		string ImageName
		{
			get;
			set;
		}
		[ModelValueCalculator("Id")]
        [Description("Название"), Localizable(true)]
		new string Caption
		{
			get;
			set;
		}
		[Description("Подсказка"), Localizable(true)]
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
        [Description("Элемент по умолчанию")]
		IModelMenuItem DefaultItem
		{
			get;
			set;
		}
		[Category("Behavior")]
        [Description("Использовать элемент по умолчанию")]
		bool UseDefaultItem
		{
			get;
			set;
		}
	}
}
