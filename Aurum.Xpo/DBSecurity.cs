using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo.DB;

namespace Aurum.Xpo
{
    /// <summary>
    /// Команда администрирования безопасности базы данных
    /// </summary>
    /// <example><code>
    /// new SecurityStatement(AdminSecurityOperations.GrantTo, 
    ///     new SecurityObject(SecurityObjectTypes.Role, "Emperor"),
    ///     new SecurityObject(SecurityObjectTypes.User, "HVV"));
    /// </code></example>
    public class SecurityStatement : BaseStatement
    {
        /// <summary>Операция</summary>
        public AdminSecurityOperations Operation;

        /// <summary>Левый операнд</summary>
        public SecurityObject LeftOperand;

        /// <summary>Правый операнд</summary>
        public SecurityObject RightOperand;

        /// <summary>
        /// Конструктор с одним операндом
        /// </summary>
        /// <param name="operation">Операция</param>
        /// <param name="left">Операнд</param>
        public SecurityStatement(AdminSecurityOperations operation, SecurityObject left)
        {
            this.Operation = operation;
            this.LeftOperand = left;
        }

        /// <summary>
        /// Конструктор с двумя операндами
        /// </summary>
        /// <param name="operation">Операция</param>
        /// <param name="left">Левый операнд</param>
        /// <param name="right">Правый операнд</param>
        public SecurityStatement(AdminSecurityOperations operation, SecurityObject left, SecurityObject right)
        {
            this.Operation = operation;
            this.LeftOperand = left;
            this.RightOperand = right;
        }

        /// <summary>
        /// Выполняет проверку валидности команды
        /// </summary>
        /// <exception cref="InvalidOperationException">Неверные параметры команды</exception>
        public void CheckValidity()
        {
            bool isValid = true;
            switch (Operation)
            {
                case AdminSecurityOperations.Create: 
                case AdminSecurityOperations.Drop:
                    isValid = LeftOperand != null && (
                        LeftOperand.ObjectType == SecurityObjectTypes.User ||
                        LeftOperand.ObjectType == SecurityObjectTypes.Role);
                    break;
                case AdminSecurityOperations.GrantTo:
                case AdminSecurityOperations.RevokeFrom:
                    isValid = LeftOperand != null && RightOperand != null && (
                        LeftOperand.ObjectType == SecurityObjectTypes.Table ||
                        LeftOperand.ObjectType == SecurityObjectTypes.Role) && (
                        RightOperand.ObjectType == SecurityObjectTypes.User ||
                        RightOperand.ObjectType == SecurityObjectTypes.Role);
                    break;
                case AdminSecurityOperations.SetUserInfo:
                    isValid = LeftOperand != null && RightOperand != null && 
                        LeftOperand.ObjectType == SecurityObjectTypes.User &&
                        RightOperand.ObjectType == SecurityObjectTypes.UserInfo;
                    break;
                case AdminSecurityOperations.GetRolePrivileges:
                    isValid = LeftOperand != null &&
                        LeftOperand.ObjectType == SecurityObjectTypes.Role;
                    break;
                case AdminSecurityOperations.GetCurrentPrivileges: 
                    break;
                case AdminSecurityOperations.GetTable:
                    isValid = LeftOperand != null && (
                        LeftOperand.ObjectType == SecurityObjectTypes.User ||
                        LeftOperand.ObjectType == SecurityObjectTypes.Role);
                    break;
            }
            if (!isValid) 
                throw new InvalidOperationException("Incorrect parameters of sequrity statement: " + ToString());
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (LeftOperand != null && RightOperand == null)
                return string.Format("{0} {1}", Operation, LeftOperand);
            if (LeftOperand != null && RightOperand != null)
                return string.Format("{1} {0} {2}", Operation, LeftOperand, RightOperand);
            return base.ToString();
        }
    }

    /// <summary>
    /// Результат команд администрирования безопасности базы данных
    /// </summary>
    public class SecurityResult
    {
        /// <summary>Объекты безопасности, полученные в результате команд администрирования</summary>
        public SecurityObject[] Objects;

        /// <summary>Конструктор</summary>
        public SecurityResult()
        {
            this.Objects = new SecurityObject[0];
        }

        /// <summary>Конструктор</summary>
        /// <param name="objects">Объекты безопасности, полученные в результате команд администрирования</param>
        public SecurityResult(SecurityObject[] objects)
        {
            this.Objects = objects;
        }

        /// <summary>Конструктор</summary>
        /// <param name="objects">Объекты безопасности, полученные в результате команд администрирования</param>
        public SecurityResult(ICollection<SecurityObject> objects)
        {
            this.Objects = objects.ToArray();
        }
    }

    /// <summary>
    /// Операции администрирования безопасности базы данных
    /// </summary>
    public enum AdminSecurityOperations
    {
        /// <summary>Создание объекта безопасности</summary>
        /// <remarks>Не возвращает результат</remarks>
        Create,

        /// <summary>Удаление объекта безопасности</summary>
        /// <remarks>Не возвращает результат</remarks>
        Drop,

        /// <summary>Назначение прав на доступ к первому объекту безопасности для второго объекта безопасности</summary>
        /// <remarks>Не возвращает результат</remarks>
        GrantTo,

        /// <summary>Отзыв прав на доступ к первому объекту безопасности от второго объекта безопасности</summary>
        /// <remarks>Не возвращает результат</remarks>
        RevokeFrom,

        /// <summary>Запрос привилегий для указанной роли</summary>
        /// <remarks>Возвращает привилегии указанной роли</remarks>
        GetRolePrivileges,

        /// <summary>Запрос привилегий на несистемные объекты для текущего пользователя</summary>
        /// <remarks>Возвращает привилегии текущего пользователя</remarks>
        GetCurrentPrivileges,

        /// <summary>Устанавливает информацию пользователя</summary>
        /// <remarks>Не возвращает результат</remarks>
        SetUserInfo,

        /// <summary>Запрос таблицы для указанного типа объектов безопасности</summary>
        /// <remarks>Возвращает таблицу, представляющую указанный тип объектов безопасности</remarks>
        GetTable
    }

    /// <summary>
    /// Объект безопасности базы данных
    /// </summary>
    public class SecurityObject
    {
        /// <summary>Тип объекта безопасности</summary>
        public SecurityObjectTypes ObjectType;

        /// <summary>Название объекта безопасности</summary>
        public string ObjectName;

        /// <summary>Права на доступ к данным таблицы</summary>
        public SecurityTableRights TableRights;

        /// <summary>
        /// Конструктор с типом и названием
        /// </summary>
        /// <param name="type">Тип объекта безопасности</param>
        /// <param name="name">Название объекта безопасности</param>
        public SecurityObject(SecurityObjectTypes type, string name)
        {
            this.ObjectType = type;
            this.ObjectName = name;
            this.TableRights = SecurityTableRights.None;
        }

        /// <summary>
        /// Конструктор объекта безопасности таблицы с правами доступа
        /// </summary>
        /// <param name="table">Название таблицы</param>
        /// <param name="rights">Права доступа к данным таблицы</param>
        public SecurityObject(string table, SecurityTableRights rights)
        {
            this.ObjectType = SecurityObjectTypes.Table;
            this.ObjectName = table;
            this.TableRights = rights;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0}[{1}{2}]", ObjectType, ObjectName,
                ObjectType == SecurityObjectTypes.Table ? "(" + TableRights.ToString() + ")" : string.Empty);
        }

        /// <summary>
        /// Возвращает сумму прав доступа на таблицы указанной коллекции
        /// </summary>
        /// <param name="collection">Исходная коллекция объектов безопасности</param>
        /// <returns>Коллекция объектов безопасности таблиц с правами доступа, сгруппированные по названию таблицы</returns>
        public static IEnumerable<SecurityObject> TablesSum(IEnumerable<SecurityObject> collection)
        {
            return collection
                .Where(so => so.ObjectType == SecurityObjectTypes.Table)
                .GroupBy(so => so.ObjectName, (tableName, tables) => 
                    new SecurityObject(tableName, tables.Aggregate(SecurityTableRights.None, 
                        (rights, table) => rights |= table.TableRights)));
        }

        /// <summary>
        /// Возвращает разницу прав доступа на таблицы первой и второй коллекции
        /// </summary>
        /// <param name="first">Первая коллекция, которая уменьшается</param>
        /// <param name="second">Вторая коллекция, которая вычитается</param>
        /// <returns>Разница прав доступа на таблицы первой и второй коллекции</returns>
        public static IEnumerable<SecurityObject> TablesDiff(IEnumerable<SecurityObject> first, IEnumerable<SecurityObject> second)
        {
            var a = TablesSum(first);
            var b = TablesSum(second).ToDictionary(so => so.ObjectName); 
            SecurityObject minus;
            return a
                .Select(table => b.TryGetValue(table.ObjectName, out minus) ?
                    new SecurityObject(table.ObjectName, table.TableRights ^ (table.TableRights & minus.TableRights)) : table)
                .Where(table => table.TableRights != SecurityTableRights.None);
        }
    }

    /// <summary>
    /// Типы объектов безопасности базы данных
    /// </summary>
    public enum SecurityObjectTypes
    {
        /// <summary>Роль базы данных</summary>
        Role,
        
        /// <summary>Пользователь базы данных</summary>
        User,
        
        /// <summary>Таблица базы данных</summary>
        Table,
        
        /// <summary>Информация о пользователе базы данных</summary>
        UserInfo
    }

    /// <summary>
    /// Права на доступ к данным таблицы базы данных
    /// </summary>
    [Flags]
    public enum SecurityTableRights
    {
        /// <summary>Отсутствие прав</summary>
        None = 0,

        /// <summary>Право на выборку данных</summary>
        Select = 1,

        /// <summary>Право на добавление данных</summary>
        Insert = 2,

        /// <summary>Право на изменение данных</summary>
        Update = 4,

        /// <summary>Право на удаление данных</summary>
        Delete = 8
    }

    /// <summary>
    /// Информация о пользователе базы данных
    /// </summary>
    public class SecurityUserInfo : SecurityObject
    {
        /// <summary>Пользователь может подключаться к базе данных</summary>
        public bool? IsActive;

        /// <summary>Пароль пользователя имеет истекший срок действия и должен быть изменен</summary>
        public bool? IsExpired;

        /// <summary>Конструктор</summary>
        public SecurityUserInfo() : base(SecurityObjectTypes.UserInfo, null) { }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="isActive">Пользователь может подключаться к базе данных</param>
        /// <param name="isExpired">Пароль пользователя имеет истекший срок действия и должен быть изменен</param>
        public SecurityUserInfo(bool? isActive, bool? isExpired) 
            : base(SecurityObjectTypes.UserInfo, null)
        {
            this.IsActive = isActive;
            this.IsExpired = isExpired;
        }
    }

    /// <summary>
    /// Исключение администрирования безопасности базы данных
    /// </summary>
    public class AdminSecurityException : Exception
    {
        /// <inheritdoc/>
        public AdminSecurityException() { }
        /// <inheritdoc/>
        public AdminSecurityException(string message) : base(message) { }
        /// <inheritdoc/>
        public AdminSecurityException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Вызывается, когда администрирование безопасности не поддерживается
    /// </summary>
    public class AdminSecurityNotSupportedException : AdminSecurityException
    {
        /// <inheritdoc/>
        public AdminSecurityNotSupportedException() : base("Security administration is not supported") { }
        /// <inheritdoc/>
        public AdminSecurityNotSupportedException(string message) : base(message) { }
    }

    /// <summary>Вызывается, когда роль базы данных конфликтует с уже существующей</summary>
    public class RoleConflictsException : AdminSecurityException { }

    /// <summary>Вызывается, когда роль базы данных не существует</summary>
    public class RoleNotExistsException : AdminSecurityException { }

    /// <summary>Вызывается, когда роль базы данных назначается самой себе (непосредственно или через несколько ролей)</summary>
    public class RoleCircularGrantException : AdminSecurityException { }

    /// <summary>Вызывается, когда пользователь базы данных конфликтует с уже существующим</summary>
    public class UserConflictsException : AdminSecurityException { }

    /// <summary>Вызывается, когда пользователь базы данных не существует</summary>
    public class UserNotExistsException : AdminSecurityException { }

    /// <summary>Вызывается, когда права на объект безопасности не назначены другому объекту</summary>
    public class ObjectNotGrantedException : AdminSecurityException { }
}
