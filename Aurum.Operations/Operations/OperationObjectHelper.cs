using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Operations
{
    /// <summary>
    /// Вспомогательные методы объекта операции
    /// </summary>
    public static class OperationObjectHelper
    {
        /// <summary>
        /// Показать (установить данные для показа) детальное представление объекта операции
        /// </summary>
        /// <param name="obj">Объект операции</param>
        /// <param name="app">Объект XafApplication</param>
        /// <param name="sp">Объект ShowViewParameters</param>
        /// <exception cref="System.ArgumentNullException" />
        public static void Show(this OperationObject obj, XafApplication app, ShowViewParameters sp)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            if (sp == null)
            {
                throw new ArgumentNullException("sp");
            }

            var isComposite = obj.Children != null && obj.Children.Count > 0;
            if (isComposite)
            {
                sp.CreatedView = app.CreateDetailView(ObjectSpaceInMemory.CreateNew(), "OperationObject_DetailView_(Composite)", true, obj);
            }
            else
            {
                sp.CreatedView = app.CreateDetailView(ObjectSpaceInMemory.CreateNew(), obj);
            }

            sp.TargetWindow = TargetWindow.NewModalWindow;
            sp.Context = TemplateContext.PopupWindow;
        }
    }
}
