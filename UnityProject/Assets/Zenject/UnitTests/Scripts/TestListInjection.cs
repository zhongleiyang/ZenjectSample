using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestListInjection : TestWithContainer
    {
        class Test1
        {
            public Test1(List<int> values)
            {
            }
        }

        class Test2
        {
            public Test2( 
                [InjectOptional] List<int> values)
            {
            }
        }

        class Test3
        {
            [Inject]
            public List<int> values = null;
        }

        class Test4
        {
            [InjectOptional]
            public List<int> values = null;
        }

        [Test]
        [ExpectedException]
        public void TestCase1()
        {
            _container.Bind<Test1>().ToSingle();

            _container.ResolveMany<Test1>();
        }

        [Test]
        public void TestCase2()
        {
            _container.Bind<Test2>().ToSingle();

            var result = _container.ResolveMany<Test2>();

            Assert.That(result != null);
        }

        [Test]
        [ExpectedException]
        public void TestCase3()
        {
            _container.Bind<Test3>().ToSingle();

            _container.ResolveMany<Test3>();
        }

        [Test]
        public void TestCase4()
        {
            _container.Bind<Test4>().ToSingle();

            var result = _container.ResolveMany<Test4>();

            Assert.That(result != null);
        }
    }
}



