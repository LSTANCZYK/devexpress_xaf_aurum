using Aurum.Menu.Base.Model;
using Aurum.Menu.Model;
using DevExpress.ExpressApp.Model;
using System;

namespace Aurum.Menu.Security.Model
{
    public interface IModelAurumSecurity : IModelAurum, IModelNode
    {
        IModelRoles Roles
        {
            get;
        }
    }
}
