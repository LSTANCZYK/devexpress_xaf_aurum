using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Aurum.Xpo;

namespace Aurum.Security
{
    /// <summary>
    /// Пользователь в базе данных, являющийся сотрудником организации собственника данных
    /// </summary>
    [MapInheritance(MapInheritanceType.OwnTable)]
    [ImageName("BO_User")]
    public class OwnedUser : ServerUser, IOwnedObject
    {
        private Owner owner;

        /// <summary>Конструктор без параметров</summary>
        public OwnedUser() { }

        /// <summary>Конструктор с указанной сессией</summary>
        /// <param name="session">Сессия для загрузки и сохранения объектов в базе данных</param>
        public OwnedUser(Session session) : base(session) { }

        /// <summary>Конструктор с указанным собственником данных</summary>
        /// <param name="owner">Собственник данных, которому принадлежит объект</param>
        public OwnedUser(Owner owner) { this.Owner = owner; }

        /// <summary>Конструктор с указанной сессией и собственником данных</summary>
        /// <param name="session">Сессия для загрузки и сохранения объектов в базе данных</param>
        /// <param name="owner">Собственник данных, которому принадлежит объект</param>
        public OwnedUser(Session session, Owner owner) : base(session) { this.Owner = owner; }

        /// <summary>
        /// Собственник данных, которому принадлежит учетная запись пользователя
        /// </summary>
        [NotNull]
        public Owner Owner
        {
            get { return owner; }
            set { SetPropertyValue<Owner>("Owner", ref owner, value); }
        }
    }
}
