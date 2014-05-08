using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class Test3 : TestWithContainer
    {
        private class Test1
        {
        }

        private class Test2
        {
            public Test1 val;

            public Test2(Test1 val)
            {
                this.val = val;
            }
        }

        [Test]
        public void TestConstructorInjection()
        {
            _container.Bind<Test2>().ToSingle();
            _container.Bind<Test1>().ToSingle();

            var test1 = _container.Resolve<Test2>();

            Assert.That(test1.val != null);
        }

        [Test]
        public void TestConstructByFactory()
        {
            _container.Bind<Test2>().ToSingle();

            var val = new Test1();
            var factory = new Factory<Test2>(_container);
            var test1 = factory.Create(val);

            Assert.That(test1.val == val);
        }
    }
}


