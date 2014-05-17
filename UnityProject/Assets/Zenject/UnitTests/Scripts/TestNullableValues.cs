using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestNullableValues : TestWithContainer
    {
        private class Test2
        {
            public int? val;

            public Test2(int? val)
            {
                this.val = val;
            }
        }

        [Test]
        public void RunTest1()
        {
            _container.Bind<Test2>().ToSingle();
            _container.BindValue<int>().To(1);

            _container.ValidateResolve<Test2>();
            var test1 = _container.Resolve<Test2>();
            TestAssert.AreEqual(test1.val, 1);
        }
    }
}
