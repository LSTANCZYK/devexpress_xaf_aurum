using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Operations
{
    /// <summary>
    /// Элемент журнала
    /// </summary>
    [DomainComponent]
    public class LogItemObject
    {
        /// <summary>
        /// Индекс
        /// </summary>
        /// <remarks>Используется только для упорядочивания элементов</remarks>
        public int Index { get; set; }

        /// <summary>
        /// Время
        /// </summary>
        [ModelDefault("DisplayFormat", "{0:G}")]
        public DateTime Time { get; set; }

        /// <summary>
        /// Текстовое сообщение
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        public LogItemType Type { get; set; }

        /// <summary>
        /// Номер родительской операции
        /// </summary>
        public int OperationId { get; set; }
    }
}
