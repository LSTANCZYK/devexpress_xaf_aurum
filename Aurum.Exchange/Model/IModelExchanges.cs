using DevExpress.ExpressApp.Model;
using System;
using System.ComponentModel;

namespace Aurum.Exchange
{
    /// <summary>
    /// Модель базового обмена
    /// </summary>
    [KeyProperty("TypeName")]
    public interface IModelExchange : IModelNode
    {
        /// <summary>
        /// Название
        /// </summary>
        [Description("Отображаемое название")]
        [Localizable(true)]
        string Name { get; set; }

        /// <summary>
        /// Тип соответствующего класса экспорта
        /// </summary>
        [Browsable(false)]
        Type Type { get; set; }

        /// <summary>
        /// Полное название типа соответствующего класса экспорта.
        /// Используется как ключ
        /// </summary>
        [Browsable(false), ReadOnly(true)]
        string TypeName { get; set; }
    }
}
