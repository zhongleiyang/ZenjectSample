using System;
using System.Linq;

namespace ModestTree.Zenject
{
    public class StandardUnityInstaller : Installer
    {
        // Install basic functionality for most unity apps
        public override void InstallBindings()
        {
            _container.Bind<UnityKernel>().ToSingleGameObject();

            _container.Bind<UnityEventManager>().ToSingleGameObject();
            _container.Bind<GameObjectInstantiator>().ToSingle();

            _container.Bind<StandardKernel>().ToSingle();
            // TODO: Do this instead:
            //_container.Bind<IKernel>().ToTransient<StandardKernel>();

            _container.Bind<InitializableHandler>().ToSingle();
            _container.Bind<ITickable>().ToLookup<UnityEventManager>();
        }
    }
}
