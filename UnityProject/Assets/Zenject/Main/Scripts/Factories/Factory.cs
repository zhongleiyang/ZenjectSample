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
            var obj = Instantiator.Instantiate<TConcrete>(_container, constructorArgs);

            if (typeof(IInitializable).IsAssignableFrom(typeof(TConcrete)))
            {
                // Runtime created classes can implement IInitializable to get an Initialize() method called
                // directly after creation
                // It's often undesirable to put too much logic in the constructor
                // (http://blog.vuscode.com/malovicn/archive/2009/10/16/inversion-of-control-single-responsibility-principle-and-nikola-s-laws-of-dependency-injection.aspx)
                // (http://blog.ploeh.dk/2011/03/03/InjectionConstructorsshouldbesimple/)
                // so this is usually a good alternative
                ((IInitializable)obj).Initialize();
            }

            return obj;
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
            var obj = (TContract)Instantiator.Instantiate(_container, _concreteType, constructorArgs);

            if (typeof(IInitializable).IsAssignableFrom(_concreteType))
            {
                // See comment above
                ((IInitializable)obj).Initialize();
            }

            return obj;
        }
    }
}
