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
        public void Setup()
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
            var expectedList = GetStandardTypeInclusions().Concat(expectedValues).ToList();

            TestAssert.That(
                TestUtil.ListsContainSameElements(
                    concreteList, expectedList),
                    "Unexpected list: " + TestUtil.PrintList(concreteList) + "\nExpected: " + TestUtil.PrintList(expectedList));
        }

        void AssertHasContracts(IEnumerable<Type> expectedValues)
        {
            var contractList = _container.AllContracts.ToList();
            var expectedList = GetStandardTypeInclusions().Concat(expectedValues).ToList();

            TestAssert.That(
                TestUtil.ListsContainSameElements(
                    contractList, expectedList),
                    "Unexpected list: " + TestUtil.PrintList(contractList) + "\nExpected: " + TestUtil.PrintList(expectedList));
        }

        List<Type> GetStandardTypeInclusions()
        {
            return new List<Type>() { typeof(Instantiator), typeof(DiContainer) };
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
                new List<Type>() { typeof(Bar), typeof(Foo2), typeof(Foo) });
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



