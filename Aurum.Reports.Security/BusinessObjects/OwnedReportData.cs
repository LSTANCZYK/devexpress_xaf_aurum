using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aurum.Security;
using Aurum.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.XtraReports.UI;

namespace Aurum.Reports.Security
{
    /// <summary>
    /// Отчет, хранимый в базе данных и принадлежащий собственнику данных
    /// </summary>
    // Код скопирован из DevExpress.Persistent.BaseImpl.ReportDataV2, наследование невозможно, 
    // поскольку конечному пользователю нужно давать права на редактирование ReportDataV2 ("дыра" в безопасности)
    [NavigationItem("Reports"), ImageName("BO_Report"), VisibleInReports(false)]
    public class OwnedReportData : XPOwnedObject, IReportDataV2Writable, IInplaceReportV2
    {
        private string displayName = string.Empty;
        private bool isInplaceReport = false;
        private string parametersObjectTypeName = string.Empty;
        private Type predefinedReportType;

        /// <summary>Конструктор без параметров</summary>
        public OwnedReportData() : base() { }

        /// <summary>Конструктор с указанной сессией</summary>
        public OwnedReportData(Session session) : base(session) { }

        /// <summary>Конструктор с указанной сессией и типом данных отчета</summary>
        /// <param name="session">Сессия для загрузки и сохранения объектов в базе данных</param>
        /// <param name="dataType">Тип данных, на которых основан отчет</param>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public OwnedReportData(Session session, Type dataType) : base(session) 
        {
			Guard.ArgumentNotNull(dataType, "dataType");
			this.dataTypeName = dataType.FullName;
		}

        /// <summary>
        /// Содержание отчета
        /// </summary>
        [Delayed(true), Persistent("Content"), NotNull, Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public byte[] Content {
			get {
				return GetDelayedPropertyValue<byte[]>("Content");
			}
			set {
				if(((IReportDataV2)this).IsPredefined) {
					throw new NotImplementedException();
				}
				SetDelayedPropertyValue<byte[]>("Content", value);
			}
		}

        /// <summary>
        /// Название отчета в списке отчетов
        /// </summary>
		[Persistent("Name")]
		public string DisplayName {
			get { return displayName; }
			set { SetPropertyValue("DisplayName", ref displayName, value); }
		}

#if MediumTrust
        private string dataTypeName = string.Empty;

        /// <summary>Тип данных, на основе которых построен отчет</summary>
        [Browsable(false)]
		[Persistent("ObjectTypeName"), NotNull, Size(256)]
		public string DataTypeName {
			get { return dataTypeName; }
			set { SetPropertyValue("ObjectTypeName", ref dataTypeName, value); }
		}
#else
        [Persistent("ObjectTypeName"), NotNull, Size(256)]
        private string dataTypeName = string.Empty;

        /// <summary>Тип данных, на основе которых построен отчет</summary>
		[Browsable(false)]
		[PersistentAlias("dataTypeName")]
		public string DataTypeName 
        {
			get { return dataTypeName; }
		}
#endif

        /// <summary>
        /// Тип параметров
        /// </summary>
		[SettingsBindable(true)]
		[VisibleInListView(false)]
		[TypeConverter(typeof(ReportParametersObjectTypeConverter))]
		[Localizable(true)]
		public Type ParametersObjectType {
			get 
            {
				if(!string.IsNullOrEmpty(ParametersObjectTypeName)) {
					ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(ParametersObjectTypeName);
					if(typeInfo != null) {
						return typeInfo.Type;
					}
				}
				return null;
			}
			set 
            {
				((IReportDataV2Writable)this).SetParametersObjectType(value);
			}
		}

        /// <summary>
        /// Название типа параметров
        /// </summary>
		[Size(512)]
		[Browsable(false)]
		public string ParametersObjectTypeName 
        {
			get { return parametersObjectTypeName; }
			set { SetPropertyValue(ReportsModuleV2.ParametersObjectTypeNameMemberName, ref parametersObjectTypeName, value); }
		}

        /// <summary>
        /// Описание типа данных, на основе которых построен отчет
        /// </summary>
		[NonPersistent, System.ComponentModel.DisplayName("Data Type")]
		public string DataTypeCaption 
        {
			get { return CaptionHelper.GetClassCaption(dataTypeName); }
		}

        /// <summary>
        /// Признак отчета, встраиваемого в интерфейс редактора сущности
        /// </summary>
		[VisibleInListView(false)]
		public bool IsInplaceReport 
        {
			get { return isInplaceReport; }
			set { SetPropertyValue(ReportsModuleV2.IsInplaceReportMemberName, ref isInplaceReport, value); }
		}

        /// <summary>
        /// Тип предопределенного в коде отчета
        /// </summary>
		[Browsable(false)]
		[ValueConverter(typeof(TypeToStringConverter))]
		[Size(512)]
		public Type PredefinedReportType 
        {
			get { return predefinedReportType; }
			set { SetPropertyValue(ReportsModuleV2.PredefinedReportTypeMemberName, ref predefinedReportType, value); }
		}

        /// <summary>
        /// Признак предопределенного в коде отчета
        /// </summary>
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [NonPersistent]
        public bool IsPredefined
        {
            get { return PredefinedReportType != null; }
        }

        /// <inheritdoc/>
        protected override void OnSaving()
        {
            if (String.IsNullOrEmpty(displayName) || (displayName.Trim() == ""))
            {
                throw new Exception(ReportsModuleV2.GetEmptyDisplayNameErrorMessage());
            }
            base.OnSaving();
        }

        #region IReportDataV2

        /// <contentfrom cref="IReportDataV2.DataType"/>
        Type IReportDataV2.DataType
        {
            get
            {
                if (!string.IsNullOrEmpty(DataTypeName))
                {
                    ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(DataTypeName);
                    if (typeInfo != null)
                    {
                        return typeInfo.Type;
                    }
                }
                return null;
            }
        }

        #endregion

        #region IReportDataV2Writable

        /// <contentfrom cref="IReportDataV2Writable.SetContent"/>
        void IReportDataV2Writable.SetContent(byte[] content)
        {
			Content = content;
		}

        /// <contentfrom cref="IReportDataV2Writable.SetPredefinedReportType"/>
        void IReportDataV2Writable.SetPredefinedReportType(Type reportType) 
        {
			if(reportType != null) {
				Guard.TypeArgumentIs(typeof(XtraReport), reportType, "reportType");
			}
			PredefinedReportType = reportType;
		}

        /// <contentfrom cref="IReportDataV2Writable.SetParametersObjectType"/>
        void IReportDataV2Writable.SetParametersObjectType(Type parametersObjectType) 
        {
			if(parametersObjectType != null) {
				Guard.TypeArgumentIs(typeof(ReportParametersObjectBase), parametersObjectType, "parametersObjectType");
			}
			ParametersObjectTypeName = parametersObjectType != null ? parametersObjectType.FullName : string.Empty;
		}

        /// <contentfrom cref="IReportDataV2Writable.SetDataType"/>
        void IReportDataV2Writable.SetDataType(Type newDataType) 
        {
			dataTypeName = newDataType != null ? newDataType.FullName : string.Empty;
		}
        
        /// <contentfrom cref="IReportDataV2Writable.SetDisplayName"/>
        void IReportDataV2Writable.SetDisplayName(string displayName) 
        {
			DisplayName = displayName;
        }

        #endregion
    }
}
