using System;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class Binder
    {
        readonly protected Type _contractType;
        readonly protected DiContainer _container;

        public Binder(
            DiContainer container,
            Type contractType)
        {
            _container = container;
            _contractType = contractType;
        }

        public virtual BindingConditionSetter ToProvider(ProviderBase provider)
        {
            _container.RegisterProvider(provider, _contractType);
            return new BindingConditionSetter(provider);
        }
    }

    public class BinderGeneric<TContract> : Binder
    {
        public BinderGeneric(DiContainer container)
            : base(container, typeof(TContract))
        {
        }

        public BindingConditionSetter ToLookup<TConcrete>() where TConcrete : TContract
        {
            return ToMethod(c => c.Resolve<TConcrete>());
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
