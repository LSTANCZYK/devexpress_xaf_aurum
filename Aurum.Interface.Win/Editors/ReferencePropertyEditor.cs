using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using Aurum.Xpo;

namespace Aurum.Interface.Win.Editors
{
    /// <summary>
    /// Редактор свойства слабосвязанной ссылки на объект
    /// </summary>
    /// <remarks>Редактор построен на основе LookupPropertyEditor. Тип объектов ссылки определяется с помощью интерфейса 
    /// <see cref="IReferenceTypeProvider"/> и устанавливается в контроле редактора через хелпера ReferenceEditorHelper.
    /// Преобразование ссылки в значения выполняется в OnFormatValue, а значения в ссылку с помощью конвертора типа ReferenceConvertorType.
    /// Методы ValueStoring и ValueStored используются для сохранения пустого значения (bug workaround).</remarks>
    [PropertyEditor(typeof(Reference), "ReferencePropertyEditor", true)]
    public class ReferencePropertyEditor : DXPropertyEditor, IDependentPropertyEditor, IComplexViewItem, ISupportViewShowing, ILookupEditProvider, IFrameContainer
    {
        public static readonly Type EditorType = typeof(ReferencePropertyEditor);

        private ReferenceEditorHelper helper;
        private object valueStoring;

        /// <summary>Конструктор</summary>
        /// <param name="objectType">Тип объекта, которому принадлежит редактируемое свойство</param>
        /// <param name="model">Модель представления редактора элемента</param>
        public ReferencePropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

        /// <inheritdoc/>
        protected override object CreateControlCore()
        {
            // Тип объектов ссылки
            Type referenceType = GetReferenceType(CurrentObject, PropertyName);
            helper.SetReferenceType(referenceType);

            // Редактор
            LookupEdit result = new LookupEdit();
            result.QueryPopUp += new CancelEventHandler(Editor_QueryPopUp);
            result.QueryCloseUp += new CancelEventHandler(Editor_QueryCloseUp);
            return result;
        }

        /// <inheritdoc/>
        protected override void OnFormatValue(ConvertEventArgs e)
        {
            if (e.Value is Reference)
            {
                e.Value = ((Reference)e.Value).GetObject(helper.ObjectSpace, helper.ReferenceType);
            }
            base.OnFormatValue(e);
        }

        /// <inheritdoc/>
        protected override void OnValueStoring(object newValue)
        {
            base.OnValueStoring(newValue);
            this.valueStoring = newValue;
        }

        /// <inheritdoc/>
        protected override void OnValueStored()
        {
            base.OnValueStored();
            if (valueStoring == null && !Reference.IsNullRef((Reference)PropertyValue))
                PropertyValue = new Reference();
        }

        /// <summary>Возвращает тип слабосвязанной ссылки</summary>
        private Type GetReferenceType(object currentObject, string propertyName)
        {
            if (currentObject == null)
                throw new PropertyEditorException(EditorType, "CurrentObject is null");
            if (!(currentObject is IReferenceTypeProvider))
                throw new PropertyEditorException(EditorType, "CurrentObject does not support IReferenceTypeProvider (" + currentObject.ToString() + ")");
            Type referenceType = ((IReferenceTypeProvider)CurrentObject).GetReferenceType(propertyName);
            if (referenceType == null)
                throw new PropertyEditorException(EditorType, "ReferenceType is unknown (" + currentObject.ToString() + ", " + propertyName + ")");
            
            return referenceType;
        }

        #region LookupPropertyEditor code

		private void DestroyPopupForm() {
			if(Control != null) {
				Control.DestroyPopupForm();
			}
		}
		private void Editor_QueryPopUp(object sender, CancelEventArgs e) {
			if(QueryPopUp != null) {
				QueryPopUp(this, e);
			}
			OnViewShowingNotification();
		}
		private void Editor_QueryCloseUp(object sender, CancelEventArgs e) {
			if(QueryCloseUp != null) {
				QueryCloseUp(this, e);
			}
		}
		private void OnViewShowingNotification() {
			if(viewShowingNotification != null) {
				viewShowingNotification(this, EventArgs.Empty);
			}
		}
		protected override void SetupRepositoryItem(RepositoryItem item) {
			base.SetupRepositoryItem(item);
			((RepositoryItemLookupEdit)item).Init(this, DisplayFormat, helper);
		}
		protected override RepositoryItem CreateRepositoryItem() {
            return new RepositoryItemLookupEdit();
		}
		protected override void OnControlCreated() {
			base.OnControlCreated();
			OnLookupEditCreated(Control);
		}
		protected override void ReadValueCore() {
			DestroyPopupForm();
			base.ReadValueCore();
		}
		protected override void Dispose(bool disposing) {
			try {
				if(Control != null) {
					OnLookupEditHide(Control);
					Control.QueryPopUp -= new CancelEventHandler(Editor_QueryPopUp);
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		public override void Refresh() {
			base.Refresh();
			if(Control != null) {
				Control.UpdateDisplayText();
			}
		}
		public void Setup(IObjectSpace objectSpace, XafApplication application) {
			if(helper == null) {
                helper = new ReferenceEditorHelper(application, objectSpace, MemberInfo.MemberTypeInfo, Model);
			}
			if(objectSpace == null) {
				DestroyPopupForm();
			}
			helper.SetObjectSpace(objectSpace);
		}
		public new LookupEdit Control {
			get { return (LookupEdit)base.Control; }
		}
		public LookupEditorHelper Helper {
			get { return helper; }
		}
		public event CancelEventHandler QueryPopUp;
		public event CancelEventHandler QueryCloseUp;
		IList<string> IDependentPropertyEditor.MasterProperties {
			get { return helper.MasterProperties; }
		}
		public override bool CanFormatPropertyValue {
			get { return true; }
		}
		#region ISupportViewShowing Members
		private event EventHandler<EventArgs> viewShowingNotification;
		event EventHandler<EventArgs> ISupportViewShowing.ViewShowingNotification {
			add { viewShowingNotification += value; }
			remove { viewShowingNotification -= value; }
		}
		#endregion
		#region ILookupEditProvider Members
		private event EventHandler<LookupEditProviderEventArgs> lookupEditCreated;
		private event EventHandler<LookupEditProviderEventArgs> lookupEditHide;
		protected void OnLookupEditCreated(LookupEdit editor) {
			if(lookupEditCreated != null) {
				lookupEditCreated(this, new LookupEditProviderEventArgs(editor));
			}
		}
		protected void OnLookupEditHide(LookupEdit editor) {
			if(lookupEditHide != null) {
				lookupEditHide(this, new LookupEditProviderEventArgs(editor));
			}
		}
		event EventHandler<LookupEditProviderEventArgs> ILookupEditProvider.ControlCreated {
			add { lookupEditCreated += value; }
			remove { lookupEditCreated -= value; }
		}
		event EventHandler<LookupEditProviderEventArgs> ILookupEditProvider.HideControl {
			add { lookupEditHide += value; }
			remove { lookupEditHide -= value; }
		}
		#endregion
		#region IFrameContainer Members
		public Frame Frame {
			get {
				if(Control != null) {
					return Control.Frame;
				}
				return null;
			}
		}
		public void InitializeFrame() {
		}
		#endregion

        #endregion
    }

    /// <summary>
    /// Вспомогательный класс редактора свойства слабосвязанной ссылки
    /// </summary>
    public class ReferenceEditorHelper : LookupEditorHelper
    {
        private Type referenceType;
        public Type ReferenceType { get { return referenceType; } }

        /// <inheritdoc/>
        public ReferenceEditorHelper(XafApplication application, IObjectSpace objectSpace, ITypeInfo lookupObjectTypeInfo, IModelMemberViewItem viewItemModel)
            : base(application, objectSpace, lookupObjectTypeInfo, viewItemModel)
        {
        }

        /// <summary>
        /// Устанавливает тип объектов, на которые указывает ссылка
        /// </summary>
        /// <param name="referenceType">Тип объектов ссылки</param>
        public void SetReferenceType(Type referenceType)
        {
            this.referenceType = referenceType;
            this.LookupObjectTypeInfo = XafTypesInfo.Instance.FindTypeInfo(referenceType);
        }
    }
}
