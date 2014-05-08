using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    public abstract class Installer
    {
        protected DiContainer _container;

        public DiContainer Container
        {
            set
            {
                _container = value;
            }
        }

        public abstract void RegisterBindings();

        protected void Install<TInstaller>() where TInstaller : Installer, new()
        {
            var subInstaller = new TInstaller();
            subInstaller.Container = _container;
            subInstaller.RegisterBindings();
        }
    }
}

