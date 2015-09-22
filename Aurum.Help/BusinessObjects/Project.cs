using Aurum.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Help
{
    /// <summary>
    /// Проект документации
    /// </summary>
    [NavigationItem("Help")]
    [DefaultClassOptions]
    public class Project : XPObject
    {
        private string name;

        public Project() : base() { }
        public Project(Session session) : base(session) { }

        /// <summary>
        /// Название
        /// </summary>
        [NotNull]
        public string Name
        {
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }

        /// <summary>
        /// Документы
        /// </summary>
        [Association("Project-Documents")]
        [Aggregated]
        public XPCollection<ProjectDocument> Documents
        {
            get { return GetCollection<ProjectDocument>("Documents"); }
        }
    }
}
