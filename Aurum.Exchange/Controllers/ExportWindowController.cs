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

namespace Aurum.App.Module.Controllers
{
    public partial class TestExportWindowController : WindowController
    {
        public static List<Type> exchanges;

        public TestExportWindowController()
        {
            InitializeComponent();
            RegisterActions(components);

            action.Execute += action_Execute;

            foreach (var ex in exchanges
                .Where(x => typeof(ExportOperationBase).IsAssignableFrom(x)))
            {
                var item = new ChoiceActionItem
                {
                    Caption = ex.Name,
                    Data = ex
                };
                
                action.Items.Add(item);
            }
        }

        void action_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            var data = e.SelectedChoiceActionItem.Data as Type;

            var space = Application.CreateObjectSpace();
            var export = (ExportOperationBase)Activator.CreateInstance(data, space);

            if (export.ParametersObject != null)
            {
                var view = Application.CreateDetailView(space, export.ParametersObject);
                e.ShowViewParameters.CreatedView = view;
                e.ShowViewParameters.Context = TemplateContext.PopupWindow;

                var ctr = new DialogController();

                // !! UNSUBSCRIBE AFTERWARDS !! //
                ctr.AcceptAction.Execute += (io, iie) =>
                {
                    
                    export.ExecuteExchange();
                };

                e.ShowViewParameters.Controllers.Add(ctr);
            }
        }

        static TestExportWindowController()
        {
            exchanges = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                exchanges
                    .AddRange(
                        assembly
                        .GetTypes()
                        .Where(x =>
                            !x.IsAbstract &&
                            typeof(ExchangeOperation)
                            .IsAssignableFrom(x)
                        )
                    );
            }
        }
    }
}
