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

            TestAssert.That(_container.ValidateResolve<Test0>().IsEmpty());
            var test1 = _container.Resolve<Test0>();

            TestAssert.That(_container.ValidateResolve<Test1>().IsEmpty());
            var test2 = _container.Resolve<Test1>();

            TestAssert.That(ReferenceEquals(test1, test2));
        }
    }
}


