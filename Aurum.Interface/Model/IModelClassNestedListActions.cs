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
    /// Настройка операций Создания, Удаления, Добавления и Убирания в Nested ListView для бизнес-объекта
    /// </summary>
    public interface IModelClassNestedListActions : IModelNode
    {
        [DefaultValue(true)]
        [Category("NestedListViewActions")]
        [Description("Разрешить Создание")]
        bool AllowCreate { get; set; }

        [DefaultValue(true)]
        [Category("NestedListViewActions")]
        [Description("Разрешить Удаление")]
        bool AllowDelete { get; set; }

        [DefaultValue(true)]
        [Category("NestedListViewActions")]
        [Description("Разрешить Добавление")]
        bool AllowLink { get; set; }

        [DefaultValue(true)]
        [Category("NestedListViewActions")]
        [Description("Разрешить Убирание")]
        bool AllowUnlink { get; set; }
    }
}
