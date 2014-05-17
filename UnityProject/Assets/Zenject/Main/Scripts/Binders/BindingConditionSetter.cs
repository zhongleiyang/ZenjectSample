using System;
using System.Collections.Generic;
using System.Linq;

namespace ModestTree.Zenject
{
    public delegate bool BindingCondition(ResolveContext c);

    public class BindingConditionSetter
    {
        readonly ProviderBase _provider;

        public BindingConditionSetter(ProviderBase provider)
        {
            _provider = provider;
        }

        public void When(BindingCondition condition)
        {
            _provider.SetCondition(condition);
        }

        public void WhenInjectedIntoInstance(object instance)
        {
            _provider.SetCondition(r => ReferenceEquals(r.EnclosingInstance, instance));
        }

        public void WhenInjectedInto(params Type[] targets)
        {
            _provider.SetCondition(r => targets.Contains(r.EnclosingType));
        }

        public void WhenInjectedInto<T>()
        {
            _provider.SetCondition(r => r.EnclosingType == typeof(T));
        }

        public void WhenInjectedInto<T>(object identifier)
        {
            Assert.IsNotNull(identifier);
            _provider.SetCondition(r => r.EnclosingType == typeof(T) && identifier.Equals(r.Identifier));
        }

        public void WhenInjectedInto(object identifier)
        {
            Assert.IsNotNull(identifier);
            _provider.SetCondition(r => identifier.Equals(r.Identifier));
        }
    }
}
