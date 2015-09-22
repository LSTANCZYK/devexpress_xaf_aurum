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
    /// �����
    /// </summary>
	[DisplayProperty("Caption")]
    [ModelDisplayName("�����")]
    [ModelPersistentName("Folder")]
    [Description("�����")]
    [ImageName("BO_Folder")]
	public interface IModelMenuFolder : IModelMenuItem, IModelNode, IModelList<IModelMenuItem>, IList<IModelMenuItem>, ICollection<IModelMenuItem>, IEnumerable<IModelMenuItem>, IEnumerable
	{
		[Category("Appearance")]
        [Description("�����������")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ImageGalleryModelEditorControl, DevExpress.ExpressApp.Win.v14.1", typeof(UITypeEditor))]
		string ImageName
		{
			get;
			set;
		}
		[ModelValueCalculator("Id")]
        [Description("��������"), Localizable(true)]
		new string Caption
		{
			get;
			set;
		}
		[Description("���������"), Localizable(true)]
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
        [Description("������� �� ���������")]
		IModelMenuItem DefaultItem
		{
			get;
			set;
		}
		[Category("Behavior")]
        [Description("������������ ������� �� ���������")]
		bool UseDefaultItem
		{
			get;
			set;
		}
	}
}
