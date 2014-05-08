using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class Test6 : TestWithContainer
    {
        private class Test1
        {
            public int f1;
            public int f2;

            public Test1(int f1, int f2)
            {
                this.f1 = f1;
                this.f2 = f2;
            }
        }

        [Test]
        public void TestExtraParametersSameType()
        {
            var factory1 = new Factory<Test1>(_container);
            var test1 = factory1.Create(5, 10);

            Assert.That(test1 != null);
            Assert.That(test1.f1 == 5 && test1.f2 == 10);

            var factory2 = new Factory<Test1>(_container);
            var test2 = factory2.Create(10, 5);

            Assert.That(test2 != null);
            Assert.That(test2.f1 == 10 && test2.f2 == 5);
        }

        [Test]
        [ExpectedException]
        public void TestMissingParameterThrows()
        {
            _container.Bind<Test1>().ToTransient();
            _container.Resolve<Test1>();
        }
    }
}


