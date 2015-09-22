using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Aurum.Xpo;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;

namespace Aurum.Security
{
    /// <summary>
    /// Интерфейс объекта, принадлежащего собственнику данных
    /// </summary>
    public interface IOwnedObject
    {
        /// <summary>
        /// Собственник данных, которому принадлежит объект
        /// </summary>
        Owner Owner { get; }
    }

    /// <summary>
    /// Базовый класс объекта с автогенерируемым идентификатором, принадлежащего собственнику данных
    /// </summary>
    /// <remarks>При сохранении объекта если собственник данных не указан, 
    /// по умолчанию устанавливается собственник данных текущего пользователя <see cref="SetOwnerDefault"/></remarks>
    [NonPersistent]
    public abstract class XPOwnedObjectBase : XPObjectBase, IOwnedObject
    {
        private Owner owner;

        /// <summary>Конструктор без параметров</summary>
        public XPOwnedObjectBase() { }

        /// <summary>Конструктор с указанной сессией</summary>
        /// <param name="session">Сессия для загрузки и сохранения объектов в базе данных</param>
        public XPOwnedObjectBase(Session session) : base(session) { }

        /// <summary>Конструктор с указанным собственником данных</summary>
        /// <param name="owner">Собственник данных, которому принадлежит объект</param>
        /// <exception cref="NullReferenceException">Не указан собственник данных</exception>
        public XPOwnedObjectBase(Owner owner) : base(owner.Session) { this.Owner = owner; }

        /// <summary>
        /// Собственник данных, которому принадлежит объект
        /// </summary>
        [Persistent, NotNull]//, ReadOnly]
        [VisibleInListView(true), VisibleInDetailView(false), VisibleInLookupListView(true), ModelDefault("AllowEdit", "False")]
        public Owner Owner
        {
            get { return owner; }
            set { SetPropertyValue<Owner>("Owner", ref owner, value); }
        }

        /// <inheritdoc/>
        protected override void OnSaving()
        {
            base.OnSaving();
            SetOwnerDefault();
        }

        /// <summary>
        /// Устанавливает собственника данных по умолчанию для новой записи
        /// </summary>
        /// <seealso cref="M:Owner.GetUserOwner"/>
        protected virtual void SetOwnerDefault()
        {
            if (Owner == null && Session.IsNewObject(this))
                Owner = Owner.GetUserOwner(Session);
        }

        /// <contentfrom cref="DevExpress.Xpo.PersistentBase.Fields" />
        public static new readonly FieldsClass Fields = new FieldsClass();
        /// <contentfrom cref="DevExpress.Xpo.PersistentBase.FieldsClass" />
        public new class FieldsClass : XPObjectBase.FieldsClass
        {
            /// <summary>Операнд свойства Owner</summary>
            public OperandProperty Owner { get { return new OperandProperty("Owner"); } }
        }
    }
    
    /// <summary>
    /// Базовый класс объекта с автогенерируемым идентификатором, 
    /// принадлежащего собственнику данных, с поддержкой безопасности между собственниками данных
    /// </summary>
    [NonPersistent]
    public abstract class XPOwnedObject : XPOwnedObjectBase, IContractObject<OwnerContext>
    {
        /// <summary>Конструктор без параметров</summary>
        public XPOwnedObject() { }

        /// <summary>Конструктор с указанной сессией</summary>
        /// <param name="session">Сессия для загрузки и сохранения объектов в базе данных</param>
        public XPOwnedObject(Session session) : base(session) { }

        /// <summary>Конструктор с указанным собственником данных</summary>
        /// <param name="owner">Собственник данных, которому принадлежит объект</param>
        /// <exception cref="NullReferenceException">Не указан собственник данных</exception>
        public XPOwnedObject(Owner owner) : base(owner) { }
    }
}
