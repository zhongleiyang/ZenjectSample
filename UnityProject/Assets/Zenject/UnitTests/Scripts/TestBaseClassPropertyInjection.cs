using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestBaseClassPropertyInjection : TestWithContainer
    {
        class Test0
        {
        }

        class Test3
        {
        }

        class Test1 : Test3
        {
            [Inject] protected Test0 val = null;

            public Test0 GetVal()
            {
                return val;
            }
        }

        class Test2 : Test1
        {
        }

        [Test]
        public void TestCaseBaseClassPropertyInjection()
        {
            _container.Bind<Test0>().ToSingle();
            _container.Bind<Test2>().ToSingle();

            var test1 = _container.Resolve<Test2>();

            Assert.That(test1.GetVal() != null);
        }
    }
}


