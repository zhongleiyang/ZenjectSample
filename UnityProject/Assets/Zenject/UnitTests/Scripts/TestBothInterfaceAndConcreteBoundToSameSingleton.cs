using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestBothInterfaceAndConcreteBoundToSameSingleton : TestWithContainer
    {
        private abstract class Test0
        {
        }

        private class Test1 : Test0
        {
        }

        [Test]
        public void TestCaseBothInterfaceAndConcreteBoundToSameSingleton()
        {
            _container.Bind<Test0>().ToSingle<Test1>();
            _container.Bind<Test1>().ToSingle();

            _container.ValidateResolve<Test0>();
            var test1 = _container.Resolve<Test0>();

            _container.ValidateResolve<Test1>();
            var test2 = _container.Resolve<Test1>();

            TestAssert.That(ReferenceEquals(test1, test2));
        }
    }
}


