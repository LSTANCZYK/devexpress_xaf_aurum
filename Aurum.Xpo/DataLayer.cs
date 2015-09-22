using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;

namespace Aurum.Xpo
{
    /// <summary>
    /// Слой данных с возможностью отмены операций с данными
    /// </summary>
    public interface IDataLayerCancelling : IDataLayer
    {
        /// <summary>
        /// Возможна ли отмена операций выборки данных
        /// </summary>
        bool CanCancelSelectData { get; }

        /// <summary>
        /// Отмена текущей операции выборки данных
        /// </summary>
        void CancelSelectData();
    }

    /// <summary>
    /// База данных Sql с возможностью отмены команд
    /// </summary>
    public interface ISqlDataStoreCancelling : ISqlDataStore
    {
        /// <summary>
        /// Возможна ли отмена команд выборки данных
        /// </summary>
        bool CanCancelSelect { get; }

        /// <summary>
        /// Отмена текущей команды выборки данных
        /// </summary>
        void CancelSelect();
    }

    /// <summary>
    /// Слой данных с поддержкой администрирования безопасности
    /// </summary>
    public interface IDataLayerSecurity : IDataLayer
    {
        /// <summary>Определяет, поддерживается ли администрирование безопасности</summary>
        bool CanAdminSecurity { get; }

        /// <summary>Выполняет команды администрирования безопасности</summary>
        /// <param name="statements">Команды администрирования безопасности</param>
        /// <returns>Результат выполнения команд администрирования безопасности</returns>
        SecurityResult AdminSecurity(params SecurityStatement[] statements);

        /// <summary>
        /// Проверяет доступ на выполнение команд администрирования безопасности
        /// </summary>
        /// <param name="statements">Команды администрирования безопасности</param>
        /// <returns>Флаги доступа для каждой перечисленной команды</returns>
        bool[] CheckAccess(params SecurityStatement[] statements);
    }

    /// <summary>
    /// База данных с поддержкой администрирования безопасности
    /// </summary>
    public interface ISqlDataStoreSecurity : ISqlDataStore
    {
        /// <summary>Выполняет команды администрирования безопасности</summary>
        /// <param name="statements">Команды администрирования безопасности</param>
        /// <returns>Результат выполнения команд администрирования безопасности</returns>
        SecurityResult AdminSecurity(params SecurityStatement[] statements);

        /// <summary>
        /// Проверяет доступ на выполнение команд администрирования безопасности
        /// </summary>
        /// <param name="statements">Команды администрирования безопасности</param>
        /// <returns>Флаги доступа для каждой перечисленной команды</returns>
        bool[] CheckAccess(params SecurityStatement[] statements);
    }

    /// <summary>
    /// Слой данных с поддержкой безопасных объектов доступа к данным
    /// </summary>
    public interface IDataLayerSafe : IDataLayer
    {
        /// <summary>Определяет, поддерживается ли обновление с безопасными объектами доступа к данным</summary>
        bool CanUpdateSafe { get; }

        /// <summary>
        /// Выполняет расширенное обновление структуры данных, включая безопасные объекты доступа к данным
        /// </summary>
        /// <param name="mode">Режим обновления структуры данных</param>
        /// <param name="dontCreateIfFirstTableNotExist">Не изменять структуру, если нет первой таблицы (обычно XPObjectType)</param>
        /// <param name="types">Типы данных, которые должны быть обновлены</param>
        /// <returns>Результат обновления структуры данных</returns>
        UpdateSchemaResult UpdateSchema(UpdateSchemaMode mode, bool dontCreateIfFirstTableNotExist, params XPClassInfo[] types);
    }

    /// <summary>
    /// База данных с поддержкой безопасных объектов доступа к данным
    /// </summary>
    /// <remarks>Под безопасными объектами доступа к данным понимаются представления и хранимые процедуры,
    /// а также сопутствующие объекты, такие как констрейнты, обеспечивающие целостность данных</remarks>
    public interface ISqlDataStoreSafe : ISqlDataStore, ISecuredSqlGeneratorFormatter
    {
        /// <summary>
        /// Выполняет расширенное обновление структуры базы данных, включая безопасные объекты доступа к данным
        /// </summary>
        /// <param name="mode">Режим обновления структуры базы данных</param>
        /// <param name="dontCreateIfFirstTableNotExist">Не изменять структуру, если нет первой таблицы (обычно XPObjectType)</param>
        /// <param name="tables">Таблицы, которые должны быть обновлены</param>
        /// <returns>Результат обновления структуры базы данных</returns>
        UpdateSchemaResult UpdateSchema(UpdateSchemaMode mode, bool dontCreateIfFirstTableNotExist, params DBTable[] tables);
    }

    /// <summary>
    /// Режим обновления схемы
    /// </summary>
    public enum UpdateSchemaMode
    {
        /// <summary>Полное обновление</summary>
        Full = 0
    }

    /// <summary>
    /// Расширенный слой данных
    /// </summary>
    /// <remarks>Расширения включают:<list type="bullet">
    /// <item>Поддержка отмены операции (<see cref="IDataLayerCancelling"/>)</item>
    /// <item>Поддержка базы данных с администрированием безопасности (<see cref="IDataLayerSecurity"/>)</item></list></remarks>
    /// <seealso cref="IDataLayerCancelling"/><seealso cref="IDataLayerSecurity"/>
    public class SimpleDataLayerEx : SimpleDataLayer, IDataLayerCancelling, IDataLayerSecurity
    {
        private ISqlDataStoreCancelling dataStoreCancelling;
        private ISqlDataStoreSecurity dataStoreSecurity;

        /// <summary>
        /// Конструктор с указанным поставщиком данных 
        /// </summary>
        /// <param name="provider">Поставщик данных</param>
		public SimpleDataLayerEx(IDataStore provider) 
            : base(provider) 
        { 
            dataStoreCancelling = provider as ISqlDataStoreCancelling;
            dataStoreSecurity = provider as ISqlDataStoreSecurity;
        }

        /// <summary>
        /// Конструктор с указанными справочником метаданных и поставщиком данных
        /// </summary>
        /// <param name="dictionary">Справочник метаданных</param>
        /// <param name="provider">Поставщик данных</param>
        public SimpleDataLayerEx(XPDictionary dictionary, IDataStore provider) 
            : base(dictionary, provider) 
        { 
            dataStoreCancelling = provider as ISqlDataStoreCancelling;
            dataStoreSecurity = provider as ISqlDataStoreSecurity;
        }

        /// <contentfrom cref="IDataLayerCancelling.CanCancelSelectData" />
        public bool CanCancelSelectData
        {
            get { return dataStoreCancelling != null ? dataStoreCancelling.CanCancelSelect : false; }
        }

        /// <contentfrom cref="IDataLayerCancelling.CancelSelectData" />
        public void CancelSelectData()
        {
            if (dataStoreCancelling != null)
                dataStoreCancelling.CancelSelect();
            else
                throw new InvalidOperationException("Select data cancelling is not supported by data provider");
        }

        /// <contentfrom cref="IDataLayerSecurity.CanAdminSecurity" />
        public bool CanAdminSecurity
        {
            get { return dataStoreSecurity != null; }
        }

        /// <contentfrom cref="IDataLayerSecurity.AdminSecurity" />
        public SecurityResult AdminSecurity(params SecurityStatement[] statements)
        {
            if (CanAdminSecurity)
                return dataStoreSecurity.AdminSecurity(statements);
            else
                throw new AdminSecurityNotSupportedException("Updating security is not supported by data provider");
        }

        /// <contentfrom cref="IDataLayerSecurity.CheckAccess" />
        public bool[] CheckAccess(params SecurityStatement[] statements)
        {
            if (CanAdminSecurity)
                return dataStoreSecurity.CheckAccess(statements);
            else
                throw new AdminSecurityNotSupportedException("Updating security is not supported by data provider");
        }
    }

    /// <summary>
    /// Расширенный слой данных с потоковой безопасностью
    /// </summary>
    /// <remarks>Расширения включают:<list type="bullet">
    /// <item>Поддержка отмены операции (<see cref="IDataLayerCancelling"/>)</item>
    /// <item>Поддержка базы данных с администрированием безопасности (<see cref="IDataLayerSecurity"/>)</item></list></remarks>
    /// <seealso cref="IDataLayerCancelling"/><seealso cref="IDataLayerSecurity"/>
    public class ThreadSafeDataLayerEx : ThreadSafeDataLayer, IDataLayerCancelling, IDataLayerSecurity
    {
        private ISqlDataStoreCancelling dataStoreCancelling;
        private ISqlDataStoreSecurity dataStoreSecurity;

        /// <summary>
        /// Конструктор с указанными справочником метаданных, поставщиком данных и сборками хранимых объектов
        /// </summary>
        /// <param name="dictionary">Справочник метаданных</param>
        /// <param name="provider">Поставщик данных</param>
        /// <param name="persistentObjectsAssemblies">Сборки хранимых объектов</param>
        public ThreadSafeDataLayerEx(XPDictionary dictionary, IDataStore provider, params Assembly[] persistentObjectsAssemblies)
            : base(dictionary, provider, persistentObjectsAssemblies)
        {
            dataStoreCancelling = provider as ISqlDataStoreCancelling;
            dataStoreSecurity = provider as ISqlDataStoreSecurity;
        }

        /// <contentfrom cref="IDataLayerCancelling.CanCancelSelectData" />
        public bool CanCancelSelectData
        {
            get { return dataStoreCancelling != null ? dataStoreCancelling.CanCancelSelect : false; }
        }

        /// <contentfrom cref="IDataLayerCancelling.CancelSelectData" />
        public void CancelSelectData()
        {
            if (dataStoreCancelling != null)
                dataStoreCancelling.CancelSelect();
            else
                throw new InvalidOperationException("Select data cancelling is not supported by data provider");
        }

        /// <contentfrom cref="IDataLayerSecurity.CanAdminSecurity" />
        public bool CanAdminSecurity
        {
            get { return dataStoreSecurity != null; }
        }

        /// <contentfrom cref="IDataLayerSecurity.AdminSecurity" />
        public SecurityResult AdminSecurity(params SecurityStatement[] statements)
        {
            if (CanAdminSecurity)
                return dataStoreSecurity.AdminSecurity(statements);
            else
                throw new AdminSecurityNotSupportedException("Updating security is not supported by data provider");
        }

        /// <contentfrom cref="IDataLayerSecurity.CheckAccess" />
        public bool[] CheckAccess(params SecurityStatement[] statements)
        {
            if (CanAdminSecurity)
                return dataStoreSecurity.CheckAccess(statements);
            else
                throw new AdminSecurityNotSupportedException("Updating security is not supported by data provider");
        }
    }

    /// <summary>
    /// Исключение, вызванное отменой операции с данными пользователем
    /// </summary>
    public class UserCancelException : Exception
    {
    }
}
