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

        public virtual BindingConditionSetter ToProvider(ProviderBase provider)
        {
            _container.RegisterProvider<TContract>(provider);
            return new BindingConditionSetter(provider);
        }

        public BindingConditionSetter ToMethod(Func<DiContainer, TContract> method)
        {
            return ToProvider(new MethodProvider<TContract>(method, _container));
        }

        public BindingConditionSetter ToGetter<TObj>(Func<TObj, TContract> method)
        {
            return ToMethod(c => method(c.Resolve<TObj>()));
        }
    }
}
