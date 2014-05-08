using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using ModestTree.Zenject;

namespace ModestTree.Asteroids
{
    // Mono behaviour wrapper so that GameInstaller can easily be dropped into the scene
    public class GameInstallerWrapper : InstallerMonoBehaviourWrapper<GameInstaller>
    {
    }

    public enum Cameras
    {
        Main,
    }

    [Serializable]
    // The main installer for Asteroids
    public class GameInstaller : Installer
    {
        public Camera MainCamera;
        public GameObject AsteroidPrefab;
        public GameObject ShipPrefab;
        public GuiHandler Gui;

        public override void RegisterBindings()
        {
            Install<StandardUnityInstaller>();

            // Root of our object graph
            _container.Bind<IDependencyRoot>().ToSingle<GameRoot>();

            // In this game there is only one camera so an enum isn't necessary
            // but used here to show how it would work if there were multiple
            _container.Bind<Camera>().ToSingle(MainCamera).WhenInjectedInto(Cameras.Main);

            _container.Bind<LevelHelper>().ToSingle();

            _container.Bind<ITickable>().ToSingle<AsteroidManager>();
            _container.Bind<AsteroidManager>().ToSingle();

            _container.Bind<GuiHandler>().ToSingle(Gui);

            _container.BindFactory<Asteroid>();
            // Every time a new asteroid is created, instantiate a new game object for it using the given prefab
            _container.Bind<AsteroidHooks>().ToTransientFromPrefab<AsteroidHooks>(AsteroidPrefab).WhenInjectedInto<Asteroid>();

            _container.Bind<IInitializable>().ToSingle<GameController>();
            _container.Bind<ITickable>().ToSingle<GameController>();
            _container.Bind<GameController>().ToSingle();

            _container.Bind<ShipStateFactory>().ToSingle();

            _container.Bind<ShipHooks>().ToTransientFromPrefab<ShipHooks>(ShipPrefab).WhenInjectedInto<Ship>();
            _container.Bind<Ship>().ToSingle();
            _container.Bind<ITickable>().ToSingle<Ship>();
            _container.Bind<IInitializable>().ToSingle<Ship>();

            new TickablePrioritiesInstaller(_container, Tickables).RegisterBindings();
            new InitializablePrioritiesInstaller(_container, Initializables).RegisterBindings();
        }

        static List<Type> Tickables = new List<Type>()
        {
            // Re-arrange this list to control update order
            typeof(AsteroidManager),
            typeof(GameController),
        };

        static List<Type> Initializables = new List<Type>()
        {
            // Re-arrange this list to control init order
            typeof(GameController),
        };
    }

    // - The root of the object graph for our main run config
    public class GameRoot : DependencyRootStandard
    {
        [Inject]
        public GuiHandler _guiHandler;
    }
}
