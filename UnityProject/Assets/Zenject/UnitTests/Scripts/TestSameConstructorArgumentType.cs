using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestSameConstructorArgumentType : TestWithContainer
    {
        class Test1
        {
            public Test2 t1;
            public float f;
            public Test2 t2;

            public Test1(Test2 t1, float f, Test2 t2)
            {
                this.t1 = t1;
                this.f = f;
                this.t2 = t2;
            }
        }

        class Test2
        {
        }

        [Test]
        public void Test()
        {
            var t1 = new Test2();
            var t2 = new Test2();

            _container.Bind<Factory<Test1>>().ToSingle();

            var factory = _container.Resolve<Factory<Test1>>();

            var test = factory.Create(t1, 5.0f, t2);

            Assert.That(ReferenceEquals(test.t1, t1));
            Assert.That(ReferenceEquals(test.t2, t2));
        }
    }
}


