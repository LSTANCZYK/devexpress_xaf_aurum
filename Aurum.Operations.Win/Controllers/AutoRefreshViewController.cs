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
using System.Timers;

namespace Aurum.Operations.Controllers
{
    /// <summary>
    ///  онтроллер дл€ поддержки автообновлени€ ListView
    /// </summary>
    public partial class AutoRefreshViewController : ViewController
    {
        // ¬нутренний таймер дл€ автообновлени€
        private Lazy<Timer> refreshTimer;

        private ChoiceActionItem autoRefreshItem;

        /// <summary>
        /// ¬ключено ли принудительное применение контроллера
        /// </summary>
        public bool ForceApply { get; set; }
        public int ForcePeriodMilliseconds { get; set; }

        public AutoRefreshViewController()
        {
            InitializeComponent();
            RegisterActions(components);

            Active["fuck"] = false;

            var item = new ChoiceActionItem("AutoRefreshDisabledActionItem", "Disabled", null);
            item.Data = 0;
            autoRefreshChoiceAction.Items.Add(item);

            refreshTimer = new Lazy<Timer>(() =>
            {
                return new Timer();
            });
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var winForm = Frame.Template as System.Windows.Forms.Control;
            if (winForm.Created)
            {
                winForm.Invoke((Action)(() =>
                {
                    ObjectSpace.Refresh();
                }));
            }
        }

        private void RegularOnViewChanged()
        {
            var viewModel = View.Model as IModelAutoRefreshListViewExtension;

            if (viewModel == null)
            {
                return;
            }

            var appModel = Application.Model as IModelAutoRefreshAppExtension;

            if (!viewModel.AutoRefreshEnabled)
            {
                autoRefreshChoiceAction.Active["Active"] = false;
                return;
            }

            int viewValue = 0;
            bool viewValueIsSet = Int32.TryParse(viewModel.AutoRefreshTime, out viewValue);

            // «аполнение возможных значений периодов автообновлени€
            foreach (var v in appModel.AllowedAutoRefreshValuesList)
            {
                int value = 0;
                if (Int32.TryParse(v, out value) && value > 0)
                {
                    var item = new ChoiceActionItem();
                    item.Caption = v;
                    item.Data = value * 1000;
                    autoRefreshChoiceAction.Items.Add(item);
                }
            }

            // ќбработка собственного значени€ периода автообновлени€ вида
            if (viewValueIsSet)
            {
                var viewValueMs = viewValue * 1000;
                var appropriateItem = autoRefreshChoiceAction.Items.Where(x => (int)x.Data == viewValueMs).FirstOrDefault();
                if (appropriateItem != null)
                {
                    autoRefreshItem = appropriateItem;
                }
                else
                {
                    // ≈сли собственного значени€ нет в возможных значени€х, то создать специальный ChoiceActionItem дл€ такого периода
                    var item = new ChoiceActionItem();
                    item.Caption = viewValue.ToString();
                    item.Data = viewValueMs;
                    autoRefreshChoiceAction.Items.Add(item);
                    autoRefreshItem = item;
                }
            }
        }

        protected override void OnViewChanged()
        {
            base.OnViewChanged();

            if (!ForceApply)
            {
                RegularOnViewChanged();
            }
        }

        protected override void OnAfterConstruction()
        {
            base.OnAfterConstruction();
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            if (ForceApply)
            {
                setAutoRefresh(ForcePeriodMilliseconds);
            }
            else
            {
                // ќбработка установленного значени€ периода автообновлени€
                if (autoRefreshItem != null)
                {
                    autoRefreshChoiceAction.SelectedItem = autoRefreshItem;
                    autoRefreshChoiceAction.DoExecute(autoRefreshItem);
                }
            }

            if (refreshTimer.IsValueCreated)
            {
                refreshTimer.Value.Elapsed += timer_Elapsed;
            }
        }

        protected override void OnViewControllersActivated()
        {
            base.OnViewControllersActivated();
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            if (refreshTimer.IsValueCreated)
            {
                refreshTimer.Value.Elapsed -= timer_Elapsed;
            }
        }

        private void autoRefreshChoiceAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            var item = e.SelectedChoiceActionItem;
            var value = (Int32)item.Data;
            setAutoRefresh(value);
        }

        private void setAutoRefresh(int period)
        {
            if (period > 0)
            {
                refreshTimer.Value.Interval = period;
                if (!refreshTimer.Value.Enabled)
                {
                    refreshTimer.Value.Enabled = true;
                }
            }
            else
            {
                refreshTimer.Value.Enabled = false;
            }
        }
    }
}
