using System;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Data;
using System.Collections;

namespace Aurum.Reports.Controllers
{
    /// <summary>
    /// Контроллер элементов мастера отчетов
    /// </summary>
    public partial class ReportWizardItemController : ViewController
    {
        /// <summary>Конструктор</summary>
        public ReportWizardItemController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        /// <inheritdoc/>
        protected override void OnActivated()
        {
            base.OnActivated();

            // Кастомизация представления списка
            if (View is ListView)
            {
                // Типы элементов мастера отчетов
                ITypeInfo objectTypeInfo = ((ObjectView)View).ObjectTypeInfo;
                List<ITypeInfo> typesInfo = new List<ITypeInfo>(ReflectionHelper.FindTypeDescendants(objectTypeInfo));
                typesInfo.Insert(0, ((ObjectView)View).ObjectTypeInfo);
                foreach (ITypeInfo typeInfo in typesInfo)
                {
                    var cls = Application.Model.BOModel.GetClass(typeInfo.Type);
                    if (!cls.IsCreatableItem) continue;
                    ChoiceActionItem item = new ChoiceActionItem(cls.Caption, typeInfo);
                    item.ImageName = cls.ImageName;
                    CreateItem.Items.Add(item);
                }

                // Кастомизация типа редактора элемента
                // (Установлена постоянно для установки ViewItem.CurrentObject, необходимым редактору выражений)
                //if (!(CreateItem.Items.Count == 1 && CreateItem.Items[0].Data == objectTypeInfo))
                ListView.CreateCustomCurrentObjectDetailView += ListView_CreateCustomCurrentObjectDetailView;
            }
        }

        /// <inheritdoc/>
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            if (View is ListView)
                ListView.CreateCustomCurrentObjectDetailView -= ListView_CreateCustomCurrentObjectDetailView;
        }

        private ListView ListView { get { return View as ListView; } }

        // Кастомизация редактора элемента
        private void ListView_CreateCustomCurrentObjectDetailView(object sender, CreateCustomCurrentObjectDetailViewEventArgs e)
        {
            if (e.ListViewCurrentObject != null)
                e.DetailView = Application.CreateDetailView(ObjectSpaceInMemory.CreateNew(), e.ListViewCurrentObject);
        }

        // Создание элемента 
        private void CreateItem_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            BeginUpdate();
            ITypeInfo itemType = (ITypeInfo)e.SelectedChoiceActionItem.Data;
            ReportWizardItem item = (ReportWizardItem)itemType.CreateInstance();

            // Добавление в список
            ReportWizardItem current = ListView.CurrentObject as ReportWizardItem;
            ReportWizardBlock parent = current != null ? current.Parent : null;
            if (parent == null)
                ListView.CollectionSource.Add(item);
            else
            {
                parent.GetList(item).Insert(current.IndexOf + 1, item);
                ListView.CollectionSource.ResetCollection();
                ListView.CurrentObject = item;
            }
            EndUpdate();
            ListView.CollectionSource.ResetCollection();
        }

        // Удаление элемента
        private void DeleteItem_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            BeginUpdate();
            foreach (ReportWizardItem item in e.SelectedObjects) item.Parent = null;
            EndUpdate();
            ListView.CollectionSource.ResetCollection();
        }

        // Перемещение по списку
        private void MoveItems(SimpleActionExecuteEventArgs e, int direction)
        {
            BeginUpdate();
            List<ReportWizardItem> items = new List<ReportWizardItem>(e.SelectedObjects.Cast<ReportWizardItem>());
            IList list = items.First().List;
            int index = Math.Max(items.Min(i => i.IndexOf) + direction, 0);
            foreach (ReportWizardItem item in items) list.Remove(item);
            bool add = index >= list.Count;
            foreach (ReportWizardItem item in items)
            {
                if (add) list.Add(item); else list.Insert(index, item);
                index++;
            }
            EndUpdate();
            ListView.CollectionSource.ResetCollection();
        }

        // Перемещение вверх по списку
        private void UpItem_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            MoveItems(e, -1);
        }

        // Перемещение вниз по списку
        private void DownItem_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            MoveItems(e, 1);
        }
    }
}
