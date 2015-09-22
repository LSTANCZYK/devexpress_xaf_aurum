using DevExpress.ExpressApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Interface.Model
{
    public interface IModelColumnAurumFilter : IModelNode
    {
        [Localizable(true)]
        [ModelValueCalculator("this.Caption")]
        [Category("Filter")]
        [Description("Заголовок")]
        string FilterCaption { get; set; }

        [ModelValueCalculator("this.FilterIndex >= 0")]
        [Category("Filter")]
        [Description("Показывать элемент в окне фильтра")]
        bool ShowFilter { get; set; }

        [ModelValueCalculator("this.Index")]
        [Category("Filter")]
        [Description("Индекс в окне фильтра")]
        int? FilterIndex { get; set; }

        [Category("Filter")]
        [Description("Фокус")]
        bool Focus { get; set; }
    }
}
