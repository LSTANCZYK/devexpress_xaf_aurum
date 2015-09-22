using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aurum.Courier.Rpc;

namespace Aurum.Courier
{
    /// <summary>
    /// Курьер для RabbitMQ
    /// </summary>
    internal class RabbitCourier : ICourier
    {
        private IBus bus;
        private List<IExchange> exchanges;
        private bool isInit = false;
        private bool isExchangesInited = false;

        public RabbitCourier()
        {
            exchanges = new List<IExchange>();
        }

        /// <summary>
        /// Инициализация конфигурации
        /// </summary>
        /// <param name="configurator">Конфигуратор</param>
        public void InitConfig(IConfigurator configurator)
        {
            lock (this)
            {
                if (isInit)
                    throw new AurumCourierException("Courier already initialized");
                bus = RabbitHutch.CreateBus(
                    configurator.Connection + (!string.IsNullOrEmpty(configurator.Product) ? ";Product=" + configurator.Product : string.Empty)
                    , x => x.Register<IEasyNetQLogger>(_ => new RabbitLogger())
                    );
                isInit = true;
            }
        }

        /// <summary>
        /// Регистрация обмена данными
        /// </summary>
        /// <param name="exchange">Обмен данными</param>
        public void RegisterExchange(IExchange exchange)
        {
            lock (this)
            {
                exchanges.Add(exchange);
                if (isExchangesInited)
                {
                    exchange.Init(this);
                }
            }
        }

        /// <summary>
        /// Инициализация обменов данными
        /// </summary>
        public void InitExchanges()
        {
            lock (this)
            {
                if (isExchangesInited)
                    throw new AurumCourierException("Courier exchanges already inited");

                foreach (var ex in exchanges)
                {
                    ex.Init(this);
                }

                isExchangesInited = true;
            }
        }

        /// <summary>
        /// Публикация сообщения (Издатель/Подписчик)
        /// </summary>
        /// <typeparam name="T">Тип сообщения</typeparam>
        /// <param name="message">Сообщение</param>
        /// <param name="topic">Тема</param>
        /// <returns></returns>
        public Task PublishAsync<T>(T message, string topic) where T : class
        {
            if (!isInit)
                throw new AurumCourierException("Courier is not initialized");
            return bus.PublishAsync<T>(message, topic);
        }

        /// <summary>
        /// Подписка на сообщения (Издатель/Подписчик)
        /// </summary>
        /// <typeparam name="T">Тип сообщения</typeparam>
        /// <param name="subscriptionId">Идентификатор подписки</param>
        /// <param name="onMessage">Обработка сообщения</param>
        /// <param name="configure">Конфигурирование</param>
        /// <returns></returns>
        public IDisposable SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage, Action<ISubscriptionConfiguration> configure) where T : class
        {
            if (!isInit)
                throw new AurumCourierException("Courier is not initialized");
            var connectionConfiguration = bus.Advanced.Container.Resolve<ConnectionConfiguration>();
            var config = new SubscriptionConfiguration(connectionConfiguration.PrefetchCount);
            configure(config);

            return bus.SubscribeAsync<T>(subscriptionId, onMessage, intConfig =>
            {
                intConfig.WithAutoDelete(config.AutoDelete)
                    .WithCancelOnHaFailover(config.CancelOnHaFailover)
                    .WithExpires(config.Expires)
                    .WithPrefetchCount(config.PrefetchCount)
                    .WithPriority(config.Priority);

                foreach (var t in config.Topics)
                {
                    intConfig.WithTopic(t);
                }
            });
        }

        /// <summary>
        /// RPC Запрос (Запрос/Ответ)
        /// </summary>
        /// <typeparam name="TRequest">Тип запроса</typeparam>
        /// <typeparam name="TResponse">Тип ответа</typeparam>
        /// <param name="tag">Метка запроса/ответа</param>
        /// <param name="request">Запрос</param>
        /// <returns></returns>
        public Task<TResponse> RequestAsync<TRequest, TResponse>(string tag, TRequest request)
            where TRequest : class
            where TResponse : class
        {
            if (!isInit)
                throw new AurumCourierException("Courier is not initialized");
            return bus.RequestAsync<TRequest, TResponse>(tag, request);
        }

        /// <summary>
        /// RPC Ответ (Запрос/Ответ)
        /// </summary>
        /// <typeparam name="TRequest">Тип запроса</typeparam>
        /// <typeparam name="TResponse">Тип ответа</typeparam>
        /// <param name="tag">Метка запроса/ответа</param>
        /// <param name="responder">Отвечатель на запросы</param>
        /// <returns></returns>
        public IDisposable RespondAsync<TRequest, TResponse>(string tag, Func<TRequest, Task<TResponse>> responder)
            where TRequest : class
            where TResponse : class
        {
            if (!isInit)
                throw new AurumCourierException("Courier is not initialized");
            return bus.RespondAsync<TRequest, TResponse>(tag, responder);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (bus != null)
            {
                try
                {
                    bus.Dispose();
                }
                catch
                {
                }
                bus = null;
            }
        }
    }

    /// <summary>
    /// Логгер для RabbitMQ
    /// </summary>
    internal class RabbitLogger : IEasyNetQLogger
    {
        private static TraceSwitch traceSwitch = new TraceSwitch("Aurum.Courier.Rabbit", "");

        public void DebugWrite(string format, params object[] args)
        {
            Trace.WriteLineIf(traceSwitch.TraceVerbose, SafeWrite("DEBUG: " + format, args));
        }

        public void ErrorWrite(Exception exception)
        {
            Trace.WriteLineIf(traceSwitch.TraceError, "ERROR: " + exception.ToString());
        }

        public void ErrorWrite(string format, params object[] args)
        {
            Trace.WriteLineIf(traceSwitch.TraceError, SafeWrite("ERROR: " + format, args));
        }

        public void InfoWrite(string format, params object[] args)
        {
            Trace.WriteLineIf(traceSwitch.TraceInfo, SafeWrite("INFO: " + format, args));
        }

        private string SafeWrite(string format, params object[] args)
        {
            // even a zero length args paramter causes WriteLine to interpret 'format' as
            // a format string. Rather than escape JSON, better to check the intention of 
            // the caller.
            if (args == null || args.Length == 0)
            {
                return format;
            }
            else
            {
                return string.Format(format, args);
            }
        }
    }
}
