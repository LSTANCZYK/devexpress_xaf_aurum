using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates.ActionContainers;
using System;

namespace Aurum.Menu.Templates.ActionContainers
{
    public interface IMenusNavigationControl : INavigationControl
    {
        SingleChoiceAction MenusAction { get; }

        event EventHandler<MenuItemControlCreatedEventArgs> MenuItemCreated;
    }
}
