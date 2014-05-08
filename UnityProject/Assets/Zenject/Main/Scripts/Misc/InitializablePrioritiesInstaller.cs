using System;
using System.Collections.Generic;
using ModestTree.Zenject;

namespace ModestTree.Zenject
{
    [Serializable]
    public class InitializablePrioritiesInstaller : Installer
    {
        public List<Type> _initializables;

        public InitializablePrioritiesInstaller(
            DiContainer container, List<Type> initializables)
            : base(container)
        {
            _initializables = initializables;
        }

        public override void RegisterBindings()
        {
            int priorityCount = 1;

            foreach (var initializableType in _initializables)
            {
                Assert.That(initializableType.DerivesFrom<IInitializable>(),
                    "Expected type '{0}' to derive from IInitializable", initializableType.GetPrettyName());

                _container.Bind<Tuple<Type, int>>().ToSingle(Tuple.New(initializableType, priorityCount)).WhenInjectedInto<InitializableHandler>();
                priorityCount++;
            }
        }
    }
}


