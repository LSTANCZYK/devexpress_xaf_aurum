using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Aurum.Menu.Security.Model
{
    [ModelNodesGenerator(typeof(ModelAurumRoleNodesGenerator))]
    public interface IModelRoles : IModelNode, IModelList<IModelRole>, IList<IModelRole>, ICollection<IModelRole>, IEnumerable<IModelRole>, IEnumerable
    {
    }
}
