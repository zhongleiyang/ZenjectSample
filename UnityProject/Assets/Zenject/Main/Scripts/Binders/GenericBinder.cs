using System;

namespace ModestTree.Zenject
{
    public class GenericBinder : Binder
    {
        readonly protected SingletonProviderMap _singletonMap;

        public GenericBinder(
            DiContainer container, Type contractType, SingletonProviderMap singletonMap)
            : base(container, contractType)
        {
            Assert.That(contractType.IsOpenGenericType(), "Expected open generic type in GenericBinder");
            _singletonMap = singletonMap;
        }

        public BindingConditionSetter ToTransient()
        {
            return ToProvider(new TransientProvider(_container, _contractType));
        }

        public BindingConditionSetter ToSingle()
        {
            return ToProvider(_singletonMap.CreateProvider(_contractType));
        }
    }
}
