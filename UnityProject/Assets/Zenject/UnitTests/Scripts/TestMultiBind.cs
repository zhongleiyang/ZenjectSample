using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestMultiBind : TestWithContainer
    {
        private class Test1
        {
        }

        private class Test2 : Test1
        {
        }

        private class Test3 : Test1
        {
        }

        private class TestImpl1
        {
            public List<Test1> tests;

            public TestImpl1(List<Test1> tests)
            {
                this.tests = tests;
            }
        }

        private class TestImpl2
        {
            [Inject]
            public List<Test1> tests = null;
        }

        [Test]
        public void TestMultiBind1()
        {
            _container.Bind<Test1>().ToSingle<Test2>();
            _container.Bind<Test1>().ToSingle<Test3>();
            _container.Bind<TestImpl1>().ToSingle();

            var test1 = _container.Resolve<TestImpl1>();

            Assert.That(test1.tests.Count == 2);
        }

        [Test]
        [ExpectedException]
        public void TestMultiBind2()
        {
            _container.Bind<TestImpl1>().ToSingle();

            // optional list dependencies should be declared as optional
            _container.Resolve<TestImpl1>();
        }

        [Test]
        public void TestMultiBindListInjection()
        {
            _container.Bind<Test1>().ToSingle<Test2>();
            _container.Bind<Test1>().ToSingle<Test3>();
            _container.Bind<TestImpl2>().ToSingle();

            var test = _container.Resolve<TestImpl2>();
            Assert.That(test.tests.Count == 2);
        }
    }
}


