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
    /// <summary>
    /// Нетипизированный форматировщик
    /// </summary>
    public abstract class InputFormatter : FormatterBase
    {
        /// <summary>
        /// Событие, происходящее непосредственно перед чтением файлов
        /// </summary>
        public event EventHandler BeforeFileReading;

        /// <summary>
        /// Событие, происходящее непосредственно после чтения файлов
        /// </summary>
        public event EventHandler AfterFileReading;

        private void RaiseBeforeFileReadingEvent()
        {
            if (BeforeFileReading != null)
            {
                BeforeFileReading(this, EventArgs.Empty);
            }
        }

        private void RaiseAfterFileReadingEvent()
        {
            if (AfterFileReading != null)
            {
                AfterFileReading(this, EventArgs.Empty);
            }
        }

        protected internal virtual void OnBeforeFileReading()
        {
            RaiseBeforeFileReadingEvent();
        }

        protected internal virtual void OnAfterFileReading()
        {
            RaiseAfterFileReadingEvent();
        }

        public override void AddFile(string file, FileMode fileMode = FileMode.Open)
        {
            base.AddFile(file, fileMode);
        }

        internal void ReadFiles(OperationInterop interop)
        {
            if (Streams.Count == 0)
            {
                throw new InvalidOperationException("Не установлен входной файл");
            }

            OnBeforeFileReading();
            foreach (var stream in Streams)
            {
                // >>>
                interop.SetStatusText("Чтение файла " + stream.Key);
                interop.ThrowIfCancellationRequested();
                // <<<
                ReadFile(stream.Key, stream.Value, interop);
            }
            OnAfterFileReading();
        }

        protected abstract void ReadFile(string file, Stream stream, OperationInterop interop);

        public abstract void ProcessData(IObjectSpace objectSpace, OperationInterop interop);
    }
}
