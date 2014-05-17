using System;
using System.Collections.Generic;
using System.Linq;
using Fasterflect;

namespace ModestTree.Zenject
{
    public class MethodProvider<T> : ProviderBase
    {
        readonly DiContainer _container;
        readonly Func<DiContainer, T> _method;

        public MethodProvider(Func<DiContainer, T> method, DiContainer container)
        {
            _method = method;
            _container = container;
        }

        public override Type GetInstanceType()
        {
            return typeof(T);
        }

        public override object GetInstance()
        {
            var obj = _method(_container);

            Assert.That(obj != null, () =>
                String.Format(
                    "Method provider returned null when looking up type '{0}'. \nObject graph:\n{1}",
                    typeof(T).Name(), _container.GetCurrentObjectGraph()));

            return obj;
        }

        public override void ValidateBinding()
        {
            // Can't validate method bindings so just assume its valid
        }
    }
}
