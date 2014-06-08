using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    // We extract the interface so that monobehaviours can be installers
    public interface IInstaller
    {
        DiContainer Container
        {
            set;
        }

        void InstallBindings();

        IEnumerable<ZenjectResolveException> ValidateSubGraphs();
    }

    public abstract class Installer : IInstaller
    {
        protected DiContainer _container;

        [Inject]
        public DiContainer Container
        {
            set
            {
                _container = value;
            }
        }

        public abstract void InstallBindings();

        public virtual IEnumerable<ZenjectResolveException> ValidateSubGraphs()
        {
            // optional
            return Enumerable.Empty<ZenjectResolveException>();
        }

        // Helper method for ValidateSubGraphs
        protected IEnumerable<ZenjectResolveException> Validate<T>(params Type[] extraTypes)
        {
            return _container.ValidateObjectGraph<T>(extraTypes);
        }
    }

    public abstract class MonoInstaller : MonoBehaviour, IInstaller
    {
        protected DiContainer _container;

        [Inject]
        public DiContainer Container
        {
            set
            {
                _container = value;
            }
        }

        public abstract void InstallBindings();

        public virtual IEnumerable<ZenjectResolveException> ValidateSubGraphs()
        {
            // optional
            return Enumerable.Empty<ZenjectResolveException>();
        }

        // Helper method for ValidateSubGraphs
        protected IEnumerable<ZenjectResolveException> Validate<T>(params Type[] extraTypes)
        {
            return _container.ValidateObjectGraph<T>(extraTypes);
        }
    }
}
