using System;
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
                    typeof(T).GetPrettyName(), _container.GetCurrentObjectGraph()));

            return obj;
        }
    }
}
