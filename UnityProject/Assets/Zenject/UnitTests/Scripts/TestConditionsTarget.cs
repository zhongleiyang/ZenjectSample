using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestConditionsTarget : TestWithContainer
    {
        private class Test0
        {
        }

        private class Test1
        {
            public Test1(Test0 test)
            {
            }
        }

        private class Test2
        {
            public Test2(Test0 test)
            {
            }
        }

        public override void Setup()
        {
            base.Setup();
            _container.Bind<Test0>().ToSingle().When(r => r.Target == typeof(Test2));
        }

        [Test]
        [ExpectedException]
        public void TestTargetConditionError()
        {
            _container.Bind<Test1>().ToSingle();
            _container.Resolve<Test1>();
        }

        [Test]
        public void TestTargetConditionSuccess()
        {
            _container.Bind<Test2>().ToSingle();
            var test2 = _container.Resolve<Test2>();

            Assert.That(test2 != null);
        }
    }
}


