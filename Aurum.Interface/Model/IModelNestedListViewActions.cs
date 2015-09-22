using DevExpress.ExpressApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Interface.Model
{
    /// <summary>
    /// Настройка операций Создания, Удаления, Добавления и Убирания в Nested ListView для представления бизнес-объекта
    /// </summary>
    public interface IModelNestedListViewActions : IModelNode
    {
        [ModelValueCalculator("ModelClass", typeof(IModelClassNestedListActions), "AllowCreate")]
        [Category("Actions")]
        [Description("Разрешить Создание")]
        bool AllowCreate { get; set; }

        [ModelValueCalculator("ModelClass", typeof(IModelClassNestedListActions), "AllowDelete")]
        [Category("Actions")]
        [Description("Разрешить Удаление")]
        bool AllowDelete { get; set; }

        [ModelValueCalculator("ModelClass", typeof(IModelClassNestedListActions), "AllowLink")]
        [Category("Actions")]
        [Description("Разрешить Добавление")]
        bool AllowLink { get; set; }

        [ModelValueCalculator("ModelClass", typeof(IModelClassNestedListActions), "AllowUnlink")]
        [Category("Actions")]
        [Description("Разрешить Убирание")]
        bool AllowUnlink { get; set; }
    }
}
