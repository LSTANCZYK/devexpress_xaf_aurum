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
using System.Threading.Tasks;

namespace Aurum.App.Module.Exports
{
    /// <summary>
    /// Экспорт домов №3
    /// Используется экспорт с типизированным форматировщиком, без параметров
    /// </summary>
    public class House3Export : CustomExportOperation<House>
    {
        public House3Export(XafApplication s)
            : base(s)
        {
            // Установка "нашего" форматтера
            AddFormatter(new House3ExportFormatter());
            Formatters[0].AddFile("D:\\parameterless_export.txt");
        }
    }

    /// <summary>
    /// Типизированный текстовый форматтер
    /// </summary>
    internal class House3ExportFormatter : OutputTextFormatter<House>
    {
        /// <summary>
        /// Переопределенный метод форматирования и записи
        /// </summary>
        /// <param name="data">Коллекция данных</param>
        protected override void CustomFormat(IEnumerable<House> data, OperationInterop interop)
        {
            Writer.WriteLine("### House 3 export ###");
            Writer.WriteLine("Num\tStreet\tFlats count");

            foreach (var d in data)
            {
                Writer.WriteLine("{0}\t{1}\t{2}", d.Num, d.Street, d.Flats.Count);
            }

            Writer.WriteLine("### End of house 2 export ###");
            Writer.Close();
        }
    }
}
