using Aurum.Exchange;
using Aurum.Operations;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.App.Module.Exchange
{
    public class ExcelImport : CustomImportOperation<ExcelImportParameters>
    {
        public ExcelImport(XafApplication app)
            : base(app)
        {
            OperationInfo.Name = "excel";
            BeforeExchangeExecute += ExcelImport_BeforeExchangeExecute;
        }

        void ExcelImport_BeforeExchangeExecute(object sender, EventArgs e)
        {
            ExcelImportFormatter formatter = new ExcelImportFormatter();
            formatter.AddFile(ParametersObject.FileToOpen);
            AddFormatter(formatter);
        }

        private class ExcelImportFormatter : InputExcelFormatter
        {
            public override void ProcessData(IObjectSpace objectSpace, OperationInterop interop)
            {
                var worksheet = Workbook.Worksheets[0];
                // Access a collection of rows.
                RowCollection rows = worksheet.Rows;

                // Access the first row by its index in the collection of rows.
                Row firstRow_byIndex = rows[0];

                // Access the first row by its unique name.
                Row firstRow_byName = rows["1"];

                // Access a collection of columns.
                ColumnCollection columns = worksheet.Columns;

                // Access the first column by its index in the collection of columns.
                Column firstColumn_byIndex = columns[0];

                // Access the first column by its unique name.
                Column firstColumn_byName = columns["A"];

                Cell cellA1 = worksheet[0, 0]; // Cell A1

                Cell cellB2 = worksheet.Cells["B2"]; // Cell B2

                var obj = cellB2.Value.ToObject();

                Cell cellC3 = worksheet.Cells[2, 2]; // Cell C3

                Cell cellD4 = worksheet.Rows[3][3]; // Cell D4
                Cell cellE5 = worksheet.Rows[4]["E"]; // Cell E5
                Cell cellF6 = worksheet.Rows["6"][5]; // Cell F6

                Cell cellG7 = worksheet.Columns[6][6]; // Cell G7
                Cell cellH8 = worksheet.Columns["H"][7]; // Cell H8
                Cell cellI9 = worksheet.Columns["I"]["9"]; // Cell I9

                // Access a cell from the range of cells.
                Cell cellB5 = worksheet.Range["B3:D8"][6]; // Cell B5
                Cell cellD6 = worksheet.Range["B3:D8"][3, 2]; // Cell D6
            }
        }
    }

    /// <summary>
    /// Параметры для импорта
    /// </summary>
    [DomainComponent]
    public class ExcelImportParameters : ExchangeParameters
    {
        [FilePathMode(FilePathMode.Open)]
        [FilePathFilter("Excel files|*.xlsx|All files (*.*)|*.*")]
        public FilePath FileToOpen { get; set; }
    }
}
