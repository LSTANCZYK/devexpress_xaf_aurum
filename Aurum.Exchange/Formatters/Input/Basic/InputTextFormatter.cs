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
    public abstract class InputTextFormatter : InputFormatter
    {
        /// <summary>
        /// Объект записи
        /// </summary>
        protected StreamReader Reader
        {
            get;
            private set;
        }

        public Encoding Encoding
        {
            get;
            set;
        }

        protected override void ReadFile(string file, Stream stream, OperationInterop interop)
        {
            if (Reader != null) throw new InvalidOperationException("Текстовый форматировщик поддерживает только один файл.");
            Reader = new StreamReader(stream, Encoding ?? Encoding.Default);
        }

        public override void Dispose()
        {
            if (Reader != null)
            {
                Reader.Dispose();
                Reader = null;
            }
            base.Dispose();
        }
    }
}
