using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Aurum.Menu.Security.Model
{
    [ModelNodesGenerator(typeof(ModelAurumGroupNodesGenerator))]
    public interface IModelGroups : IModelNode, IModelList<IModelGroup>, IList<IModelGroup>, ICollection<IModelGroup>, IEnumerable<IModelGroup>, IEnumerable
    {
    }
}
