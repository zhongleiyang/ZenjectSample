using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    public interface IModule
    {
        void AddBindings(DiContainer container);
    }

    // Helper class if you want to store the container as a member
    // to save you some parameter passing
    public abstract class Module : IModule
    {
        protected DiContainer _container;

        public void AddBindings(DiContainer container)
        {
            _container = container;
            AddBindings();
        }

        public abstract void AddBindings();
    }
}

