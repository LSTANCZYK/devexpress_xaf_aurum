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
using DevExpress.ExpressApp.Model;
using Aurum.Operations.Model;
using Aurum.Operations.Templates;

namespace Aurum.Operations.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class AutoRefreshViewController : ViewController, IModelExtender
    {
        private const string UserVisibleKey = "UserVisibility";
        protected const string NotSupportedKeyName = "NotSupported";
        protected const string RefreshActionActiveKeyName = "RefreshActionActive";
        private int refreshTimeout = 5;

        public event EventHandler Refreshed;

        protected RefreshController RefreshController
        {
            get;
            private set;
        }

        public BoolList AutoRefreshActive
        {
            get;
            private set;
        }
        public int RefreshTimeout
        {
            get
            {
                return this.refreshTimeout;
            }
            set
            {
                if (this.refreshTimeout == value)
                {
                    return;
                }
                this.refreshTimeout = value;
                ((IModelViewAutoRefresh)base.View.Model).AutoRefreshTimeout = this.refreshTimeout;
                this.RestartAutoRefresh();
            }
        }

        public AutoRefreshViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            // Target required Views (via the TargetXXX properties) and create their Actions.
            this.RefreshTimeoutAction.Active.Changed += new EventHandler<EventArgs>(this.Active_Changed);
            this.AutoRefreshActive = new BoolList(true, BoolListOperatorType.Or);
            this.AutoRefreshActive.ResultValueChanged += new EventHandler<BoolValueChangedEventArgs>(this.AutoRefreshActive_Changed);
            base.Active["UserVisibility"] = false;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            this.StopAutoRefresh();
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void Frame_TemplateChanging(object sender, EventArgs e)
        {
            INotifyUserVisibilityChanged notifyUserVisibilityChanged = base.Frame.Template as INotifyUserVisibilityChanged;
            if (notifyUserVisibilityChanged == null)
            {
                return;
            }
            notifyUserVisibilityChanged.UserVisibilityChanged -= new EventHandler(this.notify_UserVisibilityChanged);
        }

        private void Frame_TemplateChanged(object sender, EventArgs e)
        {
            base.Active.BeginUpdate();
            try
            {
                base.Active["UserVisibility"] = true;
                INotifyUserVisibilityChanged notifyUserVisibilityChanged = base.Frame.Template as INotifyUserVisibilityChanged;
                if (notifyUserVisibilityChanged != null)
                {
                    notifyUserVisibilityChanged.UserVisibilityChanged += new EventHandler(this.notify_UserVisibilityChanged);
                    base.Active["UserVisibility"] = notifyUserVisibilityChanged.UserVisible;
                }
            }
            finally
            {
                base.Active.EndUpdate();
            }
        }

        private void notify_UserVisibilityChanged(object sender, EventArgs e)
        {
            base.Active["UserVisibility"] = ((INotifyUserVisibilityChanged)sender).UserVisible;
        }

        private void AutoRefreshActive_Changed(object sender, EventArgs e)
        {
            base.Active["AutoRefreshActive"] = this.AutoRefreshActive;
        }

        private void Active_Changed(object sender, EventArgs e)
        {
            if (this.RefreshTimeoutAction.Active)
            {
                this.UpdateSelectedItem();
            }
        }

        private void UpdateSelectedItem()
        {
            this.RefreshTimeoutAction.SelectedItem = this.RefreshTimeoutAction.Items.FirstOrDefault((ChoiceActionItem a) => a.Data.Equals(this.RefreshTimeout));
        }

        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            base.Frame.TemplateChanged += new EventHandler(this.Frame_TemplateChanged);
            base.Frame.TemplateChanging += new EventHandler(this.Frame_TemplateChanging);
            this.RefreshController = base.Frame.GetController<RefreshController>();
            if (this.RefreshController != null)
            {
                this.RefreshController.RefreshAction.Enabled.Changed += new EventHandler<EventArgs>(this.Enabled_Changed);
                this.RefreshController.RefreshAction.Active.Changed += new EventHandler<EventArgs>(this.Enabled_Changed);
            }
        }

        private void Enabled_Changed(object sender, EventArgs e)
        {
            this.UpdateActivity();
        }

        protected void UpdateActivity()
        {
            base.Active["RefreshActionActive"] = (this.RefreshController != null && this.RefreshController.RefreshAction.Enabled && this.RefreshController.RefreshAction.Active);
        }

        public virtual void DoRefresh()
        {
            this.RefreshController.RefreshAction.DoExecute();
            this.OnRefreshed(EventArgs.Empty);
        }

        protected void OnRefreshed(EventArgs e)
        {
            if (this.Refreshed != null)
            {
                this.Refreshed(this, e);
            }
        }

        protected override void OnViewChanged()
        {
            base.OnViewChanged();
            if (base.View == null)
            {
                return;
            }
            bool autoRefresh = false;
            if (base.View.Model != null && base.View.Model is IModelViewAutoRefresh)
            {
                autoRefresh = ((IModelViewAutoRefresh)base.View.Model).AutoRefresh;
            }
            base.Active["NotSupported"] = autoRefresh;
            if (!autoRefresh)
            {
                return;
            }
            this.RefreshTimeout = ((IModelViewAutoRefresh)base.View.Model).AutoRefreshTimeout;
            IModelOptionsAutoRefresh appModelAutoRefresh = ((IModelOptionsAutoRefreshNode)base.Application.Model.Options).AutoRefresh;
            this.RefreshTimeoutAction.Items.Clear();
            foreach (int current in from x in appModelAutoRefresh.AllowValues.Split(new char[] { ',' }).Select(int.Parse) orderby x select x)
            {
                this.RefreshTimeoutAction.Items.Add(new ChoiceActionItem(string.Format(appModelAutoRefresh.CaptionFormat, current), current));
            }
            this.RefreshTimeoutAction.Items.Add(new ChoiceActionItem(appModelAutoRefresh.NoneCaption, 0));
            this.UpdateSelectedItem();
        }

        private void repeatTimeAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            this.RefreshTimeout = (int)e.SelectedChoiceActionItem.Data;
        }

        protected virtual void StartAutoRefreshCore()
        {
        }

        public void StartAutoRefresh()
        {
            if (this.RefreshTimeout <= 0 || !base.Active)
            {
                return;
            }
            this.StartAutoRefreshCore();
        }

        public void StopAutoRefresh()
        {
            this.StopAutoRefreshCore();
        }

        protected virtual void StopAutoRefreshCore()
        {
        }

        public void RestartAutoRefresh()
        {
            if (!base.Active)
            {
                return;
            }
            this.StopAutoRefresh();
            this.StartAutoRefresh();
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelListView, IModelViewAutoRefresh>();
            extenders.Add<IModelDetailView, IModelViewAutoRefresh>();
            extenders.Add<IModelDashboardView, IModelViewAutoRefresh>();
            extenders.Add<IModelOptions, IModelOptionsAutoRefreshNode>();
        }
    }
}
