using System.Collections.Generic;

namespace ModestTree.Zenject
{
    public class DependencyRootStandard : IDependencyRoot
    {
        // Usually we pass dependencies via contructor injection
        // but since we define a root so often (eg. unit tests)
        // just use [Inject] for the root classes

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
