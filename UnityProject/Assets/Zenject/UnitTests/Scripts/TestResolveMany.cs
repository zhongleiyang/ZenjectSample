using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestResolveMany : TestWithContainer
    {
        class Test0
        {
        }

        class Test1 : Test0
        {
        }

        class Test2 : Test0
        {
        }

        [Test]
        public void TestCase1()
        {
            _container.Bind<Test0>().ToSingle<Test1>();
            _container.Bind<Test0>().ToSingle<Test2>();

            List<Test0> many = _container.ResolveMany<Test0>();

            TestAssert.That(many.Count == 2);
        }

        [Test]
        public void TestCase2()
        {
            List<Test0> many = _container.ResolveMany<Test0>();
            TestAssert.That(many.Count == 0);
        }
    }
}


