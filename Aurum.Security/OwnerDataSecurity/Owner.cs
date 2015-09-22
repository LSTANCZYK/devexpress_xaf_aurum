using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Aurum.Xpo;
using System.ComponentModel;

namespace Aurum.Security
{
    /// <summary>
    /// Собственник данных в базе данных
    /// </summary>
    [ImageName("BO_Customer")]
    public class Owner : XPObjectBase
    {
        /// <summary>
        /// Собственник данных текущего пользователя
        /// </summary>
        public static Owner UserOwner
        {
            get { return SecuritySystem.CurrentUser is IOwnedObject ? ((IOwnedObject)SecuritySystem.CurrentUser).Owner : null; }
        }

        /// <summary>
        /// Возвращает собственника данных текущего пользователя для указанной сессии
        /// </summary>
        /// <param name="session">Сессия для сохранения и загрузки объектов</param>
        /// <returns>Собственник даных текущего пользователя</returns>
        public static Owner GetUserOwner(Session session)
        {
            if (UserOwner == null) return null;
            return (Owner)session.GetObjectByKey(UserOwner.ClassInfo, UserOwner.Id);
        }

        /// <summary>
        /// Возвращает собственника данных текущего пользователя для указанного пространства объектов
        /// </summary>
        /// <param name="objectSpace">Пространство объектов для сохранения и загрузки объектов</param>
        /// <returns>Собственник даных текущего пользователя</returns>
        public static Owner GetUserOwner(IObjectSpace objectSpace)
        {
            if (UserOwner == null) return null;
            return (Owner)objectSpace.GetObjectByKey(typeof(Owner), UserOwner.Id);
        }

        private string name;
        private OwnerRights rights;

        /// <summary>Конструктор без параметров</summary>
        public Owner() { }

        /// <summary>Конструктор с указанной сессией</summary>
        /// <param name="session">Сессия для загрузки и сохранения объектов в базе данных</param>
        public Owner(Session session) : base(session) { }

        /// <summary>
        /// Имя собственника данных
        /// </summary>
        public string Name
        {
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }

        /// <summary>
        /// Права собственника данных
        /// </summary>
        [Browsable(false)]
        public OwnerRights Rights
        {
            get { return rights; }
            set { SetPropertyValue("Rights", ref rights, value); }
        }

        /// <summary>
        /// Данные текущего собственника доступны для чтения всем собственникам данных 
        /// </summary>
        [Browsable(false)]
        public bool Public
        {
            get { return (Rights & OwnerRights.Public) == OwnerRights.Public; }
        }

        /// <summary>
        /// Возвращает коллекцию родительских договоров собственника данных
        /// </summary>
        /// <returns>Коллекция договоров на доступ к данным других собственников</returns>
        public XPCollection<Contract> GetParentContracts()
        {
            return new XPCollection<Contract>(Session, new BinaryOperator("Grantee", Id));
        }
    }

    /// <summary>
    /// Права собственника данных
    /// </summary>
    [Flags]
    public enum OwnerRights
    {
        /// <summary>Отсутствие специальных прав</summary>
        Default = 0,

        /// <summary>Данные текущего собственника доступны для чтения всем собственникам данных</summary>
        Public = 1,

        /// <summary>Собственник имеет права на чтение данных всех собственников</summary>
        Reader = 2,

        /// <summary>Собственник имеет права на чтение и запись данных всех собственников</summary>
        Administrator = 4
    }
}
