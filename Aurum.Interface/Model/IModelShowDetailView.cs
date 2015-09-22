using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using System.ComponentModel;

namespace Aurum.Interface.Model
{
    public interface IModelDefaultShowDetailView : IModelNode
    {
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("Открывать DetailView из ListView по умолчанию")]
        bool DefaultShowDetailViewFromListView { get; set; }
    }

    public interface IModelShowDetailView : IModelNode
    {
        [Category("Behavior")]
        [Description("Открывать DetailView из ListView")]
        bool ShowDetailView { get; set; }
    }

    [DomainLogic(typeof(IModelShowDetailView))]
    public static class ModelShowDetailViewLogic
    {
        public static bool Get_ShowDetailView(IModelShowDetailView showDetailView)
        {
            IModelDefaultShowDetailView defaultShowDetailViewFromListView = showDetailView.Parent as IModelDefaultShowDetailView;
            if (defaultShowDetailViewFromListView != null)
            {
                return defaultShowDetailViewFromListView.DefaultShowDetailViewFromListView;
            }
            return true;
        }
    }
}
