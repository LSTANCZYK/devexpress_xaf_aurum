using Aurum.Operations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aurum.Exchange
{
    /// <summary>
    /// Форматирование в виде списка строк, в каждой строке список отдельных полей
    /// </summary>
    public sealed class OutputTextFormatter : OutputFormatter
    {
        private const string DEFAULT_ELEM_DIVIDER = ";";

        private readonly string elemDivider;
        private readonly string listDivider;

        /// <summary>
        /// Событие запроса текста заголовка
        /// </summary>
        public event CustomHeaderEventHandler CustomHeader;

        /// <summary>
        /// Событие запроса завершающего текста
        /// </summary>
        public event CustomFooterEventHandler CustomFooter;


        private void RaiseCustomHeaderEvent(CustomHeaderEventArgs e)
        {
            if (CustomHeader != null)
            {
                CustomHeader(this, e);
            }
        }

        private void RaiseCustomFooterEvent(CustomFooterEventArgs e)
        {
            if (CustomFooter != null)
            {
                CustomFooter(this, e);
            }
        }

        public Encoding Encoding
        {
            get;
            set;
        }

        protected override void CustomFormat(IEnumerable<object[]> objects, OperationInterop interop)
        {
            using (var sr = new StreamWriter(Streams.Values.Single(), Encoding ?? Encoding.Default))
            {
                var ch = new CustomHeaderEventArgs(interop);
                RaiseCustomHeaderEvent(ch);

                if (!String.IsNullOrEmpty(ch.HeaderText))
                {
                    if (listDivider == null)
                    {
                        sr.WriteLine(ch.HeaderText);
                    }
                    else
                    {
                        sr.Write(ch.HeaderText + listDivider);
                    }
                }

                foreach (var obj in objects)
                {
                    if (listDivider == null)
                    {
                        sr.WriteLine(String.Join(elemDivider, obj));
                    }
                    else
                    {
                        sr.Write(String.Join(elemDivider, obj) + listDivider);
                    }

                    interop.ThrowIfCancellationRequested();
                }

                var cf = new CustomFooterEventArgs(interop, objects);
                RaiseCustomFooterEvent(cf);

                if (!String.IsNullOrEmpty(cf.FooterText))
                {
                    if (listDivider == null)
                    {
                        sr.WriteLine(cf.FooterText);
                    }
                    else
                    {
                        sr.Write(cf.FooterText + listDivider);
                    }
                }

                sr.Flush();
            }
        }

        /// <summary>
        /// Создать новый объект списочного форматирования. Помещает каждый выходной объект в отдельную строку, разделяя поля объекта
        /// символом "точка с запятой"
        /// </summary>
        public OutputTextFormatter()
            : this(DEFAULT_ELEM_DIVIDER, null)
        {
        }

        /// <summary>
        /// Создать новый объект списочного форматирования. Помещает каждый выходной объект в отдельную строку, разделяя поля указанным разделителем
        /// </summary>
        /// <param name="elemDivider">Разделитель полей в строке</param>
        /// <exception cref="System.ArgumentNullException" />
        public OutputTextFormatter(string elemDivider)
            : this(elemDivider, null)
        {
        }

        /// <summary>
        /// Создать новый объект списочного форматирования.
        /// Разделители строк и полей указываются в параметрах
        /// </summary>
        /// <param name="elementDivider">Разделитель полей в строке</param>
        /// <param name="listDivider">Разделитель строк, если null, то разделитель по умолчанию (символ новой строки)</param>
        /// <exception cref="System.ArgumentNullException" />
        public OutputTextFormatter(string elementDivider, string listDivider)
        {
            if (elementDivider == null)
            {
                throw new ArgumentNullException("elementDivider");
            }

            this.listDivider = listDivider;
            this.elemDivider = elementDivider;
        }

        protected internal override void OnBeforeFormatting()
        {
            base.OnBeforeFormatting();
        }
    }

    public class InteropEventArgs : EventArgs
    {
        public OperationInterop Interop
        {
            get;
            private set;
        }

        public InteropEventArgs(OperationInterop interop)
        {
            Interop = interop;
        }
    }

    /// <summary>
    /// Аргументы события "Получение заголовка"
    /// </summary>
    public class CustomHeaderEventArgs : InteropEventArgs
    {
        /// <summary>
        /// Установить текст заголовка. Данное поле только для записи
        /// </summary>
        public string HeaderText
        {
            get;
            set;
        }

        public CustomHeaderEventArgs(OperationInterop interop)
            : base(interop)
        {
        }
    }

    /// <summary>
    /// Аргументы события "Получение завершающего текста"
    /// </summary>
    public class CustomFooterEventArgs : InteropEventArgs
    {
        /// <summary>
        /// Установить завершающий текст. Данное поле только для записи
        /// </summary>
        public string FooterText
        {
            get;
            set;
        }

        public IEnumerable<object> Data
        {
            get;
            private set;
        }

        public CustomFooterEventArgs(OperationInterop interop, IEnumerable<object> data)
            : base(interop)
        {
            Data = data;
        }
    }

    /// <summary>
    /// Обработчик события "Получение заголовка"
    /// </summary>
    /// <param name="sender">Источник</param>
    /// <param name="e">Аргументы</param>
    public delegate void CustomHeaderEventHandler(object sender, CustomHeaderEventArgs e);

    /// <summary>
    /// Обработчик события "Получение завершающего текста"
    /// </summary>
    /// <param name="sender">Источник</param>
    /// <param name="e">Аргументы</param>
    public delegate void CustomFooterEventHandler(object sender, CustomFooterEventArgs e);
}
