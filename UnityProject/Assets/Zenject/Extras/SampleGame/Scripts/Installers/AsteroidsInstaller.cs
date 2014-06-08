using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using ModestTree.Zenject;
using Fasterflect;
using System.Linq;

namespace ModestTree.Asteroids
{
    public enum Cameras
    {
        Main,
    }


    public class AsteroidsInstaller : MonoInstaller
    {
        public Settings SceneSettings;

        public override void InstallBindings()
        {
            // Install any other re-usable installers
            InstallIncludes();
            // Install the main game
            InstallAsteroids();
            InstallSettings();
            InitPriorities(_container);
        }

        // In this example there is only one 'installer' but in larger projects you
        // will likely end up with many different re-usable installers
        // that you'll want to use in several different installers
        // To re-use an existing installer you can simply bind it to IInstaller like below
        // Note that this will only work if your installer is just a normal C# class
        // If it's a monobehaviour (that is, derived from MonoInstaller) then you would be
        // better off making it a prefab and then just including it in your scene to re-use it
        void InstallIncludes()
        {
            // This installer is needed for all unity projects (Yes, Zenject can support non-unity projects)
            _container.Bind<IInstaller>().ToSingle<StandardUnityInstaller>();
        }

        void InstallAsteroids()
        {
            // Root of our object graph
            _container.Bind<IDependencyRoot>().ToSingle<GameRoot>();

            // In this game there is only one camera so an enum isn't necessary
            // but used here to show how it would work if there were multiple
            _container.Bind<Camera>().ToSingle(SceneSettings.MainCamera).WhenInjectedInto(Cameras.Main);

            _container.Bind<LevelHelper>().ToSingle();

            _container.Bind<ITickable>().ToSingle<AsteroidManager>();
            _container.Bind<AsteroidManager>().ToSingle();

            _container.Bind<GuiHandler>().ToSingle(SceneSettings.Gui);

            _container.BindFactory<Asteroid>();
            // Every time a new asteroid is created, instantiate a new game object for it using the given prefab
            _container.Bind<AsteroidHooks>().ToTransientFromPrefab<AsteroidHooks>(SceneSettings.Asteroid.Prefab).WhenInjectedInto<Asteroid>();

            _container.Bind<IInitializable>().ToSingle<GameController>();
            _container.Bind<ITickable>().ToSingle<GameController>();
            _container.Bind<GameController>().ToSingle();

            _container.Bind<ShipStateFactory>().ToSingle();

            _container.Bind<ShipHooks>().ToTransientFromPrefab<ShipHooks>(SceneSettings.Ship.Prefab).WhenInjectedInto<Ship>();
            _container.Bind<Ship>().ToSingle();
            _container.Bind<ITickable>().ToSingle<Ship>();
            _container.Bind<IInitializable>().ToSingle<Ship>();
        }

        void InstallSettings()
        {
            _container.Bind<ShipStateMoving.Settings>().ToSingle(SceneSettings.Ship.StateMoving);
            _container.Bind<ShipStateDead.Settings>().ToSingle(SceneSettings.Ship.StateDead);
            _container.Bind<ShipStateWaitingToStart.Settings>().ToSingle(SceneSettings.Ship.StateStarting);

            _container.Bind<AsteroidManager.Settings>().ToSingle(SceneSettings.Asteroid.Spawner);
            _container.Bind<Asteroid.Settings>().ToSingle(SceneSettings.Asteroid.General);
        }

        // We don't need to include these bindings but often its nice to have
        // control over initialization-order and update-order
        void InitPriorities(DiContainer _container)
        {
            _container.Bind<IInstaller>().ToSingle<InitializablePrioritiesInstaller>();
            _container.Bind<List<Type>>().To(Initializables)
                .WhenInjectedInto<InitializablePrioritiesInstaller>();

            _container.Bind<IInstaller>().ToSingle<TickablePrioritiesInstaller>();
            _container.Bind<List<Type>>().To(Tickables)
                .WhenInjectedInto<TickablePrioritiesInstaller>();
        }

        // Here we override ValidateSubGraphs to indicate to Zenject the object graphs
        // that we are creating at run time
        // This isn't necessary but is good to do so that you can catch zenject errors
        // before running your unity scene
        // So in this case, if one of the dependencies of ShipStateDead was missing and
        // we didn't include it here, then we wouldn't run into this issue until we
        // played the game and then died
        // This way, we can simply either run Assets -> Zenject -> Validate or hit
        // CTRL+SHIFT+V and confirm that Zenject can generate our object graphs correctly
        public override IEnumerable<ZenjectResolveException> ValidateSubGraphs()
        {
            return Validate<Asteroid>().Concat(
                Validate<ShipStateDead>(typeof(Ship))).Concat(
                Validate<ShipStateMoving>(typeof(Ship))).Concat(
                Validate<ShipStateWaitingToStart>(typeof(Ship)));
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

        // - The root of the object graph for our main run config
        public class GameRoot : DependencyRootStandard
        {
            [Inject]
            public GuiHandler _guiHandler;
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
