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
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base.ReportsV2;
using DevExpress.XtraReports.Native.Parameters;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.Wizards;

namespace Aurum.Reports
{
    /// <summary>
    /// Базовый класс конструктора отчетов
    /// </summary>
    public abstract class ReportBuilderBase 
    {
        private readonly IComponentFactory componentFactory;
        private readonly ITypesInfo typesInfo;
        private readonly XtraReport report;
        private readonly ReportWizard wizard;
        private readonly ReportStyle style;
        private XtraReportBase current;
        private int calculatedIndex = 0;
        private int columnIndex = 0;

        /// <summary>Конструктор</summary>
        /// <param name="componentFactory">Фабрика компонентов отчета</param>
        /// <param name="typesInfo">Справочник типов</param>
        /// <param name="report">Конструируемый отчет</param>
        /// <param name="wizard">Мастер отчетов, определяющий структуру конструируемого отчета</param>
        /// <param name="style">Стиль отчетов, определяющий внешний вид конструируемого отчета</param>
        public ReportBuilderBase(IComponentFactory componentFactory, ITypesInfo typesInfo, XtraReport report, ReportWizard wizard, ReportStyle style)
        {
            Guard.ArgumentNotNull(report, "report");
            Guard.ArgumentNotNull(componentFactory, "componentFactory");
            Guard.ArgumentNotNull(wizard, "wizard");
            this.componentFactory = componentFactory;
            this.typesInfo = typesInfo;
            this.report = report;
            this.wizard = wizard;
            this.style = style ?? new ReportStyle();
        }

        /// <summary>
        /// Мастер отчетов, определяющий конструкцию нового отчета
        /// </summary>
        public ReportWizard Wizard
        {
            get { return wizard; }
        }

        /// <summary>
        /// Стиль отчетов, определяющий внешний вид нового отчета
        /// </summary>
        public ReportStyle Style
        {
            get { return style; }
        }

        /// <summary>
        /// Основной отчет, подлежащий конструированию
        /// </summary>
        public XtraReport Report
        {
            get { return report; }
        }

        /// <summary>
        /// Отчет или вложенный отчет, который конструируется в данный момент
        /// </summary>
        protected XtraReportBase Current
        {
            get { return current; }
            set { current = value; }
        }

        /// <summary>
        /// Возвращает полосу отчета указанного типа
        /// </summary>
        /// <typeparam name="T">Тип полосы отчета</typeparam>
        /// <returns>Полоса отчета типа <b>T</b> или null, если не найдена</returns>
        protected T GetBand<T>()
            where T : Band
        {
            Type type = typeof(T);
            XtraReportBase parent = type == typeof(PageHeaderBand) || type == typeof(PageFooterBand) ? Report : Current;
			Band band = parent.Bands.GetBandByType(type);
            if (type == typeof(DetailReportBand))
            {
                band = new DetailReportBand();
                parent.Bands.Add(band);
            }
			else if (band == null || type == typeof(GroupFooterBand) || type == typeof(GroupHeaderBand)) 
            {
				band = (Band)CreateComponent(typeof(T));
                parent.Bands.Add(band);
			} 
			band.HeightF = 1;
			return (T)band;
        }

        /// <summary>
        /// Возвращает имя поля для связанных данных
        /// </summary>
        /// <param name="fieldName">Имя поля, к которому нужно привязать данные</param>
        /// <returns>Имя поля связанных данных</returns>
        protected string GetBindingFieldName(string fieldName)
        {
            return String.IsNullOrEmpty(Current.DataMember) ? fieldName : string.Format("{0}.{1}", Current.DataMember, fieldName);
        }

        /// <summary>
        /// Возвращает уникальное имя вычисляемого поля
        /// </summary>
        /// <returns>Уникальное имя вычисляемого поля</returns>
        protected virtual string GetCalculatedFieldName()
        {
            calculatedIndex++;
            return string.Concat("calc", calculatedIndex.ToString());
        }

        /// <summary>
        /// Возвращает уникальное имя колонки
        /// </summary>
        /// <returns>Уникальное имя колонки представления</returns>
        protected virtual string GetColumnName()
        {
            columnIndex++;
            return string.Concat("column", columnIndex.ToString());
        }

        /// <summary>
        /// Возвращает выражение условия для источника данных с учетом параметров
        /// </summary>
        /// <param name="criteria">Исходное выражение</param>
        /// <returns>Выражение условия с учетом параметров</returns>
        protected CriteriaOperator GetFilter(CriteriaOperator criteria)
        {
            return new ParametersReplacerProtected().UpgradeCriteria(criteria);
        }

        /// <summary>
        /// Устанавливает выражение условия данных для указанного отчета
        /// </summary>
        /// <param name="report">Отчет, для которого устанавливается условие</param>
        /// <param name="filter">Выражение условия данных (фильтр данных)</param>
        protected void SetFilter(XtraReportBase report, string filter)
        {
            if (report.DataSource is CollectionDataSource && !(report is DetailReportBand))
                ((CollectionDataSource)report.DataSource).CriteriaString = filter;
            else
                report.FilterString = filter;
        }

        /// <summary>
        /// Возвращает метаданные об указанном классе
        /// </summary>
        /// <param name="target">Тип класса, метаданные которого нужны</param>
        /// <returns>Метаданные класса <b>target</b> или null, если не найдены</returns>
        protected ITypeInfo GetTypeInfo(Type target)
        {
            return typesInfo != null ? typesInfo.FindTypeInfo(target) : null;
        }

        /// <summary>
        /// Возвращает метаданные о свойстве, которым представлено указанное выражения
        /// </summary>
        /// <param name="target">Тип данных, на основе которого построено выражение</param>
        /// <param name="expression">Выражение, которое предположительно представлено свойством</param>
        /// <returns>Метаданные свойства класса <b>target</b>, если выражение <b>expression</b> свойством, иначе null</returns>
        protected IMemberInfo GetMemberInfo(Type target, string expression)
        {
            ITypeInfo typeInfo = GetTypeInfo(target);
            if (typeInfo != null && target != null && expression != null &&
                expression.Length > 2 && expression.First() == '[' && expression.Last() == ']')
            {
                string memberName = expression.Substring(1, expression.Length - 2);
                if (typeInfo != null) return typeInfo.FindMember(memberName);
            }
            return null;
        }

        /// <summary>
        /// Создает источник данных для указанного отчета
        /// </summary>
        /// <param name="report">Отчет, для которого создается источник данных</param>
        /// <param name="query">Блок отчета, основанный на запросе, который является источником данных</param>
        protected void CreateDataSource(XtraReport report, ReportWizardQuery query)
        {
            CollectionDataSource dataSource = new CollectionDataSource();
            dataSource.ObjectTypeName = query.TargetObjectType.FullName;
            report.DataSource = dataSource;
            if (dataSource is IComponent)
                report.ComponentStorage.Add((IComponent)dataSource);
        }

        /// <summary>Создает компонент указанного типа</summary>
        /// <param name="type">Тип нового компонента</param>
        /// <returns>Компонент типа <b>type</b></returns>
        protected IComponent CreateComponent(Type type)
        {
            return componentFactory.Create(type);
        }

        /// <summary>Создает компонент указанного типа</summary>
        /// <typeparam name="T">Тип нового компонента</typeparam>
        /// <returns>Компонент типа <b>T</b></returns>
        protected T CreateComponent<T>()
            where T : IComponent
        {
            return (T)componentFactory.Create(typeof(T));
        }

        /// <summary>Создает текстовое поле</summary>
        /// <returns>Текстовое поле</returns>
        protected XRLabel CreateLabel()
        {
            return (XRLabel)CreateComponent(typeof(XRLabel));
        }

        /// <summary>Создает текстовое поле с указанным заголовком в указанной области отчета</summary>
        /// <param name="band">Область отчета, которой принадлежит создаваемое текстовое поле</param>
        /// <param name="caption">Заголовок текстового поля</param>
        /// <returns>Текстовое поле</returns>
        protected XRLabel CreateLabel(Band band, string caption)
        {
            XRLabel label = CreateLabel();
            band.Controls.Add(label);
            label.Text = caption;
            return label;
        }

        /// <summary>Создает логическое поле (чекбокс)</summary>
        /// <returns>Логическое поле (чекбокс)</returns>
        protected XRCheckBox CreateCheckBox()
        {
            return (XRCheckBox)CreateComponent(typeof(XRCheckBox));
        }

        /// <summary>Создает поле изображения</summary>
        /// <returns>Поле изображения</returns>
        protected XRPictureBox CreatePictureBox()
        {
            return (XRPictureBox)CreateComponent(typeof(XRPictureBox));
        }

        /// <summary>
        /// Создает контрол представления данных указанного типа
        /// </summary>
        /// <param name="dataType">Тип данных, который должен быть представлен контролом</param>
        /// <returns>Контрол, представляющий указанный тип данных</returns>
        protected virtual XRControl CreateDataControl(Type dataType)
        {
            if (typeof(bool).Equals(dataType)) return CreateCheckBox();
            if (typeof(Byte[]).Equals(dataType)) return CreatePictureBox();
            return CreateLabel();
        }

        /// <summary>
        /// Возвращает название свойства для связывания данных к указанному контролу
        /// </summary>
        /// <param name="control">Контрол, с которым связываются данные</param>
        /// <returns>Название свойства, по которому связываются данные</returns>
        protected virtual string GetDataBindingPropertyName(XRControl control)
        {
            if (control is XRLabel) return "Text";
            else if (control is XRCheckBox) return "CheckState";
            else if (control is XRPictureBox) return "Image";
            return "Text";
        }

        /// <summary>
        /// Добавляет связывание данных с указанным контролом
        /// </summary>
        /// <param name="control">Контрол, с которым связываются данные</param>
        /// <param name="field">Название свойства, связанного с данными, или вычисляемого поля, возвращающего данные</param>
        /// <remarks>Для получения названия свойства, связанного с данными, используйте метод <see cref="GetBindingFieldName"/>,
        /// для получения названия вычисляемого поля используйте метод <see cref="CreateCalculatedBinding"/>.</remarks>
        protected void AddDataBinding(XRControl control, string field)
        {
            if (control != null)
            {
                string propertyName = GetDataBindingPropertyName(control);
                XRBinding binding = new XRBinding(propertyName, Current.DataSource, field);
                control.DataBindings.Add(binding);
            }
        }

        /// <summary>
        /// Добавляет связывание параметра с указанным контролом
        /// </summary>
        /// <param name="control">Контрол, с которым связываются данные</param>
        /// <param name="parameter">Параметр, возвращающий данные</param>
        protected void AddParameterBinding(XRControl control, Parameter parameter)
        {
            if (control != null)
            {
                string propertyName = GetDataBindingPropertyName(control);
                XRBinding binding = new XRBinding(parameter, propertyName, string.Empty);
                control.DataBindings.Add(binding);
            }
        }

        /// <summary>
        /// Создает вычисляемое поле и добавляет его в отчет
        /// </summary>
        /// <param name="expression">Выражение вычисляемого поля</param>
        /// <returns>Вычисляемое поле с выражением <b>expression</b></returns>
        protected CalculatedField CreateCalculatedField(string expression)
        {
            CalculatedField calculated = new CalculatedField(Current.DataSource, Current.DataMember);
            calculated.Name = GetCalculatedFieldName();
            calculated.Expression = expression;
            XtraReport report = Current is XtraReport ? (XtraReport)Current : Report;
            report.CalculatedFields.Add(calculated);
            return calculated;
        }

        /// <summary>
        /// Создает вычисляемое поле для связываемых данных и добавляет его в отчет
        /// </summary>
        /// <param name="expression">Выражение вычисляемого поля</param>
        /// <returns>Имя вычисляемого поля для связываемых данных</returns>
        protected string CreateCalculatedBinding(string expression)
        {
            CalculatedField calculated = CreateCalculatedField(expression);
            return GetBindingFieldName(calculated.Name);
        }

        /// <summary>
        /// Возвращает правый нижний угол всех контролов в указанной области отчета
        /// </summary>
        /// <param name="band">Область отчета</param>
        /// <returns>Правый нижний угол всех контролов в области отчета <b>band</b></returns>
        protected Point RightBottom(Band band)
        {
            int right = 0, bottom = 0;
            foreach (XRControl control in band.Controls)
            {
                right = Math.Max(right, control.Right);
                bottom = Math.Max(bottom, control.Bottom);
            }
            return new Point(right, bottom);
        }

        /// <summary>
        /// Конвертирует единицы измерения конструктора отчетов (см) в единицы измерения конструируемого отчета
        /// </summary>
        /// <param name="units">Количество единиц измерения конструктора отчетов (см)</param>
        /// <returns>Количество единиц измерения конструируемого отчета, равнозначное <b>units</b></returns>
        protected int ConvertUnits(float units)
        {
            XtraReport report = Current as XtraReport ?? Report;
            switch (report.ReportUnit)
            {
                case ReportUnit.TenthsOfAMillimeter: 
                    return (int)(units * 100);
                case ReportUnit.HundredthsOfAnInch: 
                    const float hundredthsOfAnInch = (float)(100 / 2.54);
                    return (int)(units * hundredthsOfAnInch);
                case ReportUnit.Pixels: 
                    const float pixels = (float)(96 / 2.54);
                    return (int)(units * pixels);
            }
            throw new InvalidOperationException("Unknown type of report unit");
        }

        /// <summary>
        /// Конвертирует отступы стиля отчетов в отступы конструируемого отчета
        /// </summary>
        /// <param name="margins">Отступы стиля отчетов</param>
        /// <returns>Отступы в единицах иземерения конструируемого отчета</returns>
        protected Margins ConvertMargins(ReportStyleMargins margins)
        {
            margins = margins ?? new ReportStyleMargins();
            return new Margins(ConvertUnits(margins.Left), ConvertUnits(margins.Right), ConvertUnits(margins.Top), ConvertUnits(margins.Bottom)); 
        }

        /// <summary>
        /// Возвращает длину указанного текста в сантиметрах
        /// </summary>
        /// <param name="font">Шрифт текста</param>
        /// <param name="text">Текст</param>
        /// <returns>Длина текста <b>text</b> в сантиметрах (с запасом <i>for overhanging glyphs</i>)</returns>
        protected float GetTextWidth(Font font, string text)
        {
            SizeF size = new SizeF();
            using (Bitmap b = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(b))
            {
                g.PageUnit = GraphicsUnit.Millimeter;
                size = g.MeasureString(text, font);
            }
            return size.Width / 10;
        }

        /// <summary>
        /// Возвращает оптимальную длину текстового контрола для указанного количества символов
        /// </summary>
        /// <param name="label">Текстовый контрол</param>
        /// <param name="size">Количество символов</param>
        /// <returns>Оптимальная длина контрола в сантиметрах</returns>
        protected float GetLabelWidth(XRLabel label, int size)
        {
            return GetTextWidth(label.Font, new string('T', size));
        }

        /// <summary>Обработка блока отчета, включая дочерние блоки</summary>
        /// <param name="block">Блок отчета, входящий в конструкцию отчета</param>
        protected virtual void ProcessBlock(ReportWizardBlock block)
        {
            // Обработка блока
            if (block is ReportWizard)
                Process((ReportWizard)block);
            if (block is ReportWizardQuery)
                Process((ReportWizardQuery)block);

            // Обработка дочерних блоков
            XtraReportBase saveCurrent = Current;
            foreach (ReportWizardBlock child in block.Children)
            {
                ProcessBlock(child);
                Current = saveCurrent;
            }
        }

        /// <summary>Обработка основного блока отчета</summary>
        /// <param name="main">Основной блок отчета</param>
        protected abstract void Process(ReportWizard main);

        /// <summary>Обработка блока отчета, основанного на запросе</summary>
        /// <param name="query">Блок отчета, основанный на запросе</param>
        protected abstract void Process(ReportWizardQuery query);

        /// <summary>Выполняет конструирование отчета</summary>
        public void Execute()
        {
            bool snapToGrid = Report.SnapToGrid;
            try
            {
                Report.SnapToGrid = false;
                ExecuteCore();
            }
            finally
            {
                Report.SnapToGrid = snapToGrid;
            }
        }

        /// <summary>Конструирует отчет</summary>
        protected virtual void ExecuteCore()
        {
            Current = Report;
            ProcessBlock(Wizard);
        }

        class ParametersReplacerProtected : ParametersReplacer
        {
            public CriteriaOperator UpgradeCriteria(CriteriaOperator criteria) { return Process(criteria); }
        }
    }
}
