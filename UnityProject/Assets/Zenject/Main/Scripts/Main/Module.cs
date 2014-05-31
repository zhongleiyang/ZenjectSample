using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    public abstract class Module
    {
        protected DiContainer _container;

        public DiContainer Container
        {
            set
            {
                _container = value;
            }
        }

        public abstract void AddBindings();

        public virtual IEnumerable<ZenjectResolveException> ValidateSubGraphs()
        {
            // optional
            return Enumerable.Empty<ZenjectResolveException>();
        }

        // Helper method for ValidateSubGraphs
        protected IEnumerable<ZenjectResolveException> Validate<T>(params Type[] extraTypes)
        {
            return _container.ValidateObjectGraph<T>(extraTypes);
        }
    }
}
