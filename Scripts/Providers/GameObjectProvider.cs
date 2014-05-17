using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class GameObjectSingletonProvider<T> : ProviderBase
    {
        object _instance;
        readonly string _name;
        GameObjectInstantiator _instantiator;
        DiContainer _container;

        public GameObjectSingletonProvider(DiContainer container, string name)
        {
            _name = name;
            _container = container;
        }

        public override Type GetInstanceType()
        {
            return typeof(T);
        }

        public override object GetInstance()
        {
            if (_instance == null)
            {
                if (_instantiator == null)
                {
                    _instantiator = _container.Resolve<GameObjectInstantiator>();
                }

                // We don't use the generic version here to avoid duplicate generic arguments to binder
                _instance = _instantiator.Instantiate(typeof(T), _name);
                Assert.That(_instance != null);
            }

            return _instance;
        }

        public override void ValidateBinding()
        {
            BindingValidator.ValidateCanCreateConcrete(_container, typeof(T));
        }
    }
}
