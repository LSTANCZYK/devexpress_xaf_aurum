using Aurum.App.Module.BusinessObjects;
using Aurum.Exchange;
using Aurum.Operations;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aurum.App.Module.Exchange
{
    /// <summary>
    /// Экспорт домов №5
    /// Excel экспорт
    /// </summary>
    [XafDisplayName("Excel экспорт")]
    public class House5Export : CustomExportOperation<House>
    {
        public House5Export(XafApplication s)
            : base(s)
        {
            AddFormatter(new XLFormatter());
            Formatters[0].AddFile("D:\\excel_export.xlsx");
        }
    }

    internal class XLFormatter : OutputExcelFormatter<House>
    {
        protected override void CustomFormat(IEnumerable<House> data, OperationInterop interop)
        {
            Worksheet worksheet = Workbook.Worksheets[0];

            // Add data of different types to cells.
            worksheet.Cells["B2"].Value = DateTime.Now;
            worksheet.Cells["B3"].Value = Math.PI;
            worksheet.Cells["B4"].Value = "Have a nice day!";
            worksheet.Cells["B5"].Value = CellValue.ErrorReference;
            worksheet.Cells["B6"].Value = true;
            worksheet.Cells["B7"].Value = float.MaxValue;
            worksheet.Cells["B8"].Value = 'a';
            worksheet.Cells["B9"].Value = Int32.MaxValue;

            // Fill all cells of the range with 10.
            worksheet.Range["B12:C12"].Value = 10;

            worksheet.Cells["A2"].Value = "dateTime";
            worksheet.Cells["A3"].Value = "double";
            worksheet.Cells["A4"].Value = "string";
            worksheet.Cells["A5"].Value = "error constant";
            worksheet.Cells["A6"].Value = "boolean";
            worksheet.Cells["A7"].Value = "float";
            worksheet.Cells["A8"].Value = "char";
            worksheet.Cells["A9"].Value = "int32";
            worksheet.Cells["A12"].Value = "fill range";

            Workbook.Options.Culture = CultureInfo.InvariantCulture;

        }
    }
}
