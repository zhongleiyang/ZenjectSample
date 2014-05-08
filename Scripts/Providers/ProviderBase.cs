using System;
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

        public abstract object GetInstance();
        public abstract Type GetInstanceType();

        public virtual void Dispose()
        {
        }
    }
}
