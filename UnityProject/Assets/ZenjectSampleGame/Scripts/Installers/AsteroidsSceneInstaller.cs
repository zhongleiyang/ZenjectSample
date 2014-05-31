using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Collections;
using ModestTree.Zenject;
using Fasterflect;

namespace ModestTree.Asteroids
{
    public class AsteroidsSceneInstaller : MonoBehaviour, ISceneInstaller
    {
        public AsteroidsSettings Settings;

        public void InstallModules(DiContainer container)
        {
            container.Bind<AsteroidsMainModule.Settings>().To(Settings.Main);

            container.Bind<Module>().ToSingle<StandardUnityModule>();
            container.Bind<Module>().ToSingle<InitializablePrioritiesModule>();
            container.Bind<Module>().ToSingle<TickablePrioritiesModule>();

            container.Bind<Module>().ToSingle<AsteroidsMainModule>();

            container.Bind<List<Type>>().To(Initializables)
                .WhenInjectedInto<InitializablePrioritiesModule>();

            container.Bind<List<Type>>().To(Tickables)
                .WhenInjectedInto<TickablePrioritiesModule>();
        }

        static List<Type> Initializables = new List<Type>()
        {
            // Re-arrange this list to control init order
            typeof(GameController),
        };

        static List<Type> Tickables = new List<Type>()
        {
            // Re-arrange this list to control update order
            typeof(AsteroidManager),
            typeof(GameController),
        };
    }
}
