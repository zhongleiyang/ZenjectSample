using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestSingleton : TestWithContainer
    {
        private interface IFoo
        {
            int ReturnValue();
        }

        private class Foo : IFoo
        {
            public int ReturnValue()
            {
                return 5;
            }
        }

        [Test]
        public void TestClassRegistration()
        {
            _container.Bind<Foo>().ToSingle();

            _container.ValidateResolve<Foo>();
            TestAssert.That(_container.Resolve<Foo>().ReturnValue() == 5);
        }

        [Test]
        public void TestSingletonOneInstance()
        {
            _container.Bind<Foo>().ToSingle();

            _container.ValidateResolve<Foo>();
            var test1 = _container.Resolve<Foo>();
            _container.ValidateResolve<Foo>();
            var test2 = _container.Resolve<Foo>();

            TestAssert.That(test1 != null && test2 != null);
            TestAssert.That(ReferenceEquals(test1, test2));
        }

        [Test]
        public void TestInterfaceBoundToImplementationRegistration()
        {
            _container.Bind<IFoo>().ToSingle<Foo>();

            _container.ValidateResolve<IFoo>();
            TestAssert.That(_container.Resolve<IFoo>().ReturnValue() == 5);
        }

        [Test]
        public void TestInterfaceBoundToInstanceRegistration()
        {
            IFoo instance = new Foo();

            _container.Bind<IFoo>().To(instance);

            _container.ValidateResolve<IFoo>();
            var builtInstance = _container.Resolve<IFoo>();

            TestAssert.That(ReferenceEquals(builtInstance, instance));
            TestAssert.That(builtInstance.ReturnValue() == 5);
        }

        [Test]
        public void TestDuplicateBindings()
        {
            // Note: does not error out until a request for Foo is made
            _container.Bind<Foo>().ToSingle();
            _container.Bind<Foo>().ToSingle();
        }

        [Test]
        public void TestDuplicateBindingsFail()
        {
            _container.Bind<Foo>().ToSingle();
            _container.Bind<Foo>().ToSingle();

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<Foo>(); });

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.ValidateResolve<Foo>(); });
        }

        [Test]
        public void TestDuplicateInstanceBindingsFail()
        {
            var instance = new Foo();

            _container.Bind<Foo>().To(instance);
            _container.Bind<Foo>().To(instance);

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.ValidateResolve<Foo>(); });

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<Foo>(); });
        }

        [Test]
        [ExpectedException(typeof(ZenjectBindException))]
        public void TestToSingleWithInstance()
        {
            _container.Bind<Foo>().ToSingle(new Foo());
            _container.Bind<Foo>().ToSingle(new Foo());
        }

        [Test]
        public void TestToSingleWithInstanceIsUnique()
        {
            var foo = new Foo();

            _container.Bind<Foo>().ToSingle(foo);
            _container.Bind<IFoo>().ToSingle<Foo>();

            TestAssert.That(
                ReferenceEquals(_container.Resolve<IFoo>(), _container.Resolve<Foo>()));
        }

        [Test]
        public void TestToSingleWithInstance2()
        {
            var foo = new Foo();

            _container.Bind<Foo>().To(foo);
            _container.Bind<IFoo>().ToSingle<Foo>();

            TestAssert.That(
                !ReferenceEquals(_container.Resolve<IFoo>(), _container.Resolve<Foo>()));
        }
    }
}


