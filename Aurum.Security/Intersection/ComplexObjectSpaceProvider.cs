using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;

namespace Aurum.Security
{
    /// <summary>
    /// Поставщик пространства объектов со сложной системой безопасности
    /// </summary>
    /// <remarks>Поставщик со сложной системой безопасности осуществляет перекрестное наложение более простых систем,
    /// используя сложное правило безопасности <see cref="ComplexRule"/></remarks>
    public class ComplexObjectSpaceProvider : XPObjectSpaceProvider
    {
        private bool allowICommandChannelDoWithSecurityContext;
        private ISelectDataSecurityProvider[] selectDataSecurityProviders;
        private ISelectDataSecurity[] securities = null;
        
        private void Initialize(ISelectDataSecurityProvider[] selectDataSecurityProviders)
        {
            if (selectDataSecurityProviders == null || selectDataSecurityProviders.Length == 0 ||
                selectDataSecurityProviders.Any(provider => provider == null))
                throw new ArgumentNullException("selectDataSecurityProviders");
            this.selectDataSecurityProviders = selectDataSecurityProviders;
        }

        /// <inheritdoc />
        protected override UnitOfWork CreateUnitOfWork(IDataLayer dataLayer)
        {
            UnitOfWork directBaseUow = new UnitOfWork(dataLayer);
            if (securities == null)
            {
                securities = selectDataSecurityProviders.Select(
                    provider => provider.CreateSelectDataSecurity()).ToArray();
            }
            SessionObjectLayer currentObjectLayer = new SecuredSessionObjectLayer(
                allowICommandChannelDoWithSecurityContext, directBaseUow, true, null, 
                new ComplexRuleProvider(XPDictionary, securities), null);
            return new UnitOfWork(currentObjectLayer, directBaseUow);
        }

        /// <summary>Конструктор</summary>
        /// <param name="selectDataSecurityProviders">Поставщики безопасности операций с данными</param>
        /// <param name="dataStoreProvider">Поставщик базы данных</param>
        /// <param name="typesInfo"></param>
        /// <param name="xpoTypeInfoSource"></param>
        /// <param name="threadSafe">Флаг потоковой безоопасности</param>
        public ComplexObjectSpaceProvider(ISelectDataSecurityProvider[] selectDataSecurityProviders, IXpoDataStoreProvider dataStoreProvider, ITypesInfo typesInfo, XpoTypeInfoSource xpoTypeInfoSource, Boolean threadSafe)
			: base(dataStoreProvider, typesInfo, xpoTypeInfoSource, threadSafe) {
			Initialize(selectDataSecurityProviders);
		}

        /// <summary>Конструктор</summary>
        /// <param name="selectDataSecurityProviders">Поставщики безопасности операций с данными</param>
        /// <param name="dataStoreProvider">Поставщик базы данных</param>
        /// <param name="threadSafe">Флаг потоковой безоопасности</param>
        public ComplexObjectSpaceProvider(ISelectDataSecurityProvider[] selectDataSecurityProviders, IXpoDataStoreProvider dataStoreProvider, Boolean threadSafe)
			: base(dataStoreProvider, threadSafe) {
			Initialize(selectDataSecurityProviders);
		}

        /// <summary>Конструктор</summary>
        /// <param name="selectDataSecurityProviders">Поставщики безопасности операций с данными</param>
        /// <param name="databaseConnectionString">Строка соединения с базой данных</param>
        /// <param name="connection">Соединение с базой данных</param>
        /// <param name="threadSafe">Флаг потоковой безоопасности</param>
        public ComplexObjectSpaceProvider(ISelectDataSecurityProvider[] selectDataSecurityProviders, string databaseConnectionString, IDbConnection connection, Boolean threadSafe)
			: base(databaseConnectionString, connection, threadSafe) {
			Initialize(selectDataSecurityProviders);
		}

        /// <summary>Конструктор</summary>
        /// <param name="selectDataSecurityProviders">Поставщики безопасности операций с данными</param>
        /// <param name="dataStoreProvider">Поставщик базы данных</param>
        /// <param name="typesInfo"></param>
        /// <param name="xpoTypeInfoSource"></param>
        public ComplexObjectSpaceProvider(ISelectDataSecurityProvider[] selectDataSecurityProviders, IXpoDataStoreProvider dataStoreProvider, ITypesInfo typesInfo, XpoTypeInfoSource xpoTypeInfoSource)
			: this(selectDataSecurityProviders, dataStoreProvider, typesInfo, xpoTypeInfoSource, true) {
		}

        /// <summary>Конструктор</summary>
        /// <param name="selectDataSecurityProviders">Поставщики безопасности операций с данными</param>
        /// <param name="dataStoreProvider">Поставщик базы данных</param>
        public ComplexObjectSpaceProvider(ISelectDataSecurityProvider[] selectDataSecurityProviders, IXpoDataStoreProvider dataStoreProvider)
			: this(selectDataSecurityProviders, dataStoreProvider, true) {
		}

        /// <summary>Конструктор</summary>
        /// <param name="selectDataSecurityProviders">Поставщики безопасности операций с данными</param>
        /// <param name="databaseConnectionString">Строка соединения с базой данных</param>
        /// <param name="connection">Соединение с базой данных</param>
        public ComplexObjectSpaceProvider(ISelectDataSecurityProvider[] selectDataSecurityProviders, string databaseConnectionString, IDbConnection connection)
			: this(selectDataSecurityProviders, databaseConnectionString, connection, true) {
		}

        /// <summary>
        /// Флаг разрешения вызова методов <see cref="DevExpress.Xpo.Helpers.ICommandChannel"/>
        /// </summary>
        public bool AllowICommandChannelDoWithSecurityContext
        {
            get { return allowICommandChannelDoWithSecurityContext; }
            set { allowICommandChannelDoWithSecurityContext = value; }
        }
    }
}
