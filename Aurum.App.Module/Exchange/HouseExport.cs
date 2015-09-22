using Aurum.App.Module.BusinessObjects;
using Aurum.Exchange;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.App.Module.Exports
{
    /// <summary>
    /// Экспорт домов №1
    /// Используется экспорт с обычным текстовым форматировщиком
    /// </summary>
    public class HouseExport : ExportOperation<House, HouseExportParameters>
    {
        public HouseExport(XafApplication s)
            : base(s)
        {
            // Установка текстового форматировщика
            var fr = new OutputTextFormatter();
            fr.AddFile(ParametersObject.FileToSave);

            // Установка кастомных шапки и подвала
            fr.CustomHeader += fr_CustomHeader;
            fr.CustomFooter += fr_CustomFooter;

            Formatter = fr;

            // Установка объекта параметров
            ParametersObject = new HouseExportParameters();

            // Добавление экспортируемых полей
            AddField(x => x.Num);
            AddField(x => x.Street);
            AddField("[Flats].Count()");

            BeforeExchangeExecute += HouseExport_BeforeExchangeExecute;
        }

        void HouseExport_BeforeExchangeExecute(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Установка кастомной шапки
        /// </summary>
        void fr_CustomHeader(object sender, CustomHeaderEventArgs e)
        {
            e.HeaderText = "### House 1 export ###\r\nNum\tStreet\tFlats count";
        }

        /// <summary>
        /// Установка кастомного подвала
        /// </summary>
        void fr_CustomFooter(object sender, CustomFooterEventArgs e)
        {
            e.FooterText = "### End of house 1 export ###";
        }

        protected override CriteriaOperator GetCriteria()
        {
            HouseExportParameters hep = ParametersObject as HouseExportParameters;
            if (hep != null)
                return new InOperator("this", hep.Houses);
            return null;
        }
    }

    /// <summary>
    /// Параметры для экспорта домов №1 и №2
    /// </summary>
    [DomainComponent]
    public class HouseExportParameters : ExchangeParameters
    {
        private string num;

        /// <summary>
        /// Номер дома
        /// </summary>
        public string Num
        {
            get { return num; }
            set
            {
                num = value;
                var houses = ObjectSpace.GetObjects<House>(new BinaryOperator("Num", value));
                
                foreach (var house in Houses.ToArray())
                {
                    Houses.Remove(house);
                }

                foreach (var house in houses)
                {
                    Houses.Add(house);
                }
            }
        }

        public FilePath File { get; set; }

        [
            FilePathMode(FilePathMode.Save),
            FilePathFilter("Text files|*.txt|All files (*.*)|*.*")
        ]
        public FilePath FileToSave { get; set; }

        public DirectoryPath Dir { get; set; }

        [
            FilePathMode(FilePathMode.Save),
            FilePathFilter("Dat files|*.dat;*.data|All files (*.*)|*.*")
        ]
        public MultipleFilePath FilesToOpen { get; set; }

        private XPCollection<House> houses;
        public XPCollection<House> Houses
        {
            get
            {
                if (houses == null)
                {
                    houses = ObjectSpace.CreateObject<XPCollection<House>>();
                    houses.LoadingEnabled = false;
                }
                return houses;
            }
            set
            {
                houses = value;
            }
        }
    }
}
