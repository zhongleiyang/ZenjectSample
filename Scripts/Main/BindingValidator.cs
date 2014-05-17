using System;
using System.Collections.Generic;
using System.Linq;
using Fasterflect;

namespace ModestTree.Zenject
{
    internal static class BindingValidator
    {
        public static void ValidateContract(DiContainer container, Type contractType)
        {
            ValidateContract(
                container, contractType, new ResolveContext(contractType));
        }

        public static void ValidateContract(
            DiContainer container, Type contractType, ResolveContext context)
        {
            ValidateContract(container, contractType, context, false);
        }

        public static void ValidateContract(
            DiContainer container, Type contractType, ResolveContext context, bool isOptional)
        {
            var matches = container.GetProviderMatches(contractType, context);

            bool isValid = true;

            if (matches.IsLength(1))
            {
                matches.Single().ValidateBinding();
            }
            else
            {
                if (ReflectionUtil.IsGenericList(contractType))
                {
                    var subType = contractType.GetGenericArguments().Single();

                    matches = container.GetProviderMatches(subType, context);

                    if (matches.IsEmpty())
                    {
                        isValid = isOptional;
                    }
                    else
                    {
                        foreach (var match in matches)
                        {
                            match.ValidateBinding();
                        }
                    }
                }
                else
                {
                    isValid = isOptional;
                }
            }

            if (!isValid)
            {
                throw new ZenjectResolveException(
                    "Could not find required dependency with type '{0}' \nObject graph:\n{1}",
                    contractType.Name(), container.GetCurrentObjectGraph());
            }
        }

        public static void ValidateCanCreateConcrete(DiContainer container, Type concreteType)
        {
            using (container.PushLookup(concreteType))
            {
                var dependencies = InjectablesFinder.GetAllInjectables(concreteType);

                foreach (var dependInfo in dependencies)
                {
                    Assert.IsEqual(dependInfo.EnclosingType, concreteType);

                    var context = new ResolveContext(
                        dependInfo, container.LookupsInProgress.ToList(), null);

                    ValidateContract(
                        container, dependInfo.ContractType, context, dependInfo.Optional);
                }
            }
        }
    }
}
