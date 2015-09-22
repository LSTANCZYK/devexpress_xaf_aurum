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
    public abstract partial class AutoRefreshViewController : ViewController
    {
        private Timer timer = new Timer();

        protected double Interval
        {
            get { return timer.Interval; }
            set
            {
                timer.Interval = value;
                if (!timer.Enabled)
                {
                    timer.Start();
                }
            }
        }

        public AutoRefreshViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            timer.Elapsed += timer_Elapsed;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var ctr = Frame.Template as System.Windows.Forms.Control;
            if (ctr.Created)
            {
                ctr.Invoke((Action)(() =>
                {
                    // пытаемся безопасно обновить представление
                    try
                    {
                        View.Refresh();
                    }
                    catch
                    {
                    }
                }));
            }
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            timer.Elapsed -= timer_Elapsed;
        }
    }
}
