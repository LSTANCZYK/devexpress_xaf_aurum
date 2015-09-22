using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aurum.Operations
{
    /// <summary>
    /// Менеджер операций
    /// </summary>
    public class OperationManager
    {
        private IDictionary<int, OperationUnit> units = new Dictionary<int, OperationUnit>();
        private object units_lock = new object();

        private int operationCounter;

        private List<OperationObject> operationsList = new List<OperationObject>();
        private Dictionary<string, int> counters = new Dictionary<string, int>();
        private object counters_lock = new object();
        private object operationsList_lock = new object();

        private List<OperationObject> watchOperations = new List<OperationObject>();

        /// <summary>
        /// Список операций
        /// </summary>
        public List<OperationObject> Operations { get { return operationsList; } }

        /// <summary>
        /// Индекс счетчика элементов журнала
        /// </summary>
        public const string LOG_ITEM_COUNTER_ID = "logitem";

        /// <summary>
        /// Объект уведомления
        /// </summary>
        public static INotifier Notifier
        {
            private get;
            set;
        }

        /// <summary>
        /// Добавить объект представления операции в список
        /// </summary>
        /// <param name="obj">Объект представления операции</param>
        private void AddOperation(OperationObject obj)
        {
            lock (operationsList_lock)
            {
                operationsList.Add(obj);
            }
        }

        /// <summary>
        /// Удалить указанный объект представления операции из списка
        /// </summary>
        /// <param name="obj">Объект представления операции</param>
        private void RemoveOperation(OperationObject obj)
        {
            lock (operationsList_lock)
            {
                operationsList.Remove(obj);
            }
        }

        /// <summary>
        /// Добавить вспомогательный объект в список
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="unit">Объект</param>
        /// <param name="operations">Операции, добавляемые в объект</param>
        private void AddUnit(int key, OperationUnit unit, params IOperation[] operations)
        {
            lock (units_lock)
            {
                units.Add(key, unit);
                for (int i = 0; i < operations.Length; ++i)
                {
                    units[key].Operations.Add(operations[i]);
                }
            }
        }

        private int GetNewIndex()
        {
            Interlocked.Increment(ref operationCounter);
            return operationCounter;
        }

        /// <summary>
        /// Получить значение именованного счетчика
        /// </summary>
        /// <param name="id">Идентификатор счетчика</param>
        /// <returns>Значение указанного счетчика</returns>
        public int GetNewIndex(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException();
            }

            lock (counters_lock)
            {
                if (!counters.ContainsKey(id))
                {
                    counters[id] = 0;
                }
                return counters[id]++;
            }
        }

        private void UpdateMockOperationProgress(OperationObject obj, int childrenCount, int finishedCount)
        {
            if (finishedCount < childrenCount)
            {
                if (finishedCount > 0)
                {
                    var ratio = (float)finishedCount / (float)childrenCount;
                    var percents = Convert.ToInt32(Math.Round(ratio, 2) * 100);
                    obj.Progress = percents;
                }
            }
        }

        /// <summary>
        /// Запустить одну операцию
        /// </summary>
        /// <param name="action">Метод для выполнения</param>
        /// <param name="title">Название операции</param>
        /// <returns>Объект представления операции</returns>
        /// <exception cref="System.ArgumentNullException" />
        public OperationObject Run(Action<OperationInterop> action, string title = null)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            var operation = new SimpleOperation(action, title == null ? OperationInfo.Empty : new OperationInfo { Name = title });
            return Run(operation);
        }

        /// <summary>
        /// Запустить одну операцию
        /// </summary>
        /// <param name="operation">Операция</param>
        /// <exception cref="System.ArgumentNullException" />
        public OperationObject Run(IOperation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            // Инициализация
            var tokenSource = new CancellationTokenSource();
            var operationObj = new OperationObject { OperationId = GetNewIndex() };
            var interop = new OperationInterop(operationObj, tokenSource.Token);

            // Chain :: Выполнение операции
            var task = new Task(() =>
            {
                operationObj.Status = OperationStatus.Running;
                operation.Execute(interop);
            },
            tokenSource.Token);

            // Chain :: Обработка законченной операции
            var chainEnd = task.ContinueWith(et =>
            {
                operationObj.Exception = et.Exception;
                operationObj.Status = et.Status.ToOperationStatus();
                operationObj.Progress = 100;
                operation.Dispose();

                if (!IsBeingWatched(operationObj))
                {
                    Notifier.Notify("Операция #" + operationObj.OperationId, "Операция завершена", null);
                }
            });

            // Обработка и запуск цепочки выполнения
            AddOperation(operationObj);
            AddUnit(operationObj.OperationId, new OperationUnit { CancellationTokenSource = tokenSource, ChainEndTask = chainEnd }, operation);

            operationObj.Status = OperationStatus.Created;
            operationObj.Name = operation.OperationInfo.Name;
            task.Start();

            return operationObj;
        }

        /// <summary>
        /// Запустить составную операцию
        /// </summary>
        /// <param name="operation">Составная операция</param>
        /// <exception cref="System.ArgumentException" />
        public OperationObject Run(ICompositeOperation operation)
        {
            if (operation.IsParallel)
                return RunParallel(operation);
            else
                return RunSerial(operation);
        }

        /// <summary>
        /// Запуск последовательно
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        private OperationObject RunSerial(ICompositeOperation operation)
        {
            var operations = operation.GetOperations();
            if (operations == null || operations.Length == 0)
            {
                throw new ArgumentException("No any operation to run ", "consequentOperations");
            }

            // Инициализация
            int n = operations.Length;

            var tokenSource = new CancellationTokenSource();
            var commonState = new InteropState();

            var mockOperationObj = new OperationObject { OperationId = GetNewIndex() };
            var mockInterop = new OperationInterop(mockOperationObj, tokenSource.Token, commonState);

            OperationObject[] conseqOperationObjects = new OperationObject[n];

            // Создание объектов представления операций
            for (int i = 0; i < n; ++i)
            {
                conseqOperationObjects[i] = new OperationObject
                {
                    Parent = mockOperationObj
                };
                mockOperationObj.Children.Add(conseqOperationObjects[i]);
            }

            Task chainEnd = null;
            Task firstTask = null;
            bool finalSet = false;

            // Создание цепочки выполнения
            for (int i = 0; i < n; ++i)
            {
                // --> Переменные для замыкания
                var j = i;
                var thisOperation = operations[i];
                var thisOperationObj = conseqOperationObjects[i];
                var thisInterop = new OperationInterop(thisOperationObj, tokenSource.Token, commonState);
                // <--

                thisOperationObj.OperationId = GetNewIndex();
                thisOperationObj.Name = thisOperation.OperationInfo.Name;

                // Chain :: Выполнение операции
                if (chainEnd == null)
                {
                    // Первая операция
                    chainEnd = new Task(() =>
                    {
                        mockOperationObj.Status = OperationStatus.Running;
                        operation.OnChainStart(mockInterop);
                        thisOperation.Execute(thisInterop);
                    },
                    tokenSource.Token);
                    firstTask = chainEnd;
                }
                else
                {
                    // Цепляем следующие
                    chainEnd = chainEnd.ContinueWith((_, __) =>
                    {
                        tokenSource.Token.ThrowIfCancellationRequested();

                        thisOperation.Execute(thisInterop);
                    },
                    null, tokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Current);
                }

                // Chain :: Обработка законченной операции
                chainEnd = chainEnd.ContinueWith(et =>
                {
                    thisOperation.Dispose();

                    // Установка статуса собственной операции
                    thisOperationObj.Status = et.Status.ToOperationStatus();

                    // Установка статуса главной репрезентативной операции
                    if (!finalSet)
                    {
                        if (et.Status != TaskStatus.RanToCompletion)
                        {
                            // Любая операция завершилась неуспешно
                            mockOperationObj.Status = et.Status.ToOperationStatus();

                            thisOperationObj.Exception = et.Exception;
                            mockOperationObj.Exception = et.Exception;

                            if (!IsBeingWatched(mockOperationObj))
                            {
                                Notifier.Notify("Операция #" + mockOperationObj.OperationId, "Операция не завершена", null);
                            }

                            tokenSource.Cancel();
                            finalSet = true;
                        }
                        else if (j == n - 1)
                        {
                            // Последняя операция завершилась успешно
                            mockOperationObj.Status = OperationStatus.RanToCompletion;
                            mockOperationObj.Progress = 100;

                            if (!IsBeingWatched(mockOperationObj))
                            {
                                Notifier.Notify("Операция #" + mockOperationObj.OperationId, "Операция завершена", null);
                            }

                            tokenSource.Cancel();
                            finalSet = true;
                        }
                    }

                    if (j < n - 1)
                    {
                        // Установка прогресса главной репрезентативной операции
                        UpdateMockOperationProgress(mockOperationObj, n, j + 1);
                    }
                });

                thisOperationObj.Status = OperationStatus.Created;
            }

            chainEnd.ContinueWith(t =>
            {
                operation.OnChainEnded();
                operation.Dispose();
            });

            // Обработка и запуск выполнения цепочки
            mockOperationObj.Status = OperationStatus.Created;
            mockOperationObj.Added = DateTime.Now;
            mockOperationObj.Name = operation.OperationInfo.Name;

            AddOperation(mockOperationObj);
            AddUnit(mockOperationObj.OperationId, new OperationUnit { CancellationTokenSource = tokenSource, ChainEndTask = chainEnd }, operations);

            firstTask.Start();

            return mockOperationObj;
        }

        /// <summary>
        /// Запуск параллельно
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        private OperationObject RunParallel(ICompositeOperation operation)
        {
            var operations = operation.GetOperations();
            if (operations == null || operations.Length == 0)
            {
                throw new ArgumentException("No any operation to run ", "consequentOperations");
            }

            // Инициализация
            int n = operations.Length;

            var tokenSource = new CancellationTokenSource();
            var commonState = new InteropState();

            var mockOperationObj = new OperationObject { OperationId = GetNewIndex() };
            var mockInterop = new OperationInterop(mockOperationObj, tokenSource.Token, commonState);

            OperationObject[] parallelOperationObjects = new OperationObject[n];

            // Создание объектов представления операций
            for (int i = 0; i < n; ++i)
            {
                parallelOperationObjects[i] = new OperationObject
                {
                    Parent = mockOperationObj
                };
                mockOperationObj.Children.Add(parallelOperationObjects[i]);
            }

            List<Task> tasks = new List<Task>();

            int finishedCount = 0;
            bool totalSuccess = true;
            bool canceled = false;

            // Создание цепочки выполнения
            for (int i = 0; i < n; ++i)
            {
                // текущая задача
                Task task = null;

                // --> Переменные для замыкания
                var j = i;
                var thisOperation = operations[i];
                var thisOperationObj = parallelOperationObjects[i];
                var thisInterop = new OperationInterop(thisOperationObj, tokenSource.Token, commonState);
                // <--

                thisOperationObj.OperationId = GetNewIndex();
                thisOperationObj.Name = thisOperation.OperationInfo.Name;

                // Запуск операции
                task = new Task(() =>
                {
                    thisOperationObj.Status = OperationStatus.Running;
                    thisOperation.Execute(thisInterop);
                },
                tokenSource.Token);

                // Обработка законченной операции
                task.ContinueWith(et =>
                {
                    thisOperation.Dispose();

                    finishedCount++;

                    // Установка статуса собственной операции
                    thisOperationObj.Status = et.Status.ToOperationStatus();
                    if (et.Status != TaskStatus.RanToCompletion)
                    {
                        thisOperationObj.Exception = et.Exception;
                        totalSuccess = false;
                        if (et.Status == TaskStatus.Canceled)
                        {
                            canceled = true;
                            tokenSource.Cancel();
                        }
                    }

                    // Установка прогресса главной репрезентативной операции
                    UpdateMockOperationProgress(mockOperationObj, n, finishedCount);

                    // завершились все задачи
                    // Установка статуса главной репрезентативной операции
                    if (finishedCount == n)
                    {
                        if (totalSuccess)
                        {
                            // Последняя операция завершилась успешно
                            mockOperationObj.Status = OperationStatus.RanToCompletion;
                            mockOperationObj.Progress = 100;

                            if (!IsBeingWatched(mockOperationObj))
                            {
                                Notifier.Notify("Операция #" + mockOperationObj.OperationId, "Операция завершена", null);
                            }
                        }
                        else
                        {
                            mockOperationObj.Status = canceled ? OperationStatus.Canceled : OperationStatus.Faulted;
                            if (!IsBeingWatched(mockOperationObj))
                            {
                                Notifier.Notify("Операция #" + mockOperationObj.OperationId, "Операция не завершена", null);
                            }
                        }

                        tokenSource.Cancel();

                        operation.OnChainEnded();
                        operation.Dispose();
                    }
                });

                thisOperationObj.Status = OperationStatus.Created;
                tasks.Add(task);
            }

            // Обработка и запуск выполнения цепочки
            mockOperationObj.Status = OperationStatus.Created;
            mockOperationObj.Added = DateTime.Now;
            mockOperationObj.Name = operation.OperationInfo.Name;

            AddOperation(mockOperationObj);

            AddUnit(mockOperationObj.OperationId, new OperationUnit { CancellationTokenSource = tokenSource }, operations);

            operation.OnChainStart(mockInterop);
            mockOperationObj.Status = OperationStatus.Running;

            foreach (Task task in tasks)
            {
                task.Start();
            }

            return mockOperationObj;
        }

        /// <summary>
        /// Отменить текущее выполнение операции
        /// </summary>
        /// <param name="obj">Объект операции</param>
        /// <exception cref="System.ArgumentNullException" />
        /// <exception cref="System.InvalidOperationException" />
        /// <exception cref="System.ArgumentException" />
        /// <exception cref="System.AggregateException" />
        public void Cancel(OperationObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            Cancel(obj.OperationId);
        }

        /// <summary>
        /// Добавить объект представления в список наблюдения.
        /// Уведомления об операциях, чьи объекты представления находятся под наблюдением, не всплывают
        /// </summary>
        /// <param name="obj">Объект представления операции</param>
        public void Watch(OperationObject obj)
        {
            if (!watchOperations.Contains(obj))
            {
                watchOperations.Add(obj);
            }
        }

        /// <summary>
        /// Убрать объект представления из списка наблюдения.
        /// Для операции, чей объект представления не находится под наблюдением, всплывают уведомления
        /// </summary>
        /// <param name="obj">Объект представления операции</param>
        public void Unwatch(OperationObject obj)
        {
            if (watchOperations.Contains(obj))
            {
                watchOperations.Remove(obj);
            }
        }

        /// <summary>
        /// Находится ли объект представления операции под наблюдением
        /// </summary>
        /// <param name="obj">Объект представления операции</param>
        /// <returns>Статус нахождения объекта под наблюдением</returns>
        private bool IsBeingWatched(OperationObject obj)
        {
            return watchOperations.Contains(obj);
        }

        /// <summary>
        /// Отменить текущее выполнение операции
        /// </summary>
        /// <param name="operationId">Идентификатор операции</param>
        /// <exception cref="System.InvalidOperationException" />
        /// <exception cref="System.AggregateException" />
        /// <exception cref="System.ArgumentException" />
        public void Cancel(int operationId)
        {
            try
            {
                var unit = units[operationId];
                if (unit.IsDisposed)
                {
                    throw new ObjectDisposedException("unit");
                }
                unit.CancellationTokenSource.Cancel();
            }
            catch (System.Collections.Generic.KeyNotFoundException ex)
            {
                throw new ArgumentException("Операция не найдена", ex);
            }
            catch (ObjectDisposedException ex)
            {
                throw new InvalidOperationException("Операция уже закончена", ex);
            }
        }

        // Менеджер по умолчанию
        static Lazy<OperationManager> defaultProcessManager;

        /// <summary>
        /// Менеджер операций по умолчанию
        /// </summary>
        public static OperationManager Default
        {
            get { return defaultProcessManager.Value; }
        }

        static OperationManager()
        {
            defaultProcessManager = new Lazy<OperationManager>(() =>
            {
                return new OperationManager();
            });
            Notifier = new DummyNotifier();
        }
    }

    /// <summary>
    /// Вспомогательный класс, содержащий в себе данные главных операций.
    /// Для операции запущенной в Single-режиме: общий для всех TokenSource
    /// Для операции запущенной в Waterfall-режиме: общий для всех TokenSource, список дочерних операций
    /// </summary>
    internal class OperationUnit : IDisposableExt
    {
        public CancellationTokenSource CancellationTokenSource { get; set; }

        public bool IsDisposed
        {
            get;
            private set;
        }

        public IList<IOperation> Operations
        {
            get;
            private set;
        }

        public Task ChainEndTask
        {
            get;
            set;
        }

        public void Dispose()
        {
            CancellationTokenSource.Dispose();
            CancellationTokenSource = null;

            foreach (var op in Operations)
            {
                op.Dispose();
            }

            IsDisposed = true;
        }

        public OperationUnit()
        {
            Operations = new List<IOperation>();
        }
    }
}
