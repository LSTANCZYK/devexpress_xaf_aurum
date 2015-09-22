using Aurum.Exchange;
using Aurum.Operations;
using Aurum.Operations.Controllers;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Aurum.App.Module.Controllers
{
    public partial class ExchangeWindowController : WindowController
    {
        public ExchangeWindowController()
        {
            InitializeComponent();

            foreach (var ex in
                ExchangeTypeHelper.FindAllExchanges()
                .Where(x => x.GetCustomAttributes(typeof(SubExchangeAttribute), true).Length == 0)
                .OrderBy(x => TryGetIndex(ExchangeOperationBase.GetModel(x, Application))))
            {
                var act = new SimpleAction(this.components);
                act.Id = ex.FullName + "Action";
                act.Category = "MP_Exchange";
                act.Tag = ex;
                act.ImageName = "Action_Export_Chart";
                act.Execute += exchangeChoiceAction_Execute;
            }

            RegisterActions(components);
        }

        private int TryGetIndex(IModelExchange model)
        {
            if (model == null)
            {
                return 0;
            }
            return model.Index ?? 0;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
        }

        private void RunExchangeOperation(ExchangeOperation exchange, ShowViewParameters sp)
        {
            // log
            Audit.ExchangeTrail.LogOperation(Application.CreateObjectSpace(), exchange);

            // run
            OperationManager.Default.Run(exchange).Show(Application, sp);
        }

        void exchangeChoiceAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = Application.CreateObjectSpace();
            var exchangeType = e.Action.Tag as Type;
            var parType = ExchangeOperation.GetParametersType(exchangeType);

            if (parType != null)
            {
                var parInstance = ExchangeOperation.CreateParameters(exchangeType, objectSpace);
                var view = Application.CreateDetailView(objectSpace, parInstance);

                e.ShowViewParameters.CreatedView = view;
                e.ShowViewParameters.Context = TemplateContext.PopupWindow;
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;

                var ctr = new DialogController { CanCloseWindow = false };

                ctr.AcceptAction.Execute += (object io, SimpleActionExecuteEventArgs ie) =>
                {
                    Validator.RuleSet.Validate(objectSpace, ie.CurrentObject, DefaultContexts.Save);
                    var exchangeInstance = (ExchangeOperation)Activator.CreateInstance(exchangeType, Application);
                    exchangeInstance.ParametersObject = parInstance;
                    RunExchangeOperation(exchangeInstance, ie.ShowViewParameters);
                };
                ctr.CancelAction.Execute += (_, __) => view.Close();

                e.ShowViewParameters.Controllers.Add(ctr);
            }
            else
            {
                var exchangeInstance = (ExchangeOperation)Activator.CreateInstance(exchangeType, Application);
                RunExchangeOperation(exchangeInstance, e.ShowViewParameters);
            }
        }
    }
}
