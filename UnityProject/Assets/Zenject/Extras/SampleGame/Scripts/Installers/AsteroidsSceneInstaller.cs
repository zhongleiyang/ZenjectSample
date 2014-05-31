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
            // This module is needed for all unity projects (Yes, Zenject can support non-unity projects)
            container.Bind<Module>().ToSingle<StandardUnityModule>();

            // In this example there isn't many modules besides the main game module,
            // but in larger projects you will likely end up with many different re-usable modules
            // that you'll want to use in several different installers
            container.Bind<Module>().ToSingle<AsteroidsMainModule>();
            container.Bind<AsteroidsMainModule.Settings>().To(Settings.Main);

            InitPriorities(container);
        }

        // We don't need to include these bindings but often its nice to have
        // control over initialization-order and update-order
        void InitPriorities(DiContainer container)
        {
            container.Bind<Module>().ToSingle<InitializablePrioritiesModule>();
            container.Bind<List<Type>>().To(Initializables)
                .WhenInjectedInto<InitializablePrioritiesModule>();

            container.Bind<Module>().ToSingle<TickablePrioritiesModule>();
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
