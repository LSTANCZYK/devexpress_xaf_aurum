using Aurum.App.Module.BusinessObjects;
using Aurum.Exchange;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.App.Module.Exports
{
    /// <summary>
    /// Комплексный экспорт домов, состоящий из отдельных экспортов
    /// Используется экспорт с обычным текстовым форматировщиком
    /// </summary>
    public class ComplexHouseExport : CompositeExchangeOperation
    {
        public ComplexHouseExport(XafApplication s)
            : base(s)
        {
            IsParallel = true;
            AddExchange<SubExport1>();
            AddExchange<SubExport2>();
        }

        [SubExchange]
        private class SubExport1 : ExportOperation<House>
        {
            public SubExport1(XafApplication s)
                : base(s)
            {
                AddField(x => x.Num);
                Formatter = new OutputTextFormatter();
                Formatter.AddFile("D:\\zen\\zen1.txt");
                (Formatter as OutputTextFormatter).CustomFooter += SubExport1_CustomFooter;
            }

            void SubExport1_CustomFooter(object sender, CustomFooterEventArgs e)
            {
                Task.Delay(4000).Wait();
                e.Interop.State.Value = e.Data.Count();
            }
        }

        [SubExchange]
        private class SubExport2 : ExportOperation<House>
        {
            public SubExport2(XafApplication s)
                : base(s)
            {
                AddField(x => x.Street);
                Formatter = new OutputTextFormatter();
                Formatter.AddFile("D:\\zen\\zen2.txt");
                (Formatter as OutputTextFormatter).CustomHeader += SubExport2_CustomHeader;
            }

            void SubExport2_CustomHeader(object sender, CustomHeaderEventArgs e)
            {
                Task.Delay(4000).Wait();
                e.HeaderText = "The first export contained " + ((Int32)e.Interop.State.Value) + " rows";
            }
        }
    }
}
