using Aurum.Operations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aurum.Exchange
{
    /// <summary>
    /// Типизированный форматировщик
    /// </summary>
    /// <typeparam name="TDataClass">Тип объекта экспорта</typeparam>
    public abstract class OutputFormatter<TDataClass> : FormatterBase
    {
        /// <summary>
        /// Сформатировать данные в поток
        /// </summary>
        /// <param name="data">Коллекция объектов класса TDataClass</param>
        /// <exception cref="InvalidOperationException">Не установлен выходной поток</exception>
        internal void Format(IEnumerable<TDataClass> data, OperationInterop interop)
        {
            CustomFormat(data, interop);
        }

        /// <summary>
        /// Переопределяемый метод записи форматированных данных в поток
        /// </summary>
        /// <param name="data"></param>
        protected abstract void CustomFormat(IEnumerable<TDataClass> data, OperationInterop interop);

        public override void AddFile(string file, FileMode fileMode = FileMode.Create)
        {
            base.AddFile(file, fileMode);
        }
    }
}
