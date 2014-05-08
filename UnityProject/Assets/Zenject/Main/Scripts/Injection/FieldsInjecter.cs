using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ModestTree.Zenject
{
    // Iterate over fields/properties on a given object and inject any with the [Inject] attribute
    public class FieldsInjecter
    {
        public static void Inject(DiContainer container, object injectable)
        {
            Inject(container, injectable, Enumerable.Empty<object>());
        }

        public static void Inject(DiContainer container, object injectable, IEnumerable<object> additional)
        {
            Inject(container, injectable, additional, false);
        }

        public static void Inject(DiContainer container, object injectable, IEnumerable<object> additional, bool shouldUseAll)
        {
            Assert.That(injectable != null);

            var additionalCopy = additional.ToList();

            var injectableMembers = GetInjectableMembers(injectable.GetType());

            foreach (var memberInfo in injectableMembers)
            {
                bool didInject = InjectFromExtras(memberInfo, injectable, additionalCopy);

                if (!didInject)
                {
                    InjectFromResolve(memberInfo, container, injectable);
                }
            }

            if (shouldUseAll && !additionalCopy.IsEmpty())
            {
                throw new ZenjectResolveException(
                    "Passed unnecessary parameters when injecting into type '{0}'. \nExtra Parameters: {1}\nObject graph:\n{2}",
                    injectable.GetType().GetPrettyName(),
                    String.Join(",", additionalCopy.Select(x => x.GetType().GetPrettyName()).ToArray()),
                    container.GetCurrentObjectGraph());
            }
        }

        static bool InjectFromExtras(
            InjectableMember memberInfo,
            object injectable, List<object> additional)
        {
            foreach (object obj in additional)
            {
                if (memberInfo.MemberType.IsAssignableFrom(obj.GetType()))
                {
                    memberInfo.Setter(injectable, obj);
                    additional.Remove(obj);
                    return true;
                }
            }

            return false;
        }

        static void InjectFromResolve(
            InjectableMember memberInfo, DiContainer container, object injectable)
        {
            var context = new ResolveContext()
            {
                Target = injectable.GetType(),
                FieldName = memberInfo.MemberName,
                Identifier = memberInfo.InjectInfo.Identifier,
                Parents = container.LookupsInProgress.ToList(),
                TargetInstance = injectable,
            };

            var valueObj = ResolveFromType(
                container, context, injectable, memberInfo);

            memberInfo.Setter(injectable, valueObj);
        }

        static IEnumerable<InjectableMember> GetInjectableMembers(Type injectableType)
        {
            foreach (var fieldInfo in InjectionInfoHelper.GetFieldDependencies(injectableType))
            {
                yield return new InjectableMember()
                {
                    MemberType = fieldInfo.FieldType,
                    MemberName = fieldInfo.Name,
                    Setter = ((object injectable, object value) => fieldInfo.SetValue(injectable, value)),
                    InjectInfo = InjectionInfoHelper.GetInjectInfo(fieldInfo),
                };
            }

            foreach (var propInfo in InjectionInfoHelper.GetPropertyDependencies(injectableType))
            {
                yield return new InjectableMember()
                {
                    MemberType = propInfo.PropertyType,
                    MemberName = propInfo.Name,
                    Setter = ((object injectable, object value) => propInfo.SetValue(injectable, value, null)),
                    InjectInfo = InjectionInfoHelper.GetInjectInfo(propInfo),
                };
            }
        }

        static object ResolveFromType(
            DiContainer container, ResolveContext context, object injectable, InjectableMember injectableMember)
        {
            if (container.HasBinding(injectableMember.MemberType, context))
            {
                return container.Resolve(injectableMember.MemberType, context);
            }

            // If it's a list it might map to a collection
            if (ReflectionUtil.IsGenericList(injectableMember.MemberType))
            {
                var subType = injectableMember.MemberType.GetGenericArguments().Single();
                return container.ResolveMany(subType, context, injectableMember.InjectInfo.Optional);
            }

            if (!injectableMember.InjectInfo.Optional)
            {
                throw new ZenjectResolveException(
                    "Unable to find field with type '{0}' when injecting dependencies into '{1}'. \nObject graph:\n {2}",
                    injectableMember.MemberType, injectable, container.GetCurrentObjectGraph());
            }

            return null;
        }

        class InjectableMember
        {
            public Type MemberType;
            public string MemberName;
            public Action<object, object> Setter;
            public InjectInfo InjectInfo;
        }
    }
}
