using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using System;
using System.ComponentModel;
using System.Drawing.Design;
namespace Aurum.Menu.Model
{
    /// <summary>
    /// �������������
    /// </summary>
	[DisplayProperty("Caption")]
    [ModelDisplayName("�������������")]
    [ModelPersistentName("View")]
    [Description("�������������")]
    [ImageName("BO_List")]
	public interface IModelMenuViewItem : IModelMenuItem, IModelNode
	{
		[ModelValueCalculator("View", "Caption")]
        [Description("��������")]
        [Localizable(true)]
		new string Caption
		{
			get;
			set;
		}
		[ModelValueCalculator("BackFilterCriteria")]
        [Description("������")]
        [Localizable(true)]
		string ToolTip
		{
			get;
			set;
		}
		[Browsable(false)]
		string BackFilterCriteria
		{
			get;
			set;
		}
		[Category("Data"), Description("�������� �������� (Oid)")]
		string ObjectKey
		{
			get;
			set;
		}
		[Required]
        [DataSourceProperty("Application.Views")]
        [Category("Data")]
        [Description("�������������")]
		IModelView View
		{
			get;
			set;
		}
		[ModelValueCalculator("View is IModelListView ? ((IModelListView)View).ImageName : (View is IModelDetailView ? ((IModelDetailView)View).ImageName : null)")]
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
