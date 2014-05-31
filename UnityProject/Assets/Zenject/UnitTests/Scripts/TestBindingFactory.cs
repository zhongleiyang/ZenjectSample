using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestBindingFactory : TestWithContainer
    {
        class Test1
        {
            public int Value;

            public Test1(int val)
            {
                Value = val;
            }
        }

        class Test2
        {
            [Inject] public Test1 val1 = null;
        }

        class Test1Factory : IFactory<Test1>
        {
            public Test1 Create(params object[] constructorArgs)
            {
                return new Test1(5);
            }
        }

        [Test]
        public void TestFactory1()
        {
            _container.Bind<Test1Factory>().ToSingle();
            _container.Bind<Test2>().ToSingle();
            _container.Bind<Test1>().ToFactory<Test1Factory>();

            TestAssert.That(_container.ValidateResolve<Test2>().IsEmpty());
            var test1 = _container.Resolve<Test2>();

            TestAssert.That(test1 != null);
            TestAssert.That(test1.val1.Value == 5);
        }
    }
}

