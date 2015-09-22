using Aurum.App.Module.BusinessObjects;
using Aurum.Exchange;
using Aurum.Operations;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.App.Module.Exchange
{
    [ParametersType(typeof(ComplexImportParameters))]
    public class ComplexImport : CompositeExchangeOperation<ComplexImportParameters>
    {
        public ComplexImport(XafApplication s)
            : base(s)
        {
            IsParallel = true;
            AddExchange<PersonImportInt>();
            AddExchange<HouseImportInt>();
            AddExchange<DbfImportInt>();
            BeforeExchangeExecute += ComplexImport_BeforeExchangeExecute;
        }

        void ComplexImport_BeforeExchangeExecute(object sender, EventArgs e)
        {
        }

        private class PersonImportInt : CustomImportOperation<ComplexImportParameters>
        {
            public PersonImportInt(XafApplication app)
                : base(app)
            {
                OperationInfo.Name = "Физ. лица";
                BeforeExchangeExecute += PersonImport_BeforeExchangeExecute;
            }

            void PersonImport_BeforeExchangeExecute(object sender, EventArgs e)
            {
                if (ParametersObject.Persons != null)
                {
                    PersonImportFormatter formatter = new PersonImportFormatter();
                    formatter.AddFile(ParametersObject.Persons);
                    AddFormatter(formatter);
                }
            }

            private class PersonImportFormatter : InputTextFormatter
            {
                public override void ProcessData(IObjectSpace objectSpace, OperationInterop interop)
                {
                    while (!Reader.EndOfStream)
                    {
                        interop.ThrowIfCancellationRequested();

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

        private class HouseImportInt : CustomImportOperation<ComplexImportParameters>
        {
            public HouseImportInt(XafApplication app)
                : base(app)
            {
                OperationInfo.Name = "Дома";
                BeforeExchangeExecute += HouseImportInt_BeforeExchangeExecute;
            }

            void HouseImportInt_BeforeExchangeExecute(object sender, EventArgs e)
            {
                if (ParametersObject.Houses != null)
                {
                    HouseImportFormatter formatter = new HouseImportFormatter();
                    formatter.AddFile(ParametersObject.Houses);
                    AddFormatter(formatter);
                }
            }

            private class HouseImportFormatter : InputTextFormatter
            {
                public override void ProcessData(IObjectSpace objectSpace, OperationInterop interop)
                {
                    while (!Reader.EndOfStream)
                    {
                        interop.ThrowIfCancellationRequested();

                        string line = Reader.ReadLine();
                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            if (parts != null && parts.Length >= 2)
                            {
                                var house = objectSpace.CreateObject<House>();
                                house.Street = parts[0];
                                house.Num = parts[1];
                            }
                        }
                    }
                }
            }
        }

        private class DbfImportInt : CustomImportOperation<ComplexImportParameters>
        {
            public DbfImportInt(XafApplication app)
                : base(app)
            {
                OperationInfo.Name = "Импорт из DBF-файла";
                BeforeExchangeExecute += DbfImportInt_BeforeExchangeExecute;
            }

            void DbfImportInt_BeforeExchangeExecute(object sender, EventArgs e)
            {
                if (ParametersObject.DBF != null)
                {
                    DbfImportFormatter formatter = new DbfImportFormatter();
                    formatter.AddFile(ParametersObject.DBF);
                    AddFormatter(formatter);
                }
            }

            private class DbfImportFormatter : InputDbfFormatter
            {
                public override void ProcessData(IObjectSpace objectSpace, OperationInterop interop)
                {
                    foreach (string col in Columns)
                    {
                        Console.Write(col);
                        Console.Write("\t");
                    }
                    Console.WriteLine();

                    foreach (Dictionary<string, object> row in Data)
                    {
                        interop.ThrowIfCancellationRequested();

                        foreach (var e in row)
                        {
                            Console.Write(Convert.ToString(e.Value));
                            Console.Write("\t");
                        }
                        Console.WriteLine();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Параметры для импорта
    /// </summary>
    [DomainComponent]
    public class ComplexImportParameters : ExchangeParameters
    {
        [XafDisplayName("Физ. лица")]
        [FilePathMode(FilePathMode.Open)]
        [FilePathFilter("Text files|*.txt|All files (*.*)|*.*")]
        public FilePath Persons { get; set; }

        [XafDisplayName("Дома")]
        [FilePathMode(FilePathMode.Open)]
        [FilePathFilter("Text files|*.txt|All files (*.*)|*.*")]
        public FilePath Houses { get; set; }

        [XafDisplayName("DBF")]
        [FilePathMode(FilePathMode.Open)]
        [FilePathFilter("Dbf files|*.dbf|All files (*.*)|*.*")]
        public FilePath DBF { get; set; }
    }
}
