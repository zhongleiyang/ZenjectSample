using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;
using System.Linq;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestTestUtil
    {
        [Test]
        public void TestTrue()
        {
            TestAssert.IsTrue(TestUtil.ListsContainSameElements(
                new List<int> {1},
                new List<int> {1}));

            TestAssert.IsTrue(TestUtil.ListsContainSameElements(
                new List<int> {1, 2},
                new List<int> {2, 1}));

            TestAssert.IsTrue(TestUtil.ListsContainSameElements(
                new List<int> {1, 2, 3},
                new List<int> {3, 2, 1}));

            TestAssert.IsTrue(TestUtil.ListsContainSameElements(
                new List<int> {},
                new List<int> {}));
        }

        [Test]
        public void TestFalse()
        {
            TestAssert.IsFalse(TestUtil.ListsContainSameElements(
                new List<int> {1, 2, 3},
                new List<int> {3, 2, 3}));

            TestAssert.IsFalse(TestUtil.ListsContainSameElements(
                new List<int> {1, 2},
                new List<int> {1, 2, 3}));
        }
    }
}




