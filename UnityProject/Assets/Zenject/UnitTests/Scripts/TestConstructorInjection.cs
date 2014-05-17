using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestConstructorInjection : TestWithContainer
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
        public void TestCase1()
        {
            _container.Bind<Test2>().ToSingle();
            _container.Bind<Test1>().ToSingle();

            _container.ValidateResolve<Test2>();
            var test1 = _container.Resolve<Test2>();

            TestAssert.That(test1.val != null);
        }

        [Test]
        public void TestConstructByFactory()
        {
            _container.Bind<Test2>().ToSingle();

            var val = new Test1();
            var factory = new Factory<Test2>(_container);
            var test1 = factory.Create(val);

            TestAssert.That(test1.val == val);
        }
    }
}


