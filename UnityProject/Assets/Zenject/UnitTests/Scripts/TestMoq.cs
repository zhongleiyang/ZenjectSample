using System;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;
using Moq;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestMoq
    {
        [Test]
        public void TestCase1()
        {
            var container = new DiContainer();
            container.Bind<IFoo>().ToMock();

            container.ValidateResolve<IFoo>();
            var foo = container.Resolve<IFoo>();

            TestAssert.AreEqual(foo.GetBar(), 0);
        }

        public interface IFoo
        {
            int GetBar();
        }
    }
}
