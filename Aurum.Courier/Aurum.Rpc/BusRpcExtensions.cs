using EasyNetQ;
using EasyNetQ.Producer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Courier.Rpc
{
    public static class BusRpcExtensions
    {
        private static Dictionary<IBus, ITagRpc> rpcDict = new Dictionary<IBus, ITagRpc>();

        /// <summary>
        /// Makes an RPC style request with tag.
        /// </summary>
        /// <typeparam name="TRequest">The request type.</typeparam>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <param name="tag">Tag</param>
        /// <param name="request">The request message.</param>
        /// <returns>The response</returns>
        public static TResponse Request<TRequest, TResponse>(this IBus bus, string tag, TRequest request)
            where TRequest : class
            where TResponse : class
        {
            Preconditions.CheckNotBlank(tag, "tag");
            Preconditions.CheckNotNull(request, "request");

            var task = RequestAsync<TRequest, TResponse>(bus, tag, request);
            task.Wait();
            return task.Result;
        }

        /// <summary>
        /// Makes an RPC style request asynchronously with tag.
        /// </summary>
        /// <typeparam name="TRequest">The request type.</typeparam>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <param name="tag">Tag</param>
        /// <param name="request">The request message.</param>
        /// <returns>A task that completes when the response returns</returns>
        public static Task<TResponse> RequestAsync<TRequest, TResponse>(this IBus bus, string tag, TRequest request)
            where TRequest : class
            where TResponse : class
        {
            Preconditions.CheckNotBlank(tag, "tag");
            Preconditions.CheckNotNull(request, "request");

            return createTagRpc(bus).Request<TRequest, TResponse>(tag, request);
        }

        /// <summary>
        /// Responds to an RPC request with tag.
        /// </summary>
        /// <typeparam name="TRequest">The request type.</typeparam>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <param name="tag">Tag</param>
        /// <param name="responder">
        /// A function to run when the request is received. It should return the response.
        /// </param>
        public static IDisposable Respond<TRequest, TResponse>(this IBus bus, string tag, Func<TRequest, TResponse> responder)
            where TRequest : class
            where TResponse : class
        {
            Preconditions.CheckNotBlank(tag, "tag");
            Preconditions.CheckNotNull(responder, "responder");

            Func<TRequest, Task<TResponse>> taskResponder =
                request => Task<TResponse>.Factory.StartNew(_ => responder(request), null);

            return RespondAsync(bus, tag, taskResponder);
        }

        /// <summary>
        /// Responds to an RPC request asynchronously with tag.
        /// </summary>
        /// <typeparam name="TRequest">The request type.</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <param name="tag">Tag</param>
        /// <param name="responder">
        /// A function to run when the request is received.
        /// </param>
        public static IDisposable RespondAsync<TRequest, TResponse>(this IBus bus, string tag, Func<TRequest, Task<TResponse>> responder)
            where TRequest : class
            where TResponse : class
        {
            Preconditions.CheckNotBlank(tag, "tag");
            Preconditions.CheckNotNull(responder, "responder");

            return createTagRpc(bus).Respond(tag, responder);
        }

        private static ITagRpc createTagRpc(IBus bus)
        {
            if (bus == null)
                throw new EasyNetQException("Bus is null");
            if (!rpcDict.ContainsKey(bus))
            {
                var eventBus = bus.Advanced.Container.Resolve<IEventBus>();
                var conventions = bus.Advanced.Container.Resolve<IConventions>();
                var publishExchangeDeclareStrategy = bus.Advanced.Container.Resolve<IPublishExchangeDeclareStrategy>();
                var messageDeliveryModeStrategy = bus.Advanced.Container.Resolve<IMessageDeliveryModeStrategy>();
                var timeoutStrategy = bus.Advanced.Container.Resolve<ITimeoutStrategy>();
                var configuration = bus.Advanced.Container.Resolve<ConnectionConfiguration>();

                ITagRpc tagRpc = new TagRpc(configuration, bus.Advanced, eventBus, conventions, publishExchangeDeclareStrategy, messageDeliveryModeStrategy, timeoutStrategy);
                rpcDict.Add(bus, tagRpc);
            }
            return rpcDict[bus];
        }
    }
}
