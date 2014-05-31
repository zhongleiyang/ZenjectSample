using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class GameObjectTransientProviderFromPrefab<T> : ProviderBase where T : Component
    {
        IFactory<T> _factory;
        DiContainer _container;

        public GameObjectTransientProviderFromPrefab(DiContainer container, GameObject template)
        {
            _factory = new GameObjectFactory<T>(container, template);
            _container = container;
        }

        public override Type GetInstanceType()
        {
            return typeof(T);
        }

        public override bool HasInstance()
        {
            return false;
        }

        public override object GetInstance()
        {
            return _factory.Create();
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding()
        {
            return BindingValidator.ValidateObjectGraph(_container, typeof(T));
        }
    }
}
