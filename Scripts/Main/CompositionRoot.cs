using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    // Define this class as a component of a top-level game object of your scene heirarchy
    // Then any children will get injected during resolve stage
    public class CompositionRoot : MonoBehaviour
    {
        DiContainer _container;
        IDependencyRoot _dependencyRoot;

        static Action<DiContainer> _extraBindingLookup;

        internal static Action<DiContainer> ExtraBindingsLookup
        {
            set
            {
                Assert.IsNull(_extraBindingLookup);
                _extraBindingLookup = value;
            }
        }

        public DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        void Register()
        {
            // call RegisterBindings on any installers on our game object or somewhere below in the scene heirarchy
            BroadcastMessage("RegisterBindings", _container, SendMessageOptions.RequireReceiver);
        }

        void InitContainer()
        {
            _container = new DiContainer();

            // Note: This has to go first
            _container.Bind<CompositionRoot>().ToSingle(this);

            if (_extraBindingLookup != null)
            {
                _extraBindingLookup(_container);
                _extraBindingLookup = null;
            }
        }

        void Awake()
        {
            InitContainer();
            Register();
            Resolve();
        }

        void OnDestroy()
        {
            _container.Dispose();
        }

        void Resolve()
        {
            InjectionHelper.InjectChildGameObjects(_container, gameObject);

            if (_container.HasBinding<IDependencyRoot>())
            {
                _dependencyRoot = _container.Resolve<IDependencyRoot>();
                _dependencyRoot.Start();
            }
            else
            {
                Debug.LogWarning("No dependency root found");
            }
        }
    }
}
