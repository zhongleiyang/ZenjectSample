using System;
using System.Collections.Generic;
using System.Linq;
using Fasterflect;

namespace ModestTree.Zenject
{
    internal static class BindingValidator
    {
        public static IEnumerable<ZenjectResolveException> ValidateContract(
            DiContainer container, Type contractType)
        {
            return ValidateContract(
                container, contractType, new ResolveContext(contractType));
        }

        public static IEnumerable<ZenjectResolveException> ValidateContract(
            DiContainer container, Type contractType, ResolveContext context)
        {
            return ValidateContract(container, contractType, context, false);
        }

        public static IEnumerable<ZenjectResolveException> ValidateContract(
            DiContainer container, Type contractType, ResolveContext context, bool isOptional)
        {
            var matches = container.GetProviderMatches(contractType, context);

            if (matches.IsLength(1))
            {
                foreach (var error in matches.Single().ValidateBinding())
                {
                    yield return error;
                }
            }
            else
            {
                if (ReflectionUtil.IsGenericList(contractType))
                {
                    var subType = contractType.GetGenericArguments().Single();

                    matches = container.GetProviderMatches(subType, context);

                    if (matches.IsEmpty())
                    {
                        if (!isOptional)
                        {
                            yield return new ZenjectResolveException(
                                "Could not find dependency with type 'List<{0}>' when injecting into '{1}.  If the empty list is also valid, you can allow this by using the [InjectOptional] attribute.' \nObject graph:\n{2}",
                                subType.Name(), context.EnclosingType.Name(), container.GetCurrentObjectGraph());
                        }
                    }
                    else
                    {
                        foreach (var match in matches)
                        {
                            foreach (var error in match.ValidateBinding())
                            {
                                yield return error;
                            }
                        }
                    }
                }
                else
                {
                    if (!isOptional)
                    {
                        if (matches.IsEmpty())
                        {
                            yield return new ZenjectResolveException(
                                "Could not find required dependency with type '{0}' when injecting into '{1}' \nObject graph:\n{2}",
                                contractType.Name(), context.EnclosingType.Name(), container.GetCurrentObjectGraph());
                        }
                        else
                        {
                            yield return new ZenjectResolveException(
                                "Found multiple matches when only one was expected for dependency with type '{0}' when injecting into '{1}' \nObject graph:\n{2}",
                                contractType.Name(), context.EnclosingType.Name(), container.GetCurrentObjectGraph());
                        }
                    }
                }
            }
        }

        public static IEnumerable<ZenjectResolveException> ValidateObjectGraph(
            DiContainer container, Type concreteType, params Type[] extras)
        {
            using (container.PushLookup(concreteType))
            {
                var dependencies = InjectablesFinder.GetAllInjectables(concreteType);
                var extrasList = extras.ToList();

                foreach (var dependInfo in dependencies)
                {
                    Assert.IsEqual(dependInfo.EnclosingType, concreteType);

                    if (TryTakingFromExtras(dependInfo.ContractType, extrasList))
                    {
                        continue;
                    }

                    var context = new ResolveContext(
                        dependInfo, container.LookupsInProgress.ToList(), null);

                    foreach (var error in ValidateContract(
                        container, dependInfo.ContractType, context, dependInfo.Optional))
                    {
                        yield return error;
                    }
                }

                if (!extrasList.IsEmpty())
                {
                    yield return new ZenjectResolveException(
                        "Found unnecessary extra parameters passed when injecting into '{0}' with types '{1}'.  \nObject graph:\n{2}",
                        concreteType.Name(),
                        String.Join(",", extrasList.Select(x => x.Name()).ToArray()),
                        container.GetCurrentObjectGraph());
                }
            }
        }

        static bool TryTakingFromExtras(Type contractType, List<Type> extrasList)
        {
            foreach (var extraType in extrasList)
            {
                if (extraType.DerivesFromOrEqual(contractType))
                {
                    extrasList.RemoveWithConfirm(extraType);
                    return true;
                }
            }

            return false;
        }
    }
}
