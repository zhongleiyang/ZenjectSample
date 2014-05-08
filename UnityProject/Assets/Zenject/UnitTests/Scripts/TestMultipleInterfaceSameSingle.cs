using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestMultipleInterfaceSameSingle : TestWithContainer
    {
        private interface ITest1
        {
        }

        private interface ITest2
        {
        }

        private class Test1 : ITest1, ITest2
        {
        }

        [Test]
        public void TestCase1()
        {
            _container.Bind<ITest1>().ToSingle<Test1>();
            _container.Bind<ITest2>().ToSingle<Test1>();

            var test1 = _container.Resolve<ITest1>();
            var test2 = _container.Resolve<ITest2>();

            Assert.That(ReferenceEquals(test1, test2));
        }
    }
}


