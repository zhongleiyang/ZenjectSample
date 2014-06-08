using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using Fasterflect;

namespace ModestTree.Zenject
{
    public class InitializablePrioritiesInstaller : Installer
    {
        List<Type> _initializables;

        public InitializablePrioritiesInstaller(List<Type> initializables)
        {
            _initializables = initializables;
        }

        public override void InstallBindings()
        {
            int priorityCount = 1;

            foreach (var initializableType in _initializables)
            {
                Assert.That(initializableType.DerivesFrom<IInitializable>(), "Expected type '{0}' to derive from IInitializable", initializableType.Name());

                _container.Bind<Tuple<Type, int>>().To(
                    Tuple.New(initializableType, priorityCount)).WhenInjectedInto<InitializableHandler>();
                priorityCount++;
            }
        }
    }
}

