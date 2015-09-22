// See
// Also:
// http://www.devexpress.com/scid=S30847
// CheckedListBoxControl Class
// (http://documentation.devexpress.com/#WindowsForms/clsDevExpressXtraEditorsCheckedListBoxControltopic)
// Implement
// Custom Property Editors
// (http://documentation.devexpress.com/#Xaf/CustomDocument3097)
// http://www.devexpress.com/scid=E1806
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E1807

using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.XtraEditors;
using DevExpress.Xpo;
using System.Windows.Forms;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Data.Filtering;
using System.Reflection;
using DevExpress.ExpressApp.DC;
using Aurum.Interface.Model;

namespace Aurum.Interface.Win.Editors
{
    /// <summary>
    /// Редактор для коллекций с чек-боксами, если чек стоит, то значит объект принадлежит к коллекции. 
    /// Перед использованием фильтровать источник как показано в Aurum.App.Module.BusinessObjects.Test
    /// </summary>
    [PropertyEditor(typeof(XPBaseCollection), false)]
    public class WinCheckedListBoxPropertyEditor : WinPropertyEditor, IComplexViewItem
    {
        XPBaseCollection checkedItems;
        XafApplication application;
        IObjectSpace objectSpace;

        public WinCheckedListBoxPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
        
        protected override object CreateControlCore()
        {
            return new CustomCheckedListBoxEdit() { };
            //return new AurumCheckedListBoxControl() { CheckOnClick = true };
        }
                
        protected override void ReadValueCore()
        {
            base.ReadValueCore();
            if (PropertyValue is XPBaseCollection)
            {
                // отписка от старого обработчика
                Control.ItemCheck -= new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(control_ItemCheck);
                checkedItems = (XPBaseCollection)PropertyValue;

                // дополнительные условия
                CriteriaOperator criteria = null;
                if (!string.IsNullOrEmpty(Model.DataSourceCriteria))
                {
                    criteria = CriteriaOperator.Parse(Model.DataSourceCriteria);
                }
                if (!string.IsNullOrEmpty(Model.DataSourceCriteriaProperty))
                {
                    CriteriaOperator criteria2 = null;

                    IMemberInfo propWithCriteria = MemberInfo.Owner.FindMember(Model.DataSourceCriteriaProperty);
                    criteria2 = (CriteriaOperator)propWithCriteria.GetValue(this.CurrentObject);

                    if (!ReferenceEquals(criteria2, null))
                    {
                        criteria = !ReferenceEquals(criteria, null) ? CriteriaOperator.And(criteria, criteria2) : criteria2;
                    }
                }

                // коллекция данных для списка
                XPCollection dataSource = new XPCollection(
                    checkedItems.Session,
                    MemberInfo.ListElementType,
                    !ReferenceEquals(criteria, null) ? CriteriaOperator.And(checkedItems.Criteria, criteria) : checkedItems.Criteria,
                    checkedItems.Sorting.ToArray<SortProperty>());
                IModelClass classInfo = application.Model.BOModel.GetClass(MemberInfo.ListElementTypeInfo.Type);
                if (checkedItems.Sorting.Count > 0)
                {
                    dataSource.Sorting = checkedItems.Sorting;
                }
                else if (checkedItems.Sorting.Count == 0 && !String.IsNullOrEmpty(classInfo.DefaultProperty))
                {
                    dataSource.Sorting.Add(new SortProperty(classInfo.DefaultProperty, DevExpress.Xpo.DB.SortingDirection.Ascending));
                }
                Control.DataSource = dataSource;
                Control.DisplayMember = classInfo.DefaultProperty;

                //Выполняем условие для отображения текста в контроле.
                IModelPropertyEditorDisplayItem displayItem = Model as IModelPropertyEditorDisplayItem;
                if (displayItem != null)
                {
                    if (!ReferenceEquals(displayItem.DisplayItemCriteriaProperty, null))
                    {
                        IMemberInfo propWithCriteria = MemberInfo.Owner.FindMember(displayItem.DisplayItemCriteriaProperty);
                        Control.ItemTextCriteria = (CriteriaOperator)propWithCriteria.GetValue(this.CurrentObject);
                    }
                    if (!String.IsNullOrWhiteSpace(displayItem.DisplayItemCriteriaString))
                    {
                        Control.ItemTextCriteriaString = (displayItem.DisplayItemCriteriaString);
                    }
                }

                foreach (object obj in checkedItems)
                {
                    Control.SetItemChecked(dataSource.IndexOf(obj), true);
                }
                Control.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(control_ItemCheck);
            }
        }

        void control_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
        {
            object obj = Control.GetItemValue(e.Index);
            switch (e.State)
            {
                case CheckState.Checked:
                    checkedItems.BaseAdd(obj);
                    break;
                case CheckState.Unchecked:
                    checkedItems.BaseRemove(obj);
                    break;
            }
            OnControlValueChanged();
            objectSpace.SetModified(CurrentObject);
        }

        public new CustomCheckedListBoxEdit Control
        {
            get { return (CustomCheckedListBoxEdit)base.Control; }
        }

        #region IComplexPropertyEditor Members
        public void Setup(IObjectSpace objectSpace, XafApplication application)
        {
            this.application = application;
            this.objectSpace = objectSpace;
        }
        #endregion
    }
}
