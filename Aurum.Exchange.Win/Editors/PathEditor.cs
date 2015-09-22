using Aurum.Exchange;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aurum.Exchange.Win.Editors
{
    /// <summary>
    /// Редактор путей к сущностям
    /// </summary>
    [PropertyEditor(typeof(FilePath), "FilePathEditor", true)]
    [PropertyEditor(typeof(DirectoryPath), "DirectoryPathEditor", true)]
    [PropertyEditor(typeof(MultipleFilePath), "MultipleFilePathEditor", true)]
    public class PathEditor : WinPropertyEditor
    {
        public PathEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model)
        {
        }

        protected override object CreateControlCore()
        {
            ControlBindingProperty = "Value";
            var prop = ObjectType.GetProperty(PropertyName);

            // Тип (файл / папка)
            var entityType = (prop.PropertyType == typeof(FilePath) || prop.PropertyType == typeof(MultipleFilePath)) ?
                    PathControlEntityType.File :
                    PathControlEntityType.Directory;

            // Множественное сохранение
            var multipleOpening = prop.PropertyType == typeof(MultipleFilePath);

            // Режим (открыть / сохранить)
            var mode = FilePathMode.Open;

            var attr = prop.GetCustomAttributes(typeof(FilePathModeAttribute), true);

            if (attr.Length > 0)
            {
                mode = (attr[0] as FilePathModeAttribute).Mode;
            }

            // 
            var filter = String.Empty;
            var filterAttr = prop.GetCustomAttributes(typeof(FilePathFilterAttribute), true);

            if (filterAttr.Length > 0)
            {
                filter = (filterAttr[0] as FilePathFilterAttribute).Filter;
            }

            //
            var ctr = new PathControl
            {
                EntityType = entityType,
                MultipleOpening = multipleOpening,
                Mode = mode,
                Filter = filter
            };

            return ctr;
        }
    }
}
