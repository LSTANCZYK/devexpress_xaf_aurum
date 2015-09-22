using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aurum.Exchange
{
    /// <summary>
    /// Типизированный форматировщик с определенным объектом текстовой записи
    /// </summary>
    /// <typeparam name="TDataClass">Тип объекта экспорта</typeparam>
    public abstract class OutputTextFormatter<TDataClass> : OutputFormatter<TDataClass>
    {
        /// <summary>
        /// Объект записи
        /// </summary>
        protected StreamWriter Writer
        {
            get;
            private set;
        }

        public Encoding Encoding
        {
            get;
            set;
        }

        public override void Dispose()
        {
            if (Writer != null)
            {
                Writer.Flush();
                Writer.Dispose();
            }
            base.Dispose();
        }

        protected internal override void OnBeforeFormatting()
        {
            base.OnBeforeFormatting();
            Writer = new StreamWriter(Streams.Values.Single(), Encoding ?? Encoding.Default);
        }
    }
}
