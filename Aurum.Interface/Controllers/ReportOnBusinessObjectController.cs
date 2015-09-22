using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Interface.Controllers
{
    /// <summary>
    /// Action для вызова отчета на бизнес-объекте, для которого он создан
    /// </summary>
    public class ReportOnBusinessObjectController : ObjectViewController
    {
        public enum ActionEnabledMode
        {
            None,
            ModifiedChanged,
            ViewMode
        }

        public static ActionEnabledMode ShowInReportActionEnableModeDefault = ActionEnabledMode.None;
        public const string ActiveKeyObjectHasKeyMember = "ObjectHasKeyMember";
        public const string ActiveKeyDisableActionWhenThereAreChanges = "DisableActionWhenThereAreChanges";
        public const string ActiveKeyInplaceReportsAreEnabledInModule = "reportsModule.EnableInplaceReports";
        public const string ActiveKeyViewSupportsSelection = "ViewSupportsSelection";
        public const string ActiveKeyDisableForLookupListView = "DisableForLookupListView";
        public const string ActiveKeyControlsCreated = "ControlsCreated";
        private SingleChoiceAction showInReportAction;
        private IReportInplaceActionsHandler reportInplaceActionsHandler;

        private event EventHandler<CreateCustomParametersDetailViewEventArgs> CreateCustomParametersDetailView;

        private void showInReportAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count > 0)
            {
                if (e.SelectedChoiceActionItem.Data is ReportDataInfo)
                {
                    ShowInReport(e, ((ReportDataInfo)e.SelectedChoiceActionItem.Data).ReportContainerHandle);
                }
                else
                {
                    if (reportInplaceActionsHandler != null)
                    {
                        reportInplaceActionsHandler.ProcessActionItem(e);
                    }
                }
            }
        }

        private void ObjectSpace_ModifiedChanged(object sender, EventArgs e)
        {
            UpdateActionState();
        }

        private void View_SelectionTypeChanged(object sender, EventArgs e)
        {
            if (View != null)
            {
                Active[ActiveKeyViewSupportsSelection] = (View.SelectionType != SelectionType.None);
            }
        }

        private void PrintSelectionBaseController_ViewEditModeChanged(object sender, EventArgs e)
        {
            UpdateActionState();
        }

        private void View_ControlsCreated(object sender, EventArgs e)
        {
            View.ControlsCreated -= new EventHandler(View_ControlsCreated);
            Initialize();
        }

        private IReportInplaceActionsHandler FindReportInplaceActionsHandler()
        {
            foreach (Controller controller in Frame.Controllers)
            {
                IReportInplaceActionsHandler reportInplaceActionContainer = controller as IReportInplaceActionsHandler;
                if (reportInplaceActionContainer != null)
                {
                    return reportInplaceActionContainer;
                }
            }
            return null;
        }

        protected void Initialize()
        {
            showInReportAction.Active[ActiveKeyControlsCreated] = true;
            List<ChoiceActionItem> items = new List<ChoiceActionItem>();
            InplaceReportsCacheHelper inplaceReportsCache = ReportsModuleV2.FindReportsModule(Application.Modules).InplaceReportsCacheHelper;
            if (inplaceReportsCache != null)
            {
                List<ReportDataInfo> reportDataList = inplaceReportsCache.GetReportDataInfoList(((ObjectView)View).ObjectTypeInfo.Type);
                foreach (ReportDataInfo reportData in reportDataList)
                {
                    ChoiceActionItem item = new ChoiceActionItem(reportData.DisplayName, reportData);
                    item.ImageName = "Action_Report_Object_Inplace_Preview";
                    items.Add(item);
                }
            }
            reportInplaceActionsHandler = FindReportInplaceActionsHandler();
            if (reportInplaceActionsHandler != null)
            {
                reportInplaceActionsHandler.SuspendEvents();
                foreach (ReportInplaceActionInfo reportInplaceActionInfo in reportInplaceActionsHandler.GetReportInplaceActionInfo())
                {
                    ChoiceActionItem item = new ChoiceActionItem(reportInplaceActionInfo.ActionName, reportInplaceActionInfo.ActionData);
                    item.ImageName = "Action_Report_Object_Inplace_Preview";
                    items.Add(item);
                }
            }
            items.Sort(delegate(ChoiceActionItem left, ChoiceActionItem right)
            {
                return Comparer<string>.Default.Compare(left.Caption, right.Caption);
            });
            showInReportAction.Items.Clear();
            showInReportAction.Items.AddRange(items);
            if (ShowInReportActionEnableMode == ActionEnabledMode.ModifiedChanged)
            {
                View.ObjectSpace.ModifiedChanged += new EventHandler(ObjectSpace_ModifiedChanged);
            }
            else if (ShowInReportActionEnableMode == ActionEnabledMode.ViewMode)
            {
                if (View is DetailView)
                {
                    ((DetailView)View).ViewEditModeChanged += new EventHandler<EventArgs>(PrintSelectionBaseController_ViewEditModeChanged);
                }
            }
            UpdateActionState();
        }

        protected virtual void ShowInReport(SingleChoiceActionExecuteEventArgs e, string reportContainerHandle)
        {
            // Обработка параметров
            EventHandler<CreateCustomParametersDetailViewEventArgs> handler = null;
            handler = (sender, args) =>
            {
                injectSelectedObjects(args.ReportParametersObject, e.SelectedObjects);
                this.CreateCustomParametersDetailView -= handler;
            };
            this.CreateCustomParametersDetailView += handler;

            ShowPreview(reportContainerHandle);
        }

        public void ShowPreview(string reportContainerHandle)
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
                    showViewParameters.TargetWindow = TargetWindow.NewWindow;
                    showViewParameters.Context = TemplateContext.PopupWindow;
                    Application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(Frame, null));
                }
            }
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

        protected DetailView CreateParametersDetailView(ReportParametersObjectBase reportParametersObject)
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

        protected virtual void OnCreateCustomParametersDetail(CreateCustomParametersDetailViewEventArgs args)
        {
            if (CreateCustomParametersDetailView != null)
            {
                CreateCustomParametersDetailView(this, args);
            }
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

        /// <summary>
        /// Инъекция выбранного объекта в параметры отчета
        /// </summary>
        /// <param name="reportParametersObject"></param>
        /// <param name="selectedObjects"></param>
        private void injectSelectedObjects(ReportParametersObjectBase reportParametersObject, System.Collections.IList selectedObjects)
        {
            if (reportParametersObject == null) return;
            if (selectedObjects == null) return;
            if (selectedObjects.Count > 0 && selectedObjects[0] != null)
            {
                object o = selectedObjects[0];
                Type type = o.GetType();

                bool any = false;
                foreach (var prop in reportParametersObject.GetType().GetProperties())
                {
                    if (prop.PropertyType == type && prop.CanWrite)
                    {
                        any = true;
                        if (o is XPBaseObject)
                        {
                            o = reportParametersObject.ObjectSpace.GetObject(o);
                        }
                        prop.SetValue(reportParametersObject, o);
                    }
                }
                if (!any)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        protected virtual void UpdateActionState()
        {
            if (View != null)
            {
                ShowInReportAction.Enabled[PrintSelectionBaseController.ActiveKeyDisableActionWhenThereAreChanges] = true;
                if (ShowInReportActionEnableMode == ActionEnabledMode.ModifiedChanged)
                {
                    ShowInReportAction.Enabled[PrintSelectionBaseController.ActiveKeyDisableActionWhenThereAreChanges] = !View.ObjectSpace.IsModified;
                }
                else if (ShowInReportActionEnableMode == ActionEnabledMode.ViewMode)
                {
                    if (View is DetailView)
                    {
                        ShowInReportAction.Enabled[PrintSelectionBaseController.ActiveKeyDisableActionWhenThereAreChanges] =
                            ((DetailView)View).ViewEditMode == DevExpress.ExpressApp.Editors.ViewEditMode.View;
                    }
                }
            }
        }

        protected override void OnViewChanging(View view)
        {
            base.OnViewChanging(view);
            if (View != null)
            {
                View.SelectionTypeChanged -= new EventHandler(View_SelectionTypeChanged);
            }
            if (Application != null)
            {
                ReportsModuleV2 reportsModule = ReportsModuleV2.FindReportsModule(Application.Modules);
                if (reportsModule == null)
                {
                    Active["ReportsModule in Application.Modules"] = false;
                }
                else
                {
                    Active[ActiveKeyInplaceReportsAreEnabledInModule] = reportsModule.EnableInplaceReports;
                }
            }
            Active[ActiveKeyViewSupportsSelection] = (view is ISelectionContext) && (((ISelectionContext)view).SelectionType != SelectionType.None);
            if ((view is ObjectView) && (((ObjectView)view).ObjectTypeInfo != null))
            {
                Active[ActiveKeyObjectHasKeyMember] = (((ObjectView)view).ObjectTypeInfo.KeyMember != null);
            }
            if (view != null)
            {
                view.SelectionTypeChanged += new EventHandler(View_SelectionTypeChanged);
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            showInReportAction.Active[ActiveKeyControlsCreated] = false;
            if (View.IsControlCreated)
            {
                Initialize();
            }
            else
            {
                View.ControlsCreated += new EventHandler(View_ControlsCreated);
            }

            PrintSelectionBaseController originalInplaceReportController = Frame.GetController<PrintSelectionBaseController>();
            if (originalInplaceReportController != null)
            {
                originalInplaceReportController.Active.SetItemValue("Aurum", false);
            }
        }

        protected override void OnDeactivated()
        {
            View.ObjectSpace.ModifiedChanged -= new EventHandler(ObjectSpace_ModifiedChanged);
            View.ControlsCreated -= new EventHandler(View_ControlsCreated);
            if (View is DetailView)
            {
                ((DetailView)View).ViewEditModeChanged -= new EventHandler<EventArgs>(PrintSelectionBaseController_ViewEditModeChanged);
            }
            base.OnDeactivated();
        }
        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            if ((Frame != null) && ((Frame.Context == TemplateContext.LookupWindow) || (Frame.Context == TemplateContext.LookupControl)))
            {
                this.Active.SetItemValue(ActiveKeyDisableForLookupListView, false);
            }
        }

        public ReportOnBusinessObjectController()
        {
            TypeOfView = typeof(ObjectView);
            showInReportAction = new SingleChoiceAction(this, "ShowInReportV3", PredefinedCategory.Reports);
            showInReportAction.Caption = "Отчеты";
            showInReportAction.ToolTip = "Показать выбранные записи в отчете";
            showInReportAction.Execute += new SingleChoiceActionExecuteEventHandler(showInReportAction_Execute);
            showInReportAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
            showInReportAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
            showInReportAction.ImageName = "Action_Report_Object_Inplace_Preview";
            showInReportAction.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.CaptionAndImage;
            showInReportAction.ShowItemsOnClick = true;
            ShowInReportActionEnableMode = ShowInReportActionEnableModeDefault;
        }

        public SingleChoiceAction ShowInReportAction
        {
            get { return showInReportAction; }
        }

        public ActionEnabledMode ShowInReportActionEnableMode { get; set; }
    }
}
