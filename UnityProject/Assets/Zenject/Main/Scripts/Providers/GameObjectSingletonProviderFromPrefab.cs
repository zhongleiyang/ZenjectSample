using System;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class GameObjectSingletonProviderFromPrefab<T> : ProviderBase where T : Component
    {
        IFactory<T> _factory;
        object _instance;

        public GameObjectSingletonProviderFromPrefab(DiContainer container, GameObject template)
        {
            _factory = new GameObjectFactory<T>(container, template);
        }

        public override Type GetInstanceType()
        {
            return typeof(T);
        }

        public override object GetInstance()
        {
            if (_instance == null)
            {
                _instance = _factory.Create();
                Assert.That(_instance != null);
            }

            return _instance;
        }
    }
}
