using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Interface.Win.Editors
{
    /// <summary>
    /// Расширенный lookup-редактор, добавлена кнопка открытия detail-view
    /// </summary>
    public class ExtLookupPropertyEditor : LookupPropertyEditor
    {
        protected EditorButton DetailViewButton
		{
			get;
			private set;
		}

		public new LookupEdit Control
		{
			get;
			private set;
		}

		public ExtLookupPropertyEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info)
		{
		}

        protected override object CreateControlCore()
        {
            this.Control = (LookupEdit)base.CreateControlCore();
            this.DetailViewButton = new EditorButton(ButtonPredefines.Right);
            this.DetailViewButton.ToolTip = "Открыть детальное представление объекта";
            this.Control.Properties.Buttons.Add(this.DetailViewButton);
            this.Control.Properties.ButtonClick += new ButtonPressedEventHandler(this.OnDetailViewButtonClick);
            return this.Control;
        }

        private void OnDetailViewButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == DetailViewButton)
            {
                IObjectSpace objectSpace = base.Helper.Application.CreateObjectSpace();
                if (this.ControlValue != null)
                {
                    object @object = objectSpace.GetObject(this.ControlValue);
                    View view = null;
                    if (base.Model != null)
                    {
                        view = base.Helper.Application.CreateDetailView(objectSpace, @object);
                    }
                    ShowViewParameters showViewParameters = new ShowViewParameters(view);
                    showViewParameters.TargetWindow = TargetWindow.Default;
                    base.Helper.Application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(null, null));
                }
            }
        }

        protected override void SetRepositoryItemReadOnly(RepositoryItem item, bool readOnly)
        {
            base.SetRepositoryItemReadOnly(item, readOnly);
            if (item != null && item.ReadOnly)
            {
                if (this.DetailViewButton != null && !this.DetailViewButton.Visible)
                {
                    this.DetailViewButton.Visible = true;
                }
            }
        }
    }
}
