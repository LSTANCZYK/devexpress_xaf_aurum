using Aurum.Interface.Win.Controllers;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Interface.Win.Filters
{
    public class FilterWindowView : View
    {
        private IObjectSpace objectSpace;
        private FilterWindowControl control;

        public FilterWindowView(IObjectSpace objectSpace, IModelView model, bool isRoot, List<FilterColumn> list)
            : base(isRoot)
        {
            this.objectSpace = objectSpace;
            base.model = model;
            control = new FilterWindowControl(list);
        }

        protected override object CreateControlsCore()
        {
            return control;
        }

        public override object Control
        {
            get { return control; }
        }

        protected override void LoadModelCore()
        {
        }

        protected override void RefreshCore()
        {
        }

        protected override void SaveModelCore()
        {
        }

        public override IObjectSpace ObjectSpace
        {
            get { return objectSpace; }
        }

        public override IModelView Model
        {
            get { return model; }
        }

        public List<Tuple<string, FilterValue>> GetValues()
        {
            return control.GetValues();
        }

        public List<CriteriaOperator> GetCriteries()
        {
            return control.GetCriteries();
        }

        public void UpdateValues()
        {
            control.UpdateValues();
        }
    }
}
