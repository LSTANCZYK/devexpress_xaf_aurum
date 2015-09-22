using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Aurum.Xpo;

namespace Aurum.Security
{
    /// <summary>
    /// Поставщик пространства объектов с поддержкой безопасности базы данных
    /// </summary>
    public class AdminObjectSpaceProvider : XPObjectSpaceProvider
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dataStoreProvider">Поставщик базы данных</param>
        /// <param name="threadSafe">Флаг потоковой безопасности</param>
        public AdminObjectSpaceProvider(IXpoDataStoreProvider dataStoreProvider, bool threadSafe)
            : base(dataStoreProvider, threadSafe)
        {
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="connectionString">Строка соединения с базой данных</param>
        /// <param name="connection">Соединение с базой данных</param>
        /// <param name="threadSafe">Флаг потоковой безопасности</param>
        public AdminObjectSpaceProvider(string connectionString, IDbConnection connection, bool threadSafe = false)
            : base(connectionString, connection, threadSafe)
        {
        }

        /// <summary>
        /// Создание слоя данных на основе указанного поставщика данных
        /// </summary>
        /// <param name="dataStore">Поставщик данных</param>
        /// <returns>Возвращает слой данных с потоковой безопасностью или без, в зависимости от параметра threadSafe, заданного при вызове конструктора.</returns>
        /// <remarks>Возвращаемый объект слоя данных (<see cref="SimpleDataLayerEx"/> или <see cref="ThreadSafeDataLayerEx"/>) 
        /// поддерживает администрирование безопасности <see cref="Aurum.Xpo.IDataLayerSecurity"/>.</remarks>
        protected override IDataLayer CreateDataLayer(IDataStore dataStore)
        {
            if (threadSafe)
            {
                return new ThreadSafeDataLayerEx(XPDictionary, dataStore);
            }
            else
            {
                return new SimpleDataLayerEx(XPDictionary, dataStore);
            }
        }
    }

    /// <summary>
    /// Поставщик пространства объектов с поддержкой безопасности базы данных и сложной системой безопасности
    /// </summary>
    public class ServerObjectSpaceProvider : ComplexObjectSpaceProvider
    {
        /// <summary>Конструктор</summary>
        /// <param name="selectDataSecurityProviders">Поставщики безопасности операций с данными</param>
        /// <param name="databaseConnectionString">Строка соединения с базой данных</param>
        /// <param name="connection">Соединение с базой данных</param>
        public ServerObjectSpaceProvider(ISelectDataSecurityProvider[] selectDataSecurityProviders, string databaseConnectionString, IDbConnection connection)
			: base(selectDataSecurityProviders, databaseConnectionString, connection, true) {
		}

        /// <inheritdoc />
        protected override IDataLayer CreateDataLayer(IDataStore dataStore)
        {
            return threadSafe ?
                (IDataLayer)new ThreadSafeDataLayerEx(XPDictionary, dataStore) :
                (IDataLayer)new SimpleDataLayerEx(XPDictionary, dataStore);
        }
    }
}
