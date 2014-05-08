using System;
using UnityEngine;
using System.Collections;
using ModestTree.Zenject;

namespace ModestTree.Asteroids
{
    public class SettingsInstallerWrapper : InstallerMonoBehaviourWrapper<SettingsInstaller>
    {
    }

    [Serializable]
    public class SettingsInstaller : Installer
    {
        public ShipStateMoving.Settings ShipMoving;
        public ShipStateDead.Settings ShipDead;
        public ShipStateWaitingToStart.Settings ShipStarting;

        public AsteroidManager.Settings AsteroidSpawner;
        public Asteroid.Settings Asteroid;

        public override void RegisterBindings()
        {
            _container.Bind<ShipStateMoving.Settings>().ToSingle(ShipMoving);
            _container.Bind<ShipStateDead.Settings>().ToSingle(ShipDead);
            _container.Bind<ShipStateWaitingToStart.Settings>().ToSingle(ShipStarting);

            _container.Bind<AsteroidManager.Settings>().ToSingle(AsteroidSpawner);
            _container.Bind<Asteroid.Settings>().ToSingle(Asteroid);
        }
    }
}
