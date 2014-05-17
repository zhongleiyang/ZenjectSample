using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Permissions;

namespace ModestTree.Zenject
{
    // Instantiate given concrete class
    public class Factory<TContract, TConcrete> : IFactory<TContract> where TConcrete : TContract
    {
        readonly DiContainer _container;
        Instantiator _instantiator;

        public Factory(DiContainer container)
        {
            _container = container;
        }

        public virtual TContract Create(params object[] constructorArgs)
        {
            if (_instantiator == null)
            {
                _instantiator = _container.Resolve<Instantiator>();
            }

            return _instantiator.Instantiate<TConcrete>(constructorArgs);
        }
    }

    // Instantiate given contract class
    public class Factory<TContract> : IFactory<TContract>
    {
        readonly DiContainer _container;
        readonly Type _concreteType;
        Instantiator _instantiator;

        [Inject]
        public Factory(DiContainer container)
        {
            _container = container;
            _concreteType = typeof(TContract);
        }

        public Factory(DiContainer container, Type concreteType)
        {
            Assert.That(typeof(TContract).IsAssignableFrom(concreteType));

            _container = container;
            _concreteType = concreteType;
        }

        public virtual TContract Create(params object[] constructorArgs)
        {
            if (_instantiator == null)
            {
                _instantiator = _container.Resolve<Instantiator>();
            }

            return (TContract)_instantiator.Instantiate(_concreteType, constructorArgs);
        }
    }
}
