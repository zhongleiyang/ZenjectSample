using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using NUnit.Framework;
using UnityEngine;
using TestAssert = NUnit.Framework.Assert;
using ModestTree.Zenject;
using System.Linq;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestValidateInstaller
    {
        [Test]
        public void TestBasicSuccess()
        {
            var container = new DiContainer();

            container.Bind<IFoo>().ToSingle<Foo>();
            container.Bind<Bar>().ToSingle();

            TestAssert.That(container.ValidateResolve<IFoo>().IsEmpty());
        }

        [Test]
        public void TestBasicFailure()
        {
            var container = new DiContainer();

            container.Bind<IFoo>().ToSingle<Foo>();
            //container.Bind<Bar>().ToSingle();

            TestAssert.That(!container.ValidateResolve<IFoo>().IsEmpty());
        }

        [Test]
        public void TestList()
        {
            var container = new DiContainer();

            container.Bind<IFoo>().ToSingle<Foo>();
            container.Bind<IFoo>().ToSingle<Foo2>();

            container.Bind<Bar>().ToSingle();

            container.Bind<Qux>().ToSingle();

            TestAssert.That(container.ValidateResolve<Qux>().IsEmpty());
        }

        [Test]
        public void TestValidateDynamicSuccess()
        {
            var container = new DiContainer();

            container.Bind<Foo>().ToSingle();

            TestAssert.That(container.ValidateObjectGraph<Foo>(typeof(Bar)).IsEmpty());
        }

        [Test]
        public void TestValidateDynamicFailure()
        {
            var container = new DiContainer();

            container.Bind<Foo>().ToSingle();

            TestAssert.That(!container.ValidateObjectGraph<Foo>().IsEmpty());
        }

        [Test]
        public void TestValidateDynamicFailure2()
        {
            var container = new DiContainer();

            container.Bind<Foo>().ToSingle();

            TestAssert.That(!container.ValidateObjectGraph<Foo>(typeof(Bar), typeof(string)).IsEmpty());
        }

        interface IFoo
        {
        }

        class Foo : IFoo
        {
            public Foo(Bar bar)
            {
            }
        }

        class Foo2 : IFoo
        {
            public Foo2(Bar bar)
            {
            }
        }

        class Bar
        {
        }

        class Qux
        {
            public Qux(List<IFoo> foos)
            {
            }
        }

        class TestDependencyRoot : IDependencyRoot
        {
            [Inject]
            public IFoo _foo;

            public void Start()
            {
            }
        }
    }
}
