using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    public class InjectInfo
    {
        public bool Optional;
        public object Identifier;
    }

    // Helper to find data that needs to be injected
    public static class InjectionInfoHelper
    {
        public static InjectInfo GetInjectInfo(ParameterInfo param)
        {
            return GetInjectInfoInternal(param.GetCustomAttributes(typeof(InjectAttributeBase), true));
        }

        public static InjectInfo GetInjectInfo(MemberInfo member)
        {
            return GetInjectInfoInternal(member.GetCustomAttributes(typeof(InjectAttributeBase), true));
        }

        static InjectInfo GetInjectInfoInternal(object[] attributes)
        {
            if (!attributes.Any())
            {
                return null;
            }

            var info = new InjectInfo();

            foreach (var attr in attributes)
            {
                if (attr.GetType() == typeof(InjectOptionalAttribute))
                {
                    info.Optional = true;
                }
                else if (attr.GetType() == typeof(InjectAttribute))
                {
                    var injectAttr = (InjectAttribute)attr;
                    info.Identifier = injectAttr.Identifier;
                }
            }

            return info;
        }

        public static IEnumerable<FieldInfo> GetFieldDependencies(Type type)
        {
            return type.GetFieldsWithAttribute<InjectAttributeBase>();
        }

        public static IEnumerable<PropertyInfo> GetPropertyDependencies(Type type)
        {
            return type.GetPropertiesWithAttribute<InjectAttributeBase>();
        }

        public static ParameterInfo[] GetConstructorDependencies(Type concreteType)
        {
            return GetConstructorDependencies(concreteType, true);
        }

        public static ParameterInfo[] GetConstructorDependencies(Type concreteType, bool strict)
        {
            ConstructorInfo method;
            return GetConstructorDependencies(concreteType, out method, strict);
        }

        public static ParameterInfo[] GetConstructorDependencies(Type concreteType, out ConstructorInfo method)
        {
            return GetConstructorDependencies(concreteType, out method, true);
        }

        public static ParameterInfo[] GetConstructorDependencies(Type concreteType, out ConstructorInfo method, bool strict)
        {
            var constructors = concreteType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            if (constructors.IsEmpty())
            {
                method = null;
                return new ParameterInfo[0];
            }

            if (constructors.Length > 1)
            {
                method = (from c in constructors where c.GetCustomAttributes(typeof(InjectAttribute), true).Any() select c).SingleOrDefault();

                if (!strict && method == null)
                {
                    return new ParameterInfo[0];
                }

                Assert.IsNotNull(method,
                    "More than one constructor found for type '{0}' when creating dependencies.  Use [Inject] attribute to specify which to use.", concreteType);
            }
            else
            {
                method = constructors[0];
            }

            return method.GetParameters();
        }
    }
}
