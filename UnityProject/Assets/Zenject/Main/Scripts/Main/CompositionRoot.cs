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
            var sceneInstallers = (from c in gameObject.GetComponents<MonoBehaviour>() where c.GetType().DerivesFrom<ISceneInstaller>() select ((ISceneInstaller)(object)c));

            if (sceneInstallers.HasMoreThan(1))
            {
                Debug.LogError("Found multiple scene installers when only one was expected while initializing CompositionRoot");
                return;
            }

            if (sceneInstallers.IsEmpty())
            {
                Debug.LogError("Could not find scene installer while initializing CompositionRoot");
                return;
            }

            var installer = sceneInstallers.Single();

            var moduleContainer = new DiContainer();

            installer.InstallModules(moduleContainer);

            var modules = moduleContainer.ResolveMany<IModule>();

            if (modules.IsEmpty())
            {
                Debug.LogError("No modules found while initializing CompositionRoot");
                return;
            }

            Debug.Log("Initializing Composition Root with " + modules.Count() + " modules");

            foreach (var module in modules)
            {
                module.AddBindings(_container);
            }
        }

        void InitContainer()
        {
            _container = new DiContainer();

            // Note: This has to go first
            _container.Bind<CompositionRoot>().To(this);

            if (_extraBindingLookup != null)
            {
                _extraBindingLookup(_container);
                _extraBindingLookup = null;
            }
        }

        void Awake()
        {
            //Log.Debug("CompositionRoot Started");

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
