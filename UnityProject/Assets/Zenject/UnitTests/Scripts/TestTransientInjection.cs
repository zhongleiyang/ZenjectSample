using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestTransientInjection : TestWithContainer
    {
        private class Test1
        {
        }

        [Test]
        public void TestTransientType()
        {
            _container.Bind<Test1>().ToTransient();

            TestAssert.That(_container.ValidateResolve<Test1>().IsEmpty());

            var test1 = _container.Resolve<Test1>();
            var test2 = _container.Resolve<Test1>();

            TestAssert.That(test1 != null && test2 != null);
            TestAssert.That(!ReferenceEquals(test1, test2));
        }
    }
}


