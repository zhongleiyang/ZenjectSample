using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestStructInjection : TestWithContainer
    {
        private struct Test1
        {
        }

        private class Test2
        {
            public Test2(Test1 t1)
            {
            }
        }

        [Test]
        public void TestCase1()
        {
            _container.BindValue<Test1>().To(new Test1());
            _container.Bind<Test2>().ToSingle();

            var t2 = _container.Resolve<Test2>();

            Assert.That(t2 != null);
        }
    }
}

