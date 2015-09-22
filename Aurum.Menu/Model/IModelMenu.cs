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
    /// ����
    /// </summary>
	[DisplayProperty("Caption")]
    [Description("����")]
    [ImageName("ModelEditor_Business_Object_Model")]
	public interface IModelMenu : IModelNode, IModelList<IModelMenuItem>, IList<IModelMenuItem>, ICollection<IModelMenuItem>, IEnumerable<IModelMenuItem>, IEnumerable
	{
		[Browsable(false)]
		ICollection<IModelMenuItem> AllItems
		{
			get;
		}
		[ModelValueCalculator("MenuCaption")]
        [Description("��������")]
        [Localizable(true)]
		string Caption
		{
			get;
			set;
		}
        [ModelValueCalculator("Id")]
        [Description("�������� ����")]
        [Localizable(true)]
        string MenuCaption
        {
            get;
            set;
        }
		[Category("Appearance")]
        [Description("�����������")]
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
        [Description("������")]
		IModelGroup Group
		{
			get;
			set;
		}
		[ModelValueCalculator("this.DefaultItem")]
        [DataSourceProperty("AllItems")]
        [Category("Behavior")]
        [Description("��������� �������")]
		IModelMenuItem StartupItem
		{
			get;
			set;
		}
		[DataSourceProperty("AllItems")]
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
