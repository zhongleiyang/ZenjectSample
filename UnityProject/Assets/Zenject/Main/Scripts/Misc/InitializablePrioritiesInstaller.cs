using System;
using System.Collections.Generic;
using ModestTree.Zenject;

namespace ModestTree.Zenject
{
    [Serializable]
    public class InitializablePrioritiesInstaller : Installer
    {
        public List<Type> Initializables;

        public override void RegisterBindings()
        {
            Assert.IsNotNull(Initializables);
            int priorityCount = 1;

            foreach (var initializableType in Initializables)
            {
                Assert.That(initializableType.DerivesFrom<IInitializable>(),
                    "Expected type '{0}' to derive from IInitializable", initializableType.GetPrettyName());

                _container.Bind<Tuple<Type, int>>().ToSingle(Tuple.New(initializableType, priorityCount)).WhenInjectedInto<InitializableHandler>();
                priorityCount++;
            }
        }
    }
}


