using Aurum.Menu.Base.Model;
using Aurum.Menu.Model;
using DevExpress.ExpressApp.Model;
using System;

namespace Aurum.Menu.Security.Model
{
    public interface IModelAurumLists : IModelAurum, IModelNode
    {
        IModelGroups Groups
        {
            get;
        }
    }
}
