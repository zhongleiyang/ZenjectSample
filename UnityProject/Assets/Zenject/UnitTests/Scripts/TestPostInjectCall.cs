using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestPostInjectCall : TestWithContainer
    {
        class Test0
        {
        }

        class Test1
        {
        }

        class Test2
        {
        }

        class Test3
        {
            public bool HasInitialized;
            public bool HasInitialized2;

            [Inject]
            public Test1 test1 = null;

            [Inject]
            public Test0 test0 = null;

            Test2 _test2;

            public Test3(Test2 test2)
            {
                _test2 = test2;
            }

            [PostInject]
            public void Init()
            {
                TestAssert.That(!HasInitialized);
                TestAssert.IsNotNull(test1);
                TestAssert.IsNotNull(test0);
                TestAssert.IsNotNull(_test2);
                HasInitialized = true;
            }

            [PostInject]
            void TestPrivatePostInject()
            {
                HasInitialized2 = true;
            }
        }

        [Test]
        public void Test()
        {
            _container.Bind<Test0>().ToSingle();
            _container.Bind<Test1>().ToSingle();
            _container.Bind<Test2>().ToSingle();
            _container.Bind<Test3>().ToSingle();

            var test3 = _container.Resolve<Test3>();
            TestAssert.That(test3.HasInitialized);
            TestAssert.That(test3.HasInitialized2);
        }

        [Test]
        public void TestInheritance()
        {
            _container.Bind<IFoo>().ToSingle<FooDerived>();

            var foo = _container.Resolve<IFoo>();

            TestAssert.That(((FooDerived)foo).WasDerivedCalled);
            TestAssert.That(((FooBase)foo).WasBaseCalled);
        }

        interface IFoo
        {
        }

        class FooBase : IFoo
        {
            public bool WasBaseCalled;

            [PostInject]
            void TestBase()
            {
                TestAssert.That(!WasBaseCalled);
                WasBaseCalled = true;
            }
        }

        class FooDerived : FooBase
        {
            public bool WasDerivedCalled;

            [PostInject]
            void TestDerived()
            {
                TestAssert.That(!WasDerivedCalled);
                WasDerivedCalled = true;
            }
        }
    }
}



