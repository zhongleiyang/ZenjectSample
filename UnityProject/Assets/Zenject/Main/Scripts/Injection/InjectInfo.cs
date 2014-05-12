using System;
using System.Collections.Generic;
using System.Reflection;
using Fasterflect;
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
            var info = new InjectInfo();

            info.Optional = param.HasAttribute(typeof(InjectOptionalAttribute));

            var injectAttr = param.Attribute<InjectAttribute>();

            if (injectAttr != null)
            {
                info.Identifier = injectAttr.Identifier;
            }

            return info;
        }

        public static InjectInfo GetInjectInfo(MemberInfo member)
        {
            var info = new InjectInfo();

            info.Optional = member.HasAttribute(typeof(InjectOptionalAttribute));

            var injectAttr = member.Attribute<InjectAttribute>();

            if (injectAttr != null)
            {
                info.Identifier = injectAttr.Identifier;
            }

            return info;
        }

        public static IEnumerable<MethodInfo> GetPostInjectMethods(Type type)
        {
            return type.MethodsWith(
                Fasterflect.Flags.InstanceAnyVisibility | Fasterflect.Flags.ExcludeBackingMembers,
                typeof(PostInjectAttribute));
        }

        public static IEnumerable<FieldInfo> GetFieldDependencies(Type type)
        {
            return type.FieldsWith(
                Fasterflect.Flags.InstanceAnyVisibility, typeof(InjectAttribute), typeof(InjectOptionalAttribute));
        }

        public static IEnumerable<PropertyInfo> GetPropertyDependencies(Type type)
        {
            return type.PropertiesWith(
                Fasterflect.Flags.InstanceAnyVisibility | Fasterflect.Flags.ExcludeBackingMembers,
                typeof(InjectAttribute), typeof(InjectOptionalAttribute));
        }

        public static IList<ParameterInfo> GetConstructorDependencies(Type concreteType)
        {
            return GetConstructorDependencies(concreteType, true);
        }

        public static IList<ParameterInfo> GetConstructorDependencies(Type concreteType, bool strict)
        {
            ConstructorInfo method;
            return GetConstructorDependencies(concreteType, out method, strict);
        }

        public static IList<ParameterInfo> GetConstructorDependencies(Type concreteType, out ConstructorInfo method)
        {
            return GetConstructorDependencies(concreteType, out method, true);
        }

        public static IList<ParameterInfo> GetConstructorDependencies(Type concreteType, out ConstructorInfo method, bool strict)
        {
            var constructors = concreteType.Constructors(Flags.Public | Flags.InstanceAnyVisibility);

            if (constructors.IsEmpty())
            {
                method = null;
                return new List<ParameterInfo>();
            }

            if (constructors.HasMoreThan(1))
            {
                method = (from c in constructors where c.HasAnyAttribute(typeof(InjectAttribute)) select c).SingleOrDefault();

                if (!strict && method == null)
                {
                    return new List<ParameterInfo>();
                }

                Assert.IsNotNull(method,
                    "More than one constructor found for type '{0}' when creating dependencies.  Use [Inject] attribute to specify which to use.", concreteType);
            }
            else
            {
                method = constructors[0];
            }

            return method.Parameters();
        }
    }
}
