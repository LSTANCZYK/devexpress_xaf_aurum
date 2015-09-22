using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Exchange
{
    /// <summary>
    /// Атрибут, указывающий что в операции обмена используется параметр указанного типа
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ParametersTypeAttribute : Attribute
    {
        /// <summary>
        /// Тип параметра
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Использовать параметр указанного типа для операции обмена
        /// </summary>
        /// <param name="type">Тип параметра. Должен наследоваться от Aurum.Exchange.ExchangeParameters</param>
        public ParametersTypeAttribute(Type type)
        {
            Type = type;
        }
    }
}
