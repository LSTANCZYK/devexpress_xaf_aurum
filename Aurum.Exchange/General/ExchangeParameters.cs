using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aurum.Exchange
{
    /// <summary>
    /// Базовый класс для объекта параметров операции обмена
    /// </summary>
    [Serializable]
    public abstract class ExchangeParameters
    {
        /// <summary>
        /// Пространство данных
        /// </summary>
        public IObjectSpace ObjectSpace
        {
            get;
            internal set;
        }

        /// <summary>
        /// Создать новый объект параметров операции обмена
        /// </summary>
        public ExchangeParameters()
        {
        }
    }

    public interface IWithParametersType<T>
    {
    }
}
