using System;
using System.Collections.Generic;
using System.Linq;

namespace ModestTree.Zenject
{
    public class InstanceProvider : ProviderBase
    {
        readonly object _instance;
        readonly Type _instanceType;

        public InstanceProvider(Type instanceType, object instance)
        {
            Assert.That(instance == null || instance.GetType().DerivesFromOrEqual(instanceType));

            _instance = instance;
            _instanceType = instanceType;
        }

        public override Type GetInstanceType()
        {
            return _instanceType;
        }

        public override object GetInstance()
        {
            return _instance;
        }

        public override void ValidateBinding()
        {
        }
    }
}
