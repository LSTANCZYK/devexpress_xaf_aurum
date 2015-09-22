using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aurum.Interface.Win.Controllers;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.Data.Filtering;

namespace Aurum.Interface.Win.Filters
{
    public partial class FilterWindowControl : UserControl
    {
        private const int offsetX = 3;
        private const int offsetY = 3;
        private int newHeight;
        private List<QueryGridFilterBase> formFilters;
        // Контрол, который получает фокус при показе формы
        private Control forceFocusedControl = null;

        public FilterWindowControl(List<FilterColumn> list)
        {
            InitializeComponent();
            formFilters = new List<QueryGridFilterBase>();
            generateLayout(list);

            this.ParentChanged += FilterWindowControl_ParentChanged;
        }

        private void generateLayout(List<FilterColumn> list)
        {
            if (list == null)
                return;
            int i = 0;
            int lastParamPosition = 0;
            foreach (var col in list)
            {
                // Фильтр
                QueryGridFilterBase filterControl = QueryGridFilterFactory.CreateFilter(col);
                if (filterControl == null)
                    continue;

                filterControl.FilterColumn = col;
                filterControl.Type = col.Type;
                filterControl.Value = (FilterValue)col.Value.Clone();
                filterControl.InternalValuesToExternalValues();
                filterControl.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                filterControl.Margin = new Padding(0, 0, 0, 0);
                formFilters.Add(filterControl);

                // Описание
                Label label = new Label();
                label.MinimumSize = new Size(90, 17);
                label.MaximumSize = new Size(90, 0);
                label.Size = new Size(90, 17);
                label.AutoSize = true;
                label.Margin = new Padding(0, 3, 0, 0);
                label.Text = col.Caption;                

                // Фильтр с описанием
                ControlPair labeledFilter = new ControlPair();
                labeledFilter.Dock = DockStyle.Fill;
                labeledFilter.Margin = new Padding(3, 2, 3, 1);
                labeledFilter.Height = filterControl.Height;
                labeledFilter.Control1 = label;
                labeledFilter.Control2 = filterControl;

                verticalLayoutPanel1.Controls.Add(labeledFilter);

                // Принудительная фокусировка определенного фильтра
                if (col.Focus && forceFocusedControl == null)
                    forceFocusedControl = filterControl;

                lastParamPosition += labeledFilter.Height + labeledFilter.Margin.Vertical;
                i++;
            }

            newHeight = offsetY + lastParamPosition + offsetY;
        }

        void FilterWindowControl_ParentChanged(object sender, EventArgs e)
        {
            Form form = FindForm();
            if (form != null)
            {
                form.Shown += form_Shown;
            }
            this.ParentChanged -= FilterWindowControl_ParentChanged;
        }

        void form_Shown(object sender, EventArgs e)
        {
            Form form = sender as Form;

            int dy = newHeight - Height;
            form.Height = form.Height + dy;

            var focusedColumn = formFilters.Where(c => c.FilterColumn.Focus).FirstOrDefault();
            if (focusedColumn != null)
            {
                var rowToFocus = focusedColumn;
                if (rowToFocus != null && formFilters[0] != rowToFocus)
                {
                    foreach (var row in formFilters)
                    {
                        if (row != rowToFocus)
                        {
                            row.Visible = false;
                        }
                    }

                    Task.Run(() =>
                        {
                            Task.Delay(100);
                            form.Invoke(new Action(() =>
                                {
                                    foreach (var row in formFilters)
                                    {
                                        if (row != rowToFocus)
                                        {
                                            row.Visible = true;
                                        }
                                    }
                                }));
                        });
                }
            }

            form.Shown -= form_Shown;
        }

        public List<Tuple<string, FilterValue>> GetValues()
        {
            return formFilters.Select(c => Tuple.Create(c.FilterColumn.Property, c.Value)).ToList();
        }

        public List<CriteriaOperator> GetCriteries()
        {
            return formFilters.Select(c => c.GetExpression()).ToList();
        }

        public void UpdateValues()
        {
            formFilters.ForEach(f => f.ExternalValuesToInternalValues());
        }
    }
}
