using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aurum.Exchange
{
    /// <summary>
    /// Базовый класс операции обмена данными
    /// </summary>
    public abstract class ExchangeOperationBase : ExchangeOperation
    {
        /// <summary>
        /// Модель
        /// </summary>
        public sealed override IModelExchange Model
        {
            get
            {
                return GetModel(GetType(), Application);
            }
        }

        public static IModelExchange GetModel(Type type, XafApplication app)
        {
            if (app == null)
            {
                return null;
            }

            var exchModel = app.Model as IModelExchanges;
            var model = exchModel.Exports.Where(e => e.Type == type).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Количество выбираемых объектов за раз.
        /// Если значение нулевое, одним запросом выбираются все объекты сразу (поведение по умолчанию)
        /// </summary>
        public int ChunkLoaderRowCount
        {
            get;
            set;
        }

        /// <summary>
        /// Запускать сборку мусора после выборки ChunkLoaderRowCount объектов.
        /// По умолчанию отключено
        /// </summary>
        public bool ChunkLoaderGcCollectEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Создать новую операцию обмена данными
        /// </summary>
        /// <exception cref="ArgumentNullException" />
        public ExchangeOperationBase(XafApplication app)
            : base(app)
        {
        }
    }
}
