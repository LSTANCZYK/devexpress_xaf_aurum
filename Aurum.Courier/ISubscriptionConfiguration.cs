using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Courier
{
    /// <summary>
    /// Конфигурация подписки
    /// </summary>
    public interface ISubscriptionConfiguration
    {
        /// <summary>
        /// Автоудаление
        /// </summary>
        /// <param name="autoDelete"></param>
        /// <returns></returns>
        ISubscriptionConfiguration WithAutoDelete(bool autoDelete = true);
        
        //
        // Summary:
        //     Configures the consumer's x-cancel-on-ha-failover attribute
        ISubscriptionConfiguration WithCancelOnHaFailover(bool cancelOnHaFailover = true);
        
        /// <summary>
        /// Срок истечения
        /// </summary>
        /// <param name="expires">время истечения (мс)</param>
        /// <returns></returns>
        ISubscriptionConfiguration WithExpires(int expires);
        
        /// <summary>
        /// Размер выборки
        /// </summary>
        /// <param name="prefetchCount"></param>
        /// <returns></returns>
        ISubscriptionConfiguration WithPrefetchCount(ushort prefetchCount);
        
        /// <summary>
        /// Приоритет
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        ISubscriptionConfiguration WithPriority(int priority);
        
        /// <summary>
        /// Тема
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        ISubscriptionConfiguration WithTopic(string topic);
    }

    public class SubscriptionConfiguration : ISubscriptionConfiguration
    {
        public IList<string> Topics { get; private set; }
        public bool AutoDelete { get; private set; }
        public int Priority { get; private set; }
        public bool CancelOnHaFailover { get; private set; }
        public ushort PrefetchCount { get; private set; }
        public int Expires { get; private set; }

        public SubscriptionConfiguration(ushort defaultPrefetchCount)
        {
            Topics = new List<string>();
            AutoDelete = false;
            Priority = 0;
            CancelOnHaFailover = false;
            PrefetchCount = defaultPrefetchCount;
            Expires = int.MaxValue;
        }

        public ISubscriptionConfiguration WithTopic(string topic)
        {
            Topics.Add(topic);
            return this;
        }

        public ISubscriptionConfiguration WithAutoDelete(bool autoDelete = true)
        {
            AutoDelete = autoDelete;
            return this;
        }

        public ISubscriptionConfiguration WithPriority(int priority)
        {
            Priority = priority;
            return this;
        }

        public ISubscriptionConfiguration WithCancelOnHaFailover(bool cancelOnHaFailover = true)
        {
            CancelOnHaFailover = cancelOnHaFailover;
            return this;
        }

        public ISubscriptionConfiguration WithPrefetchCount(ushort prefetchCount)
        {
            PrefetchCount = prefetchCount;
            return this;
        }

        public ISubscriptionConfiguration WithExpires(int expires)
        {
            Expires = expires;
            return this;
        }
    }
}
