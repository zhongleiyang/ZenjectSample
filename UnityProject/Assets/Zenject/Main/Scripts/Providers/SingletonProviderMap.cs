using System;
using System.Collections.Generic;

namespace ModestTree.Zenject
{
    public class SingletonProviderMap
    {
        Dictionary<Type, SingletonLazyCreator> _creators = new Dictionary<Type, SingletonLazyCreator>();
        DiContainer _container;

        public SingletonProviderMap(DiContainer container)
        {
            _container = container;
        }

        void RemoveCreator(Type instanceType)
        {
            bool success = _creators.Remove(instanceType);
            Assert.That(success);
        }

        public ProviderBase CreateProvider<TConcrete>()
        {
            return CreateProvider(typeof(TConcrete));
        }

        public ProviderBase CreateProvider(Type concreteType)
        {
            SingletonLazyCreator creator;

            if (!_creators.TryGetValue(concreteType, out creator))
            {
                creator = new SingletonLazyCreator(_container, this, concreteType);
                _creators.Add(concreteType, creator);
            }

            creator.IncRefCount();

            return new SingletonProvider(creator);
        }

        ////////////////////// Internal classes

        class SingletonLazyCreator
        {
            int _referenceCount;
            object _instance;
            Type _instanceType;
            SingletonProviderMap _owner;
            DiContainer _container;

            public SingletonLazyCreator(
                DiContainer container, SingletonProviderMap owner, Type instanceType)
            {
                _container = container;
                _owner = owner;
                _instanceType = instanceType;
            }

            public void IncRefCount()
            {
                _referenceCount += 1;
            }

            public void DecRefCount()
            {
                _referenceCount -= 1;

                if (_referenceCount <= 0)
                {
                    _owner.RemoveCreator(_instanceType);
                }
            }

            public Type GetInstanceType()
            {
                return _instanceType;
            }

            public object GetInstance()
            {
                if (_instance == null)
                {
                    _instance = Instantiator.Instantiate(_container, _instanceType);
                    Assert.That(_instance != null);
                }

                return _instance;
            }
        }

        // NOTE: we need the provider seperate from the creator because
        // if we return the same provider multiple times then the condition
        // will get over-written
        class SingletonProvider : ProviderBase
        {
            SingletonLazyCreator _creator;

            public SingletonProvider(SingletonLazyCreator creator)
            {
                _creator = creator;
            }

            public override void Dispose()
            {
                _creator.DecRefCount();
            }

            public override Type GetInstanceType()
            {
                return _creator.GetInstanceType();
            }

            public override object GetInstance()
            {
                return _creator.GetInstance();
            }
        }
    }
}
