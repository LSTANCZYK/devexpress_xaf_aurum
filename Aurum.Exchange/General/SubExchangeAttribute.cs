using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Exchange
{
    /// <summary>
    /// Помечает указанный класс как подоперация обмена.
    /// Такая подоперация, как правило, не попадает в общий список операций
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SubExchangeAttribute : Attribute
    {
        /// <summary>
        /// Пометить указанный класс как подоперацию обмена
        /// </summary>
        public SubExchangeAttribute()
            : base()
        {
        }
    }
}
