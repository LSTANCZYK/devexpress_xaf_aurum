using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aurum.Xpo;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace Aurum.Security
{
    /// <summary>
    /// Группа пользователей в базе данных
    /// </summary>
    public abstract class UserGroup : XPObjectBase
    {
        private string name;

        /// <summary>Конструктор без параметров</summary>
        public UserGroup() { }

        /// <summary>Конструктор с указанной сессией</summary>
        /// <param name="session">Сессия для загрузки и хранения объектов в базе данных</param>
        public UserGroup(Session session) : base(session) { }

        /// <summary>Название</summary>
        [NotNull, RuleRequiredField]
        public string Name
        {
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }

        /// <summary>Пользователи, входящие в группу</summary>
        [Association("Group-Users")]
        public XPCollection<ServerUser> Users
        {
            get { return GetCollection<ServerUser>("Users"); }
        }
    }
}
