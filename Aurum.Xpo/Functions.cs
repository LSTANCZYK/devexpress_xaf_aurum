using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;

namespace Aurum.Xpo
{
    /// <summary>
    /// Функция, возвращающая взаимное исключение (XOR) двух логических параметров
    /// </summary>
    public class XorFunction : ICustomFunctionOperatorFormattable, ICustomFunctionOperatorBrowsable
    {
        #region ICustomFunctionOperator

        object ICustomFunctionOperator.Evaluate(params object[] operands)
        {
            return (bool)operands[0] ^ (bool)operands[1];
        }

        string ICustomFunctionOperator.Name
        {
            get { return "Xor"; }
        }

        Type ICustomFunctionOperator.ResultType(params Type[] operands)
        {
            return typeof(bool);
        }

        #endregion

        #region ICustomFunctionOperatorFormattable
        
        string ICustomFunctionOperatorFormattable.Format(Type providerType, params string[] operands)
        {
            if (providerType == typeof(ODPConnectionProvider) || providerType.IsSubclassOf(typeof(ODPConnectionProvider)))
                return string.Format("(({0} and not {1}) or (not {0} and {1}))", operands[0], operands[1]);
            throw new NotSupportedException(
                string.Concat("This provider is not supported: ", providerType.Name));
        }

        #endregion

        #region ICustomFunctionOperatorBrowsable

        FunctionCategory ICustomFunctionOperatorBrowsable.Category
        {
            get { return FunctionCategory.Logical; }
        }

        string ICustomFunctionOperatorBrowsable.Description
        {
            get { return "Возвращает взаимное исключение (XOR) двух логических параметров"; }
        }

        bool ICustomFunctionOperatorBrowsable.IsValidOperandCount(int count)
        {
            return count == 2;
        }

        bool ICustomFunctionOperatorBrowsable.IsValidOperandType(int operandIndex, int operandCount, Type type)
        {
            return type == typeof(bool);
        }

        int ICustomFunctionOperatorBrowsable.MaxOperandCount
        {
            get { return 2; }
        }

        int ICustomFunctionOperatorBrowsable.MinOperandCount
        {
            get { return 2; }
        }

        #endregion
    }
}
