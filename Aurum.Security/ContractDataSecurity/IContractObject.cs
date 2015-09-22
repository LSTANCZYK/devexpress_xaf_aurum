using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;

namespace Aurum.Security
{
    /// <summary>
    /// Объект, операции над которым проверяются по контрактам
    /// </summary>
    public interface IContractObject
    {
    }

    /// <summary>
    /// Объект, операции над которым проверяются по контрактам, с типизированным контекстом безопасности
    /// </summary>
    public interface IContractObject<ContextType> : IContractObject
        where ContextType : IContractContext
    {
    }

    /// <summary>
    /// Объект, операции над которым проверяются по контрактам, с типизированным контекстом безопасности и датой контракта
    /// </summary>
    public interface IContractObjectDate<ContextType> : IContractObject<ContextType>
        where ContextType : IContractContext
    {
        /// <summary>
        /// Дата контракта, которая учитывается при проверке контрактов на доступ к операциям
        /// </summary>
        DateTime ContractDate { get; }
    }

    /// <summary>
    /// Объект, операции над которым проверяются по контрактам, с реализацией типизированной системы безопасности
    /// </summary>
    /// <typeparam name="ContextType">Тип контекста безопасности</typeparam>
    public interface IContractObjectImplementation<ContextType> : IContractObject<ContextType>
        where ContextType : IContractContext
    {
        /// <summary>
        /// Возвращает флаг разрешения на операцию над объектом
        /// </summary>
        /// <param name="operation">Операция, разрешение на которую запрашивается</param>
        /// <param name="context">Контекст безопасности операций с данными</param>
        /// <returns>True - если операция над объектом разрешена, иначе false</returns>
        /// <remarks>Базовые операции над объектом описаны в классе <see cref="T:SecurityOperations"/></remarks>
        bool IsGranted(string operation, ContextType context);

        /// <summary>
        /// Возвращает флаг разрешения на операцию со свойством или полем объекта
        /// </summary>
        /// <param name="operation">Операция, разрешение на которую запрашивается</param>
        /// <param name="memberName">Свойство или поле объекта, над которым должна быть выполнена операция</param>
        /// <param name="context">Контекст безопасности операций с данными</param>
        /// <returns>True - если операция над объектом разрешена, иначе false</returns>
        /// <remarks>Базовые операции над объектом описаны в классе <see cref="T:SecurityOperations"/></remarks>
        bool IsGranted(string operation, string memberName, ContextType context);
    }

    /// <summary>
    /// Контекст безопасности операций с данными, основанной на контрактах
    /// </summary>
    /// <remarks>Контекст безопасности определяет окружение, в котором выполняется проверка доступа к операциям 
    /// над объектами <see cref="IContractObject"/></remarks>
    public interface IContractContext
    {
        /// <summary>
        /// Значения контекста безопасности
        /// </summary>
        Dictionary<string, object> Values { get; }

        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="objectSpace">Пространство объектов для инициализации</param>
        /// <param name="user">Пользователь системы, для которого инициализируется контекст безопасности</param>
        void Initialize(IObjectSpace objectSpace, object user);
    }

    /// <summary>
    /// Контекст безопасности операций с данными, основанной на контрактах
    /// </summary>
    public abstract class ContractContext : IContractContext
    {
        private Dictionary<string, object> values = new Dictionary<string, object>();

        /// <contentfrom cref="IContractContext.Values"/>
        public Dictionary<string, object> Values { get { return values; } }

        /// <contentfrom cref="IContractContext.Initialize"/>
        public abstract void Initialize(IObjectSpace objectSpace, object user);
    }
}
