using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using System;
using System.ComponentModel;

namespace Aurum.Menu.Security.Model
{
    [DisplayProperty("Caption")]
    public interface IModelGroup : IModelNode
    {
        [Required]
        string Id
        {
            get;
        }
        [ModelValueCalculator("this", "Id"), Localizable(true)]
        string Caption
        {
            get;
            set;
        }
    }
}
