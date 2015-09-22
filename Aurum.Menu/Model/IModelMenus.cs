using Aurum.Menu.Model.NodeGenerators;
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
    /// Список меню
    /// </summary>
	[DisplayProperty("Caption")]
    [ModelNodesGenerator(typeof(MenusNodesGenerator))]
    public interface IModelMenus : IModelNode, IModelList<IModelMenu>, IList<IModelMenu>, ICollection<IModelMenu>, IEnumerable<IModelMenu>, IEnumerable
	{
		[Category("Appearance")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ImageGalleryModelEditorControl, DevExpress.ExpressApp.Win.v14.1", typeof(UITypeEditor))]
		string ImageName
		{
			get;
			set;
		}
		[Localizable(true)]
		string Caption
		{
			get;
			set;
		}
	}
}
