using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModestTree.Zenject
{
    // This class should ONLY be used the following way:
    //
    //  using (_container.PushLookup(currentType))
    //  {
    //      new PropertiesInjector()
    //      container.Resolve()
    //      ...  etc.
    //  }
    //
    // It is very useful to track the object graph when debugging a DI related problem
    // so the way we track this is by pushing and popping from LookupsInProgress
    // using C# using() pattern
    public class LookupInProgressAdder : IDisposable
    {
        DiContainer _container;
        Type _concreteType;

        internal LookupInProgressAdder(DiContainer container, Type concreteType)
        {
            if (container.LookupsInProgress.Contains(concreteType))
            {
                Assert.That(false, () => "Circular dependency detected! \nObject graph:\n" + container.GetCurrentObjectGraph());
            }

            container.LookupsInProgress.Push(concreteType);

            _container = container;
            _concreteType = concreteType;
        }

        public void Dispose()
        {
            Assert.That(_container.LookupsInProgress.Peek() == _concreteType);
            _container.LookupsInProgress.Pop();
        }
    }
}

