using System.Collections.Generic;

namespace ModestTree.Zenject
{
    public class DependencyRootStandard : IDependencyRoot
    {
        [Inject]
        public UnityKernel UnityKernel;

        [Inject]
        public InitializableHandler Initializer;

        public virtual void Start()
        {
            Initializer.Initialize();
        }
    }

    public class DependencyRootStandard<TRoot> : DependencyRootStandard
        where TRoot : class
    {
        [Inject]
        public TRoot Root;
    }
}
