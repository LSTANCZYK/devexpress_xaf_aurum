using DevExpress.ExpressApp.Model;
using System;

namespace Aurum.Menu.Base.Model
{
    public interface IModelApplicationAurum : IModelApplication, IModelNode
    {
        IModelAurum Aurum
        {
            get;
        }
    }
}
