using System;
using System.Collections.Generic;
using System.Linq;

namespace ModestTree.Zenject.Test
{
    public static class TestUtil
    {
        public static bool ListsContainSameElements<T>(
            List<T> listA, List<T> listB)
        {
            // We don't care how they are sorted as long as they are sorted the same way so just use hashcode
            Comparison<T> comparer = (T left, T right) => (left.GetHashCode().CompareTo(right.GetHashCode()));

            listA.Sort(comparer);
            listB.Sort(comparer);

            return Enumerable.SequenceEqual(listA, listB);
        }

        public static string PrintList<T>(List<T> list)
        {
            return string.Join(",", list.Select(x => x.ToString()).ToArray());
        }
    }
}
