using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class Test7 : TestWithContainer
    {
        private class Test0
        {
        }
        private class Test3
        {
        }

        private class Test1 : Test3
        {
            [Inject] protected Test0 val = null;

            public Test0 GetVal()
            {
                return val;
            }
        }

        private class Test2 : Test1
        {
        }

        [Test]
        public void TestBaseClassPropertyInjection()
        {
            _container.Bind<Test0>().ToSingle();
            _container.Bind<Test2>().ToSingle();

            var test1 = _container.Resolve<Test2>();

            Assert.That(test1.GetVal() != null);
        }
    }
}


