using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.XtraReports.UI;

namespace Aurum.Reports
{
    /// <summary>
    /// Блок мастера отчетов, основанный на запросе
    /// </summary>
    [DomainComponent, CreatableItem(true)]
    public class ReportWizardQuery : ReportWizardBlock
    {
        private ReportWizardItemList<ReportWizardColumn> columns;
        private ReportWizardItemList<ReportWizardCondition> conditions;
        private ReportWizardItemList<ReportWizardGroup> groups;
        private ReportWizardItemList<ReportWizardSort> sorts;

        /// <summary>Конструктор</summary>
        public ReportWizardQuery()
        {
            columns = new ReportWizardItemList<ReportWizardColumn>(this, "Columns");
            conditions = new ReportWizardItemList<ReportWizardCondition>(this, "Conditions");
            groups = new ReportWizardItemList<ReportWizardGroup>(this, "Groups");
            sorts = new ReportWizardItemList<ReportWizardSort>(this, "Sorting");
        }

        /// <inheritdoc/>
        protected override string DefaultName
        {
            get { return TargetObjectType != null ? string.Concat(base.DefaultName, " (", TargetObjectCaption, ")") : base.DefaultName; }
        }

        /// <summary>Колонки запроса</summary>
        public BindingList<ReportWizardColumn> Columns { get { return columns; } }

        /// <summary>Условия запроса</summary>
        public BindingList<ReportWizardCondition> Conditions { get { return conditions; } }
        
        /// <summary>Группы запроса</summary>
        public BindingList<ReportWizardGroup> Groups { get { return groups; } }
        
        /// <summary>Сортировка запроса</summary>
        public BindingList<ReportWizardSort> Sorting { get { return sorts; } }

        /// <inheritdoc/>
        protected internal override IList GetList(ReportWizardItem item)
        {
            if (item is ReportWizardColumn) return Columns;
            if (item is ReportWizardCondition) return Conditions;
            if (item is ReportWizardGroup) return Groups;
            if (item is ReportWizardSort) return Sorting;
            return base.GetList(item);
        }
        
        /// <inheritdoc/>
        public override bool Validate(out string errorMessage)
        {
            if (!columns.Validate(out errorMessage)) return false;
            if (!conditions.Validate(out errorMessage)) return false;
            if (!groups.Validate(out errorMessage)) return false;
            if (!sorts.Validate(out errorMessage)) return false;
            return base.Validate(out errorMessage);
        }
    }

    /// <summary>
    /// Колонка запроса мастера отчетов
    /// </summary>
    [DomainComponent, CreatableItem(true)]
    public class ReportWizardColumn : ReportWizardItem
    {
        private string expression;
        private SummaryFunc? summary;

        /// <summary>Выражение, возвращающее значение колонки</summary>
        [ElementTypeProperty("TargetObjectType")]
        [EditorAlias(ReportWizardItem.ExpressionPropertyEditorAlias), ModelDefault("RowCount", "0")]
        public string Expression
        {
            get { return expression; }
            set { SetPropertyValue("Expression", ref expression, value); }
        }

        /// <summary>Суммирование колонки в группах и общее по отчету</summary>
        public SummaryFunc? Summary 
        {
            get { return summary; }
            set { SetPropertyValue("Summary", ref summary, value); }
        }
    }

    /// <summary>
    /// Условие запроса мастера отчетов
    /// </summary>
    [DomainComponent, CreatableItem(true)]
    public class ReportWizardCondition : ReportWizardItem
    {
        private string expression;

        /// <summary>Выражение, определяющее условие запроса</summary>
        [ElementTypeProperty("TargetObjectType")]
        [EditorAlias(ReportWizardItem.ExpressionPropertyEditorAlias), ModelDefault("RowCount", "0")]
        public string Expression 
        {
            get { return expression; }
            set { SetPropertyValue("Expression", ref expression, value); }
        }

        /// <inheritdoc/>
        public override bool Validate(out string errorMessage)
        {
            if (string.IsNullOrEmpty(Expression)) { errorMessage = "Не указано выражение условия"; return false; }
            return base.Validate(out errorMessage);
        }
    }

    /// <summary>
    /// Группа запроса мастера отчетов
    /// </summary>
    [DomainComponent, CreatableItem(true)]
    public class ReportWizardGroup : ReportWizardItem
    {
        private string expression;
        private XRColumnSortOrder direction = XRColumnSortOrder.Ascending;
        private string descriptionExpression;

        /// <summary>Выражение, возвращающее значение группы</summary>
        [ElementTypeProperty("TargetObjectType")]
        [EditorAlias(ReportWizardItem.ExpressionPropertyEditorAlias), ModelDefault("RowCount", "0")]
        public string Expression
        {
            get { return expression; }
            set { SetPropertyValue("Expression", ref expression, value); }
        }

        /// <summary>Направление сортировки группы</summary>
        public XRColumnSortOrder Direction
        {
            get { return direction; }
            set { SetPropertyValue("Direction", ref direction, value); }
        }

        /// <summary>Выражение, возвращающее описание группы</summary>
        [ElementTypeProperty("TargetObjectType")]
        [EditorAlias(ReportWizardItem.ExpressionPropertyEditorAlias), ModelDefault("RowCount", "0")]
        public string DescriptionExpression
        {
            get { return descriptionExpression; }
            set { SetPropertyValue("DescriptionExpression", ref descriptionExpression, value); }
        }

        /// <inheritdoc/>
        public override bool Validate(out string errorMessage)
        {
            if (string.IsNullOrEmpty(Expression)) { errorMessage = "Не указано выражение группы"; return false; }
            return base.Validate(out errorMessage);
        }
    }

    /// <summary>
    /// Сортировка запроса мастера отчетов
    /// </summary>
    [DomainComponent, CreatableItem(true)]
    public class ReportWizardSort : ReportWizardItem
    {
        private string expression;
        private XRColumnSortOrder direction = XRColumnSortOrder.Ascending;

        /// <summary>Выражение, возвращающее значение сортировки</summary>
        [ElementTypeProperty("TargetObjectType")]
        [EditorAlias(ReportWizardItem.ExpressionPropertyEditorAlias), ModelDefault("RowCount", "0")]
        public string Expression
        {
            get { return expression; }
            set { SetPropertyValue("Expression", ref expression, value); }
        }

        /// <summary>Направление сортировки</summary>
        public XRColumnSortOrder Direction 
        { 
            get { return direction; }
            set { SetPropertyValue("Direction", ref direction, value); } 
        }

        /// <inheritdoc/>
        public override bool Validate(out string errorMessage)
        {
            if (string.IsNullOrEmpty(Expression)) { errorMessage = "Не указано выражение сортировки"; return false; }
            return base.Validate(out errorMessage);
        }
    }
}
