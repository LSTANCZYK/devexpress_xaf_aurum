using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
namespace Aurum.Menu.Model
{
    /// <summary>
    /// ��������
    /// </summary>
	[DisplayProperty("Caption")]
    [ModelDisplayName("��������")]
    [ModelPersistentName("Action")]
    [Description("��������")]
    [ImageName("Action_SimpleAction")]
	public interface IModelMenuActionItem : IModelMenuItem, IModelNode
	{
		[ModelValueCalculator("Action", "Caption")]
        [Description("��������")]
        [Localizable(true)]
		new string Caption
		{
			get;
			set;
		}
		[Description("������")]
        [Localizable(true)]
		string ToolTip
		{
			get;
			set;
		}
		[Browsable(false)]
		ICollection<IModelAction> MenuActions
		{
			get;
		}
		[Required]
        [DataSourceProperty("MenuActions")]
        [Category("Data")]
        [Description("��������")]
		IModelAction Action
		{
			get;
			set;
		}
		[ModelValueCalculator("Action", "ImageName")]
        [Category("Appearance")]
        [Description("�����������")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ImageGalleryModelEditorControl, DevExpress.ExpressApp.Win.v14.1", typeof(UITypeEditor))]
		string ImageName
		{
			get;
			set;
		}
	}
}
