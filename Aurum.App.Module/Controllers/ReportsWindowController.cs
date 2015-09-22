using Aurum.App.Module.BusinessObjects;
using Aurum.Interface.Controllers;
using DevExpress.ExpressApp.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.App.Module.Controllers
{
    /// <summary>
    /// Регистрация отчетов
    /// </summary>
    public class ReportsWindowController : ReportsWindowControllerBase
    {
        // список отчетов: тип отчета, тип объекта отчета, тип параметров, название, inPlaceReport, постфикс
        internal static List<Tuple<Type, Type, Type, string, bool, string>> Reports = new List<Tuple<Type, Type, Type, string, bool, string>>();

        static ReportsWindowController()
        {
            AddPredefinedReport<Aurum.App.Module.Reports.XtraReport1>("Отчет 1", typeof(Person), typeof(Aurum.App.Module.Reports.ReportParametersObject1), true);
        }

        static void AddPredefinedReport<T>(string displayName)
        {
            AddPredefinedReport<T>(displayName, null, null, false);
        }

        static void AddPredefinedReport<T>(string displayName, Type dataType, bool isInplaceReport)
        {
            AddPredefinedReport<T>(displayName, dataType, null, isInplaceReport);
        }

        static void AddPredefinedReport<T>(string displayName, Type dataType, Type parametersObjectType, bool isInplaceReport)
        {
            AddPredefinedReport<T>(displayName, dataType, parametersObjectType, isInplaceReport, null);
        }

        static void AddPredefinedReport<T>(string displayName, Type dataType, Type parametersObjectType, bool isInplaceReport, string ext)
        {
            Reports.Add(Tuple.Create(typeof(T), dataType, parametersObjectType, displayName, isInplaceReport, ext));
        }

        public ReportsWindowController()
        {
            foreach (var report in Reports)
            {
                new SimpleAction(this, "Report_" + report.Item1.FullName + (!string.IsNullOrEmpty(report.Item6) ? "_" + report.Item6 : string.Empty), "MP_Report", Handler) { Tag = report.Item1, ImageName = "Navigation_Item_Report", Caption = report.Item4 };
            }
        }
    }
}
