using System;
using System.Linq;

namespace ModestTree.Zenject
{
    public class StandardUnityModule : IModule
    {
        // Install basic functionality for most unity apps
        public void AddBindings(DiContainer container)
        {
            container.Bind<UnityKernel>().ToSingleGameObject();

            container.Bind<UnityEventManager>().ToSingleGameObject();
            container.Bind<GameObjectInstantiator>().ToSingle();

            container.Bind<StandardKernel>().ToSingle();
            // Uncomment this once you remove dependency in PlayerSandboxWrapper
            //container.Bind<StandardKernel>().ToTransient().WhenInjectedInto<UnityKernel>();

            container.Bind<InitializableHandler>().ToSingle();
            container.Bind<ITickable>().ToLookup<UnityEventManager>();
        }
    }
}
