using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

namespace Aurum.Reports.Controllers
{
    /// <summary>
    /// Контроллер блоков мастера отчетов
    /// </summary>
    public partial class ReportWizardBlockController : ViewController
    {
        /// <summary>Конструктор</summary>
        public ReportWizardBlockController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        private ListView ListView { get { return View as ListView; } }

        // Перенос блока отчета в указанном направлении
        private void Move(ReportWizardBlock current, bool up)
        {
            if (current == null || current.Parent == null) return;
            BeginUpdate();
            ReportWizardBlock parent = current.Parent;
            int index = current.IndexOf + (up ? -1 : 1);

            // Вверх
            if (up)
            {
                if (index < 0 && parent.Parent != null)
                    parent.Parent.Children.Insert(parent.IndexOf, current);
                else if (index >= 0)
                {
                    parent = parent.Children[index];
                    while (parent.Children.Count > 0)
                        parent = parent.Children[parent.Children.Count - 1];
                    parent.Children.Add(current);
                }
            }
            // Вниз
            else
            {
                if (index >= parent.Children.Count && parent.Parent != null)
                    parent.Parent.Children.Insert(parent.IndexOf + 1, current);
                else if (index < parent.Children.Count)
                {
                    parent = parent.Children[index];
                    parent.Children.Insert(0, current);
                }
            }
            EndUpdate();
            ListView.CollectionSource.ResetCollection();
            ListView.CurrentObject = current;
        }

        // Перенос блока отчета вверх
        private void MoveUp_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            Move(e.CurrentObject as ReportWizardBlock, true);
        }

        // Перенос блока отчета вниз
        private void MoveDown_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            Move(e.CurrentObject as ReportWizardBlock, false);
        }
    }
}
