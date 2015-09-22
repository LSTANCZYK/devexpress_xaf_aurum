using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Exchange
{
    public abstract class OutputExcelFormatter<TDataClass> : OutputFormatter<TDataClass>
    {
        private SpreadsheetControl spreadsheetControl;
        public IWorkbook Workbook { get { return spreadsheetControl.Document; } }
        public DocumentFormat DocumentFormat { get; set; }

        public OutputExcelFormatter()
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

        protected internal override void OnAfterFormatting()
        {
            base.OnAfterFormatting();
            Workbook.SaveDocument(Streams.Values.Single(), DocumentFormat);
        }
    }
}
