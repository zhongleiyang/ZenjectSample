using System;
using UnityEngine;

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
            _prefab = prefab;
            _container = container;
            _instantiator = _container.Resolve<GameObjectInstantiator>();
        }

        public TContract Create(params object[] constructorArgs)
        {
            var gameObj = _instantiator.Instantiate(_prefab, constructorArgs);

            var component = gameObj.GetComponentInChildren<TContract>();

            if (component == null)
            {
                throw new ZenjectResolveException(
                    "Could not find component '{0}' when creating game object from prefab", typeof(TContract).GetPrettyName());
            }

            return component;
        }
    }
}
