using System;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class GameObjectTransientProviderFromPrefab<T> : ProviderBase where T : Component
    {
        IFactory<T> _factory;

        public GameObjectTransientProviderFromPrefab(DiContainer container, GameObject template)
        {
            _factory = new GameObjectFactory<T>(container, template);
        }

        public override Type GetInstanceType()
        {
            return typeof(T);
        }

        public override object GetInstance()
        {
            return _factory.Create();
        }
    }
}
