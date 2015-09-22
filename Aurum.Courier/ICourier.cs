using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Courier
{
    /// <summary>
    /// Курьер (обмен данными через MQ систему)
    /// </summary>
    public interface ICourier : IDisposable
    {
        /// <summary>
        /// Регистрация обмена данными
        /// </summary>
        /// <param name="exchange">Обмен данными</param>
        void RegisterExchange(IExchange exchange);

        /// <summary>
        /// Инициализация конфигурации
        /// </summary>
        /// <param name="configurator">Конфигуратор</param>
        void InitConfig(IConfigurator configurator);

        /// <summary>
        /// Инициализация обменов данными
        /// </summary>
        void InitExchanges();

        /// <summary>
        /// Публикация сообщения (Издатель/Подписчик)
        /// </summary>
        /// <typeparam name="T">Тип сообщения</typeparam>
        /// <param name="message">Сообщение</param>
        /// <param name="topic">Тема</param>
        /// <returns></returns>
        Task PublishAsync<T>(T message, string topic) where T : class;

        /// <summary>
        /// Подписка на сообщения (Издатель/Подписчик)
        /// </summary>
        /// <typeparam name="T">Тип сообщения</typeparam>
        /// <param name="subscriptionId">Идентификатор подписки</param>
        /// <param name="onMessage">Обработка сообщения</param>
        /// <param name="configure">Конфигурирование</param>
        /// <returns></returns>
        IDisposable SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage, Action<ISubscriptionConfiguration> configure) where T : class;
        
        /// <summary>
        /// RPC Запрос (Запрос/Ответ)
        /// </summary>
        /// <typeparam name="TRequest">Тип запроса</typeparam>
        /// <typeparam name="TResponse">Тип ответа</typeparam>
        /// <param name="tag">Метка запроса/ответа</param>
        /// <param name="request">Запрос</param>
        /// <returns></returns>
        Task<TResponse> RequestAsync<TRequest, TResponse>(string tag, TRequest request)
            where TRequest : class
            where TResponse : class;
        
        /// <summary>
        /// RPC Ответ (Запрос/Ответ)
        /// </summary>
        /// <typeparam name="TRequest">Тип запроса</typeparam>
        /// <typeparam name="TResponse">Тип ответа</typeparam>
        /// <param name="tag">Метка запроса/ответа</param>
        /// <param name="responder">Отвечатель на запросы</param>
        /// <returns></returns>
        IDisposable RespondAsync<TRequest, TResponse>(string tag, Func<TRequest, Task<TResponse>> responder)
            where TRequest : class
            where TResponse : class;        
    }
}
