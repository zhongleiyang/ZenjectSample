using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

namespace ModestTree
{
    public static class LinqExtensions
    {
        public static Vector2 Average(this IEnumerable<Vector2> positions)
        {
            Vector2 sum = Vector2.zero;
            int count = 0;

            foreach (var pos in positions)
            {
                sum += pos;
                count += 1;
            }

            if (count == 0)
            {
                throw new InvalidOperationException("Cannot compute average for an empty set.");
            }

            return sum / count;
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> first, T item)
        {
            yield return item;

            foreach (T t in first)
            {
                yield return t;
            }
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> first, T item)
        {
            foreach (T t in first)
            {
                yield return t;
            }

            yield return item;
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            foreach (T t in second)
            {
                yield return t;
            }

            foreach (T t in first)
            {
                yield return t;
            }
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            foreach (T t in first)
            {
                yield return t;
            }

            foreach (T t in second)
            {
                yield return t;
            }
        }

        public static IEnumerable<T> ReplaceOrAppend<T>(this IEnumerable<T> enumerable, Predicate<T> match, T replacement)
        {
            bool replaced = false;

            foreach (T t in enumerable)
            {
                if (match(t))
                {
                    replaced = true;
                    yield return replacement;
                }
                else
                {
                    yield return t;
                }
            }

            if (!replaced)
            {
                yield return replacement;
            }
        }

        public static T GetSingle<T>(this object[] objectArray, bool required)
        {
            if (required)
            {
                return objectArray.Where(x => x is T).Cast<T>().Single();
            }
            else
            {
                return objectArray.Where(x => x is T).Cast<T>().SingleOrDefault();
            }
        }

        // These are more efficient than Count() in cases where the size of the collection is not known
        public static bool HasAtLeast<T>(this IEnumerable<T> enumerable, int amount)
        {
            return enumerable.Take(amount).Count() == amount;
        }

        public static bool HasMoreThan<T>(this IEnumerable<T> enumerable, int amount)
        {
            return enumerable.HasAtLeast(amount+1);
        }

        public static bool HasLessThan<T>(this IEnumerable<T> enumerable, int amount)
        {
            return enumerable.HasAtMost(amount-1);
        }

        public static bool HasAtMost<T>(this IEnumerable<T> enumerable, int amount)
        {
            return enumerable.Take(amount + 1).Count() <= amount;
        }

        // This is more efficient than just Count() < x because it will end early
        // rather than iterating over the entire collection
        public static bool IsLength<T>(this IEnumerable<T> enumerable, int amount)
        {
            return enumerable.Take(amount+1).Count() == amount;
        }

        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.Any();
        }

        public static IEnumerable<T> Duplicates<T>(this IEnumerable<T> list)
        {
            return list.GroupBy(x => x).Where(x => x.Skip(1).Any()).Select(x => x.Key);
        }

        public static T Second<T>(this IEnumerable<T> list)
        {
            return list.Skip(1).First();
        }

        public static T SecondOrDefault<T>(this IEnumerable<T> list)
        {
            return list.Skip(1).FirstOrDefault();
        }

        public static IEnumerable<T> FindDuplicates<T>(this IEnumerable<T> list)
        {
            return list.GroupBy(x => x).Where(x => x.Skip(1).Any()).Select(x => x.Key);
        }
    }
}
