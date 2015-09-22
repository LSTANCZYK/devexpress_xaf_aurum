using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;

namespace Aurum.Interface.Win.Editors
{
    [PropertyEditor(typeof(Enum), EditorAliases.EnumPropertyEditor)]
    public class FilterableEnumPropertyEditor : DXPropertyEditor, IComplexViewItem
    {
        private void UpdateControlWithCurrentObject()
        {
            var control = Control as IGridInplaceEdit;
            if (control != null)
                control.GridEditingObject = CurrentObject;
        }

        protected override object CreateControlCore()
        {
            return new XafEnumEdit();
        }

        protected override RepositoryItem CreateRepositoryItem()
        {
            return new RepositoryItemXafEnumEdit();
        }

        protected override void SetupRepositoryItem(RepositoryItem item)
        {
            base.SetupRepositoryItem(item);
            var enumEditRepositoryItem = item as RepositoryItemXafEnumEdit;
            enumEditRepositoryItem.Setup(Application, ObjectSpace, Model);
            UpdateControlWithCurrentObject();
        }

        protected override void OnCurrentObjectChanged()
        {
            base.OnCurrentObjectChanged();
            UpdateControlWithCurrentObject();
        }

        public FilterableEnumPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model)
        {
            ImmediatePostData = model.ImmediatePostData;
        }

        public new ComboBoxEdit Control
        {
            get { return (ComboBoxEdit)base.Control; }
        }

        public void Setup(IObjectSpace objectSpace, XafApplication application)
        {
            Application = application;
            ObjectSpace = objectSpace;
        }

        public XafApplication Application { get; private set; }
        public IObjectSpace ObjectSpace { get; private set; }
    }

    [System.ComponentModel.DesignerCategory("")]
    [System.ComponentModel.ToolboxItem(false)]
    public class RepositoryItemXafEnumEdit : RepositoryItemImageComboBox
    {
        private XafApplication _application;
        private IObjectSpace _objectSpace;
        private IModelMemberViewItem _model;
        private IMemberInfo _propertyMemberInfo;
        private IMemberInfo _dataSourceMemberInfo;

        private ITypeInfo GetObjectTypeInfo(IModelMemberViewItem model)
        {
            var objectView = model.ParentView as IModelObjectView;
            return objectView != null ? objectView.ModelClass.TypeInfo : null;
        }

        internal const string EditorName = "XafEnumEdit";

        internal static void Register()
        {
            if (!EditorRegistrationInfo.Default.Editors.Contains(EditorName))
            {
                EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(XafEnumEdit),
                    typeof(RepositoryItemXafEnumEdit), typeof(ImageComboBoxEditViewInfo),
                     new ImageComboBoxEditPainter(), true, EditImageIndexes.ImageComboBoxEdit, typeof(DevExpress.Accessibility.PopupEditAccessible)));
            }
        }

        protected override RepositoryItem CreateRepositoryItem()
        {
            return new RepositoryItemXafEnumEdit();
        }

        static RepositoryItemXafEnumEdit()
        {
            RepositoryItemXafEnumEdit.Register();
        }

        public RepositoryItemXafEnumEdit()
        {
            ReadOnly = true;
            TextEditStyle = TextEditStyles.Standard;
            ShowDropDown = ShowDropDown.SingleClick;
        }

        public override void Assign(RepositoryItem item)
        {
            if (item is RepositoryItemXafEnumEdit)
            {
                var source = item as RepositoryItemXafEnumEdit;
                if (source != null)
                {
                    _application = source._application;
                    _objectSpace = source._objectSpace;
                    _model = source._model;
                    _propertyMemberInfo = source._propertyMemberInfo;
                    _dataSourceMemberInfo = source._dataSourceMemberInfo;
                }
            }
            base.Assign(item);
        }

        public override BaseEdit CreateEditor()
        {
            return base.CreateEditor() as XafEnumEdit;
        }

        public void Init(Type type)
        {
            EnumImagesLoader loader = new EnumImagesLoader(type);
            Items.AddRange(loader.GetImageComboBoxItems());
            if (loader.Images.Images.Count > 0)
            {
                SmallImages = loader.Images;
            }

        }
        public void Setup(XafApplication application, IObjectSpace objectSpace, IModelMemberViewItem model)
        {
            this._application = application;
            this._objectSpace = objectSpace;
            this._model = model;
            _propertyMemberInfo = null;
            _dataSourceMemberInfo = null;
            var typeInfo = GetObjectTypeInfo(model);
            if (typeInfo == null) return;
            _propertyMemberInfo = typeInfo.FindMember(model.PropertyName);
            if (!String.IsNullOrEmpty(model.DataSourceProperty))
            {
                StringBuilder builder = new StringBuilder(model.DataSourceProperty);
                var path = _propertyMemberInfo.GetPath();
                for (int index = path.Count - 2; index >= 0; index--)
                    builder.Insert(0, ".").Insert(0, path[index].Name);
                _dataSourceMemberInfo = typeInfo.FindMember(builder.ToString());
            }
            Init(_propertyMemberInfo.MemberType);
        }

        public override string EditorTypeName { get { return EditorName; } }

        public XafApplication Application
        {
            get { return _application; }
        }

        public IObjectSpace ObjectSpace
        {
            get { return _objectSpace; }
        }

        public IModelMemberViewItem Model { get { return _model; } }
        public IMemberInfo PropertyMemberInfo { get { return _propertyMemberInfo; } }
        public IMemberInfo DataSourceMemberInfo { get { return _dataSourceMemberInfo; } }
    }

    [System.ComponentModel.DesignerCategory("")]
    [System.ComponentModel.ToolboxItem(false)]
    public partial class XafEnumEdit : ImageComboBoxEdit, IGridInplaceEdit
    {
        private static PropertyDescriptorCollection _imageComboBoxItemProperties;
        private object _gridEditingObject;
        private IObjectSpace _objectSpace;

        internal IList GetDataSource(object forObject)
        {
            CriteriaOperator criteria = null;
            if (Properties == null) return null;
            IList propertyDataSource =
                (Properties.DataSourceMemberInfo != null) &&
                (/*Properties.DataSourceMemberInfo.IsStatic || */GridEditingObject != null) ?
                    Properties.DataSourceMemberInfo.GetValue(forObject) as IList :
                    null;
            IList dataSource = new List<ImageComboBoxItem>();
            if (propertyDataSource == null)
                for (int i = 0; i < Properties.Items.Count; i++)
                    dataSource.Add((ImageComboBoxItem)Properties.Items[i]);
            else
                for (int i = 0; i < Properties.Items.Count; i++)
                {
                    var item = (ImageComboBoxItem)Properties.Items[i];
                    if (propertyDataSource.Contains(item.Value))
                        dataSource.Add(item);
                }
            string criteriaString = Properties.Model.DataSourceCriteria;
            if (!String.IsNullOrEmpty(criteriaString))
                criteria = CriteriaOperator.Parse(criteriaString);
            
            //if (!ReferenceEquals(criteria, null))
            //{
            //    criteria.Accept(new EnumCriteriaParser(
            //        Properties.PropertyMemberInfo.Name,
            //        Properties.PropertyMemberInfo.MemberType));
            //    var filteredDataSource = new ExpressionEvaluator(ImageComboBoxItemProperties, criteria, true).Filter(dataSource);
            //    dataSource.Clear();
            //    foreach (ImageComboBoxItem item in filteredDataSource)
            //        dataSource.Add(item);
            //}
            
            return dataSource;
        }

        private void ObjectSpaceObjectChanged(object sender, ObjectChangedEventArgs e)
        {
            if (e.Object == GridEditingObject && (
                String.IsNullOrEmpty(e.PropertyName) || (
                Properties.DataSourceMemberInfo != null &&
                Properties.DataSourceMemberInfo.Name.Equals(e.PropertyName))))
            {
                /* Datasource property changed. Nothing to do right now as the windows editor
                 * obtains the datasource on each dropdown. */
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ClosePopup();
            }
            base.OnKeyDown(e);
        }

        protected override void OnPropertiesChanged()
        {
            base.OnPropertiesChanged();
            if (Properties != null)
                ObjectSpace = Properties.ObjectSpace;
        }

        protected override void OnPopupClosed(PopupCloseMode closeMode)
        {
            base.OnPopupClosed(closeMode);
            DestroyPopupForm();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ObjectSpace = null;
            }
            base.Dispose(disposing);
        }
        protected override PopupBaseForm CreatePopupForm()
        {
            var popupForm = new XafEnumEditPopupForm(this);
            // popupForm.TopMost = true;
            return popupForm;
        }

        protected static PropertyDescriptorCollection ImageComboBoxItemProperties
        {
            get
            {
                if (_imageComboBoxItemProperties == null)
                    _imageComboBoxItemProperties = TypeDescriptor.GetProperties(typeof(ImageComboBoxItem));
                return _imageComboBoxItemProperties;
            }
        }

        static XafEnumEdit()
        {
            RepositoryItemXafEnumEdit.Register();
        }

        public XafEnumEdit()
        {
            Properties.TextEditStyle = TextEditStyles.Standard;
            Properties.ReadOnly = true;
            Height = WinPropertyEditor.TextControlHeight;
        }

        public override string EditorTypeName { get { return RepositoryItemXafEnumEdit.EditorName; } }

        public object EditingObject
        {
            get { return BindingHelper.GetEditingObject(this); }
        }

        public new RepositoryItemXafEnumEdit Properties
        {
            get
            {
                return (RepositoryItemXafEnumEdit)base.Properties;
            }
        }

        public object GridEditingObject
        {
            get { return _gridEditingObject; }
            set
            {
                if (_gridEditingObject == value) return;
                _gridEditingObject = value;
            }
        }

        public IObjectSpace ObjectSpace
        {
            get { return _objectSpace; }
            set
            {
                if (_objectSpace != null) _objectSpace.ObjectChanged -= ObjectSpaceObjectChanged;
                _objectSpace = value;
                if (_objectSpace != null) _objectSpace.ObjectChanged += ObjectSpaceObjectChanged;
            }
        }

        public new XafEnumEditPopupForm PopupForm
        {
            get { return (XafEnumEditPopupForm)base.PopupForm; }
        }
    }

    public partial class XafEnumEdit { }

    public class XafEnumEditPopupForm : PopupListBoxForm
    {
        protected override void OnBeforeShowPopup()
        {
            UpdateDataSource();
            base.OnBeforeShowPopup();
        }

        protected override void SetupListBoxOnShow()
        {
            /* Base. */
            base.SetupListBoxOnShow();
            /* Gather meta to determine whether and which item to select. */
            var visibleItems = ListBox.DataSource as IList;
            var currentItem = (ImageComboBoxItem)OwnerEdit.SelectedItem;
            var currentItemInVisibleItems = visibleItems == null || visibleItems.Contains(currentItem);
            var selectedItem = (ImageComboBoxItem)ListBox.SelectedItem;
            /* When the listbox selected item differs from the edit value of the editor, or when
             * the current item isn't present in the listbox, reset the selected item. */
            if (selectedItem != currentItem || !currentItemInVisibleItems)
                selectedItem = null;
            /* Only when there's not a selected item while the current item exists in the list,
             * make the current item the selected item. */
            if (selectedItem == null && currentItemInVisibleItems)
                selectedItem = currentItem;
            /* Set the selected item. */
            ListBox.SelectedIndex = -1;  // Bug work-around, setting null doesn't always reset SelectedItem to null.            
            ListBox.SelectedItem = selectedItem;
        }

        public XafEnumEditPopupForm(XafEnumEdit ownerEdit) : base(ownerEdit)
        {
        }

        public void UpdateDataSource()
        {
            if (Properties == null) return;
            var dataSource = OwnerEdit.GetDataSource(OwnerEdit.EditingObject) as IList;
            ListBox.DataSource = dataSource != null ? (object)dataSource : (object)Properties.Items;
        }

        public new XafEnumEdit OwnerEdit { get { return (XafEnumEdit)base.OwnerEdit; } }
        
        public new RepositoryItemXafEnumEdit Properties
        {
            get
            {
                return (RepositoryItemXafEnumEdit)base.Properties;
            }
        }
    }
}
