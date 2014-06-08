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

        public abstract Type GetInstanceType();

        // Note: We pass in contractType for these methods for the case where
        // the contract type is used to determine what instance to return
        // (this is only needed for mapping contracts with generic parameters)

        // Returns true if this provider already has an instance to return
        // and false in the case where the provider would create it next time
        // GetInstance is called
        public abstract bool HasInstance(Type contractType);

        public abstract object GetInstance(Type contractType);

        public abstract IEnumerable<ZenjectResolveException> ValidateBinding();

        public virtual void Dispose()
        {
        }
    }
}
