using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestTestOptional : TestWithContainer
    {
        class Test1
        {
        }

        class Test2
        {
            [Inject] public Test1 val1 = null;
        }

        class Test3
        {
            [InjectOptional] public Test1 val1 = null;
        }

        [Test]
        public void TestFieldRequired()
        {
            _container.Bind<Test2>().ToSingle();

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.ValidateResolve<Test2>(); });

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<Test2>(); });
        }

        [Test]
        public void TestFieldOptional()
        {
            _container.Bind<Test3>().ToSingle();

            _container.ValidateResolve<Test3>();
            var test = _container.Resolve<Test3>();
            TestAssert.That(test.val1 == null);
        }

        [Test]
        public void TestFieldOptional2()
        {
            _container.Bind<Test3>().ToSingle();

            var test1 = new Test1();
            _container.Bind<Test1>().To(test1);

            _container.ValidateResolve<Test3>();
            TestAssert.AreEqual(_container.Resolve<Test3>().val1, test1);
        }

        class Test4
        {
            public Test4(Test1 val1)
            {
            }
        }

        class Test5
        {
            public Test1 Val1;

            public Test5(
                [InjectOptional]
                Test1 val1)
            {
                Val1 = val1;
            }
        }

        [Test]
        public void TestParameterRequired()
        {
            _container.Bind<Test4>().ToSingle();

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<Test4>(); });

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.ValidateResolve<Test4>(); });
        }

        [Test]
        public void TestParameterOptional()
        {
            _container.Bind<Test5>().ToSingle();

            _container.ValidateResolve<Test5>();
            var test = _container.Resolve<Test5>();
            TestAssert.That(test.Val1 == null);
        }

        class Test6
        {
            public Test6(Test2 test2)
            {
            }
        }

        [Test]
        public void TestChildDependencyOptional()
        {
            _container.Bind<Test6>().ToSingle();
            _container.Bind<Test2>().ToSingle();

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.ValidateResolve<Test6>(); });

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<Test6>(); });
        }
    }
}



