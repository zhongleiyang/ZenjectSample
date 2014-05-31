using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using Fasterflect;

namespace ModestTree.Zenject
{
    public class InitializablePrioritiesModule : Module
    {
        List<Type> _initializables;

        public InitializablePrioritiesModule(List<Type> initializables)
        {
            _initializables = initializables;
        }

        public override void AddBindings()
        {
            int priorityCount = 1;

            foreach (var initializableType in _initializables)
            {
                Assert.That(initializableType.DerivesFrom<IInitializable>(),
                    "Expected type '{0}' to derive from IInitializable", initializableType.Name());

                _container.Bind<Tuple<Type, int>>().To(
                    Tuple.New(initializableType, priorityCount)).WhenInjectedInto<InitializableHandler>();
                priorityCount++;
            }
        }
    }
}


