using Aurum.Menu.Security.Model;
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
    /// Меню
    /// </summary>
	[DisplayProperty("Caption")]
    [Description("Меню")]
    [ImageName("ModelEditor_Business_Object_Model")]
	public interface IModelMenu : IModelNode, IModelList<IModelMenuItem>, IList<IModelMenuItem>, ICollection<IModelMenuItem>, IEnumerable<IModelMenuItem>, IEnumerable
	{
		[Browsable(false)]
		ICollection<IModelMenuItem> AllItems
		{
			get;
		}
		[ModelValueCalculator("MenuCaption")]
        [Description("Название")]
        [Localizable(true)]
		string Caption
		{
			get;
			set;
		}
        [ModelValueCalculator("Id")]
        [Description("Название меню")]
        [Localizable(true)]
        string MenuCaption
        {
            get;
            set;
        }
		[Category("Appearance")]
        [Description("Изображение")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ImageGalleryModelEditorControl, DevExpress.ExpressApp.Win.v14.1", typeof(UITypeEditor))]
		string ImageName
		{
			get;
			set;
		}
		[Description("Specifies the tooltip text.")]
        [Localizable(true)]
		string ToolTip
		{
			get;
			set;
		}
		[Browsable(false)]
		ICollection<IModelGroup> Groups
		{
			get;
		}
        [DataSourceProperty("Groups")]
        [Description("Группа")]
		IModelGroup Group
		{
			get;
			set;
		}
		[ModelValueCalculator("this.DefaultItem")]
        [DataSourceProperty("AllItems")]
        [Category("Behavior")]
        [Description("Стартовый элемент")]
		IModelMenuItem StartupItem
		{
			get;
			set;
		}
		[DataSourceProperty("AllItems")]
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
