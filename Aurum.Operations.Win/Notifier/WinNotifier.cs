using DevExpress.XtraBars.Alerter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aurum.Operations.Win
{
    internal class WinNotifier : INotifier
    {
        public static Form Form { get; set; }

        public void Notify(string caption, string message, Action action)
        {
            if (Form == null)
            {
                return;
            }

            var ctr = new AlertControl();
            var info = new AlertInfo(caption, message);

            var handler = new AlertClickEventHandler(
                (o, e) =>
                {
                    if (action != null)
                    {
                        action.Invoke();
                    }
                }
            );

            ctr.AlertClick += handler;

            ctr.FormClosing += (o, e) =>
            {
                ctr.AlertClick -= handler;
            };

            Form.Invoke((Action)(() =>
            {
                ctr.Show(Form, info);
            }));
        }
    }
}
