using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Interface.Win.Editors
{
    [PropertyEditor(typeof(string), false)]
    public class PasswordEditor : WinPropertyEditor
    {
        public PasswordEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model)
        {
        }

        protected override object CreateControlCore()
        {
            ControlBindingProperty = "Value";
            return new PasswordControl();
        }
    }
}
