using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using ModestTree.Zenject;

namespace ModestTree.Asteroids
{
    public enum Cameras
    {
        Main,
    }

    // The main installer for Asteroids
    public class AsteroidsMainModule : Module
    {
        readonly Settings _settings;

        public AsteroidsMainModule(Settings settings)
        {
            _settings = settings;
        }

        public override void AddBindings()
        {
            // Root of our object graph
            _container.Bind<IDependencyRoot>().ToSingle<GameRoot>();

            // In this game there is only one camera so an enum isn't necessary
            // but used here to show how it would work if there were multiple
            _container.Bind<Camera>().ToSingle(_settings.MainCamera).WhenInjectedInto(Cameras.Main);

            _container.Bind<LevelHelper>().ToSingle();

            _container.Bind<ITickable>().ToSingle<AsteroidManager>();
            _container.Bind<AsteroidManager>().ToSingle();

            _container.Bind<GuiHandler>().ToSingle(_settings.Gui);

            _container.BindFactory<Asteroid>();
            // Every time a new asteroid is created, instantiate a new game object for it using the given prefab
            _container.Bind<AsteroidHooks>().ToTransientFromPrefab<AsteroidHooks>(_settings.Asteroid.Prefab).WhenInjectedInto<Asteroid>();

            _container.Bind<IInitializable>().ToSingle<GameController>();
            _container.Bind<ITickable>().ToSingle<GameController>();
            _container.Bind<GameController>().ToSingle();

            _container.Bind<ShipStateFactory>().ToSingle();

            _container.Bind<ShipHooks>().ToTransientFromPrefab<ShipHooks>(_settings.Ship.Prefab).WhenInjectedInto<Ship>();
            _container.Bind<Ship>().ToSingle();
            _container.Bind<ITickable>().ToSingle<Ship>();
            _container.Bind<IInitializable>().ToSingle<Ship>();

            InstallSettings();
        }

        void InstallSettings()
        {
            _container.Bind<ShipStateMoving.Settings>().ToSingle(_settings.Ship.StateMoving);
            _container.Bind<ShipStateDead.Settings>().ToSingle(_settings.Ship.StateDead);
            _container.Bind<ShipStateWaitingToStart.Settings>().ToSingle(_settings.Ship.StateStarting);

            _container.Bind<AsteroidManager.Settings>().ToSingle(_settings.Asteroid.Spawner);
            _container.Bind<Asteroid.Settings>().ToSingle(_settings.Asteroid.General);
        }

        [Serializable]
        public class Settings
        {
            public Camera MainCamera;
            public GuiHandler Gui;
            public ShipSettings Ship;
            public AsteroidSettings Asteroid;

            [Serializable]
            public class ShipSettings
            {
                public GameObject Prefab;
                public ShipStateMoving.Settings StateMoving;
                public ShipStateDead.Settings StateDead;
                public ShipStateWaitingToStart.Settings StateStarting;
            }

            [Serializable]
            public class AsteroidSettings
            {
                public GameObject Prefab;
                public AsteroidManager.Settings Spawner;
                public Asteroid.Settings General;
            }
        }
    }

    // - The root of the object graph for our main run config
    public class GameRoot : DependencyRootStandard
    {
        [Inject]
        public GuiHandler _guiHandler;
    }
}
