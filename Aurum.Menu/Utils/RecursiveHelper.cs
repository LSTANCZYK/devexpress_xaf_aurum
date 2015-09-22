using DevExpress.Persistent.Base.General;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aurum.Menu.Utils
{
    public class RecursiveHelper
    {
        public enum CycleExeptionMode
        {
            RecursionCycleExeption,
            None,
            Skip
        }
        internal class RecursionParameters<TContext, TItem> : IDisposable
        {
            public Func<TContext, TItem, IEnumerable<TItem>> GetChidlren;
            public Func<TContext, IEnumerable<TItem>, bool> BeforeListFunc;
            public Func<TContext, TItem, bool> BeforeFunc;
            public Func<TContext, TItem, TContext> Action;
            public Action<TContext, TItem> AfterAction;
            public Action<TContext, IEnumerable<TItem>> AfterListAction;
            public Dictionary<TItem, bool> CycleItems;
            public RecursiveHelper.CycleExeptionMode CycleExeptionMode;
            public void Dispose()
            {
                this.GetChidlren = null;
                this.BeforeListFunc = null;
                this.BeforeFunc = null;
                this.Action = null;
                this.AfterAction = null;
                this.AfterListAction = null;
                this.CycleItems.Clear();
                this.CycleItems = null;
            }
        }
        internal static void RecursiveInternal<TItem, TContext>(TContext context, IEnumerable<TItem> list, RecursiveHelper.RecursionParameters<TContext, TItem> recursionParameters)
        {
            if (list == null)
            {
                return;
            }
            bool flag = true;
            if (list.Intersect(recursionParameters.CycleItems.Keys).Any<TItem>() && recursionParameters.CycleExeptionMode == RecursiveHelper.CycleExeptionMode.RecursionCycleExeption)
            {
                throw new RecursionCycleExeption(recursionParameters.CycleItems.Keys.ToArray<TItem>());
            }
            if (recursionParameters.BeforeListFunc != null)
            {
                flag = recursionParameters.BeforeListFunc(context, list);
            }
            if (flag)
            {
                foreach (TItem current in list)
                {
                    if (recursionParameters.CycleExeptionMode != RecursiveHelper.CycleExeptionMode.None && !recursionParameters.CycleItems.ContainsKey(current))
                    {
                        recursionParameters.CycleItems.Add(current, true);
                    }
                    bool flag2 = true;
                    if (recursionParameters.BeforeFunc != null)
                    {
                        flag2 = recursionParameters.BeforeFunc(context, current);
                    }
                    if (flag2)
                    {
                        TContext tContext = recursionParameters.Action(context, current);
                        TContext arg_167_0 = tContext;
                        IEnumerable<TItem> arg_167_1;
                        if (recursionParameters.CycleExeptionMode == RecursiveHelper.CycleExeptionMode.Skip)
                        {
                            arg_167_1 =
                                from a in recursionParameters.GetChidlren(tContext, current)
                                where !recursionParameters.CycleItems.ContainsKey(a)
                                select a;
                        }
                        else
                        {
                            arg_167_1 = recursionParameters.GetChidlren(tContext, current);
                        }
                        RecursiveHelper.RecursiveInternal<TItem, TContext>(arg_167_0, arg_167_1, recursionParameters);
                    }
                    if (recursionParameters.CycleExeptionMode != RecursiveHelper.CycleExeptionMode.None && recursionParameters.CycleItems.ContainsKey(current))
                    {
                        recursionParameters.CycleItems.Remove(current);
                    }
                    if (recursionParameters.AfterAction != null)
                    {
                        recursionParameters.AfterAction(context, current);
                    }
                }
            }
            if (recursionParameters.AfterListAction != null)
            {
                recursionParameters.AfterListAction(context, list);
            }
        }

        public static void Recursive<TItem, TContext>(TContext context, IEnumerable<TItem> list, Func<TContext, TItem, IEnumerable<TItem>> getChidlren, Func<TContext, IEnumerable<TItem>, bool> beforeListFunc, Func<TContext, TItem, bool> beforeFunc, Func<TContext, TItem, TContext> action, Action<TContext, TItem> afterAction, Action<TContext, IEnumerable<TItem>> afterListAction, RecursiveHelper.CycleExeptionMode cycleExeptionMode = RecursiveHelper.CycleExeptionMode.RecursionCycleExeption)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            if (getChidlren == null)
            {
                throw new ArgumentNullException("getChidlren");
            }
            Dictionary<TItem, bool> cycleItems = new Dictionary<TItem, bool>();
            using (RecursiveHelper.RecursionParameters<TContext, TItem> recursionParameters = new RecursiveHelper.RecursionParameters<TContext, TItem>
            {
                GetChidlren = getChidlren,
                BeforeListFunc = beforeListFunc,
                BeforeFunc = beforeFunc,
                Action = action,
                AfterAction = afterAction,
                AfterListAction = afterListAction,
                CycleItems = cycleItems,
                CycleExeptionMode = cycleExeptionMode
            })
            {
                RecursiveHelper.RecursiveInternal<TItem, TContext>(context, list, recursionParameters);
            }
        }

        public static void Recursive<TItem, TContext>(TContext context, IEnumerable<TItem> list, Func<TContext, TItem, IEnumerable<TItem>> getChidlren, Action<TContext, IEnumerable<TItem>> beforeListAction, Action<TContext, TItem> beforeAction, Func<TContext, TItem, TContext> action, Action<TContext, TItem> afterAction, Action<TContext, IEnumerable<TItem>> afterListAction)
        {
            RecursiveHelper.Recursive<TItem, TContext>(context, list, getChidlren, (beforeListAction == null) ? (Func<TContext, IEnumerable<TItem>, bool>)null : delegate(TContext a, IEnumerable<TItem> b)
            {
                beforeListAction(a, b);
                return true;
            }, (beforeAction == null) ? (Func<TContext, TItem, bool>)null : delegate(TContext a, TItem b)
            {
                beforeAction(a, b);
                return true;
            }, action, afterAction, afterListAction, RecursiveHelper.CycleExeptionMode.RecursionCycleExeption);
        }

        public static void Recursive<TItem>(IEnumerable<TItem> list, Func<TItem, IEnumerable<TItem>> getChidlren, Func<IEnumerable<TItem>, bool> beforeListFunc, Func<TItem, bool> beforeFunc, Action<TItem> action, Action<TItem> afterAction, Action<IEnumerable<TItem>> afterListAction)
        {
            if (getChidlren == null)
            {
                throw new ArgumentNullException("getChidlren");
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            RecursiveHelper.Recursive<TItem, object>(null, list, (object context, TItem a) => getChidlren(a), (beforeListFunc == null) ? (Func<object, IEnumerable<TItem>, bool>)null : ((object context, IEnumerable<TItem> a) => beforeListFunc(a)), (beforeFunc == null) ? (Func<object, TItem, bool>)null : ((object context, TItem a) => beforeFunc(a)), delegate(object context, TItem item)
            {
                action(item);
                return context;
            }, (afterAction == null) ? (Action<object, TItem>)null : delegate(object context, TItem a)
            {
                afterAction(a);
            }, (afterListAction == null) ? (Action<object, IEnumerable<TItem>>)null : delegate(object context, IEnumerable<TItem> a)
            {
                afterListAction(a);
            }, RecursiveHelper.CycleExeptionMode.RecursionCycleExeption);
        }

        public static void Recursive<TItem>(IEnumerable<TItem> list, Func<TItem, IEnumerable<TItem>> getChidlren, Action<IEnumerable<TItem>> beforeListAction, Action<TItem> beforeAction, Action<TItem> action, Action<TItem> afterAction, Action<IEnumerable<TItem>> afterListAction)
        {
            if (getChidlren == null)
            {
                throw new ArgumentNullException("getChidlren");
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            RecursiveHelper.Recursive<TItem, object>(null, list, (object context, TItem a) => getChidlren(a), (beforeListAction == null) ? (Action<object, IEnumerable<TItem>>)null : delegate(object context, IEnumerable<TItem> a)
            {
                beforeListAction(a);
            }, (beforeAction == null) ? (Action<object, TItem>)null : delegate(object context, TItem a)
            {
                beforeAction(a);
            }, delegate(object context, TItem item)
            {
                action(item);
                return context;
            }, (afterAction == null) ? (Action<object, TItem>)null : delegate(object context, TItem a)
            {
                afterAction(a);
            }, (afterListAction == null) ? (Action<object, IEnumerable<TItem>>)null : delegate(object context, IEnumerable<TItem> a)
            {
                afterListAction(a);
            });
        }

        public static void Recursive<TItem, TContext>(TContext context, IEnumerable<TItem> list, Func<TContext, TItem, IEnumerable<TItem>> getChidlren, Func<TContext, TItem, TContext> action)
        {
            RecursiveHelper.Recursive<TItem, TContext>(context, list, getChidlren, null, null, action, null, null, RecursiveHelper.CycleExeptionMode.RecursionCycleExeption);
        }

        public static void Recursive<TItem>(IEnumerable<TItem> list, Func<TItem, IEnumerable<TItem>> getChidlren, Action<TItem> action)
        {
            RecursiveHelper.Recursive<TItem>(list, getChidlren, (Func<IEnumerable<TItem>, bool>)null, (Func<TItem, bool>)null, action, (Action<TItem>)null, (Action<IEnumerable<TItem>>)null);
        }

        public static void Recursive<TItem, TContext>(TContext context, TItem root, Func<TContext, TItem, IEnumerable<TItem>> getChidlren, Func<TContext, TItem, TContext> action)
        {
            RecursiveHelper.Recursive<TItem, TContext>(context, new TItem[]
			{
				root
			}, getChidlren, action);
        }

        public static void Recursive<TItem>(TItem root, Func<TItem, IEnumerable<TItem>> getChidlren, Action<TItem> action)
        {
            RecursiveHelper.Recursive<TItem>(new TItem[]
			{
				root
			}, getChidlren, action);
        }

        public static void ITreeNodeRecursive<TContext>(TContext context, IEnumerable<ITreeNode> list, Func<TContext, ITreeNode, TContext> action)
        {
            RecursiveHelper.Recursive<ITreeNode, TContext>(context, list, delegate(TContext context2, ITreeNode item)
            {
                if (item.Children != null)
                {
                    return item.Children.Cast<ITreeNode>();
                }
                return null;
            }, action);
        }

        public static void ITreeNodeRecursive(IEnumerable<ITreeNode> list, Action<ITreeNode> action)
        {
            RecursiveHelper.Recursive<ITreeNode>(list, delegate(ITreeNode item)
            {
                if (item.Children != null)
                {
                    return item.Children.Cast<ITreeNode>();
                }
                return null;
            }, action);
        }

        public static TItem RecursiveFind<TItem>(IEnumerable<TItem> list, Func<TItem, IEnumerable<TItem>> getChidlren, Func<TItem, bool> predicate) where TItem : class
        {
            TItem found = default(TItem);
            RecursiveHelper.Recursive<TItem>(list, getChidlren, (IEnumerable<TItem> item) => found == null, (TItem item) => found == null, delegate(TItem item)
            {
                IEnumerable<TItem> enumerable = getChidlren(item);
                found = ((enumerable == null) ? default(TItem) : enumerable.FirstOrDefault(predicate));
            }, null, null);
            return found;
        }

        public static TItem RecursiveFind<TItem>(TItem root, Func<TItem, IEnumerable<TItem>> getChidlren, Func<TItem, bool> predicate) where TItem : class
        {
            return RecursiveHelper.RecursiveFind<TItem>(new TItem[]
			{
				root
			}, getChidlren, predicate);
        }

        public static TItem FindByPath<TItem, TPathItem>(IList<TPathItem> path, IEnumerable<TItem> list, Func<TItem, IEnumerable<TItem>> getChidlren, Func<TItem, TPathItem, bool> predicate) where TItem : class
        {
            if (getChidlren == null)
            {
                throw new ArgumentNullException("getChidlren");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            TItem found = default(TItem);
            if (path == null || path.Count == 0)
            {
                return default(TItem);
            }
            (
                from a in list
                where predicate(a, path[0])
                select a).Recursive(path, delegate(IList<TPathItem> path2, TItem a)
                {
                    if (path2.Count <= 0)
                    {
                        return null;
                    }
                    return
                        from b in getChidlren(a)
                        where predicate(b, path2[0])
                        select b;
                }, (IList<TPathItem> path2, IEnumerable<TItem> a) => found == null, (IList<TPathItem> path2, TItem a) => found == null, delegate(IList<TPathItem> path2, TItem a)
                {
                    if (path2.Count == 1)
                    {
                        found = a;
                    }
                    return path2.Skip(1).ToArray<TPathItem>();
                }, null, null);
            return found;
        }

        public static TItem FindByPath<TItem, TPathItem>(IList<TPathItem> path, TItem root, Func<TItem, IEnumerable<TItem>> getChidlren, Func<TItem, TPathItem, bool> predicate) where TItem : class
        {
            return RecursiveHelper.FindByPath<TItem, TPathItem>(path, new TItem[]
			{
				root
			}, getChidlren, predicate);
        }

        public static TItem FindByPath<TItem>(string pathStr, IEnumerable<TItem> list, Func<TItem, IEnumerable<TItem>> getChidlren, Func<TItem, string, bool> predicate) where TItem : class
        {
            if (string.IsNullOrEmpty(pathStr))
            {
                return default(TItem);
            }
            List<string> path = pathStr.Split(new char[]
			{
				'/',
				'\\'
			}).ToList<string>();
            return RecursiveHelper.FindByPath<TItem, string>(path, list, getChidlren, predicate);
        }

        public static TItem FindByPath<TItem>(string pathStr, TItem root, Func<TItem, IEnumerable<TItem>> getChidlren, Func<TItem, string, bool> predicate) where TItem : class
        {
            return RecursiveHelper.FindByPath<TItem>(pathStr, new TItem[]
			{
				root
			}, getChidlren, predicate);
        }

        public static ITreeNode FindByPath(string pathStr, ITreeNode root)
        {
            return RecursiveHelper.FindByPath<ITreeNode>(pathStr, root, delegate(ITreeNode a)
            {
                if (a.Children != null)
                {
                    return a.Children.Cast<ITreeNode>();
                }
                return null;
            }, (ITreeNode a, string pathItem) => a.Name == pathItem);
        }

        public static IList<TItem> ToList<TItem>(IEnumerable<TItem> list, Func<TItem, IEnumerable<TItem>> getChidlren)
        {
            return RecursiveHelper.ToList<TItem>(list, getChidlren, (TItem a) => true);
        }

        public static IList<TItem> ToList<TItem>(IEnumerable<TItem> list, Func<TItem, IEnumerable<TItem>> getChidlren, Func<TItem, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            List<TItem> result = new List<TItem>();
            RecursiveHelper.Recursive<TItem>(list, getChidlren, delegate(TItem a)
            {
                if (predicate(a))
                {
                    result.Add(a);
                }
            });
            return result;
        }

        public static IList<TItem> ToList<TItem>(TItem root, Func<TItem, IEnumerable<TItem>> getChidlren)
        {
            return RecursiveHelper.ToList<TItem>(new TItem[]
			{
				root
			}, getChidlren);
        }
    }
}
