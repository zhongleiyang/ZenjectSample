using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestConditionsFieldName : TestWithContainer
    {
        class Test0
        {

        }

        class Test1
        {
            public Test1(Test0 name1)
            {
            }
        }

        class Test2
        {
            public Test2(Test0 name2)
            {
            }
        }

        public override void Setup()
        {
            base.Setup();
            _container.Bind<Test0>().ToSingle().When(r => r.SourceName == "name1");
        }

        [Test]
        public void TestNameConditionError()
        {
            _container.Bind<Test2>().ToSingle();

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<Test2>(); });

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.ValidateResolve<Test2>(); });
        }

        [Test]
        public void TestNameConditionSuccess()
        {
            _container.Bind<Test1>().ToSingle();

            _container.ValidateResolve<Test1>();
            var test1 = _container.Resolve<Test1>();

            TestAssert.That(test1 != null);
        }
    }
}


