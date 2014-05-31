using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestMultiBindAgain : TestWithContainer
    {
        private class Test0
        {
        }

        private class Test3 : Test0
        {
        }

        private class Test4 : Test0
        {
        }

        private class Test2
        {
            public Test0 test;

            public Test2(Test0 test)
            {
                this.test = test;
            }
        }

        private class Test1
        {
            public List<Test0> test;

            public Test1(List<Test0> test)
            {
                this.test = test;
            }
        }

        [Test]
        [ExpectedException]
        public void TestMultiBind2()
        {
            // Multi-binds should not map to single-binds
            _container.Bind<Test0>().ToSingle<Test3>();
            _container.Bind<Test0>().ToSingle<Test4>();
            _container.Bind<Test2>().ToSingle();

            TestAssert.That(_container.ValidateResolve<Test2>().IsEmpty());
            _container.Resolve<Test2>();
        }
    }
}


