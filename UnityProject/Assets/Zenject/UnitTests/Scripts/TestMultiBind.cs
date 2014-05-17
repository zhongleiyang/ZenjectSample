using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;

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

            _container.ValidateResolve<TestImpl1>();
            var test1 = _container.Resolve<TestImpl1>();

            TestAssert.That(test1.tests.Count == 2);
        }

        [Test]
        public void TestMultiBind2()
        {
            _container.Bind<TestImpl1>().ToSingle();

            // optional list dependencies should be declared as optional
            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<TestImpl1>(); });

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.ValidateResolve<TestImpl1>(); });
        }

        [Test]
        public void TestMultiBind2Validate()
        {
            _container.Bind<TestImpl1>().ToSingle();

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<TestImpl1>(); });

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.ValidateResolve<TestImpl1>(); });
        }

        [Test]
        public void TestMultiBindListInjection()
        {
            _container.Bind<Test1>().ToSingle<Test2>();
            _container.Bind<Test1>().ToSingle<Test3>();
            _container.Bind<TestImpl2>().ToSingle();

            _container.ValidateResolve<TestImpl2>();
            var test = _container.Resolve<TestImpl2>();
            TestAssert.That(test.tests.Count == 2);
        }
    }
}


