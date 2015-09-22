using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;

namespace Aurum.Reports.ModelUpdate
{
    /// <summary>
    /// Настройка списков элементов мастера отчетов в модели по умолчанию
    /// </summary>
    public class ReportWizardListViewUpdater : ModelNodesGeneratorUpdater<ModelListViewColumnsNodesGenerator>
    {
        /// <inheritdoc/>
        public override void UpdateNode(ModelNode node)
        {
            if (!(node is IModelColumns) || !(node.Parent is IModelListView)) return;
            IModelColumns columns = (IModelColumns)node;
            IModelListView listView = (IModelListView)node.Parent;
            Type objectType = listView.ModelClass.TypeInfo.Type;

            // Список элементов мастера отчета
            if (objectType.IsSubclassOf(typeof(ReportWizardItem)))
            {
                // Колонки списка
                string visibleProperty = objectType == typeof(ReportWizardBlock) ? "LevelName" : "Name";
                string sortProperty = objectType == typeof(ReportWizardBlock) ? "HierarchyOrder" : "IndexOf";
                foreach (IModelColumn column in columns)
                {
                    column.Index = column.PropertyName == visibleProperty ? 0 : -1;
                    column.SortIndex = column.PropertyName == sortProperty ? 0 : -1;
                    column.SortOrder = column.PropertyName == sortProperty ? ColumnSortOrder.Ascending : ColumnSortOrder.None;
                }

                // Спрятанные действия
                IModelViewHiddenActions hiddenActions = listView as IModelViewHiddenActions;
                if (hiddenActions != null)
                {
                    hiddenActions.HiddenActions.AddNode<IModelActionLink>().ActionId = "PreviousObject";
                    hiddenActions.HiddenActions.AddNode<IModelActionLink>().ActionId = "NextObject";
                }

                // Редактор элементов рядом со списком 
                // (для корректной работы редактирования неперсистентных объектов)
                listView.MasterDetailMode = MasterDetailMode.ListViewAndDetailView;
            }

            // Список групп стиля отчета
            if (objectType == typeof(ReportStyleGroup))
            {
                listView.MasterDetailMode = MasterDetailMode.ListViewAndDetailView;
            }
        }
    }
}
