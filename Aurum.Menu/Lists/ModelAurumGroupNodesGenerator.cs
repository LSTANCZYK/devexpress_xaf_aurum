using Aurum.Menu.Security.Model;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using System;

namespace Aurum.Menu.Security
{
    public class ModelAurumGroupNodesGenerator : ModelNodesGeneratorBase
    {
        protected override void GenerateNodesCore(ModelNode node)
        {
            node.AddNode<IModelGroup>("Admin");
        }
    }
}
