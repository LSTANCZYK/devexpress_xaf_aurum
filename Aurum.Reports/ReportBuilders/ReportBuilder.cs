using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base.ReportsV2;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Wizards;

namespace Aurum.Reports
{
    /// <summary>
    /// Стандартный конструктор отчета
    /// </summary>
    /// <todo>Реализация самостоятельных подотчетов</todo>
    public class ReportBuilder : ReportBuilderBase
    {
        //private int subreportIndex = 0;

        /// <summary>Конструктор</summary>
        /// <param name="componentFactory">Фабрика компонентов отчета</param>
        /// <param name="typesInfo">Справочник типов</param>
        /// <param name="report">Конструируемый отчет</param>
        /// <param name="wizard">Мастер отчетов, определяющий конструкцию отчета</param>
        /// <param name="style">Стиль отчетов, определяющий внешний вид конструируемого отчета</param>
        public ReportBuilder(IComponentFactory componentFactory, ITypesInfo typesInfo, XtraReport report, ReportWizard wizard, ReportStyle style)
            : base(componentFactory, typesInfo, report, wizard, style)
        {
        }

        /// <inheritdoc/>
        protected override void Process(ReportWizard main)
        {
            // Стиль страницы
            Report.ReportUnit = ReportUnit.TenthsOfAMillimeter;
            Report.Landscape = Style.Landscape;
            Report.Margins = ConvertMargins(Style.PageMargins);
            
            // Заголовок
            ReportHeaderBand header = GetBand<ReportHeaderBand>();
            XRLabel headerLabel = CreateLabel(header, main.Name);
            headerLabel.Font = Style.CaptionFont;
            headerLabel.Width = Current.Bounds.Width;

            // Размеры и позиция параметров
            int parameterCaptionWidth = 0;
            foreach (ReportWizardParameter parameter in main.Parameters)
            {
                if (parameter.VisibleInReportHeader)
                    parameterCaptionWidth = Math.Max(parameterCaptionWidth,
                        ConvertUnits(GetTextWidth(Style.ParameterFont, parameter.Caption + " : ")));
            }
            int parameterTop = headerLabel.Bottom + 1;

            // Параметры
            foreach (ReportWizardParameter parameter in main.Parameters)
            {
                // Параметр отчета, вводимый пользователем
                Parameter reportParameter = null;
                if (parameter.DataType != null)
                {
                    reportParameter = new Parameter();
                    reportParameter.Name = parameter.Name;
                    reportParameter.Type = parameter.DataType;
                    reportParameter.Description = parameter.Caption;
                    Report.Parameters.Add(reportParameter);
                }

                // Описание параметра
                if (parameter.VisibleInReportHeader && (reportParameter != null || !string.IsNullOrEmpty(parameter.DescriptionExpression)))
                {
                    XRLabel parameterCaption = CreateLabel(header, parameter.Caption + " : ");
                    parameterCaption.Font = Style.ParameterFont;
                    parameterCaption.Left = 0;
                    parameterCaption.Top = parameterTop;
                    parameterCaption.Width = parameterCaptionWidth;
                    XRLabel parameterDescription = CreateLabel(header, string.Empty);
                    if (!string.IsNullOrEmpty(parameter.DescriptionExpression))
                        AddDataBinding(parameterDescription, CreateCalculatedBinding(parameter.DescriptionExpression)); else
                        AddParameterBinding(parameterDescription, reportParameter);
                    parameterDescription.Left = parameterCaptionWidth;
                    parameterDescription.Top = parameterTop;
                    parameterDescription.Width = Current.Bounds.Width - parameterCaptionWidth;
                    parameterDescription.Font = Style.ParameterFont;
                    parameterTop += parameterDescription.Height + 1;
                }
            }
        }

        /// <inheritdoc/>
        protected override void Process(ReportWizardQuery query)
        {
            // Расположение блока в иерархии
            ReportWizardBlock parentQuery = query.Parent as ReportWizardQuery;
            string dataMember = null;
            if (parentQuery != null && parentQuery.TargetObjectType != null)
            {
                ITypeInfo typeInfo = GetTypeInfo(parentQuery.TargetObjectType);
                IMemberInfo memberInfo = typeInfo.Members.FirstOrDefault(mi => mi.ListElementType == query.TargetObjectType);
                dataMember = memberInfo != null ? memberInfo.Name : null;
            }
            
            // Детальный подотчет
            if (parentQuery != null && dataMember != null)
            {
                DetailReportBand detailReport = GetBand<DetailReportBand>();
                string dataMemberPrefix = !string.IsNullOrEmpty(Current.DataMember) ? Current.DataMember + "." : string.Empty;
                detailReport.DataSource = Current.DataSource;
                detailReport.DataMember = dataMemberPrefix + dataMember;
                Current = detailReport;
            }
            // Самостоятельный подотчет
            else if (parentQuery != null || query.IndexOf > 0)
            {
                throw new NotImplementedException("Subreport is not implemeted");

                // Когда попадает в report footer, то возникает ошибка при запуске отчета, а в дизайнере недоступен
                // Кроме этого два dataSource в основном отчете

                /*
                subreportIndex++;
                XtraReport subreport = null;
                switch (subreportIndex)
                {
                    case 1: subreport = new XtraReportSerialization.XtraReport1(); break;
                    case 2: subreport = new XtraReportSerialization.XtraReport2(); break;
                    case 3: subreport = new XtraReportSerialization.XtraReport3(); break;
                    case 4: subreport = new XtraReportSerialization.XtraReport4(); break;
                    case 5: subreport = new XtraReportSerialization.XtraReport5(); break;
                    case 6: subreport = new XtraReportSerialization.XtraReport6(); break;
                    case 7: subreport = new XtraReportSerialization.XtraReport7(); break;
                    case 8: subreport = new XtraReportSerialization.XtraReport8(); break;
                    default: throw new InvalidOperationException("Count of individual subreports is limited by 8");
                }
                subreport.ReportUnit = ReportUnit.TenthsOfAMillimeter;
                CreateDataSource(subreport, query);
                ReportFooterBand footer = GetBand<ReportFooterBand>();
                XRSubreport inplaceReport = new XRSubreport();
                inplaceReport.ReportSource = subreport;
                inplaceReport.Top = RightBottom(footer).Y + 1;
                footer.Controls.Add(subreport);
                Current = subreport;
                */
            }
            // Основной отчет
            else
            {
                CreateDataSource(Report, query);
            }

            // Колонки
            DetailBand detail = GetBand<DetailBand>();
            bool hasSummaries = false;
            foreach (ReportWizardColumn column in query.Columns)
            {
                // Тип значения и контрол
                IMemberInfo memberInfo = GetMemberInfo(column.TargetObjectType, column.Expression);
                Type columnDataType = memberInfo != null ? memberInfo.MemberType : typeof(string);
                XRControl columnData = CreateDataControl(columnDataType);
                detail.Controls.Add(columnData);
                columnData.Font = Style.ColumnFont;

                // Значение колонки
                if (!string.IsNullOrEmpty(column.Expression))
                {
                    string field = memberInfo != null ? GetBindingFieldName(memberInfo.Name) : CreateCalculatedBinding(column.Expression);
                    AddDataBinding(columnData, field);
                    column.Tag = columnData;
                }

                // Размер
                float width = 0;
                if (columnData is XRLabel && memberInfo != null && memberInfo.Size > 0)
                {
                    width = GetLabelWidth((XRLabel)columnData, memberInfo.Size);
                }
                if (width == 0) width = 2;
                if (width > 8) width = 8;
                columnData.Width = ConvertUnits(width);

                // Суммирование колонок
                hasSummaries |= column.Summary.HasValue;
            }

            // Расположение и итоговый размер колонок
            int columnMarginLeft = ConvertUnits(Style.ColumnMargins.Left);
            int columnMarginRight = ConvertUnits(Style.ColumnMargins.Right);
            int columnsWidth = query.Columns.Sum(c => ((XRControl)c.Tag).Width);
            int columnsRest = Current.Bounds.Width - columnsWidth - (query.Columns.Count - 1) * (columnMarginLeft + columnMarginRight) - 2;
            int columnLeft = columnMarginLeft;
            foreach (ReportWizardColumn column in query.Columns)
            {
                XRControl control = (XRControl)column.Tag;
                control.Left = columnLeft + columnMarginLeft;
                control.Width = control.Width + (int)(columnsRest * control.Width / columnsWidth);
                columnLeft += control.Width + columnMarginRight + columnMarginLeft;
            }

            // Заголовки колонок
            PageHeaderBand pageHeader = GetBand<PageHeaderBand>();
            int columnHeaderTop = (int)pageHeader.BottomF;
            foreach (ReportWizardColumn column in query.Columns)
            {
                XRControl control = (XRControl)column.Tag;
                XRLabel label = CreateLabel(pageHeader, column.Name);
                label.Top = columnHeaderTop;
                label.Left = control.Left;
                label.Width = control.Width;
                label.Font = Style.ColumnFont;
            }

            // Заголовок с названиями групп
            string groupsText = string.Join(" / ", query.Groups.Select(g => g.Name).ToArray());
            if (!string.IsNullOrEmpty(groupsText))
            {
                XRLabel label = CreateLabel(pageHeader, groupsText);
                label.Top = (int)pageHeader.BottomF + 1;
                label.Width = Current.Bounds.Width;
                label.Font = Style.ColumnFont;
            }

            // Условие запроса
            var conditions = query.Conditions;
            if (conditions.Count() > 0)
            {
                var filter = conditions.Select(condition => CriteriaOperator.Parse(condition.Expression));
                string criteriaString = GetFilter(filter.Count() == 1 ? filter.ElementAt(0) : CriteriaOperator.And(filter)).ToString();
                SetFilter(Current, criteriaString);
            }

            // Группировка записей
            int groupIndex = 0;
            foreach (ReportWizardGroup group in query.Groups)
            {
                GroupHeaderBand groupHeader = GetBand<GroupHeaderBand>();
                ReportStyleGroup groupStyle = groupIndex < Style.Groups.Count ? Style.Groups[groupIndex] : null;
                groupHeader.Level = 0;

                // Значение группы
                string groupField = CreateCalculatedBinding(group.Expression);
                groupHeader.GroupFields.Add(new GroupField(groupField, group.Direction));

                // Суммирование по группе
                if (hasSummaries)
                {
                    GroupFooterBand groupFooter = GetBand<GroupFooterBand>();
                    Font summaryFont = groupStyle != null ? groupStyle.SummaryFont : Style.SummaryFont;
                    CreateSummaries(query.Columns, groupFooter, SummaryRunning.Group, summaryFont);
                }

                // Заголовок группы
                XRLabel label = CreateLabel();
                groupHeader.Controls.Add(label);
                label.Left = columnMarginLeft + (groupStyle != null ? ConvertUnits(groupStyle.Left) : 0);
                label.Width = Current.Bounds.Width - label.Left;
                if (groupStyle != null) label.Font = groupStyle.CaptionFont;
                string description = !string.IsNullOrEmpty(group.DescriptionExpression) ?
                    CreateCalculatedBinding(group.DescriptionExpression) : groupField;
                AddDataBinding(label, description);

                // Индекс группы
                groupIndex++;
            }

            // Суммирование по отчету
            if (hasSummaries)
            {
                ReportFooterBand reportFooter = GetBand<ReportFooterBand>();
                CreateSummaries(query.Columns, reportFooter, SummaryRunning.Report, Style.SummaryFont);
            }

            // Сортировка записей
            foreach (ReportWizardSort sort in query.Sorting)
            {
                string name = CreateCalculatedBinding(sort.Expression);
                detail.SortFields.Add(new GroupField(name, sort.Direction));
            }
        }
        
        private void CreateSummaries(IEnumerable<ReportWizardColumn> columns, Band band, SummaryRunning running, Font summaryFont)
        {
            foreach (ReportWizardColumn column in columns)
                if (!string.IsNullOrEmpty(column.Expression) && column.Summary.HasValue && column.Tag is XRControl)
                {
                    XRControl control = (XRControl)column.Tag;
                    if (control.DataBindings.Count() == 0) continue;
                    XRLabel label = CreateLabel();
                    band.Controls.Add(label);
                    AddDataBinding(label, control.DataBindings[0].DataMember);
                    label.Summary.Running = running;
                    label.Summary.Func = column.Summary.Value;
                    label.Summary.IgnoreNullValues = true;
                    label.Left = control.Left;
                    label.Width = control.Width;
                    label.Font = summaryFont;
                }
        }
    }
}