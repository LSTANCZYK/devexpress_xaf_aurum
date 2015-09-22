using Aurum.Operations;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Exchange
{
    public abstract class InputExcelFormatter : InputFormatter
    {
        private SpreadsheetControl spreadsheetControl;
        public IWorkbook Workbook { get { return spreadsheetControl.Document; } }
        public DocumentFormat DocumentFormat { get; set; }

        public InputExcelFormatter()
        {
            DocumentFormat = DocumentFormat.OpenXml;
            spreadsheetControl = new SpreadsheetControl();
        }

        public override void Dispose()
        {
            if (spreadsheetControl != null)
            {
                spreadsheetControl = null;
            }
            base.Dispose();
        }

        protected override void ReadFile(string file, Stream stream, OperationInterop interop)
        {
            Workbook.LoadDocument(file, DocumentFormat);
        }
    }
}
