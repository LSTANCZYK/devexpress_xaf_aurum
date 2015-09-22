using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using Aurum.Interface.Model;

namespace Aurum.Interface.Controllers
{
    /// <summary>
    /// Переопределяет стандартный контроллер списочного представления (ListView):
    /// Добавлена возможность отмены вызова DetailView из ListView путем установки флага в модели (ShowDetailView)
    /// </summary>
    public class AurumListViewController : ViewController<ListView>
    {
        private const string EnabledKeyShowDetailView = "AurumListViewController";
        protected override void OnActivated()
        {
            base.OnActivated();
            ListViewProcessCurrentObjectController controller = Frame.GetController<ListViewProcessCurrentObjectController>();
            if (controller != null)
            {
                IModelShowDetailView modelShowDetailView = View.Model as IModelShowDetailView;
                controller.ProcessCurrentObjectAction.Enabled[EnabledKeyShowDetailView] = modelShowDetailView == null ? true : modelShowDetailView.ShowDetailView;
            }
        }
    }
}
