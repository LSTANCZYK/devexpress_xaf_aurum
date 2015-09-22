using Aurum.Exchange.Lib;
using Aurum.Operations;
using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Exchange
{
    public abstract class InputDbfFormatter : InputFormatter
    {
        private static readonly Encoding Windows1251 = Encoding.GetEncoding(1251);
        private static readonly Encoding CP866 = Encoding.GetEncoding(866);

        /// <summary>
        /// Использовать кодировку Windows-1251
        /// </summary>
        public bool UseEncoding1251
        {
            get;
            set;
        }

        /// <summary>
        /// Использовать кодировку CP866
        /// </summary>
        public bool UseEncoding866
        {
            get;
            set;
        }

        /// <summary>
        /// Колонки
        /// </summary>
        public List<string> Columns { get; private set; }

        /// <summary>
        /// Данные
        /// </summary>
        public List<Dictionary<string, object>> Data { get; private set; }

        protected override void ReadFile(string file, Stream stream, OperationInterop interop)
        {
            if (UseEncoding1251 && UseEncoding866)
                throw new InvalidOperationException("Установлены взаимоисключающие параметры кодировки");

            DbfReader dbfReader = new DbfReader();
            try
            {
                dbfReader.Open(stream, UseEncoding1251 ? Windows1251 : UseEncoding866 ? CP866 : null);

                Columns = dbfReader.Columns;
                Data = new List<Dictionary<string, object>>();

                Dictionary<string, object> record = null;
                while ((record = dbfReader.ReadRecord()) != null)
                {
                    Data.Add(record);
                }
            }
            finally
            {
                try
                {
                    dbfReader.Close();
                }
                catch { }
            }
        }
    }
}
