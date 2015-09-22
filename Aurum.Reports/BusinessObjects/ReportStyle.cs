using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

namespace Aurum.Reports
{
    /// <summary>
    /// Стиль отчетов
    /// </summary>
    /// <todo>Сохранение стилей (<c>PredefinedReportStyle</c>)</todo>
    [DomainComponent, Serializable]
    public class ReportStyle : IDisposable
    {
        private BindingList<ReportStyleGroup> groups;

        /// <summary>
        /// Конструктор
        /// </summary>
        public ReportStyle()
        {
            Landscape = false;
            CaptionFont = new Font("Times New Roman", 14, FontStyle.Bold);
            ParameterFont = new Font("Times New Roman", 10);
            PageMargins = new ReportStyleMargins(2, 1, 1, 1);
            ColumnMargins = new ReportStyleMargins(0, 0.1f, 0, 0);
            ColumnFont = new Font("Times New Roman", 10);
            SummaryFont = new Font("Times New Roman", 10, FontStyle.Bold);

            // Группы отчета
            groups = new BindingList<ReportStyleGroup>();
            groups.Add(new ReportStyleGroup(0.0f, new Font("Times New Roman", 12, FontStyle.Bold), SummaryFont));
            groups.Add(new ReportStyleGroup(0.5f, new Font("Times New Roman", 10, FontStyle.Bold), SummaryFont));
            groups.Add(new ReportStyleGroup(1.0f, new Font("Times New Roman", 10, FontStyle.Regular), SummaryFont));
            groups.Add(new ReportStyleGroup(1.5f, new Font("Times New Roman", 10, FontStyle.Italic), SummaryFont));
        }

        /// <summary>Альбомная ориентация</summary>
        public bool Landscape { get; set; }

        /// <summary>Шрифт заголовка отчета</summary>
        public Font CaptionFont { get; set; }

        /// <summary>Шрифт параметра</summary>
        public Font ParameterFont { get; set; }

        /// <summary>Отступы страницы</summary>
        [ExpandObjectMembers(ExpandObjectMembers.Always)]
        public ReportStyleMargins PageMargins { get; set; }

        /// <summary>Отступы колонок</summary>
        [ExpandObjectMembers(ExpandObjectMembers.Always)]
        public ReportStyleMargins ColumnMargins { get; set; }

        /// <summary>Шрифт колонок</summary>
        public Font ColumnFont { get; set; }

        /// <summary>Шрифт суммарных значений</summary>
        public Font SummaryFont { get; set; }

        /// <summary>Стиль групп отчета</summary>
        public BindingList<ReportStyleGroup> Groups { get { return groups; } }

        #region IDisposable

        /// <contentfrom cref="System.IDisposable.Dispose"/>
        public void Dispose()
        {
            if (CaptionFont != null) { CaptionFont.Dispose(); CaptionFont = null; }
            if (ParameterFont != null) { ParameterFont.Dispose(); ParameterFont = null; }
            if (ColumnFont != null) { ColumnFont.Dispose(); ColumnFont = null; }
            if (SummaryFont != null) { SummaryFont.Dispose(); SummaryFont = null; }
            foreach (ReportStyleGroup group in Groups) group.Dispose();
        }

        #endregion
    }

    /// <summary>
    /// Отступы стиля отчетов
    /// </summary>
    [DomainComponent, Serializable]
    public class ReportStyleMargins
    {
        /// <summary>Конструктор без параметров</summary>
        public ReportStyleMargins() { }
        
        /// <summary>Конструктор с указанными значениями</summary>
        /// <param name="left">Левый отступ</param>
        /// <param name="right">Правый отступ</param>
        /// <param name="top">Верхний отступ</param>
        /// <param name="bottom">Нижний отступ</param>
        public ReportStyleMargins(float left, float right, float top, float bottom)
        {
            Left = left; Right = right; Top = top; Bottom = bottom;
        }

        /// <summary>Левый отступ</summary>
        public float Left { get; set; }
        /// <summary>Правый отступ</summary>
        public float Right { get; set; }
        /// <summary>Верхний отступ</summary>
        public float Top { get; set; }
        /// <summary>Нижний отступ</summary>
        public float Bottom { get; set; }
    }

    /// <summary>
    /// Стиль группы в отчете
    /// </summary>
    [DomainComponent, Serializable]
    public class ReportStyleGroup : IDisposable
    {
        /// <summary>Конструктор без параметров</summary>
        public ReportStyleGroup() { }

        /// <summary>
        /// Конструктор с указанными значениями
        /// </summary>
        /// <param name="left">Левый отступ</param>
        /// <param name="captionFont">Шрифт заголовка</param>
        /// <param name="summaryFont">Шрифт суммарных значений</param>
        public ReportStyleGroup(float left, Font captionFont, Font summaryFont) 
        {
            Left = left;
            CaptionFont = captionFont;
            SummaryFont = summaryFont;
        }

        /// <summary>Левый отступ</summary>
        public float Left { get; set; }

        /// <summary>Шрифт заголовка</summary>
        public Font CaptionFont { get; set; }

        /// <summary>Шрифт суммарных значений</summary>
        public Font SummaryFont { get; set; }

        #region

        /// <contentfrom cref="System.IDisposable.Dispose"/>
        public void Dispose()
        {
            if (CaptionFont != null) { CaptionFont.Dispose(); CaptionFont = null; }
            if (SummaryFont != null) { SummaryFont.Dispose(); SummaryFont = null; }
        }

        #endregion
    }
}
