using System;
using System.Collections.Generic;
using System.Reflection;
using Fasterflect;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    // Helper to find data that needs to be injected
    internal static class InjectablesFinder
    {
        public static IEnumerable<MethodInfo> GetPostInjectMethods(Type type)
        {
            return type.MethodsWith(
                Fasterflect.Flags.InstanceAnyVisibility | Fasterflect.Flags.ExcludeBackingMembers,
                typeof(PostInjectAttribute));
        }

        public static IEnumerable<InjectableInfo> GetAllInjectables(Type type)
        {
            return GetAllInjectables(type, true);
        }

        public static IEnumerable<InjectableInfo> GetAllInjectables(Type type, bool strict)
        {
            return GetConstructorInjectables(type, strict)
                .Concat(GetFieldAndPropertyInjectables(type));
        }

        public static IEnumerable<InjectableInfo> GetFieldAndPropertyInjectables(Type type)
        {
            return GetFieldInjectables(type).Concat(GetPropertyInjectables(type));
        }

        public static IEnumerable<InjectableInfo> GetPropertyInjectables(Type type)
        {
            var propInfos = type.PropertiesWith(
                Fasterflect.Flags.InstanceAnyVisibility | Fasterflect.Flags.ExcludeBackingMembers,
                typeof(InjectAttribute), typeof(InjectOptionalAttribute));

            foreach (var propInfo in propInfos)
            {
                yield return CreateForMember(propInfo, type);
            }
        }

        public static IEnumerable<InjectableInfo> GetFieldInjectables(Type type)
        {
            var fieldInfos = type.FieldsWith(
                Fasterflect.Flags.InstanceAnyVisibility, typeof(InjectAttribute), typeof(InjectOptionalAttribute));

            foreach (var fieldInfo in fieldInfos)
            {
                yield return CreateForMember(fieldInfo, type);
            }
        }

        public static IEnumerable<InjectableInfo> GetConstructorInjectables(Type enclosingType)
        {
            return GetConstructorInjectables(enclosingType, true);
        }

        public static IEnumerable<InjectableInfo> GetConstructorInjectables(
            Type enclosingType, bool strict)
        {
            ConstructorInfo method;
            return GetConstructorInjectables(enclosingType, out method, strict);
        }

        public static IEnumerable<InjectableInfo> GetConstructorInjectables(Type enclosingType, out ConstructorInfo method)
        {
            return GetConstructorInjectables(enclosingType, out method, true);
        }

        public static IList<InjectableInfo> GetConstructorInjectables(
            Type enclosingType, out ConstructorInfo method, bool strict)
        {
            var constructors = enclosingType.Constructors(Flags.Public | Flags.InstanceAnyVisibility);

            if (constructors.IsEmpty())
            {
                method = null;
                return new List<InjectableInfo>();
            }

            if (constructors.HasMoreThan(1))
            {
                method = (from c in constructors where c.HasAnyAttribute(typeof(InjectAttribute)) select c).SingleOrDefault();

                if (!strict && method == null)
                {
                    return new List<InjectableInfo>();
                }

                Assert.IsNotNull(method,
                    "More than one constructor found for type '{0}' when creating dependencies.  Use [Inject] attribute to specify which to use.".With(enclosingType));
            }
            else
            {
                method = constructors[0];
            }

            return method.Parameters().Select(paramInfo => CreateForConstructorParam(paramInfo, enclosingType)).ToList();
        }

        static InjectableInfo CreateForConstructorParam(
            ParameterInfo paramInfo, Type enclosingType)
        {
            var injectAttr = paramInfo.Attribute<InjectAttribute>();

            return new InjectableInfo()
            {
                Optional = paramInfo.HasAttribute(typeof(InjectOptionalAttribute)),
                Identifier = (injectAttr == null ? null : injectAttr.Identifier),
                SourceName = paramInfo.Name,
                ContractType = paramInfo.ParameterType,
                EnclosingType = enclosingType,
            };
        }

        static InjectableInfo CreateForMember(MemberInfo memInfo, Type enclosingType)
        {
            var injectAttr = memInfo.Attribute<InjectAttribute>();

            var info = new InjectableInfo()
            {
                Optional = memInfo.HasAttribute(typeof(InjectOptionalAttribute)),
                Identifier = (injectAttr == null ? null : injectAttr.Identifier),
                SourceName = memInfo.Name,
                EnclosingType = enclosingType,
            };

            if (memInfo is FieldInfo)
            {
                var fieldInfo = (FieldInfo)memInfo;
                info.Setter = ((object injectable, object value) => fieldInfo.SetValue(injectable, value));
                info.ContractType = fieldInfo.FieldType;
            }
            else
            {
                Assert.That(memInfo is PropertyInfo);
                var propInfo = (PropertyInfo)memInfo;
                info.Setter = ((object injectable, object value) => propInfo.SetValue(injectable, value, null));
                info.ContractType = propInfo.PropertyType;
            }

            return info;
        }
    }
}
