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
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Xpo;
using DevExpress.Persistent.BaseImpl;

namespace Aurum.Interface.Controllers
{
    /// <summary>
    /// Регистрация отчетов как действий (Action)
    /// </summary>
    public abstract class ReportsWindowControllerBase : WindowController
    {
        private event EventHandler<CreateCustomParametersDetailViewEventArgs> CreateCustomParametersDetailView;

        public ReportsWindowControllerBase()
        {
            TargetWindowType = WindowType.Main;
        }

        public void Handler(object sender, SimpleActionExecuteEventArgs e)
        {
            Type reportType = (Type)(sender as SimpleAction).Tag;
            IObjectSpace objectSpace = ReportDataProvider.ReportObjectSpaceProvider.CreateObjectSpace(typeof(ReportDataV2));
            IReportDataV2 reportData1 = objectSpace.FindObject<XtraReportData>(
                new BinaryOperator("PredefinedReportType", reportType));
            IReportDataV2 reportData2 = objectSpace.FindObject<ReportDataV2>(
                new BinaryOperator("PredefinedReportType", reportType));
            string reportContainerHandle = ReportDataProvider.ReportsStorage.GetReportContainerHandle(reportData1 ?? reportData2);
            ShowPreview(reportContainerHandle);
        }

        private void ShowPreview(string reportContainerHandle)
        {
            Guard.ArgumentNotNullOrEmpty(reportContainerHandle, "reportContainerHandle");
            IReportContainer reportContainer = ReportDataProvider.ReportsStorage.GetReportContainerByHandle(reportContainerHandle);
            ShowReportPreview(reportContainerHandle, reportContainer.ParametersObjectType);
        }

        private void ShowReportPreview(string reportContainerHandle, Type parametersObjectType)
        {
            if (parametersObjectType != null)
            {
                ShowParametersDetailView(reportContainerHandle, parametersObjectType);
            }
            else
            {
                ShowReportPreview(reportContainerHandle, null, null, false, null, false, null);
            }
        }

        private void ShowParametersDetailView(string reportContainerHandle, Type parametersObjectType)
        {
            ReportParametersObjectBase reportParametersObject = CreateReportParametersObject(parametersObjectType, ApplicationReportObjectSpaceProvider.Instance);
            if (reportParametersObject != null)
            {
                DetailView parametersDetailView = CreateParametersDetailView(reportParametersObject);
                if (parametersDetailView != null && reportParametersObject != null)
                {
                    parametersDetailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
                    IReportContainer reportContainer = ReportDataProvider.ReportsStorage.GetReportContainerByHandle(reportContainerHandle);
                    parametersDetailView.Caption = reportContainer.DisplayName;
                    DialogController controller = CreatePreviewReportDialogController();
                    controller.Tag = reportContainerHandle;
                    controller.Accepting += HandleAccept;
                    ShowViewParameters showViewParameters = new ShowViewParameters();
                    showViewParameters.Controllers.Add(controller);
                    ShowEmptyReportViewController showEmptyReportViewController = new ShowEmptyReportViewController();
                    showEmptyReportViewController.ShowEmptyReportAction.TargetObjectType = reportParametersObject.GetType();
                    showEmptyReportViewController.ShowEmptyReportAction.Execute += (sender, e) => { ShowReportPreview(reportContainerHandle, null, CriteriaOperator.Parse("1 = 0"), true, null, false, null); };
                    showViewParameters.Controllers.Add(showEmptyReportViewController);
                    showViewParameters.CreatedView = parametersDetailView;
                    showViewParameters.TargetWindow = TargetWindow.Current;
                    showViewParameters.Context = TemplateContext.PopupWindow;                    
                    Application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(Frame, null));
                }
            }
        }

        private DetailView CreateParametersDetailView(ReportParametersObjectBase reportParametersObject)
        {
            Guard.ArgumentNotNull(reportParametersObject, "reportParametersObject");
            CreateCustomParametersDetailViewEventArgs args = new CreateCustomParametersDetailViewEventArgs(reportParametersObject, Application);
            OnCreateCustomParametersDetail(args);
            DetailView detailView = null;
            if (args.Handled)
            {
                detailView = args.DetailView;
            }
            else
            {
                detailView = Application.CreateDetailView(reportParametersObject.ObjectSpace, reportParametersObject, false);
            }
            if (detailView != null && detailView.Items.Count == 0)
            {
                detailView.Dispose();
                detailView = null;
            }
            return detailView;
        }

        protected ReportParametersObjectBase CreateReportParametersObject(Type parametersObjectType, IObjectSpaceCreator objectSpaceProvider)
        {
            Guard.ArgumentNotNull(objectSpaceProvider, "objectSpaceProvider");
            ReportParametersObjectBase reportParametersObject = null;
            if (typeof(ReportParametersObjectBase).IsAssignableFrom(parametersObjectType))
            {
                reportParametersObject = (ReportParametersObjectBase)TypeHelper.CreateInstance(parametersObjectType, objectSpaceProvider);
            }
            return reportParametersObject;
        }

        protected virtual DialogController CreatePreviewReportDialogController()
        {
            return Application.CreateController<PreviewReportDialogController>();
        }

        protected void HandleAccept(Object sender, DialogControllerAcceptingEventArgs e)
        {
            string reportContainerHandle = (string)((WindowController)sender).Tag;
            ReportParametersObjectBase reportParametersObject = (ReportParametersObjectBase)e.AcceptActionArgs.CurrentObject;
            var criteria = reportParametersObject.GetCriteria();
            var sorting = reportParametersObject.GetSorting();
            // ((DialogController)sender).Accepting -= HandleAccept;

            var reportController = Frame.GetController<ReportServiceController>();
            Guard.ArgumentNotNull(reportController, "reportController");

            ShowReportPreview(reportContainerHandle, reportParametersObject, criteria, true, sorting, true, e.AcceptActionArgs.ShowViewParameters);

            // отмена действия для поддержки множественного запуска отчета с разными параметрами
            e.Cancel = true;
        }

        protected virtual void OnCreateCustomParametersDetail(CreateCustomParametersDetailViewEventArgs args)
        {
            if (CreateCustomParametersDetailView != null)
            {
                CreateCustomParametersDetailView(this, args);
            }
        }

        /// <summary>
        /// Вызов приватного метода ReportServiceController.ShowReportPreview()
        /// Public Morozov
        /// </summary>
        /// <param name="reportContainerHandle"></param>
        /// <param name="parametersObject"></param>
        /// <param name="criteria"></param>
        /// <param name="canApplyCriteria"></param>
        /// <param name="sortProperty"></param>
        /// <param name="canApplySortProperty"></param>
        /// <param name="showViewParameters"></param>
        private void ShowReportPreview(string reportContainerHandle, ReportParametersObjectBase parametersObject, CriteriaOperator criteria, bool canApplyCriteria, SortProperty[] sortProperty, bool canApplySortProperty, ShowViewParameters showViewParameters)
        {
            var reportController = Frame.GetController<ReportServiceController>();
            Guard.ArgumentNotNull(reportController, "reportController");

            reportController.ShowPreview(reportContainerHandle, parametersObject, criteria, canApplyCriteria, sortProperty, canApplySortProperty, showViewParameters);
        }
    }

    // TODO: Вынести отсюда
    /// <summary>
    /// Набор методов расширения для DevExpress.ExpressApp.ReportsV2.ReportServiceController
    /// </summary>
    public static class ReportServiceControllerExtensions
    {
        /// <summary>
        /// Показать окно предпросмотра отчета
        /// </summary>
        /// <param name="reportController">Контроллер</param>
        /// <param name="reportContainerHandle">Дескриптор</param>
        /// <param name="parametersObject">Объект параметров</param>
        /// <param name="criteria">Критерий</param>
        /// <param name="canApplyCriteria">Можно ли применить критерий</param>
        /// <param name="sortProperty">Сортировка</param>
        /// <param name="canApplySortProperty">Можно ли применить сортировку</param>
        /// <param name="showViewParameters">Объект DevExpress.ExpressApp.ShowViewParameters</param>
        public static void ShowPreview(this ReportServiceController reportController, string reportContainerHandle, ReportParametersObjectBase parametersObject, CriteriaOperator criteria, bool canApplyCriteria, SortProperty[] sortProperty, bool canApplySortProperty, ShowViewParameters showViewParameters)
        {
            var objectSpace = ReportDataProvider.ReportObjectSpaceProvider.CreateObjectSpace(typeof(ReportDataV2));
            Audit.ReportTrail.LogOperation(objectSpace, reportContainerHandle, parametersObject);

            var type = reportController.GetType().BaseType;
            var method = type.GetMethod("ShowReportPreview", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, new Type[] { typeof(string), typeof(ReportParametersObjectBase), typeof(CriteriaOperator), typeof(bool), typeof(SortProperty[]), typeof(bool), typeof(ShowViewParameters) }, null);

            if (method != null)
            {
                method.Invoke(reportController, new object[] { reportContainerHandle, parametersObject, criteria, canApplyCriteria, sortProperty, canApplySortProperty, showViewParameters });
            }
        }

        /// <summary>
        /// Показать окно предпросмотра отчета
        /// </summary>
        /// <param name="controller">Контроллер</param>
        /// <param name="reportType">Тип отчета</param>
        /// <param name="parametersObject">Объект параметров</param>
        /// <param name="showViewParameters">Объект DevExpress.ExpressApp.ShowViewParameters</param>
        public static void ShowPreview(this ReportServiceController controller, Type reportType, ReportParametersObjectBase parametersObject, ShowViewParameters showViewParameters)
        {
            var objectSpace = ReportDataProvider.ReportObjectSpaceProvider.CreateObjectSpace(typeof(ReportDataV2));

            var reportData1 = objectSpace.FindObject<ReportDataV2>(new BinaryOperator("PredefinedReportType", reportType));
            var reportData2 = (IReportDataV2)objectSpace.FindObject<XtraReportData>(new BinaryOperator("PredefinedReportType", reportType));

            var report = ReportDataProvider.ReportsStorage.LoadReport(reportData1 ?? reportData2);
            var reportContainerHandler = ReportDataProvider.ReportsStorage.GetReportContainerHandle(reportData1 ?? reportData2);

            controller.ShowPreview(reportContainerHandler, parametersObject, parametersObject.GetCriteria(), true, parametersObject.GetSorting(), true, showViewParameters);
        }
    }
}
