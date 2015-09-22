using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aurum.Exchange
{
    /// <summary>
    /// Генератор моделей доступных экспортов
    /// </summary>
    public class ModelExportsGenerator : ModelNodesGeneratorBase
    {
        private string GetXafDisplayAttributeValue(Type t)
        {
            var displayAttr = t.GetCustomAttributes(typeof(XafDisplayNameAttribute), true);
            if (displayAttr.Length == 0)
            {
                return null;
            }
            return (displayAttr[0] as XafDisplayNameAttribute).DisplayName;
        }

        protected override void GenerateNodesCore(DevExpress.ExpressApp.Model.Core.ModelNode node)
        {
            foreach (var t in ExchangeTypeHelper.FindAllExchanges())
            {
                var _node = node as IModelExports;

                if (!_node.Any(x => x.Type == t)) // Дизайнер иногда глючит и обрабатывает по два раза
                {
                    var n = node.AddNode<IModelExport>(t.FullName);
                    var attrName = GetXafDisplayAttributeValue(t);

                    n.Type = t;
                    n.Name = attrName ?? t.Name; // Имя по умолчанию -- имя класса
                    n.TypeName = t.FullName;
                }
            }
        }
    }
}
