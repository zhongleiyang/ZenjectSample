using System;

namespace ModestTree.Zenject
{
    public class InjectAttributeBase : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class InjectAttribute : InjectAttributeBase
    {
        public InjectAttribute(object identifier)
        {
            Identifier = identifier;
        }

        public InjectAttribute()
        {
        }

        public object Identifier
        {
            get;
            private set;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class InjectOptionalAttribute : InjectAttributeBase
    {
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PostInjectAttribute : Attribute
    {
    }
}

