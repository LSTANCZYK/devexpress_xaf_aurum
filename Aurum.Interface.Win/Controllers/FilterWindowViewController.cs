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
using Aurum.Interface.Win.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using Aurum.Interface.Model;
using Aurum.Interface.Win.Filters;

namespace Aurum.Interface.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class FilterWindowViewController : ViewController
    {
        public FilterWindowViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            // Target required Views (via the TargetXXX properties) and create their Actions.
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
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void filterWindowShowAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IModelListView model = (IModelListView)View.Model;

            List<FilterColumn> columns = model.Columns
                .Where(c => c is IModelColumnAurumFilter && ((IModelColumnAurumFilter)c).ShowFilter)
                .OrderBy(c => ((IModelColumnAurumFilter)c).FilterIndex)
                .Select(c => new FilterColumn()
                {
                    Type = c.ModelMember.Type, 
                    Caption = ((IModelColumnAurumFilter)c).FilterCaption,
                    Property = c.PropertyName,
                    Focus = ((IModelColumnAurumFilter)c).Focus
                }).ToList();

            // значения
            ListView listView = View as ListView;
            if (listView != null)
            {
                GridListEditor editor = listView.Editor as GridListEditor;
                if (editor != null)
                {
                    GridControl grid = editor.Grid;
                    ColumnView colView = grid.FocusedView as ColumnView;
                    if (colView != null)
                    {
                        var criteria = colView.ActiveFilterCriteria;
                        if (colView.Tag != null && colView.Tag is List<Tuple<string, FilterValue>>)
                        {
                            var oldValues = (List<Tuple<string, FilterValue>>)colView.Tag;
                            oldValues.ForEach(t =>
                            {
                                var col = columns.Find(c => c.Property == t.Item1);
                                if (col != null)
                                {
                                    col.Value = t.Item2;
                                }
                            });
                        }
                    }
                }
            }
            columns.ForEach(c => { if (c.Value == null) c.Value = QueryGridFilterFactory.CreateEmptyValue(c); });

            e.View = new FilterWindowView(View.ObjectSpace, View.Model, false, columns);
            e.View.Caption = "Фильтр";
        }

        private void filterWindowShowAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            FilterWindowView filter = e.PopupWindowView as FilterWindowView;

            ListView listView = View as ListView;
            if (listView != null)
            {
                filter.UpdateValues();

                List<CriteriaOperator> ops = new List<CriteriaOperator>();
                foreach (var cr in filter.GetCriteries())
                {
                    if (!ReferenceEquals(cr, null))
                        ops.Add(cr);
                }
                CriteriaOperator criteria = CriteriaOperator.And(ops);
                GridListEditor editor = listView.Editor as GridListEditor;
                if (editor != null)
                {
                    GridControl grid = editor.Grid;
                    ColumnView colView = grid.FocusedView as ColumnView;
                    if (colView != null)
                    {
                        colView.ActiveFilterCriteria = criteria;
                        colView.Tag = filter.GetValues();
                    }
                }
            }
        }

        private void clearFilterAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ListView listView = View as ListView;
            if (listView != null)
            {
                GridListEditor editor = listView.Editor as GridListEditor;
                if (editor != null)
                {
                    GridControl grid = editor.Grid;
                    ColumnView colView = grid.FocusedView as ColumnView;
                    if (colView != null)
                    {
                        colView.ActiveFilterCriteria = null;
                        colView.Tag = null;
                    }
                }
            }
        }
    }

    public class FilterColumn
    {
        public string Property { get; set; }
        public Type Type { get; set; }
        public string Caption { get; set; }
        public FilterValue Value { get; set; }
        public bool Focus { get; set; }
    }
}
