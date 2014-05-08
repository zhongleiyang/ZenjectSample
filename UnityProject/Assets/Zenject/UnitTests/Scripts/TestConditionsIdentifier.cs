using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestConditionsIdentifier : TestWithContainer
    {
        class Test0
        {
        }

        class Test1
        {
            public Test1(
                [Inject("foo")]
                Test0 name1)
            {
            }
        }

        class Test2
        {
            [Inject("foo")]
            Test0 name2;
        }

        public override void Setup()
        {
            base.Setup();

            _container.Bind<Test1>().ToTransient();
            _container.Bind<Test2>().ToTransient();
            _container.Bind<Test3>().ToTransient();
            _container.Bind<Test4>().ToTransient();
        }

        [Test]
        [ExpectedException(typeof(ZenjectResolveException))]
        public void TestUnspecifiedNameConstructorInjection()
        {
            _container.Bind<Test0>().ToTransient();
            _container.Resolve<Test1>();
        }

        [Test]
        [ExpectedException(typeof(ZenjectResolveException))]
        public void TestUnspecifiedNameFieldInjection()
        {
            _container.Bind<Test0>().ToTransient();
            _container.Resolve<Test2>();
        }

        [Test]
        [ExpectedException(typeof(ZenjectResolveException))]
        public void TestTooManySpecified()
        {
            _container.Bind<Test0>().ToTransient();
            _container.Bind<Test0>().ToTransient();

            TestAssert.IsNotNull(_container.Resolve<Test1>());
        }

        [Test]
        public void TestSuccessConstructorInjectionString()
        {
            _container.Bind<Test0>().ToSingle(new Test0());
            _container.Bind<Test0>().ToSingle(new Test0()).WhenInjectedInto("foo");

            TestAssert.IsNotNull(_container.Resolve<Test1>());
        }

        [Test]
        public void TestSuccessFieldInjectionString()
        {
            _container.Bind<Test0>().ToSingle(new Test0());
            _container.Bind<Test0>().ToSingle(new Test0()).WhenInjectedInto("foo");

            TestAssert.IsNotNull(_container.Resolve<Test2>());
        }

        enum TestEnum
        {
            TestValue1,
            TestValue2,
            TestValue3,
        }

        class Test3
        {
            public Test3(
                [Inject(TestEnum.TestValue2)]
                Test0 test0)
            {
            }
        }

        class Test4
        {
            [Inject(TestEnum.TestValue3)]
            Test0 test0;
        }

        [Test]
        [ExpectedException(typeof(ZenjectResolveException))]
        public void TestFailConstructorInjectionEnum()
        {
            _container.Bind<Test0>().ToSingle(new Test0());
            _container.Bind<Test0>().ToSingle(new Test0()).WhenInjectedInto(TestEnum.TestValue1);

            TestAssert.IsNotNull(_container.Resolve<Test3>());
        }

        [Test]
        public void TestSuccessConstructorInjectionEnum()
        {
            _container.Bind<Test0>().ToSingle(new Test0());
            _container.Bind<Test0>().ToSingle(new Test0()).WhenInjectedInto(TestEnum.TestValue2);

            TestAssert.IsNotNull(_container.Resolve<Test3>());
        }

        [Test]
        [ExpectedException(typeof(ZenjectResolveException))]
        public void TestFailFieldInjectionEnum()
        {
            _container.Bind<Test0>().ToSingle(new Test0());
            _container.Bind<Test0>().ToSingle(new Test0()).WhenInjectedInto(TestEnum.TestValue1);

            TestAssert.IsNotNull(_container.Resolve<Test3>());
        }

        [Test]
        public void TestSuccessFieldInjectionEnum()
        {
            _container.Bind<Test0>().ToSingle(new Test0());
            _container.Bind<Test0>().ToSingle(new Test0()).WhenInjectedInto(TestEnum.TestValue3);

            TestAssert.IsNotNull(_container.Resolve<Test4>());
        }
    }
}
