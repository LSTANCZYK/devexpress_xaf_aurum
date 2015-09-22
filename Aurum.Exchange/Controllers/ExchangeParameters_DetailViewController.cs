using System;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Model.NodeGenerators;
using Aurum.Exchange;

namespace Aurum.Exchange.Controllers
{
    public partial class ExchangeParameters_DetailViewController : ViewController
    {
        public ExchangeParameters_DetailViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof(ExchangeParameters);
        }

        private void validateAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            try
            {
                Validator.RuleSet.Validate(ObjectSpace, e.CurrentObject, DefaultContexts.Save);
                e.Action.ImageName = "State_Validation_Valid";
            }
            catch (ValidationException)
            {
                e.Action.ImageName = "State_Validation_Invalid";
                throw;
            }
        }
    }
}
