using System;
using System.ComponentModel;
using System.Globalization;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;

namespace Aurum.Xpo
{
    /// <summary>
    /// Интерфейс поставщика типа для слабосвязанной ссылки
    /// </summary>
    public interface IReferenceTypeProvider
    {
        /// <summary>
        /// Возвращает тип объектов для слабосвязанной ссылки
        /// </summary>
        /// <param name="propertyName">Свойство класса, для которого определяется тип объектов ссылки</param>
        /// <returns>Тип объектов, на которые должна указывать ссылка в свойстве propertyName</returns>
        Type GetReferenceType(string propertyName);
    }

    /// <summary>
    /// Конвертор значений в слабосвязанную ссылку
    /// </summary>
    public class ReferenceTypeConverter : TypeConverter
    {
        /// <inheritdoc/>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return typeof(IXPSimpleObject).IsAssignableFrom(sourceType);
        }

        /// <inheritdoc/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                return new Reference();
            if (value is IXPSimpleObject)
            {
                Reference reference = new Reference();
                reference.SetObject((IXPSimpleObject)value);
                return reference;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    /// <summary>
    /// Структура для хранения слабосвязанной ссылки на объект
    /// </summary>
    [TypeConverter(typeof(ReferenceTypeConverter))]
    public struct Reference
    {
        private int? intRef;
        private string strRef;

        /// <summary>
        /// Ссылка на объект с целочисленным идентификатором
        /// </summary>
        [Persistent]
        public int? IntRef
        {
            get { return intRef; }
            private set { intRef = value; }
        }

        /// <summary>
        /// Ссылка на объект со строковым идентификатором (ограничение 50 символов)
        /// </summary>
        [Persistent, Size(50)]
        public string StrRef
        {
            get { return strRef; }
            private set { strRef = value; }
        }

        /// <summary>
        /// Конструктор с явно заданной ссылкой на объект
        /// </summary>
        /// <param name="intRef">Ссылка на объект с целочисленным идентификатором</param>
        /// <param name="strRef">Ссылка на объект со строковым идентификатором</param>
        public Reference(int? intRef, string strRef) { this.intRef = intRef; this.strRef = strRef; }

        /// <summary>
        /// Определяет, является ли ссылка пустой (null)
        /// </summary>
        /// <param name="reference">Слабосвязанная ссылка</param>
        /// <returns>True, если ссылка пустая (null)</returns>
        public static bool IsNullRef(Reference reference)
        {
            return !reference.IntRef.HasValue && string.IsNullOrEmpty(reference.StrRef);
        }

        /// <summary>
        /// Возвращает объект по ссылке
        /// </summary>
        /// <param name="session">Сессия для хранения и загрузки хранимых объектов</param>
        /// <param name="classInfo">Информация о классе</param>
        /// <returns>Объект или null, если ссылка пустая</returns>
        public object GetObject(Session session, XPClassInfo classInfo)
        {
            object key = TargetKeyValue;
            return key == null ? null : session.GetObjectByKey(classInfo, TargetKeyValue);
        }

        /// <summary>
        /// Возвращает объект по ссылке
        /// </summary>
        /// <param name="session">Сессия для хранения и загрузки хранимых объектов</param>
        /// <param name="classType">Тип класса</param>
        /// <returns>Объект или null, если ссылка пустая</returns>
        public object GetObject(Session session, Type classType)
        {
            return GetObject(session, session.Dictionary.QueryClassInfo(classType));
        }

        /// <summary>
        /// Возвращает объект по ссылке
        /// </summary>
        /// <param name="objectSpace">Пространство объектов</param>
        /// <param name="classType">Тип класса</param>
        /// <returns>Объект или null, если ссылка пустая</returns>
        public object GetObject(IObjectSpace objectSpace, Type classType)
        {
            object key = TargetKeyValue;
            return objectSpace.GetObjectByKey(classType, key);
        }

        /// <summary>
        /// Устанавливает ссылку на объект
        /// </summary>
        /// <param name="session">Сессия для хранения и загрузки хранимых объектов</param>
        /// <param name="value">Объект, на который устанавливается ссылка</param>
        /// <exception cref="ArgumentException">Попытка установить ссылку на несохраненный объект</exception>
        public void SetObject(Session session, object value)
        {
            if (value == null)
                TargetKeyValue = null;
            else if (session.IsNewObject(value))
                throw new ArgumentException("Referenced saved object expected");
            else
                TargetKeyValue = session.GetKeyValue(value);
        }

        /// <summary>
        /// Устанавливает ссылку на объект 
        /// </summary>
        /// <param name="objectSpace">Пространство объектов</param>
        /// <param name="value">Объект, на который устанавливается ссылка</param>
        /// <exception cref="ArgumentException">Попытка установить ссылку на несохраненный объект</exception>
        public void SetObject(IObjectSpace objectSpace, object value)
        {
            if (value == null)
                TargetKeyValue = null;
            else if (objectSpace.IsNewObject(value))
                throw new ArgumentException("Referenced saved object expected");
            else
                TargetKeyValue = objectSpace.GetKeyValue(value);
        }

        /// <summary>
        /// Устанавливает ссылку на объект
        /// </summary>
        /// <param name="value">Объект, на который устанавливается ссылка</param>
        public void SetObject(IXPSimpleObject value)
        {
            if (value == null)
                TargetKeyValue = null;
            else
                SetObject(value.Session, value);
        }

        [NonPersistent]
        private object TargetKeyValue
        {
            get
            {
                if (IntRef.HasValue)
                    return IntRef.Value;
                else if (!string.IsNullOrEmpty(StrRef))
                    return XPWeakReference.StringToKey(StrRef);
                else
                    return null;
            }
            set
            {
                IntRef = null;
                StrRef = null;
                if (Convert.GetTypeCode(value) == TypeCode.Int32)
                    IntRef = (int)value;
                else if (value != null)
                    StrRef = XPWeakReference.KeyToString(value);
            }
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is Reference && this == (Reference)obj;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            if (intRef.HasValue) return intRef.Value;
            if (strRef != null) return strRef.GetHashCode();
            return base.GetHashCode();
        }

        /// <summary>
        /// Оператор равенства
        /// </summary>
        /// <param name="x">Первый операнд</param>
        /// <param name="y">Второй операнд</param>
        /// <returns>True, если ссылки x и y имеют одинаковое значение, иначе false</returns>
        public static bool operator ==(Reference x, Reference y)
        {
            return x.intRef == y.intRef && x.strRef == y.strRef;
        }

        /// <summary>
        /// Оператор неравенства
        /// </summary>
        /// <param name="x">Первый операнд</param>
        /// <param name="y">Второй операнд</param>
        /// <returns>True, если ссылки x и y имеют неодинаковое значение, иначе false</returns>
        public static bool operator !=(Reference x, Reference y)
        {
            return !(x == y);
        }
    }

    /// <summary>
    /// Слабосвязанная ссылка на объект
    /// </summary>
    /// <remarks>XPReference отличается от <see cref="XPWeakReference"/> тем, 
    /// что имеет целочисленный идентификатор и отдельное поле для хранения целочисленной ссылки</remarks>
    public class XPReference : XPObjectBase, IXPOServiceClass
    {
        /// <summary>Определяет, указывает ли ссылка на объект</summary>
        [NonPersistent]
        public bool IsAlive { get { return Target != null; } }

        /// <summary>Идентификатор объекта, на которого указывает ссылка</summary>
        [NonPersistent]
        protected object TargetKeyValue;

        /// <summary>Тип объекта, на которого указывает ссылка</summary>
        [Persistent, NotNull]
        protected XPObjectType TargetType;

        /// <summary>Определяет, является ли идентификатор объекта целочисленным</summary>
        protected bool IsIntRef { get { return Convert.GetTypeCode(TargetKeyValue) == TypeCode.Int32; } }

        /// <summary>Строковое представление идентификатора объекта, на которого указывает ссылка</summary>
        [Persistent]
        protected string StrRef
        {
            get { return IsIntRef ? null : XPWeakReference.KeyToString(TargetKeyValue); }
            set { if (!string.IsNullOrEmpty(value)) TargetKeyValue = XPWeakReference.StringToKey(value); }
        }

        /// <summary>Целочисленное представление идентификатора объекта, на которого указывает ссылка</summary>
        [Persistent]
        protected int? IntRef
        {
            get { return IsIntRef ? (int)TargetKeyValue : (int?)null; }
            set { if (value.HasValue) TargetKeyValue = value; }
        }

        /// <summary>Объект, на который указывает ссылка</summary>
        [NonPersistent]
        public object Target
        {
            get
            {
                if (TargetKeyValue == null)
                    return null;
                if (!TargetType.IsValidType)
                    return null;
                return this.Session.GetObjectByKey(TargetType.TypeClassInfo, TargetKeyValue);
            }
            set
            {
                if (value == null)
                {
                    this.TargetType = null;
                    this.TargetKeyValue = null;
                }
                else if (this.Session.IsNewObject(value))
                    throw new ArgumentException("XPReference saved object expected");
                else
                {
                    this.TargetType = Session.GetObjectType(value);
                    this.TargetKeyValue = Session.GetKeyValue(value);
                }
                OnChanged();
            }
        }

        /// <summary>Конструктор</summary>
        public XPReference() : base() { }

        /// <summary>Конструктор с сессией</summary>
        /// <param name="session">Сессия для хранения и загрузки хранимых объектов</param>
        public XPReference(Session session) : base(session) { }

        /// <summary>Конструктор с сессией и объектом ссылки</summary>
        /// <param name="session">Сессия для хранения и загрузки хранимых объектов</param>
        /// <param name="target">Объект, на который указывает ссылка</param>
        public XPReference(Session session, object target) : base(session) { this.Target = target; }

        /// <summary>Конструктор с объектом ссылки</summary>
        /// <param name="target">Объект, на который указывает ссылка</param>
        public XPReference(IXPSimpleObject target) : this(target.Session, target) { }

        /// <inheritdoc/>
        public override string ToString()
        {
            object target = Target;
            return target != null ? target.ToString() : null;
        }
    }
}
