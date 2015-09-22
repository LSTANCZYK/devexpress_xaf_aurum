// Developer Express Code Central Example:
// How to represent a collection property using a checked list box
// 
// Scenario:
// 
// 1. It is required to show a check list. It should be possible to
// add and remove items for this list dynamically.
// 2. There are a couple of child
// records, and it is required to show all available records in a compact manner,
// and link and unlink them from the master object quickly with check boxes. When
// an item is checked, this means that this record is associated with the master
// object.
// 
// Steps to implement:
// 
// This functionality is implemented via a custom
// property editor that can be used to edit XPCollection properties. There are two
// separate editors for WinForms and ASP.NET: WinCheckedListBoxPropertyEditor and
// WebCheckedListBoxPropertyEditor.
// Common implementation details:
// 1. Create a
// custom property editor and specify that it should be used for collection
// properties via the PropertyEditorAttribute.
// 2. Use a control that can populate
// the check boxes list based on the passed data source - CheckedListBoxControl in
// WinForms and ASPxCheckBoxList in ASP.NET.
// 3. Since an Object Space instance is
// required to populate the control's data source, implement the IComplexViewItem
// interface to pass this instance to the property editor.
// 4. Since control's
// settings depend on the property value, it is required to configure the control
// when the value is written to the property editor. An appropriate method is
// ReadValueCore.
// 5. Assign the control's DataSource based on the collection's
// items type, and check the generated list box items that present in the
// collection displayed via the property editor (it can be accessed via the
// PropertyValue property).
// 6. Modify the associated collection when the list box
// item's checked state is changed. This can be done by handling the
// CheckedListBoxControl.ItemCheck event in WinForms and
// ASPxCheckBoxList.SelectedIndexChanged in ASP.NET.
// 7. Open the Model Editor and
// assign the created property editor to the PropertyEditorType property of the
// required view item.
// ASP.NET implementation's specifics:
// 1. Since the ASP.NET
// property editor (WebCheckedListBoxPropertyEditor) works with the ASPxEditBase
// descendant (ASPxCheckBoxList), inherit it from the ASPxPropertyEditor class to
// use the existing code to configure the property editor's settings.
// 2.
// ASPxPropertyEditor provides separate methods to create controls -
// CreateEditModeControlCore to create a control for the Edit mode, and
// CreateViewModeControlCore for the View mode. Since in both cases it is required
// to show a check box list, return the ASPxCheckBoxList control in both methods,
// but disable it in the CreateViewModeControlCore method.
// 3. Call the
// ASPxCheckBoxList.DataBind method after assigning the control's data source to
// generate items.
// 4. Store object keys instead of entire objects in the editor to
// avoid issues with transferring data between requests.
// 5. Override the
// SetImmediatePostDataScript method to support the ImmediatePostData
// functionality. It is required to specify what client-side event should be used
// to raise an XAF callback that passes the new value to the server application.
// Use the SelectedIndexChanged event.
// 6. Return False in the overridden
// IsMemberSetterRequired method to specify that the editor should not be read-only
// if the bound property is read-only (because collection properties are
// read-only).
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
using System.ComponentModel;

using DevExpress.Xpo;
using DevExpress.Data.Filtering;

using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;

namespace Aurum.App.Module.BusinessObjects
{
    [DefaultClassOptions]
    public class Detail : BaseObject
    {
        public Detail(Session session) : base(session) { }
        private string _DetailName;
        public string DetailName
        {
            get { return _DetailName; }
            set { SetPropertyValue("DetailName", ref _DetailName, value); }
        }
        //private Master _Master;
        //[Association("Master-Details")]
        //public Master Master
        //{
        //    get { return _Master; }
        //    set { SetPropertyValue("Master", ref _Master, value); }
        //}
    }
}