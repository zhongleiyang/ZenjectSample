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

        public Factory(DiContainer container)
        {
            _container = container;
        }

        public virtual TContract Create(params object[] constructorArgs)
        {
            return Instantiator.Instantiate<TConcrete>(_container, constructorArgs);
        }
    }

    // Instantiate given contract class
    public class Factory<TContract> : IFactory<TContract>
    {
        readonly DiContainer _container;
        readonly Type _concreteType;

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
            return (TContract)Instantiator.Instantiate(_container, _concreteType, constructorArgs);
        }
    }
}
