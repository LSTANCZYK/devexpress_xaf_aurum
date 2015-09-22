using System;
using System.Globalization;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpo;

namespace Aurum.Xpo
{
    /// <summary>
    /// Структура для хранения значения произвольного типа
    /// </summary>
    /// <remarks>Поддерживается только интерфейс <see cref="IConvertible"/></remarks>
    public struct Variant
    {
        /// <summary>
        /// Значение в строковом виде
        /// </summary>
        [Persistent("Value"), Size(128)]
        private string stringValue;
        private object value;

        /// <summary>
        /// Значение
        /// </summary>
        [NonPersistent]
        public object Value
        {
            get
            {
                if (value != null) return value;
                if (!string.IsNullOrEmpty(stringValue)) value = StringToValue(stringValue);
                return value;
            }
            set
            {
                this.value = value;
                this.stringValue = ValueToString(value);
            }
        }

        /// <summary>
        /// Преобразование указанного значения в строку формата Variant
        /// </summary>
        /// <param name="value">Значение</param>
        /// <returns>Строка, представляющее значение в формате Variant</returns>
        /// <exception cref="ArgumentException">Значение не поддерживает интерфейс IConvertible</exception>
        public static string ValueToString(object value)
        {
            if (value == null) return null;
            string typeString, valueString;
            if (value is IConvertible)
            {
                typeString = Type.GetTypeCode(value.GetType()).ToString();
                valueString = Convert.ToString(value, CultureInfo.InvariantCulture);
            }
            else
                throw new ArgumentException();
            object t = new OperandProperty(typeString);
            object v = new OperandValue(valueString);
            return string.Concat(t.ToString(), ":", v.ToString());
        }

        /// <summary>
        /// Преобразование указанной строки формата Variant в значение
        /// </summary>
        /// <param name="str">Строка в формате Variant</param>
        /// <returns>Значение объекта, представленное строкой</returns>
        /// <exception cref="ArgumentException">Ошибка в формате строки</exception>
        public static object StringToValue(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;
            CriteriaLexer lexer = new CriteriaLexer(new System.IO.StringReader(str));
            if (!lexer.Advance())
                throw new ArgumentException();
            OperandProperty prop = lexer.CurrentValue as OperandProperty;
            if (ReferenceEquals(prop, null))
                throw new ArgumentException();
            if (!lexer.Advance())
                throw new ArgumentException();
            if (lexer.CurrentToken != ':')
                throw new ArgumentException();
            if (!lexer.Advance())
                throw new ArgumentException();
            OperandValue val = lexer.CurrentValue as OperandValue;
            if (ReferenceEquals(val, null))
                throw new ArgumentException();
            TypeCode typeCode = (TypeCode)Enum.Parse(typeof(TypeCode), prop.PropertyName, false);
            return Convert.ChangeType(val.Value, typeCode, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
