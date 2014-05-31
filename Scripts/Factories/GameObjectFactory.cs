using System;
using UnityEngine;
using Fasterflect;

namespace ModestTree.Zenject
{
    // Instantiate via prefab
    public class GameObjectFactory<TContract> : IFactory<TContract> where TContract : Component
    {
        DiContainer _container;
        GameObject _prefab;
        GameObjectInstantiator _instantiator;

        public GameObjectFactory(DiContainer container, GameObject prefab)
        {
            if (!container.AllowNullBindings && prefab == null)
            {
                throw new ZenjectBindException(
                    "Null prefab given for binding with type '{0}'", typeof(TContract));
            }

            _prefab = prefab;
            _container = container;
        }

        public TContract Create(params object[] constructorArgs)
        {
            if (_instantiator == null)
            {
                _instantiator = _container.Resolve<GameObjectInstantiator>();
            }

            var gameObj = _instantiator.Instantiate(_prefab, constructorArgs);

            var component = gameObj.GetComponentInChildren<TContract>();

            if (component == null)
            {
                throw new ZenjectResolveException(
                    "Could not find component '{0}' when creating game object from prefab", typeof(TContract).Name());
            }

            return component;
        }
    }
}
