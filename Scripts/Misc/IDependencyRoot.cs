using System.Collections.Generic;

namespace ModestTree.Zenject
{
    // Derived class should contain all dependencies
    // for the the given run configuration
    public interface IDependencyRoot
    {
        // Entry point of the app
        void Start();
    }
}

