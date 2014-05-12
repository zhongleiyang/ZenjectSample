using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestFieldInjection : TestWithContainer
    {
        class Test0
        {
            public float Foo = 2;
        }

        class Test1
        {
            [Inject]
            public Test0 Bar;
        }

        [Test]
        public void TestCaseFieldInjection()
        {
            _container.Bind<Test0>().ToSingle();
            _container.Bind<Test1>().ToSingle();

            var test1 = _container.Resolve<Test1>();

            TestAssert.AreEqual(test1.Bar.Foo, 2);
        }
    }
}



