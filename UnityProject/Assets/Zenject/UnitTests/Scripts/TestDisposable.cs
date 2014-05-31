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
            public static bool ConstructorWasCalled;

            public Test1()
            {
                ConstructorWasCalled = true;
            }

            public bool HasDisposed;

            public void Dispose()
            {
                HasDisposed = true;
            }
        }

        [Test]
        public void Test()
        {
            _container.Bind<Test1>().ToSingle();
            _container.Bind<IDisposable>().ToSingle<Test1>();

            TestAssert.That(_container.ValidateResolve<Test1>().IsEmpty());
            var test = _container.Resolve<Test1>();

            TestAssert.That(!test.HasDisposed);

            _container.Dispose();

            TestAssert.That(test.HasDisposed);
        }

        [Test]
        public void TestDisposeNoResolve()
        {
            _container.Bind<IDisposable>().ToSingle<Test1>();

            Test1.ConstructorWasCalled = false;

            // Should not create new objects during dispose
            _container.Dispose();

            TestAssert.That(!Test1.ConstructorWasCalled);
        }
    }
}



