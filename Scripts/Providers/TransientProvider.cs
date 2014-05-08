using System;
namespace ModestTree.Zenject
{
    public class TransientProvider<T> : ProviderBase
    {
        readonly IFactory<T> _factory;

        public TransientProvider(DiContainer container)
        {
            _factory = new Factory<T>(container);
        }

        public override Type GetInstanceType()
        {
            return typeof(T);
        }

        public override object GetInstance()
        {
            var obj = _factory.Create();
            Assert.That(obj != null);
            return obj;
        }
    }
}
