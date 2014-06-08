using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using Fasterflect;

namespace ModestTree.Zenject
{
    public class TickablePrioritiesInstaller : Installer
    {
        List<Type> _tickables;

        public TickablePrioritiesInstaller(List<Type> tickables)
        {
            _tickables = tickables;
        }

        public override void InstallBindings()
        {
            int priorityCount = 1;

            foreach (var tickableType in _tickables)
            {
                Assert.That(tickableType.DerivesFrom<ITickable>(),
                    "Expected type '{0}' to derive from ITickable", tickableType.Name());

                _container.Bind<Tuple<Type, int>>().To(Tuple.New(tickableType, priorityCount)).WhenInjectedInto<StandardKernel>();
                priorityCount++;
            }
        }
    }
}
