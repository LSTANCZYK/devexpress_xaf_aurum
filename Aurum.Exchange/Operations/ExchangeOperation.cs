using Aurum.Operations;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aurum.Exchange
{
    /// <summary>
    /// Операция обмена
    /// </summary>
    public abstract class ExchangeOperation : IDisposable
    {
        /// <summary>
        /// Событие, происходящее непосредственно перед выполнением операции и после установки параметров
        /// </summary>
        public event EventHandler BeforeExchangeExecute;

        /// <summary>
        /// Событие, возникающее непосредственно после выполнения операции
        /// </summary>
        public event EventHandler AfterExchangeExecute;

        /// <summary>
        /// Объект параметров
        /// </summary>
        public ExchangeParameters ParametersObject { get; set; }

        /// <summary>
        /// Родительская операция обмена
        /// </summary>
        public ExchangeOperation Parent { get; set; }

        /// <summary>
        /// Порядковый номер этой операции, если она выполняется по порядку. Нумерация начинается с 1.
        /// Если текущая операция выполняется сама по себе, то значение равно 0
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Тип, над которым выполняется операция обмена
        /// </summary>
        protected abstract Type ObjectType { get; }

        /// <summary>
        /// Объект приложения
        /// </summary>
        public XafApplication Application { get; private set; }

        /// <summary>
        /// Модель
        /// </summary>
        public abstract IModelExchange Model { get; }

        /// <summary>
        /// Тип параметров
        /// </summary>
        public virtual Type ParametersType { get { return null; } }

        /// <summary>
        /// Запрещены ли изменения параметров операции
        /// </summary>
        protected bool Locked
        {
            get;
            private set;
        }

        protected void RaiseBeforeExecuteEvent()
        {
            if (BeforeExchangeExecute != null)
            {
                BeforeExchangeExecute(this, EventArgs.Empty);
            }
        }

        protected void RaiseAfterExecuteEvent()
        {
            if (AfterExchangeExecute != null)
            {
                AfterExchangeExecute(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Помечена ли текущая операция обмена атрибутом SubExchangeAttribute, как подоперация
        /// </summary>
        public bool IsMarkedSubExchange
        {
            get
            {
                var type = GetType();
                var attrs = type.GetCustomAttributes(typeof(SubExchangeAttribute), true);
                if (attrs.Length > 0)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Создать новый объект параметров для операции обмена указанного типа
        /// </summary>
        /// <param name="spaceToAssign">ObjectSpace</param>
        /// <returns>Новый объект параметров с идентичным ObjectSpace</returns>
        /// <exception cref="System.ArgumentNullException" />
        /// <exception cref="System.ArgumentException" />
        public static ExchangeParameters CreateParameters(Type type, IObjectSpace spaceToAssign)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (spaceToAssign == null)
            {
                throw new ArgumentNullException("spaceToAssign");
            }

            if (!typeof(ExchangeOperation).IsAssignableFrom(type))
            {
                throw new ArgumentException("specified type is not exchange operation");
            }

            var obj = (ExchangeParameters)Activator.CreateInstance(GetParametersType(type));
            obj.ObjectSpace = spaceToAssign;
            return obj;
        }

        /// <summary>
        /// Создать новый объект параметров для операции
        /// </summary>
        /// <returns>Новый объект параметров</returns>
        /// <exception cref="System.ArgumentNullException" />
        public ExchangeParameters CreateParameters(IObjectSpace space)
        {
            if (space == null)
            {
                throw new ArgumentNullException("space");
            }
            return CreateParameters(this.GetType(), space);
        }

        public static Type GetParametersType(Type type)
        {
            Type parType = null;
            var intfType = type.GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IWithParametersType<>)).FirstOrDefault();
            if (intfType != null)
            {
                // Тип задан параметром интерфейса
                parType = intfType.GetGenericArguments()[0];
            }
            else
            {
                // Тип задан атрибутом
                var ptAttr = type.GetCustomAttributes(typeof(ParametersTypeAttribute), false);
                if (ptAttr.Length == 0)
                {
                    return null;
                }
                parType = (ptAttr[0] as ParametersTypeAttribute).Type;
            }

            return parType;
        }

        /// <summary>
        /// Освободить ресурсы
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Создать новую операцию обмена
        /// </summary>
        /// <exception cref="ArgumentNullException" />
        public ExchangeOperation(XafApplication application)
        {
            Application = application;
        }
    }
}
