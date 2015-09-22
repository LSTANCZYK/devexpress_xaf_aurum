using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aurum.Interface.Win.Editors
{
    /// <summary>
    /// Исключение редактора свойства
    /// </summary>
    public class PropertyEditorException : Exception
    {
        public PropertyEditorException() { }
        public PropertyEditorException(string message) : base(message) { }
        public PropertyEditorException(string message, Exception innerException) : base(message, innerException) { }

        public PropertyEditorException(Type editorType, string message)
            : base(string.Format("{0}. {1}", editorType.Name, message))
        {
        }
    }
}
