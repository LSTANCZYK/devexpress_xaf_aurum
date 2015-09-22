using Aurum.Operations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aurum.Exchange
{
    /// <summary>
    /// Нетипизированный форматировщик
    /// </summary>
    public abstract class OutputFormatter : FormatterBase
    {
        /// <summary>
        /// Сформатировать данные в поток
        /// </summary>
        /// <param name="data">Коллекция массивов значений</param>
        /// <exception cref="InvalidOperationException">Не установлен выходной поток</exception>
        internal void Format(IEnumerable<object[]> data, OperationInterop interop)
        {
            if (Streams.Count == 0)
            {
                throw new InvalidOperationException("Не установлен выходной поток");
            }

            OnBeforeFormatting();
            CustomFormat(data, interop);
            OnAfterFormatting();
        }

        protected abstract void CustomFormat(IEnumerable<object[]> data, OperationInterop interop);

        public override void AddFile(string file, FileMode fileMode = FileMode.Create)
        {
            base.AddFile(file, fileMode);
        }
    }
}
