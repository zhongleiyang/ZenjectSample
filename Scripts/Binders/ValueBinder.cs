using System;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class ValueBinder<TContract> : Binder<TContract> where TContract : struct
    {
        public ValueBinder(DiContainer container, SingletonProviderMap singletonMap)
            : base(container, singletonMap)
        {
        }

        public BindingConditionSetter To(TContract value)
        {
            return ToProvider(new InstanceProvider(typeof(TContract), value));
        }

        public override BindingConditionSetter ToProvider(ProviderBase provider)
        {
            var conditionSetter = base.ToProvider(provider);

            // Also bind to nullable primitives
            // this is useful so that we can have optional primitive dependencies
            _container.RegisterProvider<Nullable<TContract>>(provider);

            return conditionSetter;
        }
    }
}
