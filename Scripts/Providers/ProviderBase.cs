using System;
using System.Collections.Generic;
namespace ModestTree.Zenject
{
    public abstract class ProviderBase : IDisposable
    {
        BindingCondition _condition;

        public bool Matches(ResolveContext ctx)
        {
            if (_condition == null)
            {
                // By default do not match if the target is named
                // and we do not have a condition
                return ctx.Identifier == null;
            }

            return _condition(ctx);
        }

        public void SetCondition(BindingCondition condition)
        {
            _condition = condition;
        }

        // Returns true if this provider already has an instance to return
        // and false in the case where the provider would create it next time
        // GetInstance is called
        public abstract bool HasInstance();

        public abstract object GetInstance();
        public abstract Type GetInstanceType();

        public abstract IEnumerable<ZenjectResolveException> ValidateBinding();

        public virtual void Dispose()
        {
        }
    }
}
