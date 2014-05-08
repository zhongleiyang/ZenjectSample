using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

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

            var test1 = _container.Resolve<Test1>();
            var test2 = _container.Resolve<Test1>();

            Assert.That(test1 != null && test2 != null);
            Assert.That(!ReferenceEquals(test1, test2));
        }
    }
}


