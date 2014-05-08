using System;
using System.Collections.Generic;
using ModestTree.Zenject;

namespace ModestTree.Zenject
{
    [Serializable]
    public class TickablePrioritiesInstaller : Installer
    {
        List<Type> _tickables;

        public TickablePrioritiesInstaller(
            DiContainer container, List<Type> tickables)
            : base(container)
        {
            _tickables = tickables;
        }

        public override void RegisterBindings()
        {
            int priorityCount = 1;

            foreach (var tickableType in _tickables)
            {
                Assert.That(tickableType.DerivesFrom<ITickable>(),
                    "Expected type '{0}' to derive from ITickable", tickableType.GetPrettyName());

                _container.Bind<Tuple<Type, int>>().ToSingle(Tuple.New(tickableType, priorityCount)).WhenInjectedInto<StandardKernel>();
                priorityCount++;
            }
        }
    }
}

