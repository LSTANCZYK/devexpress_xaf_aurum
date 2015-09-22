using Aurum.App.Module.BusinessObjects;
using Aurum.Exchange;
using Aurum.Operations;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aurum.App.Module.Exports
{
    /// <summary>
    /// Экспорт домов №4
    /// Длительный экспорт
    /// </summary>
    [XafDisplayName("Длительный экспорт")]
    public class House4Export : CustomExportOperation<House>
    {
        public House4Export(XafApplication s)
            : base(s)
        {
            AddFormatter(new LongFormatter());
            Formatters[0].AddFile("D:\\long_export.txt");
        }
    }

    internal class LongFormatter : OutputFormatter<House>
    {
        protected override void CustomFormat(IEnumerable<House> data, OperationInterop interop)
        {
            for (int i = 0; i < 10; ++i)
            {
                Thread.Sleep(1000);
                interop.WriteToLog("Halo");
                interop.ThrowIfCancellationRequested();
            }
        }
    }
}
