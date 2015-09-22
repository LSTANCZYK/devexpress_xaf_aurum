using Aurum.Menu.Templates;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.ExpressApp.Win.Templates.Controls;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Aurum.Menu.Win.Templates
{
    public class MenusMainForm : MainFormTemplateBase, IDockManagerHolder, ISupportClassicToRibbonTransform, IBarManagerHolder, ISupportStoreSettingsEx, ISupportStoreSettings, ISupportMenus
    {
        private IContainer components;
        private BarDockControl barDockControlTop;
        private BarDockControl barDockControlBottom;
        private BarDockControl barDockControlLeft;
        private BarDockControl barDockControlRight;
        private XafBar _mainMenuBar;
        private XafBar standardToolBar;
        private XafBar _statusBar;
        private ActionContainerBarItem cFile;
        private ActionContainerBarItem cObjectsCreation;
        private ActionContainerBarItem cPrint;
        private ActionContainerBarItem cExport;
        private ActionContainerBarItem cSave;
        private ActionContainerBarItem cUndoRedo;
        private ActionContainerMenuBarItem cAppearance;
        private ActionContainerBarItem cReports;
        private ActionContainerBarItem cEdit;
        private ActionContainerBarItem cExit;
        private ActionContainerBarItem cOpenObject;
        private MainMenuItem barSubItemFile;
        private MainMenuItem barSubItemEdit;
        private MainMenuItem barSubItemView;
        private MainMenuItem barSubItemTools;
        private MainMenuItem barSubItemHelp;
        private ActionContainerMenuBarItem cMenus;
        private ActionContainerBarItem cRecordEdit;
        private ActionContainerBarItem cRecordsNavigation;
        private ActionContainerBarItem cViewsHistoryNavigation;
        private ActionContainerBarItem cSearch;
        private ActionContainerBarItem cFullTextSearch;
        private ActionContainerBarItem cFilters;
        private ActionContainerBarItem cView;
        private ActionContainerBarItem cDefault;
        private ActionContainerMenuBarItem cTools;
        private ActionContainerMenuBarItem cDiagnostic;
        private ActionContainerMenuBarItem cOptions;
        private ActionContainerMenuBarItem cAbout;
        private XafBarLinkContainerItem cWindows;
        private ActionContainerMenuBarItem cPanels;
        private ActionContainerBarItem cMenu;
        private MainMenuItem barSubItemWindow;
        private BarMdiChildrenListItem barMdiChildrenListItem;
        protected FormStateModelSynchronizer formStateModelSynchronizerComponent;
        private BarAndDockingController mainBarAndDockingController;
        private DockManager mainDockManager;
        protected XafBarManager mainBarManager;
        private PanelControl viewSitePanel;
        private ActionContainerBarItem Window;
        private BarSubItem barSubItemPanels;
        private DockPanel dockPanelMenus;
        private ControlContainer dockPanel1_Container;
        private ImageList imageList1;
        private ActionContainerMenuBarItem cHelp;
        private MenuItemsActionContainer rootMenuItemsActionContainer1;
        public static event EventHandler<EventArgs> TemplateCreated;
        public event EventHandler SettingsSaved;
        public Bar ClassicStatusBar
        {
            get
            {
                return this._statusBar;
            }
        }
        public DockPanel DockPanelMenus
        {
            get
            {
                return this.dockPanelMenus;
            }
        }
        public DockManager DockManager
        {
            get
            {
                return this.mainDockManager;
            }
        }
        public override void SetSettings(IModelTemplate modelTemplate)
        {
            base.SetSettings(modelTemplate);
            this.formStateModelSynchronizerComponent.Model = this.GetFormStateNode();
        }
        protected virtual void InitializeImages()
        {
            this.barMdiChildrenListItem.Glyph = ImageLoader.Instance.GetImageInfo("Action_WindowList").Image;
            this.barMdiChildrenListItem.LargeGlyph = ImageLoader.Instance.GetLargeImageInfo("Action_WindowList").Image;
            this.barSubItemPanels.Glyph = ImageLoader.Instance.GetImageInfo("Action_Navigation").Image;
            this.barSubItemPanels.LargeGlyph = ImageLoader.Instance.GetLargeImageInfo("Action_Navigation").Image;
        }
        public MenusMainForm()
        {
            this.InitializeComponent();
            this.InitializeImages();
            this.UpdateMdiModeDependentProperties();
            this.documentManager.BarAndDockingController = this.mainBarAndDockingController;
            this.documentManager.MenuManager = this.mainBarManager;
            if (MenusMainForm.TemplateCreated != null)
            {
                MenusMainForm.TemplateCreated(this, EventArgs.Empty);
            }
        }
        protected override void UpdateMdiModeDependentProperties()
        {
            base.UpdateMdiModeDependentProperties();
            bool flag = base.UIType == UIType.StandardMDI || base.UIType == UIType.TabbedMDI;
            this.viewSitePanel.Visible = !flag;
            if (flag)
            {
                this.barSubItemWindow.Visibility = BarItemVisibility.Always;
                this.barMdiChildrenListItem.Visibility = BarItemVisibility.Always;
                return;
            }
            this.barSubItemWindow.Visibility = BarItemVisibility.Never;
            this.barMdiChildrenListItem.Visibility = BarItemVisibility.Never;
        }
        public void SetMenusCaption(string caption)
        {
            this.DockPanelMenus.Text = caption;
        }
        public override void SaveSettings()
        {
            base.SaveSettings();
            if (this.SettingsSaved != null)
            {
                this.SettingsSaved(this, EventArgs.Empty);
            }
        }
        private void mainBarManager_Disposed(object sender, EventArgs e)
        {
            if (this.mainBarManager != null)
            {
                this.mainBarManager.Disposed -= new EventHandler(this.mainBarManager_Disposed);
            }
            this.modelSynchronizationManager.ModelSynchronizableComponents.Remove(this.barManager);
            this.barManager = null;
            this.mainBarManager = null;
            this._mainMenuBar = null;
            this._statusBar = null;
            this.standardToolBar = null;
            this.barDockControlBottom = null;
            this.barDockControlLeft = null;
            this.barDockControlRight = null;
            this.barDockControlTop = null;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.components = new Container();
            this.mainBarManager = new XafBarManager(this.components);
            this._mainMenuBar = new XafBar();
            this.barSubItemFile = new MainMenuItem();
            this.cObjectsCreation = new ActionContainerBarItem();
            this.cFile = new ActionContainerBarItem();
            this.cSave = new ActionContainerBarItem();
            this.cPrint = new ActionContainerBarItem();
            this.cExport = new ActionContainerBarItem();
            this.cExit = new ActionContainerBarItem();
            this.barSubItemEdit = new MainMenuItem();
            this.cUndoRedo = new ActionContainerBarItem();
            this.cEdit = new ActionContainerBarItem();
            this.cRecordEdit = new ActionContainerBarItem();
            this.cOpenObject = new ActionContainerBarItem();
            this.barSubItemView = new MainMenuItem();
            this.cPanels = new ActionContainerMenuBarItem();
            this.cViewsHistoryNavigation = new ActionContainerBarItem();
            this.cMenus = new ActionContainerMenuBarItem();
            this.cRecordsNavigation = new ActionContainerBarItem();
            this.cView = new ActionContainerBarItem();
            this.cReports = new ActionContainerBarItem();
            this.cDefault = new ActionContainerBarItem();
            this.cSearch = new ActionContainerBarItem();
            this.cFilters = new ActionContainerBarItem();
            this.cFullTextSearch = new ActionContainerBarItem();
            this.cAppearance = new ActionContainerMenuBarItem();
            this.barSubItemTools = new MainMenuItem();
            this.cTools = new ActionContainerMenuBarItem();
            this.cOptions = new ActionContainerMenuBarItem();
            this.cDiagnostic = new ActionContainerMenuBarItem();
            this.barSubItemWindow = new MainMenuItem();
            this.cWindows = new XafBarLinkContainerItem();
            this.barMdiChildrenListItem = new BarMdiChildrenListItem();
            this.Window = new ActionContainerBarItem();
            this.barSubItemHelp = new MainMenuItem();
            this.cHelp = new ActionContainerMenuBarItem();
            this.cAbout = new ActionContainerMenuBarItem();
            this.standardToolBar = new XafBar();
            this._statusBar = new XafBar();
            this.mainBarAndDockingController = new BarAndDockingController(this.components);
            this.barDockControlTop = new BarDockControl();
            this.barDockControlBottom = new BarDockControl();
            this.barDockControlLeft = new BarDockControl();
            this.barDockControlRight = new BarDockControl();
            this.mainDockManager = new DockManager(this.components);
            this.imageList1 = new ImageList(this.components);
            this.dockPanelMenus = new DockPanel();
            this.dockPanel1_Container = new ControlContainer();
            this.rootMenuItemsActionContainer1 = new MenuItemsActionContainer();
            this.cMenu = new ActionContainerBarItem();
            this.barSubItemPanels = new BarSubItem();
            this.viewSitePanel = new PanelControl();
            this.formStateModelSynchronizerComponent = new FormStateModelSynchronizer(this.components);
            ((ISupportInitialize)this.documentManager).BeginInit();
            ((ISupportInitialize)this.mainBarManager).BeginInit();
            ((ISupportInitialize)this.mainBarAndDockingController).BeginInit();
            ((ISupportInitialize)this.mainDockManager).BeginInit();
            this.dockPanelMenus.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            ((ISupportInitialize)this.viewSitePanel).BeginInit();
            base.SuspendLayout();
            this.actionsContainersManager.ActionContainerComponents.Add(this.cObjectsCreation);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cFile);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cSave);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cPrint);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cExport);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cExit);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cUndoRedo);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cEdit);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cRecordEdit);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cOpenObject);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cPanels);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cViewsHistoryNavigation);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cMenus);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cRecordsNavigation);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cView);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cReports);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cDefault);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cSearch);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cFilters);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cFullTextSearch);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cAppearance);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cTools);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cOptions);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cDiagnostic);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cAbout);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cMenu);
            this.actionsContainersManager.ActionContainerComponents.Add(this.Window);
            this.actionsContainersManager.ActionContainerComponents.Add(this.cHelp);
            this.actionsContainersManager.ActionContainerComponents.Add(this.rootMenuItemsActionContainer1);
            this.actionsContainersManager.ContextMenuContainers.Add(this.cObjectsCreation);
            this.actionsContainersManager.ContextMenuContainers.Add(this.cSave);
            this.actionsContainersManager.ContextMenuContainers.Add(this.cEdit);
            this.actionsContainersManager.ContextMenuContainers.Add(this.cRecordEdit);
            this.actionsContainersManager.ContextMenuContainers.Add(this.cOpenObject);
            this.actionsContainersManager.ContextMenuContainers.Add(this.cUndoRedo);
            this.actionsContainersManager.ContextMenuContainers.Add(this.cPrint);
            this.actionsContainersManager.ContextMenuContainers.Add(this.cView);
            this.actionsContainersManager.ContextMenuContainers.Add(this.cReports);
            this.actionsContainersManager.ContextMenuContainers.Add(this.cExport);
            this.actionsContainersManager.ContextMenuContainers.Add(this.cMenu);
            this.actionsContainersManager.DefaultContainer = this.cDefault;
            this.modelSynchronizationManager.ModelSynchronizableComponents.Add(this.formStateModelSynchronizerComponent);
            this.modelSynchronizationManager.ModelSynchronizableComponents.Add(this.mainBarManager);
            this.viewSiteManager.ViewSiteControl = this.viewSitePanel;
            this.mainBarManager.Bars.AddRange(new Bar[]
			{
				this._mainMenuBar,
				this.standardToolBar,
				this._statusBar
			});
            this.mainBarManager.Controller = this.mainBarAndDockingController;
            this.mainBarManager.DockControls.Add(this.barDockControlTop);
            this.mainBarManager.DockControls.Add(this.barDockControlBottom);
            this.mainBarManager.DockControls.Add(this.barDockControlLeft);
            this.mainBarManager.DockControls.Add(this.barDockControlRight);
            this.mainBarManager.DockManager = this.mainDockManager;
            this.mainBarManager.Form = this;
            this.mainBarManager.Items.AddRange(new BarItem[]
			{
				this.barSubItemFile,
				this.barSubItemEdit,
				this.barSubItemView,
				this.barSubItemTools,
				this.barSubItemHelp,
				this.cFile,
				this.cObjectsCreation,
				this.cPrint,
				this.cExport,
				this.cSave,
				this.cEdit,
				this.cOpenObject,
				this.cUndoRedo,
				this.cAppearance,
				this.cReports,
				this.cExit,
				this.cRecordEdit,
				this.cRecordsNavigation,
				this.cViewsHistoryNavigation,
				this.cSearch,
				this.cFullTextSearch,
				this.cFilters,
				this.cView,
				this.cDefault,
				this.cTools,
				this.cMenus,
				this.cDiagnostic,
				this.cOptions,
				this.cAbout,
				this.cWindows,
				this.cPanels,
				this.cMenu,
				this.barSubItemWindow,
				this.barMdiChildrenListItem,
				this.Window,
				this.barSubItemPanels,
				this.cHelp
			});
            this.mainBarManager.MainMenu = this._mainMenuBar;
            this.mainBarManager.MaxItemId = 37;
            this.mainBarManager.StatusBar = this._statusBar;
            this.mainBarManager.Disposed += new EventHandler(this.mainBarManager_Disposed);
            this._mainMenuBar.BarName = "Main Menu";
            this._mainMenuBar.DockCol = 0;
            this._mainMenuBar.DockRow = 0;
            this._mainMenuBar.DockStyle = BarDockStyle.Top;
            this._mainMenuBar.LinksPersistInfo.AddRange(new LinkPersistInfo[]
			{
				new LinkPersistInfo(this.barSubItemFile),
				new LinkPersistInfo(this.barSubItemEdit),
				new LinkPersistInfo(this.barSubItemView),
				new LinkPersistInfo(this.barSubItemTools),
				new LinkPersistInfo(this.barSubItemWindow),
				new LinkPersistInfo(this.barSubItemHelp)
			});
            this._mainMenuBar.OptionsBar.MultiLine = true;
            this._mainMenuBar.OptionsBar.UseWholeRow = true;
            this._mainMenuBar.TargetPageCategoryColor = Color.Empty;
            this._mainMenuBar.Text = "Main menu";
            this.barSubItemFile.Caption = "Файл";
            this.barSubItemFile.Id = 0;
            this.barSubItemFile.LinksPersistInfo.AddRange(new LinkPersistInfo[]
			{
				new LinkPersistInfo(this.cObjectsCreation, true),
				new LinkPersistInfo(this.cFile, true),
				new LinkPersistInfo(this.cSave, true),
				new LinkPersistInfo(this.cPrint, true),
				new LinkPersistInfo(this.cExport, true),
				new LinkPersistInfo(this.cExit, true)
			});
            this.barSubItemFile.MergeType = BarMenuMerge.MergeItems;
            this.barSubItemFile.Name = "barSubItemFile";
            this.barSubItemFile.VisibleInRibbon = false;
            this.cObjectsCreation.ApplicationMenuIndex = 1;
            this.cObjectsCreation.ApplicationMenuItem = true;
            this.cObjectsCreation.TargetPageGroupCaption = null;
            this.cObjectsCreation.Caption = "Создание объектов";
            this.cObjectsCreation.ContainerId = "ObjectsCreation";
            this.cObjectsCreation.Id = 18;
            this.cObjectsCreation.MergeType = BarMenuMerge.MergeItems;
            this.cObjectsCreation.Name = "cObjectsCreation";
            this.cObjectsCreation.TargetPageCategoryColor = Color.Empty;
            this.cFile.ApplicationMenuIndex = 2;
            this.cFile.ApplicationMenuItem = true;
            this.cFile.TargetPageGroupCaption = null;
            this.cFile.Caption = "Файл";
            this.cFile.ContainerId = "File";
            this.cFile.Id = 5;
            this.cFile.MergeOrder = 2;
            this.cFile.MergeType = BarMenuMerge.MergeItems;
            this.cFile.Name = "cFile";
            this.cFile.TargetPageCategoryColor = Color.Empty;
            this.cSave.ApplicationMenuIndex = 7;
            this.cSave.ApplicationMenuItem = true;
            this.cSave.TargetPageGroupCaption = null;
            this.cSave.Caption = "Сохранить";
            this.cSave.ContainerId = "Save";
            this.cSave.Id = 8;
            this.cSave.MergeType = BarMenuMerge.MergeItems;
            this.cSave.Name = "cSave";
            this.cSave.TargetPageCategoryColor = Color.Empty;
            this.cPrint.ApplicationMenuIndex = 11;
            this.cPrint.ApplicationMenuItem = true;
            this.cPrint.TargetPageGroupCaption = null;
            this.cPrint.Caption = "Печать";
            this.cPrint.ContainerId = "Print";
            this.cPrint.Id = 6;
            this.cPrint.MergeType = BarMenuMerge.MergeItems;
            this.cPrint.Name = "cPrint";
            this.cPrint.TargetPageCategoryColor = Color.Empty;
            this.cExport.ApplicationMenuIndex = 10;
            this.cExport.ApplicationMenuItem = true;
            this.cExport.TargetPageGroupCaption = null;
            this.cExport.Caption = "Экспорт";
            this.cExport.ContainerId = "Export";
            this.cExport.Id = 7;
            this.cExport.MergeType = BarMenuMerge.MergeItems;
            this.cExport.Name = "cExport";
            this.cExport.TargetPageCategoryColor = Color.Empty;
            this.cExit.ApplicationMenuIndex = 900;
            this.cExit.ApplicationMenuItem = true;
            this.cExit.TargetPageGroupCaption = null;
            this.cExit.Caption = "Выход";
            this.cExit.ContainerId = "Exit";
            this.cExit.Id = 8;
            this.cExit.MergeOrder = 900;
            this.cExit.Name = "cExit";
            this.cExit.TargetPageCategoryColor = Color.Empty;
            this.barSubItemEdit.Caption = "Правка";
            this.barSubItemEdit.Id = 1;
            this.barSubItemEdit.LinksPersistInfo.AddRange(new LinkPersistInfo[]
			{
				new LinkPersistInfo(this.cUndoRedo, true),
				new LinkPersistInfo(this.cEdit, true),
				new LinkPersistInfo(this.cRecordEdit, true),
				new LinkPersistInfo(this.cOpenObject, true)
			});
            this.barSubItemEdit.MergeType = BarMenuMerge.MergeItems;
            this.barSubItemEdit.Name = "barSubItemEdit";
            this.barSubItemEdit.VisibleInRibbon = false;
            this.cUndoRedo.Caption = "Отменить/Повторить";
            this.cUndoRedo.TargetPageGroupCaption = "Edit";
            this.cUndoRedo.ContainerId = "UndoRedo";
            this.cUndoRedo.Id = 10;
            this.cUndoRedo.MergeType = BarMenuMerge.MergeItems;
            this.cUndoRedo.Name = "cUndoRedo";
            this.cUndoRedo.TargetPageCategoryColor = Color.Empty;
            this.cEdit.TargetPageGroupCaption = null;
            this.cEdit.Caption = "Правка";
            this.cEdit.ContainerId = "Edit";
            this.cEdit.Id = 9;
            this.cEdit.MergeType = BarMenuMerge.MergeItems;
            this.cEdit.Name = "cEdit";
            this.cEdit.TargetPageCategoryColor = Color.Empty;
            this.cRecordEdit.TargetPageGroupCaption = null;
            this.cRecordEdit.Caption = "Изменить запись";
            this.cRecordEdit.ContainerId = "RecordEdit";
            this.cRecordEdit.Id = 9;
            this.cRecordEdit.MergeType = BarMenuMerge.MergeItems;
            this.cRecordEdit.Name = "cRecordEdit";
            this.cRecordEdit.TargetPageCategoryColor = Color.Empty;
            this.cOpenObject.TargetPageGroupCaption = null;
            this.cOpenObject.Caption = "Открыть объект";
            this.cOpenObject.ContainerId = "OpenObject";
            this.cOpenObject.Id = 9;
            this.cOpenObject.MergeType = BarMenuMerge.MergeItems;
            this.cOpenObject.Name = "cOpenObject";
            this.cOpenObject.TargetPageCategoryColor = Color.Empty;
            this.barSubItemView.Caption = "Вид";
            this.barSubItemView.Id = 2;
            this.barSubItemView.LinksPersistInfo.AddRange(new LinkPersistInfo[]
			{
				new LinkPersistInfo(this.cPanels, true),
				new LinkPersistInfo(this.cViewsHistoryNavigation, true),
				new LinkPersistInfo(this.cMenus, true),
				new LinkPersistInfo(this.cRecordsNavigation, true),
				new LinkPersistInfo(this.cView, true),
				new LinkPersistInfo(this.cReports, true),
				new LinkPersistInfo(this.cDefault, true),
				new LinkPersistInfo(this.cSearch, true),
				new LinkPersistInfo(BarLinkUserDefines.None, false, this.cFilters, true),
				new LinkPersistInfo(BarLinkUserDefines.None, false, this.cFullTextSearch, true),
				new LinkPersistInfo(this.cAppearance, true)
			});
            this.barSubItemView.MergeType = BarMenuMerge.MergeItems;
            this.barSubItemView.Name = "barSubItemView";
            this.cPanels.Caption = "Панели";
            this.cPanels.TargetPageGroupCaption = null;
            this.cPanels.TargetPageCaption = "View";
            this.cPanels.ContainerId = "Panels";
            this.cPanels.Id = 16;
            this.cPanels.MergeType = BarMenuMerge.MergeItems;
            this.cPanels.Name = "cPanels";
            this.cPanels.TargetPageCategoryColor = Color.Empty;
            this.cViewsHistoryNavigation.TargetPageGroupCaption = null;
            this.cViewsHistoryNavigation.Caption = "Просмотр истории навигации";
            this.cViewsHistoryNavigation.ContainerId = "ViewsHistoryNavigation";
            this.cViewsHistoryNavigation.Id = 35;
            this.cViewsHistoryNavigation.MergeType = BarMenuMerge.MergeItems;
            this.cViewsHistoryNavigation.Name = "cViewsHistoryNavigation";
            this.cViewsHistoryNavigation.TargetPageCategoryColor = Color.Empty;
            this.cMenus.TargetPageGroupCaption = null;
            this.cMenus.Caption = "Меню";
            this.cMenus.ContainerId = "Menus";
            this.cMenus.Id = 14;
            this.cMenus.MergeType = BarMenuMerge.MergeItems;
            this.cMenus.Name = "cMenus";
            this.cMenus.TargetPageCategoryColor = Color.Empty;
            this.cRecordsNavigation.TargetPageGroupCaption = null;
            this.cRecordsNavigation.Caption = "Записи навигации";
            this.cRecordsNavigation.ContainerId = "RecordsNavigation";
            this.cRecordsNavigation.Id = 10;
            this.cRecordsNavigation.MergeType = BarMenuMerge.MergeItems;
            this.cRecordsNavigation.Name = "cRecordsNavigation";
            this.cRecordsNavigation.TargetPageCategoryColor = Color.Empty;
            this.cView.TargetPageGroupCaption = null;
            this.cView.Caption = "Вид";
            this.cView.ContainerId = "View";
            this.cView.Id = 12;
            this.cView.MergeType = BarMenuMerge.MergeItems;
            this.cView.Name = "cView";
            this.cView.TargetPageCategoryColor = Color.Empty;
            this.cReports.ApplicationMenuIndex = 12;
            this.cReports.ApplicationMenuItem = true;
            this.cReports.TargetPageGroupCaption = "View";
            this.cReports.Caption = "Отчеты";
            this.cReports.ContainerId = "Reports";
            this.cReports.Id = 11;
            this.cReports.MergeType = BarMenuMerge.MergeItems;
            this.cReports.Name = "cReports";
            this.cReports.TargetPageCategoryColor = Color.Empty;
            this.cDefault.TargetPageGroupCaption = null;
            this.cDefault.Caption = "По умолчанию";
            this.cDefault.ContainerId = "Default";
            this.cDefault.Id = 50;
            this.cDefault.MergeType = BarMenuMerge.MergeItems;
            this.cDefault.Name = "cDefault";
            this.cDefault.TargetPageCategoryColor = Color.Empty;
            this.cSearch.TargetPageGroupCaption = null;
            this.cSearch.Caption = "Поиск";
            this.cSearch.ContainerId = "Search";
            this.cSearch.Id = 11;
            this.cSearch.MergeType = BarMenuMerge.MergeItems;
            this.cSearch.Name = "cSearch";
            this.cSearch.TargetPageCategoryColor = Color.Empty;
            this.cFilters.TargetPageGroupCaption = null;
            this.cFilters.Caption = "Фильтры";
            this.cFilters.ContainerId = "Filters";
            this.cFilters.Id = 26;
            this.cFilters.MergeType = BarMenuMerge.MergeItems;
            this.cFilters.Name = "cFilters";
            this.cFilters.TargetPageCategoryColor = Color.Empty;
            this.cFullTextSearch.Alignment = BarItemLinkAlignment.Right;
            this.cFullTextSearch.TargetPageGroupCaption = null;
            this.cFullTextSearch.Caption = "Полнотекстовый поиск";
            this.cFullTextSearch.ContainerId = "FullTextSearch";
            this.cFullTextSearch.Id = 12;
            this.cFullTextSearch.MergeType = BarMenuMerge.MergeItems;
            this.cFullTextSearch.Name = "cFullTextSearch";
            this.cFullTextSearch.TargetPageCategoryColor = Color.Empty;
            this.cAppearance.TargetPageGroupCaption = null;
            this.cAppearance.Caption = "Внешний вид";
            this.cAppearance.ContainerId = "Appearance";
            this.cAppearance.Id = 9;
            this.cAppearance.MergeType = BarMenuMerge.MergeItems;
            this.cAppearance.Name = "cAppearance";
            this.cAppearance.TargetPageCategoryColor = Color.Empty;
            this.barSubItemTools.Caption = "Инструменты";
            this.barSubItemTools.Id = 3;
            this.barSubItemTools.LinksPersistInfo.AddRange(new LinkPersistInfo[]
			{
				new LinkPersistInfo(this.cTools, true),
				new LinkPersistInfo(this.cOptions, true),
				new LinkPersistInfo(this.cDiagnostic, true)
			});
            this.barSubItemTools.MergeType = BarMenuMerge.MergeItems;
            this.barSubItemTools.Name = "barSubItemTools";
            this.cTools.TargetPageGroupCaption = null;
            this.cTools.Caption = "Инструменты";
            this.cTools.ContainerId = "Tools";
            this.cTools.Id = 13;
            this.cTools.MergeType = BarMenuMerge.MergeItems;
            this.cTools.Name = "cTools";
            this.cTools.TargetPageCategoryColor = Color.Empty;
            this.cOptions.TargetPageGroupCaption = null;
            this.cOptions.Caption = "Настройки";
            this.cOptions.ContainerId = "Options";
            this.cOptions.Id = 14;
            this.cOptions.MergeType = BarMenuMerge.MergeItems;
            this.cOptions.Name = "cOptions";
            this.cOptions.TargetPageCategoryColor = Color.Empty;
            this.cDiagnostic.TargetPageGroupCaption = null;
            this.cDiagnostic.Caption = "Диагностика";
            this.cDiagnostic.ContainerId = "Diagnostic";
            this.cDiagnostic.Id = 16;
            this.cDiagnostic.MergeType = BarMenuMerge.MergeItems;
            this.cDiagnostic.Name = "cDiagnostic";
            this.cDiagnostic.TargetPageCategoryColor = Color.Empty;
            this.barSubItemWindow.Caption = "Окно";
            this.barSubItemWindow.Id = 32;
            this.barSubItemWindow.LinksPersistInfo.AddRange(new LinkPersistInfo[]
			{
				new LinkPersistInfo(this.cWindows),
				new LinkPersistInfo(this.Window, true)
			});
            this.barSubItemWindow.MergeType = BarMenuMerge.MergeItems;
            this.barSubItemWindow.Name = "barSubItemWindow";
            this.cWindows.TargetPageCaption = "View";
            this.cWindows.TargetPageCategoryCaption = null;
            this.cWindows.Caption = "Окна";
            this.cWindows.TargetPageGroupCaption = null;
            this.cWindows.Id = 16;
            this.cWindows.LinksPersistInfo.AddRange(new LinkPersistInfo[]
			{
				new LinkPersistInfo(this.barMdiChildrenListItem)
			});
            this.cWindows.MergeType = BarMenuMerge.MergeItems;
            this.cWindows.Name = "cWindows";
            this.cWindows.TargetPageCategoryColor = Color.Empty;
            this.barMdiChildrenListItem.Caption = "Список окон";
            this.barMdiChildrenListItem.Id = 37;
            this.barMdiChildrenListItem.Name = "barMdiChildrenListItem";
            this.Window.TargetPageCaption = "View";
            this.Window.TargetPageGroupCaption = "Windows";
            this.Window.Caption = "Окно";
            this.Window.ContainerId = "Windows";
            this.Window.Id = 34;
            this.Window.MergeType = BarMenuMerge.MergeItems;
            this.Window.Name = "Window";
            this.Window.TargetPageCategoryColor = Color.Empty;
            this.barSubItemHelp.Caption = "Помощь";
            this.barSubItemHelp.Id = 4;
            this.barSubItemHelp.LinksPersistInfo.AddRange(new LinkPersistInfo[]
			{
				new LinkPersistInfo(this.cHelp, true),
				new LinkPersistInfo(this.cAbout, true)
			});
            this.barSubItemHelp.MergeType = BarMenuMerge.MergeItems;
            this.barSubItemHelp.Name = "barSubItemHelp";
            this.barSubItemHelp.VisibleInRibbon = false;
            this.cHelp.Caption = "Помощь";
            this.cHelp.TargetPageGroupCaption = null;
            this.cHelp.ContainerId = "Help";
            this.cHelp.Id = 36;
            this.cHelp.MergeType = BarMenuMerge.MergeItems;
            this.cHelp.Name = "cHelp";
            this.cHelp.TargetPageCategoryColor = Color.Empty;
            this.cAbout.ApplicationMenuIndex = 15;
            this.cAbout.ApplicationMenuItem = true;
            this.cAbout.Caption = "О программе";
            this.cAbout.TargetPageGroupCaption = null;
            this.cAbout.ContainerId = "About";
            this.cAbout.Id = 15;
            this.cAbout.MergeType = BarMenuMerge.MergeItems;
            this.cAbout.Name = "cAbout";
            this.cAbout.TargetPageCategoryColor = Color.Empty;
            this.standardToolBar.BarName = "Main Toolbar";
            this.standardToolBar.DockCol = 0;
            this.standardToolBar.DockRow = 1;
            this.standardToolBar.DockStyle = BarDockStyle.Top;
            this.standardToolBar.LinksPersistInfo.AddRange(new LinkPersistInfo[]
			{
				new LinkPersistInfo(this.cViewsHistoryNavigation, true),
				new LinkPersistInfo(this.cObjectsCreation, true),
				new LinkPersistInfo(this.cSave, true),
				new LinkPersistInfo(this.cEdit, true),
				new LinkPersistInfo(this.cUndoRedo, true),
				new LinkPersistInfo(this.cRecordEdit, true),
				new LinkPersistInfo(this.cOpenObject),
				new LinkPersistInfo(this.cView, true),
				new LinkPersistInfo(this.cReports),
				new LinkPersistInfo(this.cDefault, true),
				new LinkPersistInfo(this.cRecordsNavigation, true),
				new LinkPersistInfo(this.cFilters, true),
				new LinkPersistInfo(this.cSearch, true),
				new LinkPersistInfo(this.cFullTextSearch)
			});
            this.standardToolBar.OptionsBar.UseWholeRow = true;
            this.standardToolBar.TargetPageCategoryColor = Color.Empty;
            this.standardToolBar.Text = "Main Toolbar";
            this._statusBar.BarName = "StatusBar";
            this._statusBar.CanDockStyle = BarCanDockStyle.Bottom;
            this._statusBar.DockCol = 0;
            this._statusBar.DockRow = 0;
            this._statusBar.DockStyle = BarDockStyle.Bottom;
            this._statusBar.OptionsBar.AllowQuickCustomization = false;
            this._statusBar.OptionsBar.DisableClose = true;
            this._statusBar.OptionsBar.DisableCustomization = true;
            this._statusBar.OptionsBar.DrawDragBorder = false;
            this._statusBar.OptionsBar.DrawSizeGrip = true;
            this._statusBar.OptionsBar.UseWholeRow = true;
            this._statusBar.TargetPageCategoryColor = Color.Empty;
            this._statusBar.Text = "Status Bar";
            this.mainBarAndDockingController.PropertiesBar.AllowLinkLighting = false;
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Parent = this;
            this.barDockControlTop.Size = new Size(792, 51);
            this.barDockControlTop.Dock = DockStyle.Top;
            this.barDockControlTop.Location = new Point(0, 0);            
            // this.barDockControlTop.ZOrder = 6;
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Parent = this;
            this.barDockControlBottom.Size = new Size(792, 23);
            this.barDockControlBottom.Dock = DockStyle.Bottom;
            this.barDockControlBottom.Location = new Point(0, 543);
            // this.barDockControlBottom.ZOrder = 5;
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Parent = this;
            // this.barDockControlLeft.ZOrder = 3;
            this.barDockControlLeft.Location = new Point(0, 51);
            this.barDockControlLeft.Dock = DockStyle.Left;
            this.barDockControlLeft.Size = new Size(0, 492);
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Parent = this;
            // this.barDockControlRight.ZOrder = 4;
            this.barDockControlRight.Location = new Point(792, 51);
            this.barDockControlRight.Dock = DockStyle.Right;
            this.barDockControlRight.Size = new Size(0, 492);
            this.mainDockManager.Controller = this.mainBarAndDockingController;
            this.mainDockManager.Form = this;
            this.mainDockManager.Images = this.imageList1;
            this.mainDockManager.MenuManager = this.mainBarManager;
            this.mainDockManager.RootPanels.AddRange(new DockPanel[]
			{
				this.dockPanelMenus
			});
            this.mainDockManager.TopZIndexControls.AddRange(new string[]
			{
				"DevExpress.XtraBars.BarDockControl",
				"System.Windows.Forms.StatusBar",
				"DevExpress.ExpressApp.Win.Templates.Controls.XafRibbonControl",
				"DevExpress.XtraBars.Ribbon.RibbonStatusBar"
			});
            this.imageList1.ColorDepth = ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new Size(16, 16);
            this.imageList1.TransparentColor = Color.Transparent;
            this.dockPanelMenus.Controls.Add(this.dockPanel1_Container);
            this.dockPanelMenus.Dock = DockingStyle.Left;
            this.dockPanelMenus.ID = new Guid("17f71733-1ca6-4a29-aa2c-56cbcc2b43dd");
            this.dockPanelMenus.Size = new Size(200, 492);
            this.dockPanelMenus.Text = "Навигация";
            this.dockPanelMenus.Parent = this;
            this.dockPanelMenus.Location = new Point(0, 51);
            this.dockPanelMenus.Name = "dockPanelMenus";
            this.dockPanelMenus.Options.AllowDockBottom = false;
            this.dockPanelMenus.Options.AllowDockTop = false;
            this.dockPanelMenus.OriginalSize = new Size(200, 200);
            this.dockPanelMenus.TabStop = false;
            this.dockPanelMenus.Tag = "Menus";
            this.dockPanel1_Container.Controls.Add(this.rootMenuItemsActionContainer1);
            this.dockPanel1_Container.Parent = dockPanelMenus;
            this.dockPanel1_Container.Location = new Point(4, 23);
            this.dockPanel1_Container.Size = new Size(192, 465);            
            this.dockPanel1_Container.TabIndex = 0;
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.rootMenuItemsActionContainer1.Parent = dockPanel1_Container;
            this.rootMenuItemsActionContainer1.TabIndex = 0;
            this.rootMenuItemsActionContainer1.Size = new Size(192, 465);
            this.rootMenuItemsActionContainer1.Dock = DockStyle.Fill;
            this.rootMenuItemsActionContainer1.Location = new Point(0, 0);            
            this.rootMenuItemsActionContainer1.Name = "rootMenuItemsActionContainer1";
            this.cMenu.Caption = "Меню";
            this.cMenu.TargetPageGroupCaption = null;
            this.cMenu.ContainerId = "Menu";
            this.cMenu.Id = 7;
            this.cMenu.Name = "cMenu";
            this.cMenu.TargetPageCategoryColor = Color.Empty;
            this.barSubItemPanels.Caption = "Панели";
            this.barSubItemPanels.Id = 35;
            this.barSubItemPanels.Name = "barSubItemPanels";
            this.viewSitePanel.BorderStyle = BorderStyles.NoBorder;
            this.viewSitePanel.Parent = this;
            this.viewSitePanel.Location = new Point(200, 51);
            this.viewSitePanel.Size = new Size(592, 492);
            this.viewSitePanel.Dock = DockStyle.Fill;
            this.viewSitePanel.TabIndex = 4;
            this.viewSitePanel.Name = "viewSitePanel";
            this.formStateModelSynchronizerComponent.Form = this;
            this.formStateModelSynchronizerComponent.Model = null;
            this.Text = "MainForm";
            this.ClientSize = new Size(792, 566);
            // this.Margin = new Padding(3, 5, 3, 5);
            this.AutoScaleDimensions = new SizeF(6, 13);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.BarManager = this.mainBarManager;
            base.Controls.Add(this.viewSitePanel);
            base.Controls.Add(this.dockPanelMenus);
            base.Controls.Add(this.barDockControlLeft);
            base.Controls.Add(this.barDockControlRight);
            base.Controls.Add(this.barDockControlBottom);
            base.Controls.Add(this.barDockControlTop);
            base.IsMdiContainer = true;
            base.Name = "MenusMainForm";
            ((ISupportInitialize)this.documentManager).EndInit();
            ((ISupportInitialize)this.mainBarManager).EndInit();
            ((ISupportInitialize)this.mainBarAndDockingController).EndInit();
            ((ISupportInitialize)this.mainDockManager).EndInit();
            this.dockPanelMenus.ResumeLayout(false);
            this.dockPanel1_Container.ResumeLayout(false);
            ((ISupportInitialize)this.viewSitePanel).EndInit();
            base.ResumeLayout(false);
        }
    }
}
