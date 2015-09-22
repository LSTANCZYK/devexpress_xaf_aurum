using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Courier
{
    /// <summary>
    /// Фабрика курьеров
    /// </summary>
    public static class CourierFactory
    {
        private static Dictionary<ICourier, Action<IConfigurator>> couriers = new Dictionary<ICourier, Action<IConfigurator>>();
        private static bool isConfigured = false;
        private static bool isInited = false;

        /// <summary>
        /// Конфигурация по умолчанию
        /// </summary>
        /// <param name="configure"></param>
        public static void ConfigureDefault(Action<IConfigurator> configure)
        {
            if (isConfigured)
                throw new AurumCourierException("CourierFactory configuration already initialized");
            if (configure != null)
                configure(RabbitConfigurator.Default);
            isConfigured = true;
        }

        /// <summary>
        /// Инициализация курьеров
        /// </summary>
        public static void InitCouriers()
        {
            if (isInited)
                throw new AurumCourierException("CourierFactory couriers already initialized");
            foreach (var kv in couriers)
            {
                ICourier courier = kv.Key;
                Action<IConfigurator> configure = kv.Value;
                RabbitConfigurator configurator = (RabbitConfigurator)RabbitConfigurator.Default.Clone();
                if (configure != null)
                    configure(configurator);
                courier.InitConfig(configurator);
                courier.InitExchanges();
            }
            isInited = true;
        }

        public static void DisposeAll()
        {
            foreach (ICourier courier in couriers.Keys)
            {
                courier.Dispose();
            }
        }

        /// <summary>
        /// Новый курьер
        /// </summary>
        /// <returns></returns>
        public static ICourier New()
        {
            return New(null);
        }

        /// <summary>
        /// Новый курьер
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static ICourier New(Action<IConfigurator> configure)
        {
            ICourier courier = NewRabbitCourier(configure);
            couriers.Add(courier, configure);
            if (isInited)
            {
                RabbitConfigurator configurator = (RabbitConfigurator)RabbitConfigurator.Default.Clone();
                if (configure != null)
                    configure(configurator);
                courier.InitConfig(configurator);
                courier.InitExchanges();
            }
            return courier;
        }

        private static RabbitCourier NewRabbitCourier(Action<IConfigurator> configure)
        {
            return new RabbitCourier();
        }
    }
}
