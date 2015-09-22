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
    /// Документ проекта
    /// </summary>
    [Hierarchy("Parent")]
    public class ProjectDocument : XPObject, ITreeNode
    {
        private string name;
        private int index;
        private string text;
        private ProjectDocument parent;
        private Project project;

        public ProjectDocument() : base() { }
        public ProjectDocument(Session session) : base(session) { }

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
        /// Индекс
        /// </summary>
        public int Index
        {
            get { return index; }
            set { SetPropertyValue("Index", ref index, value); }
        }

        /// <summary>
        /// Текст
        /// </summary>
        [Size(SizeAttribute.Unlimited)]
        public string Text
        {
            get { return text; }
            set { SetPropertyValue("Text", ref text, value); }
        }

        [Association("Project-Documents")]
        public Project Project
        {
            get { return project; }
            set { SetPropertyValue<Project>("Project", ref project, value); }
        }

        [Association("ProjectDocument-Childs")]
        public ProjectDocument Parent
        {
            get { return parent; }
            set { SetPropertyValue<ProjectDocument>("Parent", ref parent, value); }
        }

        [Association("ProjectDocument-Childs")]
        [Aggregated]
        [VisibleInDetailView(false)]
        public XPCollection<ProjectDocument> Childs
        {
            get { return GetCollection<ProjectDocument>("Childs"); }
        }

        System.ComponentModel.IBindingList ITreeNode.Children
        {
            get { return Childs; }
        }

        string ITreeNode.Name
        {
            get { return Name; }
        }

        ITreeNode ITreeNode.Parent
        {
            get
            {
                if (Parent != null)
                    return Parent;
                return null;
            }
        }
    }
}
