using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;
using System.Linq;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestDiContainer
    {
        DiContainer _container;

        [SetUp]
        void SetUp()
        {
            _container = new DiContainer();
        }

        [Test]
        public void TestSimple()
        {
            _container.Bind<IFoo>().ToSingle<Foo>();
            _container.Bind<Bar>().ToSingle();

            AssertHasContracts(
                new List<Type>() { typeof(Bar), typeof(IFoo) });

            AssertHasConcreteTypes(
                new List<Type>() { typeof(Bar), typeof(Foo) });
        }

        void AssertHasConcreteTypes(IEnumerable<Type> expectedValues)
        {
            var concreteList = _container.AllConcreteTypes.ToList();

            var standardTypes = new List<Type>() { typeof(Instantiator), typeof(DiContainer) };

            TestAssert.That(
                TestUtil.ListsContainSameElements(
                    concreteList, standardTypes.Append(expectedValues).ToList()),
                    "Unexpected list: " + TestUtil.PrintList(concreteList));
        }

        void AssertHasContracts(IEnumerable<Type> expectedValues)
        {
            var contractList = _container.AllContracts.ToList();

            var standardTypes = new List<Type>() { typeof(Instantiator), typeof(DiContainer) };

            TestAssert.That(
                TestUtil.ListsContainSameElements(
                    contractList, standardTypes.Append(expectedValues).ToList()),
                    "Unexpected list: " + TestUtil.PrintList(contractList));
        }

        [Test]
        public void TestComplex()
        {
            _container.Bind<IFoo>().ToSingle<Foo>();
            _container.Bind<IFoo>().ToSingle<Foo2>();

            _container.Bind<Bar>().To(new Bar());
            _container.Bind<Bar>().To(new Bar());

            AssertHasContracts(
                new List<Type>() { typeof(Bar), typeof(IFoo) });

            AssertHasConcreteTypes(
                new List<Type>() { typeof(Bar), typeof(Foo) });
        }

        interface IFoo
        {
        }

        class Foo : IFoo
        {
        }

        class Foo2 : IFoo
        {
        }

        class Bar
        {
        }
    }
}



