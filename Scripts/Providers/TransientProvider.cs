using System;
using System.Collections.Generic;
using System.Linq;
namespace ModestTree.Zenject
{
    public class TransientProvider<T> : ProviderBase
    {
        readonly IFactory<T> _factory;
        readonly DiContainer _container;

        public TransientProvider(DiContainer container)
        {
            _factory = new Factory<T>(container);
            _container = container;
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

        public override void ValidateBinding()
        {
            BindingValidator.ValidateCanCreateConcrete(_container, typeof(T));
        }
    }
}
