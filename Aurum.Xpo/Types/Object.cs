using System;
using System.Globalization;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Model;

namespace Aurum.Xpo
{
    /// <summary>
    /// Базовый класс для хранимых объектов с автогенерируемым идентификатором Id
    /// </summary>
    /// <remarks>Не содержит специального поля блокировки <see cref="DevExpress.Xpo.OptimisticLockingAttribute"/>
    /// и устанавливает логическое удаление объекта <see cref="DevExpress.Xpo.DeferredDeletionAttribute"/>.
    /// Также знак равенства переопределен с проверкой равенства идентификаторов вместо равенства ссылок. 
    /// Для равенства ссылок следует использовать Object.ReferenceEquals.</remarks>
    [NonPersistent, OptimisticLocking(false), DeferredDeletion(true)]
    public abstract class XPObjectBase : XPBaseObject
    {
        [Persistent("ID"), Key(true)]
        private int id = -1;

        /// <inheritdoc />
        protected XPObjectBase() : base() { }
        /// <inheritdoc />
        protected XPObjectBase(Session session) : base(session) { }
        /// <inheritdoc />
        protected XPObjectBase(Session session, XPClassInfo classInfo) : base(session, classInfo) { }

        /// <summary>
        /// Идентификатор объекта с автогенерацией
        /// </summary>
        /// <value>Значение устанавливается только для нового объекта</value>
        [PersistentAlias("id")]
        [VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false), ModelDefault("AllowEdit", "False")]
		public int Id 
        {
			get { return id; }
            // Требуется доработка процедуры Add для передачи установленного значения идентификатора (in out)
            set { if (!IsLoading && !IsSaving && id == -1) id = value; } 
		}

        /*/// <summary>Указывает, установлен ли идентификатор в значение по умолчанию для нового объекта</summary>
        /// <remarks>True, если значение идентификатора равно -1</remarks>
        protected bool HasDefaultNewId { get { return id == -1; } }*/

        /// <contentfrom cref="DevExpress.Xpo.PersistentBase.Fields" />
        new public static FieldsClass Fields { get { return new FieldsClass(); } }
        /// <contentfrom cref="DevExpress.Xpo.PersistentBase.FieldsClass" />
        new public class FieldsClass : XPCustomObject.FieldsClass
        {
            /// <inheritdoc />           
            public FieldsClass() : base() { }
            /// <inheritdoc />           
            public FieldsClass(string propertyName) : base(propertyName) { }
            
            /// <summary>
            /// Операнд свойства <see cref="XPObjectBase.Id"/>
            /// </summary>
            public OperandProperty Id { get { return new OperandProperty(GetNestedName("Id")); } }
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            // TODO: проблема с аудитом, который помещает объекты в свой словарь (Dictionary), а потом не может их найти, т.к. хешкод изменился
            // return Id == -1 ? base.GetHashCode() : Id;
            return base.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (Id == -1)
                return object.ReferenceEquals(this, obj);
            if (GetType() != obj.GetType())
                return false;
            return Id == ((XPObjectBase)obj).Id;
        }

        /// <summary>
        /// Возвращает результат равенства объектов a и b по идентификаторам
        /// </summary>
        /// <param name="a">Объект a</param>
        /// <param name="b">Объект b</param>
        /// <returns>True, если объекты одного типа и их идентификаторы равны, иначе false</returns>
        public static bool operator ==(XPObjectBase a, XPObjectBase b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;
            return a.Equals(b);
        }

        /// <summary>
        /// Возвращает результат неравенства объектов a и b по идентификаторам
        /// </summary>
        /// <param name="a">Объект a</param>
        /// <param name="b">Объект b</param>
        /// <returns>True, если объекты разных типов или их идентификаторы не равны, иначе false</returns>
        public static bool operator !=(XPObjectBase a, XPObjectBase b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Устанавливает свойство в отношении один к одному
        /// </summary>
        /// <typeparam name="T">Тип устанавливаемого свойства</typeparam>
        /// <typeparam name="R">Тип свойства, находящегося в отношении к данному</typeparam>
        /// <param name="propertyName">Название свойства</param>
        /// <param name="propertyValueHolder">Ссылка на значение, содержащее свойство</param>
        /// <param name="newValue">Новое значение</param>
        /// <param name="getRelation">Функция, возвращающее значение свойства объекта отношения</param>
        /// <param name="setRelation">Делегат, устанавливающий значение свойства объекта отношения</param>
        protected void SetOneToOneValue<T, R>(string propertyName, ref T propertyValueHolder, T newValue, 
            Func<T, R> getRelation, Action<T, R> setRelation)
            where T : class
            where R : XPObjectBase
        {
            if (propertyValueHolder == newValue)
                return;

            T prevValue = propertyValueHolder;
            propertyValueHolder = newValue;

            if (IsLoading) return;

            if (prevValue != null && getRelation(prevValue) == this)
                setRelation(prevValue, null);

            if (propertyValueHolder != null)
                setRelation(propertyValueHolder, (R)this);
            OnChanged(propertyName);
        }
    }

    /// <summary>
    /// Базовый класс для хранимых объектов с идентификатором Id типа KeyType
    /// </summary>
    /// <typeparam name="IdType">Тип идентификатора (ключа) класса</typeparam>
    /// <remarks>Не содержит специального поля блокировки <see cref="DevExpress.Xpo.OptimisticLockingAttribute"/>
    /// и устанавливает логическое удаление объекта <see cref="DevExpress.Xpo.DeferredDeletionAttribute"/>.</remarks>
    [NonPersistent, MemberDesignTimeVisibility(false), OptimisticLocking(false), DeferredDeletion(true)]
    public abstract class XPObjectBase<IdType> : XPBaseObject
    {
        private IdType id;

        /// <summmary>Конструктор объекта с указанным идентификатором</summmary>
        /// <param name="id">Идентификатор объекта</param>
        protected XPObjectBase(IdType id) { this.Id = id; }

        /// <summmary>Конструктор объекта в указанной сессии и с указанным идентификатором</summmary>
        /// <param name="session">Сессия соединения с БД экземпляра класса</param>
        /// <param name="id">Идентификатор объекта</param>
        protected XPObjectBase(Session session, IdType id) : base(session) { this.Id = id; }

        /// <inheritdoc />
        protected XPObjectBase() { }

        /// <inheritdoc />
        protected XPObjectBase(Session session) : base(session) { }
        
        /// <inheritdoc />
        protected XPObjectBase(Session session, XPClassInfo classInfo) : base(session, classInfo) { }

        /// <summary>
        /// Идентификатор объекта
        /// </summary>
        /// <remarks>Идентификатор не поддерживает автогенерацию, даже если тип целочисленный.
        /// Для целочисленного идентификатора с автогенерацией используйте класс <see cref="XPObjectBase"/>.</remarks>
        [Persistent("ID"), Key(false)]
        [VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false), ModelDefault("AllowEdit", "False")]
        public IdType Id 
        {
            get { return id; }
            set { SetPropertyValue<IdType>("Id", ref id, value); } 
        }

        /// <contentfrom cref="DevExpress.Xpo.PersistentBase.Fields" />
        new public static FieldsClass Fields { get { return new FieldsClass(); } }
        /// <contentfrom cref="DevExpress.Xpo.PersistentBase.FieldsClass" />
        new public class FieldsClass : XPCustomObject.FieldsClass
        {
            /// <inheritdoc />           
            public FieldsClass() : base() { }
            /// <inheritdoc />           
            public FieldsClass(string propertyName) : base(propertyName) { }

            /// <summary>
            /// Операнд свойства <see cref="P:XPObjectBase`1.Id"/>
            /// </summary>
            public OperandProperty Id { get { return new OperandProperty(GetNestedName("Id")); } }
        }
    }
    
    /// <summary>
    /// Базовый класс для хранимых объектов с идентификатором Id типа string[20]
    /// </summary>
    /// <remarks>Не содержит специального поля блокировки <see cref="DevExpress.Xpo.OptimisticLockingAttribute"/>
    /// и устанавливает логическое удаление объекта <see cref="DevExpress.Xpo.DeferredDeletionAttribute"/>.</remarks>
    [NonPersistent, MemberDesignTimeVisibility(false), OptimisticLocking(false), DeferredDeletion(true)]
    public abstract class XPObjectString : XPBaseObject
    {
        private string id;

        /// <summmary>Конструктор объекта с указанным идентификатором</summmary>
        /// <param name="id">Идентификатор объекта</param>
        protected XPObjectString(string id) { this.Id = id; }

        /// <summmary>Конструктор объекта в указанной сессии и с указанным идентификатором</summmary>
        /// <param name="session">Сессия соединения с БД экземпляра класса</param>
        /// <param name="id">Идентификатор объекта</param>
        protected XPObjectString(Session session, string id) : base(session) { this.Id = id; }

        /// <inheritdoc />
        protected XPObjectString() { }

        /// <inheritdoc />
        protected XPObjectString(Session session) : base(session) { }

        /// <inheritdoc />
        protected XPObjectString(Session session, XPClassInfo classInfo) : base(session, classInfo) { }

        /// <summary>
        /// Идентификатор объекта
        /// </summary>
        [Persistent("ID"), Key(false), Size(20)]
        [VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false), ModelDefault("AllowEdit", "False")]
        public string Id
        {
            get { return id; }
            set { SetPropertyValue<string>("Id", ref id, value); }
        }

        /// <contentfrom cref="DevExpress.Xpo.PersistentBase.Fields" />
        new public static FieldsClass Fields { get { return new FieldsClass(); } }
        /// <contentfrom cref="DevExpress.Xpo.PersistentBase.FieldsClass" />
        new public class FieldsClass : XPCustomObject.FieldsClass
        {
            /// <inheritdoc />           
            public FieldsClass() : base() { }
            /// <inheritdoc />           
            public FieldsClass(string propertyName) : base(propertyName) { }

            /// <summary>
            /// Операнд свойства <see cref="P:XPObjectBase`1.Id"/>
            /// </summary>
            public OperandProperty Id { get { return new OperandProperty(GetNestedName("Id")); } }
        }
    }

    /// <summary>
    /// Базовый класс для хранимых объектов с целочисленным идентификатором Id без автогенерации
    /// </summary>
    /// <remarks>Не содержит специального поля блокировки <see cref="DevExpress.Xpo.OptimisticLockingAttribute"/>
    /// и устанавливает логическое удаление объекта <see cref="DevExpress.Xpo.DeferredDeletionAttribute"/>.</remarks>
    [NonPersistent, MemberDesignTimeVisibility(false), OptimisticLocking(false), DeferredDeletion(true)]
    public abstract class XPObjectInt : XPObjectBase<int>
    {
        /// <summmary>Конструктор объекта с указанным идентификатором</summmary>
        /// <param name="id">Идентификатор объекта</param>
        protected XPObjectInt(int id) { this.Id = id; }

        /// <summmary>Конструктор объекта с указанной сессией и указанным идентификатором</summmary>
        /// <param name="session">Сессия соединения с БД экземпляра класса</param>
        /// <param name="id">Идентификатор объекта</param>
        protected XPObjectInt(Session session, int id) : base(session) { this.Id = id; }

        /// <inheritdoc />
        protected XPObjectInt() { }
        /// <inheritdoc />
        protected XPObjectInt(Session session) : base(session) { }
        /// <inheritdoc />
        protected XPObjectInt(Session session, XPClassInfo classInfo) : base(session, classInfo) { } 
    }
}
