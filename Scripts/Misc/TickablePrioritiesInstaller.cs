using System;
using System.Collections.Generic;
using ModestTree.Zenject;

namespace ModestTree.Zenject
{
    [Serializable]
    public class TickablePrioritiesInstaller : Installer
    {
        public List<Type> Tickables;

        public override void RegisterBindings()
        {
            Assert.IsNotNull(Tickables);
            int priorityCount = 1;

            foreach (var tickableType in Tickables)
            {
                Assert.That(tickableType.DerivesFrom<ITickable>(),
                    "Expected type '{0}' to derive from ITickable", tickableType.GetPrettyName());

                _container.Bind<Tuple<Type, int>>().ToSingle(Tuple.New(tickableType, priorityCount)).WhenInjectedInto<StandardKernel>();
                priorityCount++;
            }
        }
    }
}

