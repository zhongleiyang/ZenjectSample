using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestDisposable : TestWithContainer
    {
        class Test1 : IDisposable
        {
            public bool HasDisposed;

            public void Dispose()
            {
                HasDisposed = true;
            }
        }

        class Test2
        {
            [Inject]
            public Test2 test = null;
        }

        [Test]
        public void Test()
        {
            _container.Bind<Test1>().ToSingle();
            _container.Bind<IDisposable>().ToSingle<Test1>();

            _container.ValidateResolve<Test1>();
            var test = _container.Resolve<Test1>();

            TestAssert.That(!test.HasDisposed);

            _container.Dispose();

            TestAssert.That(test.HasDisposed);
        }
    }
}



