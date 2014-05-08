using System;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class Binder<TContract>
    {
        readonly protected DiContainer _container;
        readonly protected SingletonProviderMap _singletonMap;

        public Binder(DiContainer container, SingletonProviderMap singletonMap)
        {
            _container = container;
            _singletonMap = singletonMap;
        }

        public BindingConditionSetter ToLookup<TConcrete>() where TConcrete : TContract
        {
            return ToMethod(c => c.Resolve<TConcrete>());
        }

        public BindingConditionSetter ToFactory<TConcrete>() where TConcrete : IFactory<TContract>
        {
            return ToMethod(c => c.Resolve<TConcrete>().Create());
        }

        public virtual BindingConditionSetter To(ProviderBase provider)
        {
            _container.RegisterProvider<TContract>(provider);
            return new BindingConditionSetter(provider);
        }

        public BindingConditionSetter ToMethod(Func<DiContainer, TContract> method)
        {
            return To(new MethodProvider<TContract>(method, _container));
        }
    }
}
