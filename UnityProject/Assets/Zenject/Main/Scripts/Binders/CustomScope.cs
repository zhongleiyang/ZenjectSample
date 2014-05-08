using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModestTree.Zenject
{
    // This class should ONLY be used the following way:
    //
    //  using (var scope = _container.CreateScope())
    //  {
    //      scope.Bind(playerWrapper);
    //      ...
    //      ...
    //      var bar = _container.Resolve<Foo>();
    //  }
    public class CustomScope : IDisposable
    {
        DiContainer _container;
        List<ProviderBase> _scopedProviders = new List<ProviderBase>();
        SingletonProviderMap _singletonMap;

        internal CustomScope(DiContainer container, SingletonProviderMap singletonMap)
        {
            _container = container;
            _singletonMap = singletonMap;
        }

        public ReferenceBinder<TContract> Bind<TContract>() where TContract : class
        {
            return new CustomScopeReferenceBinder<TContract>(this, _container, _singletonMap);
        }

        public ValueBinder<TContract> BindValue<TContract>() where TContract : struct
        {
            return new CustomScopeValueBinder<TContract>(this, _container, _singletonMap);
        }

        void AddProvider(ProviderBase provider)
        {
            Assert.That(!_scopedProviders.Contains(provider));
            _scopedProviders.Add(provider);
        }

        public void Dispose()
        {
            foreach (var provider in _scopedProviders)
            {
                _container.UnregisterProvider(provider);
            }
        }

        class CustomScopeValueBinder<TContract> : ValueBinder<TContract> where TContract : struct
        {
            CustomScope _owner;

            public CustomScopeValueBinder(
                CustomScope owner,
                DiContainer container, SingletonProviderMap singletonMap)
                : base(container, singletonMap)
            {
                _owner = owner;
            }

            public override BindingConditionSetter To(ProviderBase provider)
            {
                _owner.AddProvider(provider);
                return base.To(provider);
            }
        }

        class CustomScopeReferenceBinder<TContract> : ReferenceBinder<TContract> where TContract : class
        {
            CustomScope _owner;

            public CustomScopeReferenceBinder(
                CustomScope owner,
                DiContainer container, SingletonProviderMap singletonMap)
                : base(container, singletonMap)
            {
                _owner = owner;
            }

            public override BindingConditionSetter To(ProviderBase provider)
            {
                _owner.AddProvider(provider);
                return base.To(provider);
            }
        }
    }
}
