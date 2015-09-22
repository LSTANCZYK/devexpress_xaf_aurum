using EasyNetQ.Producer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Courier.Rpc
{
    /// <summary>
    /// An RPC style request-response pattern with tag
    /// </summary>
    public interface ITagRpc
    {
        /// <summary>
        /// Make a request to an RPC service
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <param name="tag">Tag</param>
        /// <param name="request">The request message</param>
        /// <returns>Returns a task that yields the result when the response arrives</returns>
        Task<TResponse> Request<TRequest, TResponse>(string tag, TRequest request)
            where TRequest : class
            where TResponse : class;

        /// <summary>
        /// Set up a responder for an RPC service.
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <param name="tag">Tag</param>
        /// <param name="responder">A function that performs the response</param>
        IDisposable Respond<TRequest, TResponse>(string tag, Func<TRequest, Task<TResponse>> responder)
            where TRequest : class
            where TResponse : class;

        /// <summary>
        /// Set up a responder for an RPC service.
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <param name="tag">Tag</param>
        /// <param name="responder">A function that performs the response</param>
        /// <param name="configure">A function that performs the configuration</param>
        IDisposable Respond<TRequest, TResponse>(string tag, Func<TRequest, Task<TResponse>> responder, Action<IResponderConfiguration> configure)
            where TRequest : class
            where TResponse : class;
    }
}
