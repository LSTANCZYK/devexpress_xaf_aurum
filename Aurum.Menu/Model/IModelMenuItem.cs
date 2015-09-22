using DevExpress.ExpressApp.Model;
using System;
using System.ComponentModel;
namespace Aurum.Menu.Model
{
    /// <summary>
    /// ������� ����
    /// </summary>
	[DisplayProperty("Caption")]
    [ModelAbstractClass]
	public interface IModelMenuItem : IModelNode
	{
		[Description("��������")]
		string Caption
		{
			get;
			set;
		}
		[Category("Appearance")]
        [DefaultValue(false)]
        [Description("������")]
		bool BeginGroup
		{
			get;
			set;
		}
	}
}
