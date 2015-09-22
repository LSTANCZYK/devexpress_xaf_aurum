using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aurum.Exchange
{
    /// <summary>
    /// Базовый класс форматировщика
    /// </summary>
    public abstract class FormatterBase : IDisposable
    {
        // потоки ввода/вывода
        private Dictionary<string, Stream> streams = new Dictionary<string, Stream>();

        /// <summary>
        /// Событие, происходящее непосредственно перед форматированием
        /// </summary>
        public event EventHandler BeforeFormatting;

        /// <summary>
        /// Событие, происходящее непосредственно после форматирования
        /// </summary>
        public event EventHandler AfterFormatting;

        /// <summary>
        /// Потоки
        /// </summary>
        public Dictionary<string, Stream> Streams
        {
            get { return streams; }
        }

        private void RaiseBeforeFormattingEvent()
        {
            if (BeforeFormatting != null)
            {
                BeforeFormatting(this, EventArgs.Empty);
            }
        }

        private void RaiseAfterFormattingEvent()
        {
            if (AfterFormatting != null)
            {
                AfterFormatting(this, EventArgs.Empty);
            }
        }

        protected internal virtual void OnBeforeFormatting()
        {
            RaiseBeforeFormattingEvent();
        }

        protected internal virtual void OnAfterFormatting()
        {
            RaiseAfterFormattingEvent();
        }

        /// <summary>
        /// Установить файловый ввод/вывод
        /// </summary>
        /// <param name="file">Путь к файлу</param>
        /// <param name="fileMode">Режим открытия файла</param>
        /// <exception cref="ArgumentException">Ошибка открытия файла</exception>
        public virtual void AddFile(string file, FileMode fileMode)
        {
            try
            {
                var stream = new FileStream(file, fileMode);
                streams.Add(file, stream);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("file", "File access error", ex);
            }
        }

        public virtual void Dispose()
        {
            if (streams != null && streams.Count > 0)
            {
                foreach (var e in streams)
                {
                    e.Value.Dispose();
                }
                streams.Clear();
                streams = null;
            }
        }
    }
}
