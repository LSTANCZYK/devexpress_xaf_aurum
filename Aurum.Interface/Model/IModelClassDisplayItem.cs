using DevExpress.ExpressApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Interface.Model
{
    public interface IModelPropertyEditorDisplayItem : IModelNode
    {
        [Category("CheckedListBox")]
        [Description("Отображение элемента в списке")]
        string DisplayItemCriteriaString { get; set; }

        [Category("CheckedListBox")]
        [Description("Отображение элемента в списке по условию описанному в свойстве.")]
        string DisplayItemCriteriaProperty { get; set; }
    }
}
