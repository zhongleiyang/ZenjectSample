using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestCustomScope : TestWithContainer
    {
        class Test0
        {
        }

        [Test]
        public void TestIsRemoved()
        {
            using (var scope = _container.CreateScope())
            {
                var test1 = new Test0();

                scope.Bind<Test0>().ToSingle(test1);

                Assert.That(ReferenceEquals(test1, _container.Resolve<Test0>()));
            }

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<Test0>(); });
        }

        class Test1
        {
            [Inject]
            public Test0 Test;
        }

        [Test]
        public void TestCase2()
        {
            Test0 test0;
            Test1 test1;

            using (var scope = _container.CreateScope())
            {
                var test0Local = new Test0();

                scope.Bind<Test0>().ToSingle(test0Local);
                scope.Bind<Test1>().ToSingle();

                test0 = _container.Resolve<Test0>();
                TestAssert.AreEqual(test0Local, test0);

                test1 = _container.Resolve<Test1>();
            }

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<Test0>(); });

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<Test1>(); });

            _container.Bind<Test0>().ToSingle();
            _container.Bind<Test1>().ToSingle();

            TestAssert.That(_container.Resolve<Test0>() != test0);
            TestAssert.That(_container.Resolve<Test1>() != test1);
        }
    }
}


