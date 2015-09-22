using Aurum.App.Module.BusinessObjects;
using Aurum.Exchange;
using Aurum.Operations;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.App.Module.Exchange
{
    public class PersonImport : CustomImportOperation<PersonImportParameters>
    {
        public PersonImport(XafApplication app)
            : base(app)
        {
            OperationInfo.Name = "Физ. лица";
            BeforeExchangeExecute += PersonImport_BeforeExchangeExecute;
        }

        void PersonImport_BeforeExchangeExecute(object sender, EventArgs e)
        {
            PersonImportFormatter formatter = new PersonImportFormatter();
            formatter.AddFile(ParametersObject.FileToOpen);
            AddFormatter(formatter);
        }

        private class PersonImportFormatter : InputTextFormatter
        {
            public override void ProcessData(IObjectSpace objectSpace, OperationInterop interop)
            {
                while (!Reader.EndOfStream)
                {
                    string lastname = Reader.ReadLine();
                    if (!string.IsNullOrEmpty(lastname))
                    {
                        var person = objectSpace.CreateObject<Person>();
                        person.Lastname = lastname;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Параметры для импорта
    /// </summary>
    [DomainComponent]
    public class PersonImportParameters : ExchangeParameters
    {
        [FilePathMode(FilePathMode.Open)]
        [FilePathFilter("Text files|*.txt|All files (*.*)|*.*")]
        public FilePath FileToOpen { get; set; }

        public DirectoryPath Dir { get; set; }

        [FilePathMode(FilePathMode.Open)]
        [FilePathFilter("Text files|*.txt|All files (*.*)|*.*")]
        public MultipleFilePath FilesToOpen { get; set; }
    }
}
