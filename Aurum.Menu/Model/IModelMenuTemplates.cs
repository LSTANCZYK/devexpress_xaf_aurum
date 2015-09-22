using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
namespace Aurum.Menu.Model
{
    /// <summary>
    /// Список шаблонов
    /// </summary>
	public interface IModelMenuTemplates : IModelNode, IModelList<IModelMenuTemplate>, IList<IModelMenuTemplate>, ICollection<IModelMenuTemplate>, IEnumerable<IModelMenuTemplate>, IEnumerable
	{
	}
}
