using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    public class TestWithContainer
    {
        protected DiContainer _container;

        [SetUp]
        public virtual void Setup()
        {
            _container = new DiContainer();

            Install(_container);

            FieldsInjecter.Inject(_container, this);
        }

        protected virtual void Install(DiContainer container)
        {
        }

        [TearDown]
        public virtual void Destroy()
        {
            _container = null;
        }
    }
}
