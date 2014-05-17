using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using NUnit.Framework;
using UnityEngine;
using TestAssert = NUnit.Framework.Assert;
using ModestTree.Zenject;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestValidateModule
    {
        [Test]
        public void TestBasicSuccess()
        {
            var container = new DiContainer();

            container.Bind<IFoo>().ToSingle<Foo>();
            container.Bind<Bar>().ToSingle();

            container.ValidateResolve<IFoo>();
        }

        [Test]
        [ExpectedException(typeof(ZenjectResolveException))]
        public void TestBasicFailure()
        {
            var container = new DiContainer();

            container.Bind<IFoo>().ToSingle<Foo>();
            //container.Bind<Bar>().ToSingle();

            container.ValidateResolve<IFoo>();
        }

        [Test]
        public void TestList()
        {
            var container = new DiContainer();

            container.Bind<IFoo>().ToSingle<Foo>();
            container.Bind<IFoo>().ToSingle<Foo2>();

            container.Bind<Bar>().ToSingle();

            container.Bind<Qux>().ToSingle();

            container.ValidateResolve<Qux>();
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
