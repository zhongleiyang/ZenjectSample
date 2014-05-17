using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using Fasterflect;

namespace ModestTree.Zenject
{
    [Serializable]
    public class InitializablePrioritiesModule : IModule
    {
        List<Type> _initializables;

        public InitializablePrioritiesModule(List<Type> initializables)
        {
            _initializables = initializables;
        }

        public void AddBindings(DiContainer container)
        {
            int priorityCount = 1;

            foreach (var initializableType in _initializables)
            {
                Assert.That(initializableType.DerivesFrom<IInitializable>(),
                    "Expected type '{0}' to derive from IInitializable", initializableType.Name());

                container.Bind<Tuple<Type, int>>().To(
                    Tuple.New(initializableType, priorityCount)).WhenInjectedInto<InitializableHandler>();
                priorityCount++;
            }
        }
    }
}


