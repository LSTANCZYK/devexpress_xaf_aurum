using Aurum.Menu.Model;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using System;
using System.Linq;

namespace Aurum.Menu
{
    public class ViewMenuItem : MenuItem
    {
        private readonly ViewShortcut shortcut;
        public new IModelMenuViewItem Model
        {
            get
            {
                return (IModelMenuViewItem)base.Model;
            }
        }

        public bool HasRights
        {
            get
            {
                if (this.Model.View is IModelDashboardView)
                {
                    return true;
                }
                IModelObjectView modelObjectView = this.Model.View as IModelObjectView;
                if (modelObjectView == null)
                {
                    throw new ArgumentException(string.Format("Cannot find the '{0}' view specified by the shortcut: {1}", this.shortcut.ViewId, this.shortcut));
                }
                Type type = modelObjectView.ModelClass.TypeInfo.Type;
                if (type != null)
                {
                    using (IObjectSpace objectSpace = base.MenuItemsAction.Controller.Application.CreateObjectSpace())
                    {
                        return DataManipulationRight.CanRead(type, null, null, null, objectSpace) && DataManipulationRight.CanNavigate(type, null, objectSpace);
                    }
                }
                return true;
            }
        }

        public ViewMenuItem(IModelMenuViewItem info, SingleChoiceAction menuItemsAction, string prefixId = "")
            : base(info, menuItemsAction, prefixId)
        {
            base.Caption = base.RemoveAmpersand(info.Caption);
            base.ImageName = info.ImageName;
            base.ToolTip = info.ToolTip;
            string objectKey = info.ObjectKey;
            IModelView view = info.View;
            if (view != null)
            {
                this.shortcut = new ViewShortcut(view.Id, objectKey);
                base.Enabled["HasRights"] = this.HasRights;
            }
        }

        public override void Execute(SingleChoiceActionExecuteEventArgs args)
        {
            base.Execute(args);
            if (this.shortcut == null)
            {
                return;
            }
            View view = args.Action.Application.ProcessShortcut(this.shortcut);
            ListView listView = view as ListView;
            if (listView != null)
            {
                string backFilterCriteria = ((ViewMenuItem)args.SelectedChoiceActionItem).Model.BackFilterCriteria;
                if (!string.IsNullOrEmpty(backFilterCriteria) && listView.CollectionSource.CanApplyCriteria)
                {
                    CriteriaOperator criteriaOperator = CriteriaOperator.Parse(backFilterCriteria, new object[0]);
                    LocalizedCriteriaWrapper localizedCriteriaWrapper = new LocalizedCriteriaWrapper(listView.ObjectTypeInfo.Type, criteriaOperator);
                    if (localizedCriteriaWrapper.EditableParameters.Count > 0)
                    {
                        string message = localizedCriteriaWrapper.EditableParameters.Keys.Aggregate("Cannot process editable parameters:\n", (string current, string parameterName) => current + string.Format("'@{0}'\n", parameterName));
                        throw new InvalidOperationException(message);
                    }
                    localizedCriteriaWrapper.UpdateParametersValues();
                    listView.CollectionSource.Criteria["BackFilterCriteria"] = criteriaOperator;
                }
            }
            args.ShowViewParameters.CreatedView = view;
            args.ShowViewParameters.TargetWindow = TargetWindow.Current;
        }
    }
}
