using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestResolveMany : TestWithContainer
    {
        private class Test0
        {
        }

        private class Test1 : Test0
        {
        }

        private class Test2 : Test0
        {
        }

        [Test]
        public void TestCase1()
        {
            _container.Bind<Test0>().ToSingle<Test1>();
            _container.Bind<Test0>().ToSingle<Test2>();

            List<Test0> many = _container.ResolveMany<Test0>();

            Assert.That(many.Count == 2);
        }

        [Test]
        public void TestCase2()
        {
            List<Test0> many = _container.ResolveMany<Test0>();

            Assert.That(many.Count == 0);
        }
    }
}


