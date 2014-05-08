using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;
using System.Linq;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestConditionsParents : TestWithContainer
    {
        class Test0
        {
        }

        interface ITest1
        {
        }

        class Test1 : ITest1
        {
            public Test0 test0;

            public Test1(Test0 test0)
            {
                this.test0 = test0;
            }
        }

        class Test2 : ITest1
        {
            public Test0 test0;

            public Test2(Test0 test0)
            {
                this.test0 = test0;
            }
        }

        class Test3 : ITest1
        {
            public Test1 test1;

            public Test3(Test1 test1)
            {
                this.test1 = test1;
            }
        }

        class Test4 : ITest1
        {
            public Test1 test1;

            public Test4(Test1 test1)
            {
                this.test1 = test1;
            }
        }

        [Test]
        [ExpectedException]
        public void TestCase1()
        {
            _container.Bind<Test1>().ToSingle();
            _container.Bind<Test0>().ToSingle().When(c => c.Parents.Contains(typeof(Test2)));

            _container.Resolve<Test1>();
        }

        [Test]
        public void TestCase2()
        {
            _container.Bind<Test1>().ToSingle();
            _container.Bind<Test0>().ToSingle().When(c => c.Parents.Contains(typeof(Test1)));

            var test1 = _container.Resolve<Test1>();
            Assert.That(test1 != null);
        }

        // Test using parents to look deeper up the heirarchy..
        [Test]
        public void TestCase3()
        {
            var t0a = new Test0();
            var t0b = new Test0();

            _container.Bind<Test3>().ToSingle();
            _container.Bind<Test4>().ToSingle();
            _container.Bind<Test1>().ToTransient();

            _container.Bind<Test0>().ToSingle(t0a).When(c => c.Parents.Contains(typeof(Test3)));
            _container.Bind<Test0>().ToSingle(t0b).When(c => c.Parents.Contains(typeof(Test4)));

            var test3 = _container.Resolve<Test3>();
            var test4 = _container.Resolve<Test4>();

            Assert.That(ReferenceEquals(test3.test1.test0, t0a));
            Assert.That(ReferenceEquals(test4.test1.test0, t0b));
        }

        [Test]
        [ExpectedException]
        public void TestCase4()
        {
            _container.Bind<ITest1>().ToSingle<Test2>();
            _container.Bind<Test0>().ToSingle().When(c => c.Parents.Contains(typeof(ITest1)));

            var test1 = _container.Resolve<ITest1>();
            Assert.That(test1 != null);
        }

        [Test]
        public void TestCase5()
        {
            _container.Bind<ITest1>().ToSingle<Test2>();
            _container.Bind<Test0>().ToSingle().When(c => c.Parents.Contains(typeof(Test2)));

            var test1 = _container.Resolve<ITest1>();
            Assert.That(test1 != null);
        }

        [Test]
        public void TestCase6()
        {
            _container.Bind<ITest1>().ToSingle<Test2>();
            _container.Bind<Test0>().ToSingle().When(c => c.Parents.Where(x => typeof(ITest1).IsAssignableFrom(x)).Any());

            var test1 = _container.Resolve<ITest1>();
            Assert.That(test1 != null);
        }
    }
}

