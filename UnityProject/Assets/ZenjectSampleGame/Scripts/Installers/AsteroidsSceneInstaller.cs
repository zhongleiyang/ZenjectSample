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

            container.Bind<IModule>().ToSingle<StandardUnityModule>();
            container.Bind<IModule>().ToSingle<InitializablePrioritiesModule>();
            container.Bind<IModule>().ToSingle<TickablePrioritiesModule>();

            container.Bind<IModule>().ToSingle<AsteroidsMainModule>();

            container.Bind<List<Type>>().To(Initializables)
                .WhenInjectedInto<InitializablePrioritiesModule>();

            container.Bind<List<Type>>().To(Tickables)
                .WhenInjectedInto<TickablePrioritiesModule>();
        }

        // We can select this menu item to quickly validate the object graphs in our zenject app
        // before running it
        // Every time we change an installer we can quickly hit CTRL+SHIFT+V to validate the scene
        // before running it
        // If your team has a CI server this can also be included for automated testing!
        [MenuItem("Asteroids/Validate Object Graph #%v")]
        public static void ValidateObjectGraph()
        {
            ZenUtil.ValidateSceneInstaller<AsteroidsSceneInstaller>(typeof(Asteroid));
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
