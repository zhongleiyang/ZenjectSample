using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Fasterflect;

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

            var injectInfos = InjectablesFinder.GetFieldAndPropertyInjectables(injectable.GetType());

            foreach (var injectInfo in injectInfos)
            {
                bool didInject = InjectFromExtras(injectInfo, injectable, additionalCopy);

                if (!didInject)
                {
                    InjectFromResolve(injectInfo, container, injectable);
                }
            }

            if (shouldUseAll && !additionalCopy.IsEmpty())
            {
                throw new ZenjectResolveException(
                    "Passed unnecessary parameters when injecting into type '{0}'. \nExtra Parameters: {1}\nObject graph:\n{2}",
                    injectable.GetType().Name(),
                    String.Join(",", additionalCopy.Select(x => x.GetType().Name()).ToArray()),
                    container.GetCurrentObjectGraph());
            }

            foreach (var methodInfo in InjectablesFinder.GetPostInjectMethods(injectable.GetType()))
            {
                methodInfo.Invoke(injectable, new object[0]);
            }
        }

        static bool InjectFromExtras(
            InjectableInfo injectInfo,
            object injectable, List<object> additional)
        {
            foreach (object obj in additional)
            {
                if (injectInfo.ContractType.IsAssignableFrom(obj.GetType()))
                {
                    Assert.IsNotNull(injectInfo.Setter);

                    injectInfo.Setter(injectable, obj);
                    additional.Remove(obj);
                    return true;
                }
            }

            return false;
        }

        static void InjectFromResolve(
            InjectableInfo injectInfo, DiContainer container, object targetInstance)
        {
            var valueObj = container.Resolve(injectInfo, targetInstance);

            Assert.IsNotNull(injectInfo.Setter);

            injectInfo.Setter(targetInstance, valueObj);
        }
    }
}
