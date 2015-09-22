using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Aurum.Xpo;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace Aurum.Reports
{
    /// <summary>
    /// Предопределенный мастер отчетов
    /// </summary>
    [DefaultClassOptions, NavigationItem("Reports"), VisibleInReports(false)]
    public class PredefinedReportWizard : XPObjectBase
    {
        private ReportWizard main;
        private string name;

        /// <summary>Конструктор</summary>
        public PredefinedReportWizard() { main = new ReportWizard(); }

        /// <summary>Конструктор с указанной сессией</summary>
        /// <param name="session">Сессия для загрузки и хранения объектов базы данных</param>
        public PredefinedReportWizard(Session session) : base(session) { main = new ReportWizard(); }

        /// <summary>
        /// Название мастера отчетов
        /// </summary>
        [Persistent, NotNull, RuleRequiredField]
        [ModelDefault("Caption", "Wizard Name")]
        public string Name
        {
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }

        /// <summary>
        /// Мастер отчетов
        /// </summary>
        [Persistent, NotNull]
        [ExpandObjectMembers(ExpandObjectMembers.Always)]
        [ValueConverter(typeof(ReportWizardItemToBytesConverter))]
        [RuleRequiredField]
        public ReportWizard Main
        {
            get { return main; }
            set 
            {
                if (main != null) main.WizardChanged -= MainChanged;
                SetPropertyValue("Main", ref main, value);
                if (main != null) main.WizardChanged += MainChanged;
            }
        }

        void MainChanged(object sender, EventArgs e)
        {
            OnChanged("Main");
        }
    }

    /// <summary>
    /// Конвертор элемента мастера отчетов в Blob
    /// </summary>
    public class ReportWizardItemToBytesConverter : ValueConverter
    {
        /// <inheritdoc/>
        public override Type StorageType
        {
            get { return typeof(byte[]); }
        }

        /// <inheritdoc/>
        public override object ConvertFromStorageType(object value)
        {
            if (value == null) return null;
            byte[] bytes = (byte[]) value;
            ReportWizardItem item = null;
            using (MemoryStream stream = new MemoryStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Position = 0;
                XmlSerializer serializer = new XmlSerializer(typeof(ReportWizardItem));
                item = (ReportWizardItem)serializer.Deserialize(reader);
                reader.Close();
                stream.Close();
            }
            return item;
        }

        /// <inheritdoc/>
        public override object ConvertToStorageType(object value)
        {
            byte[] bytes = null;
            using (MemoryStream stream = new MemoryStream())
            using (StreamWriter writer = new StreamWriter(stream))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ReportWizardItem));
                serializer.Serialize(writer, value);
                stream.Position = 0;
                bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
                writer.Close();
                stream.Close();
            }
            return bytes;
        }
    }

    /// <summary>
    /// Валидатор сохранения предопределенного мастера отчетов
    /// </summary>
    [CodeRule]
    public class PredefinedReportWizardSaveRule : RuleBase<PredefinedReportWizard>
    {
        /// <summary>Конструктор</summary>
        public PredefinedReportWizardSaveRule() : base("PredefinedReportWizardSaveRule", ContextIdentifier.Save) { }

        /// <summary>Конструктор</summary>
        /// <param name="properties">Валидируемые свойства</param>
        public PredefinedReportWizardSaveRule(IRuleBaseProperties properties) : base(properties) { }

        /// <inheritdoc/>
        protected override bool IsValidInternal(PredefinedReportWizard target, out string errorMessageTemplate)
        {
            errorMessageTemplate = string.Empty;
            if (target == null || target.Main == null) return true;
            return target.Main.Validate(out errorMessageTemplate);
        }
    }
}
