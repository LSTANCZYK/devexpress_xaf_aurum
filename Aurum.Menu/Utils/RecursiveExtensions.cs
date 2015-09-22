using DevExpress.Persistent.Base.General;
using System;
using System.Collections.Generic;

namespace Aurum.Menu.Utils
{
    public static class RecursiveExtensions
    {
        public static void Recursive<TItem, TContext>(this IEnumerable<TItem> list, TContext context, Func<TContext, TItem, IEnumerable<TItem>> getChidlren, Func<TContext, IEnumerable<TItem>, bool> beforeListFunc, Func<TContext, TItem, bool> beforeFunc, Func<TContext, TItem, TContext> action, Action<TContext, TItem> afterAction, Action<TContext, IEnumerable<TItem>> afterListAction) where TItem : class
        {
            RecursiveHelper.Recursive<TItem, TContext>(context, list, getChidlren, beforeListFunc, beforeFunc, action, afterAction, afterListAction, RecursiveHelper.CycleExeptionMode.RecursionCycleExeption);
        }

        public static void Recursive<TItem, TContext>(this IEnumerable<TItem> list, TContext context, Func<TContext, TItem, IEnumerable<TItem>> getChidlren, Action<TContext, IEnumerable<TItem>> beforeListAction, Action<TContext, TItem> beforeAction, Func<TContext, TItem, TContext> action, Action<TContext, TItem> afterAction, Action<TContext, IEnumerable<TItem>> afterListAction) where TItem : class
        {
            RecursiveHelper.Recursive<TItem, TContext>(context, list, getChidlren, beforeListAction, beforeAction, action, afterAction, afterListAction);
        }

        public static void Recursive<TItem>(this IEnumerable<TItem> list, Func<TItem, IEnumerable<TItem>> getChidlren, Action<IEnumerable<TItem>> beforeListAction, Action<TItem> beforeAction, Action<TItem> action, Action<TItem> afterAction, Action<IEnumerable<TItem>> afterListAction) where TItem : class
        {
            RecursiveHelper.Recursive<TItem>(list, getChidlren, beforeListAction, beforeAction, action, afterAction, afterListAction);
        }

        public static void Recursive<TItem>(this IEnumerable<TItem> list, Func<TItem, IEnumerable<TItem>> getChidlren, Func<IEnumerable<TItem>, bool> beforeListFunc, Func<TItem, bool> beforeFunc, Action<TItem> action, Action<TItem> afterAction, Action<IEnumerable<TItem>> afterListAction) where TItem : class
        {
            RecursiveHelper.Recursive<TItem>(list, getChidlren, beforeListFunc, beforeFunc, action, afterAction, afterListAction);
        }

        public static void Recursive<TItem, TContext>(this IEnumerable<TItem> list, TContext context, Func<TContext, TItem, IEnumerable<TItem>> getChidlren, Func<TContext, TItem, TContext> action) where TItem : class
        {
            RecursiveHelper.Recursive<TItem, TContext>(context, list, getChidlren, action);
        }

        public static void Recursive<TItem>(this IEnumerable<TItem> list, Func<TItem, IEnumerable<TItem>> getChidlren, Action<TItem> action) where TItem : class
        {
            RecursiveHelper.Recursive<TItem>(list, getChidlren, action);
        }

        public static void Recursive<TItem, TContext>(this TItem root, TContext context, Func<TContext, TItem, IEnumerable<TItem>> getChidlren, Func<TContext, TItem, TContext> action) where TItem : class
        {
            RecursiveHelper.Recursive<TItem, TContext>(context, root, getChidlren, action);
        }

        public static void Recursive<TItem>(this TItem root, Func<TItem, IEnumerable<TItem>> getChidlren, Action<TItem> action) where TItem : class
        {
            RecursiveHelper.Recursive<TItem>(root, getChidlren, action);
        }

        public static void Recursive<TContext>(this IEnumerable<ITreeNode> list, TContext context, Func<TContext, ITreeNode, TContext> action)
        {
            RecursiveHelper.ITreeNodeRecursive<TContext>(context, list, action);
        }

        public static void Recursive(this IEnumerable<ITreeNode> list, Action<ITreeNode> action)
        {
            RecursiveHelper.ITreeNodeRecursive(list, action);
        }

        public static TItem RecursiveFind<TItem>(this IEnumerable<TItem> list, Func<TItem, IEnumerable<TItem>> getChidlren, Func<TItem, bool> predicate) where TItem : class
        {
            return RecursiveHelper.RecursiveFind<TItem>(list, getChidlren, predicate);
        }

        public static TItem RecursiveFind<TItem>(TItem root, Func<TItem, IEnumerable<TItem>> getChidlren, Func<TItem, bool> predicate) where TItem : class
        {
            return RecursiveHelper.RecursiveFind<TItem>(root, getChidlren, predicate);
        }

        public static TItem FindByPath<TItem, TPathItem>(this IEnumerable<TItem> list, IList<TPathItem> path, Func<TItem, IEnumerable<TItem>> getChidlren, Func<TItem, TPathItem, bool> predicate) where TItem : class
        {
            return RecursiveHelper.FindByPath<TItem, TPathItem>(path, list, getChidlren, predicate);
        }

        public static TItem FindByPath<TItem, TPathItem>(this TItem root, IList<TPathItem> path, Func<TItem, IEnumerable<TItem>> getChidlren, Func<TItem, TPathItem, bool> predicate) where TItem : class
        {
            return RecursiveHelper.FindByPath<TItem, TPathItem>(path, root, getChidlren, predicate);
        }

        public static TItem FindByPath<TItem>(this IEnumerable<TItem> list, string pathStr, Func<TItem, IEnumerable<TItem>> getChidlren, Func<TItem, string, bool> predicate) where TItem : class
        {
            return RecursiveHelper.FindByPath<TItem>(pathStr, list, getChidlren, predicate);
        }

        public static TItem FindByPath<TItem>(this TItem root, string pathStr, Func<TItem, IEnumerable<TItem>> getChidlren, Func<TItem, string, bool> predicate) where TItem : class
        {
            return RecursiveHelper.FindByPath<TItem>(pathStr, root, getChidlren, predicate);
        }

        public static ITreeNode FindByPath(this ITreeNode root, string pathStr)
        {
            return RecursiveHelper.FindByPath(pathStr, root);
        }

        public static IList<TItem> ToList<TItem>(this IEnumerable<TItem> list, Func<TItem, IEnumerable<TItem>> getChidlren)
        {
            return RecursiveHelper.ToList<TItem>(list, getChidlren);
        }

        public static IList<TItem> ToList<TItem>(IEnumerable<TItem> list, Func<TItem, IEnumerable<TItem>> getChidlren, Func<TItem, bool> predicate)
        {
            return RecursiveHelper.ToList<TItem>(list, getChidlren, predicate);
        }

        public static IList<TItem> ToList<TItem>(this TItem root, Func<TItem, IEnumerable<TItem>> getChidlren)
        {
            return RecursiveHelper.ToList<TItem>(root, getChidlren);
        }
    }
}
