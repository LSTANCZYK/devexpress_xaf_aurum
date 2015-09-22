using Aurum.Operations;
using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Exchange
{
    public abstract class CompositeExchangeOperation : ExchangeOperationBase, ICompositeOperation
    {
        private IList<ExchangeItem> exchanges = new List<ExchangeItem>();

        /// <summary>
        /// Добавить обмен данными с указанным объектом параметров
        /// </summary>
        /// <typeparam name="T">Тип добавляемого обмена данными</typeparam>
        protected void AddExchange<T>() where T : ExchangeOperationBase, IOperation
        {
            exchanges.Add(new ExchangeItem
            {
                Type = typeof(T)
            });
        }

        protected override sealed Type ObjectType
        {
            get { return null; }
        }

        public void OnChainStart(OperationInterop interop)
        {
            RaiseBeforeExecuteEvent();
        }

        public void OnChainEnded()
        {
            RaiseAfterExecuteEvent();
        }

        public OperationInfo OperationInfo
        {
            get
            {
                if (Model == null)
                {
                    return OperationInfo.Empty;
                }
                return new OperationInfo { Name = Model.Name };
            }
        }

        public IOperation[] GetOperations()
        {
            var list = new List<IOperation>();

            for (int i = 0; i < exchanges.Count; ++i)
            {
                var exchangeType = exchanges[i];
                var exchangeInstance = (ExchangeOperationBase)Activator.CreateInstance(exchangeType.Type, Application);
                exchangeInstance.Parent = this;
                exchangeInstance.ParametersObject = this.ParametersObject;
                exchangeInstance.Index = i + 1;
                list.Add((IOperation)exchangeInstance);
            }

            return list.ToArray();
        }

        public virtual bool IsParallel
        {
            get;
            set;
        }

        public CompositeExchangeOperation(XafApplication app)
            : base(app)
        {
        }

        private class ExchangeItem
        {
            public Type Type { get; set; }
        }
    }

    public abstract class CompositeExchangeOperation<TParametersClass> : CompositeExchangeOperation,
        IWithParametersType<TParametersClass>
        where TParametersClass : ExchangeParameters
    {
        /// <summary>
        /// Объект параметров конкретно указанного типа TParametersClass
        /// </summary>
        public new TParametersClass ParametersObject
        {
            get { return (TParametersClass)base.ParametersObject; }
            set { base.ParametersObject = value; }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="app">Объект приложения</param>
        /// <exception cref="System.ArgumentNullException" />
        public CompositeExchangeOperation(XafApplication app)
            : base(app)
        {
        }
    }
}
