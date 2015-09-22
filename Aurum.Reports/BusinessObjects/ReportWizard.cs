using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Xml.Serialization;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.Wizards;

namespace Aurum.Reports
{
    /// <summary>
    /// Мастер отчетов, содержащий заголовок и параметры отчета
    /// </summary>
    /// <todo>Создание и просмотр отчета без сохранения</todo>
    /// <todo>Иконка</todo>
    [DomainComponent]
    public class ReportWizard : ReportWizardBlock
    {
        private ReportWizardItemList<ReportWizardParameter> parameters;
        private EventHandler changed;

        /// <summary>Конструктор</summary>
        public ReportWizard()
        {
            parameters = new ReportWizardItemList<ReportWizardParameter>(this, "Parameters");
        }

        /// <summary>
        /// Все дочерние блоки мастера отчетов
        /// </summary>
        [XmlIgnore]
        public BindingList<ReportWizardBlock> ChildrenList
        {
            get
            {
                List<ReportWizardBlock> list = new List<ReportWizardBlock>();
                GetChildrenTree(list, this);
                return new ReportWizardItemList<ReportWizardBlock>(this, list, "ChildrenList");
            }
        }

        private void GetChildrenTree(List<ReportWizardBlock> list, ReportWizardBlock block)
        {
            list.AddRange(block.Children);
            foreach (ReportWizardBlock child in block.Children) GetChildrenTree(list, child);
        }

        /// <summary>Параметры мастера отчетов</summary>
        public BindingList<ReportWizardParameter> Parameters
        {
            get { return parameters; }
        }

        /// <inheritdoc/>
        protected internal override IList GetList(ReportWizardItem item)
        {
            if (item is ReportWizardParameter) return Parameters;
            return base.GetList(item);
        }

        /// <inheritdoc/>
        public override void FillParameters(Dictionary<string, string> table)
        {
            foreach (ReportWizardParameter parameter in Parameters)
                if (parameter.DataType != null) table.Add(parameter.FormulaName, parameter.Caption);
        }

        /// <summary>
        /// Генерация отчета с сохранением в указанном объектом данных
        /// </summary>
        /// <param name="typesInfo">Справочник типов</param>
        /// <param name="reportData">Объект данных, используемый для хранения отчета</param>
        /// <param name="reportStyle">Стиль для конструируемого отчета</param>
        /// <returns>Компонент отчета, сохраненный в объекте данных <b>reportData</b></returns>
        public XtraReport Generate(ITypesInfo typesInfo, IReportDataV2Writable reportData, ReportStyle reportStyle)
        {
            reportData.SetDisplayName(this.Name);
            reportData.SetDataType(this.DataType);
            XtraReport report = ReportDataProvider.ReportsStorage.LoadReport(reportData);

            // Построение отчета
            using (XRDesignFormEx form = new XRDesignFormEx())
            {
                form.OpenReport(report);
                IDesignerHost designerHost = (IDesignerHost)report.Site.GetService(typeof(IDesignerHost));
                ReportBuilderBase builder = CreateBuilder(new ComponentFactory(designerHost), typesInfo, report, reportStyle);
                builder.Execute();
            }

            // Сохранение содержания
            using (MemoryStream stream = new MemoryStream())
            {
                report.SaveLayout(stream, true);
                stream.Position = 0;
                reportData.SetContent(stream.ToArray());
                stream.Close();
            }

            return report;
        }

        /// <summary>
        /// Создает конструктор отчета для текущего мастера
        /// </summary>
        /// <param name="componentFactory">Фабрика компонентов</param>
        /// <param name="typesInfo">Справочник типов</param>
        /// <param name="report">Конструируемый отчет</param>
        /// <param name="style">Стиль для конструируемого отчета</param>
        /// <returns>Конструктор отчета</returns>
        protected virtual ReportBuilderBase CreateBuilder(IComponentFactory componentFactory, ITypesInfo typesInfo, XtraReport report, ReportStyle style)
        {
            return new ReportBuilder(componentFactory, typesInfo, report, this, style);
        }

        /// <summary>
        /// Вызов события изменения мастера отчетов
        /// </summary>
        protected override void RaiseWizardChanged()
        {
            if (changed != null)
                changed(this, EventArgs.Empty);
        }

        /// <summary>
        /// Событие изменения мастера отчетов
        /// </summary>
        public event EventHandler WizardChanged
        {
            add { changed += value; }
            remove { changed -= value; }
        }
    }

    /// <summary>
    /// Параметр мастера отчетов
    /// </summary>
    [DomainComponent, CreatableItem(true)]
    public class ReportWizardParameter : ReportWizardItem
    {
        /// <summary>Конструктор</summary>
        public ReportWizardParameter()
        {
            VisibleInReportHeader = true;
        }

        /// <inheritdoc/>
        protected override string DefaultName 
        { 
            get { return string.Concat("parameter", IndexOf.ToString()); } 
        }

        /// <summary>Тип данных параметра</summary>
        [TypeConverter(typeof(ReportDataTypeConverter))] // IsVisibleInReports
        [ValueConverter(typeof(TypeToStringConverter))]
        [XmlIgnore]
        public Type DataType { get; set; }

        /// <summary>Сериализация типа данных</summary>
        [XmlElement("DataType"), Browsable(false)]
        public string DataTypeSerializable 
        { 
            get { return TypeToString(DataType); } 
            set { DataType = StringToType(value); } 
        }

        /// <summary>Заголовок (краткое описание)</summary>
        public string Caption { get; set; }

        /// <summary>Название параметра в выражениях формул</summary>
        [Browsable(false)]
        public string FormulaName { get { return string.Concat("[Parameters.", Name, "]"); } }

        /// <summary>Признак видимости в заголовке отчета</summary>
        public bool VisibleInReportHeader { get; set; }

        /// <summary>Выражение, возвращающее описание параметра в заголовке отчета</summary>
        [ElementTypeProperty("TargetObjectType")]
        [EditorAlias(ReportWizardItem.ExpressionPropertyEditorAlias), ModelDefault("RowCount", "0")]
        public string DescriptionExpression { get; set; }
    }

    /// <summary>
    /// Валидатор мастера отчетов
    /// </summary>
    [CodeRule]
    public class ReportWizardValidationRule : RuleBase<ReportWizard>
    {
        /// <summary>Конструктор</summary>
        public ReportWizardValidationRule() : base("ReportWizardValidationRule", ContextIdentifier.Save) { }

        /// <summary>Конструктор</summary>
        /// <param name="properties">Валидируемые свойства</param>
        public ReportWizardValidationRule(IRuleBaseProperties properties) : base(properties) { }

        /// <inheritdoc/>
        protected override bool IsValidInternal(ReportWizard target, out string errorMessageTemplate)
        {
            errorMessageTemplate = string.Empty;
            if (target == null) return true;
            return target.Validate(out errorMessageTemplate);
        }
    }
}
