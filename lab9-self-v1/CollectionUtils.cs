using System;
using System.Collections.Generic;

namespace CollectionUtilsDemo
{
    public static class CollectionUtils
    {
        public static IEnumerable<TResult> Map<TSource, TResult>(
            IEnumerable<TSource> collection, Func<TSource, TResult> mapper)
        {
            ArgumentNullException.ThrowIfNull(collection);
            ArgumentNullException.ThrowIfNull(mapper);
            var result = new List<TResult>();
            foreach (TSource item in collection)
            {
                result.Add(mapper(item));
            }
            return result;
        }

        public static T MaxBy<T, TKey>(IEnumerable<T> collection,
            Func<T, TKey> keySelector) where TKey : IComparable<TKey>
        {
            ArgumentNullException.ThrowIfNull(collection);
            ArgumentNullException.ThrowIfNull(keySelector);
            using IEnumerator<T> enumerator = collection.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                throw new InvalidOperationException("Колекція порожня.");
            }

            T maxItem = enumerator.Current;
            TKey maxKey = keySelector(maxItem);
            while (enumerator.MoveNext())
            {
                TKey key = keySelector(enumerator.Current);
                if (Comparer<TKey>.Default.Compare(key, maxKey) > 0)
                {
                    maxKey = key;
                    maxItem = enumerator.Current;
                }
            }
            return maxItem;
        }

        public static IEnumerable<T> Filter<T>(IEnumerable<T> collection, Predicate<T> predicate)
        {
            ArgumentNullException.ThrowIfNull(collection);
            ArgumentNullException.ThrowIfNull(predicate);
            var result = new List<T>();
            foreach (T item in collection)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static void ForEach<T>(IEnumerable<T> collection, Action<T> action)
        {
            ArgumentNullException.ThrowIfNull(collection);
            ArgumentNullException.ThrowIfNull(action);
            foreach (T item in collection)
            {
                action(item);
            }
        }
    }
}
