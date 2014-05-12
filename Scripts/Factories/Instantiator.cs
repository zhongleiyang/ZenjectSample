using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace ModestTree.Zenject
{
    public static class Instantiator
    {
        public static T Instantiate<T>(
            DiContainer container, params object[] constructorArgs)
        {
            return (T)Instantiate(container, typeof(T), constructorArgs);
        }

        public static object Instantiate(
            DiContainer container, Type concreteType, params object[] constructorArgs)
        {
            using (container.PushLookup(concreteType))
            {
                return InstantiateInternal(container, concreteType, constructorArgs);
            }
        }

        static object InstantiateInternal(
            DiContainer container, Type concreteType, params object[] constructorArgs)
        {
            ConstructorInfo method;
            var parameterInfos = InjectionInfoHelper.GetConstructorDependencies(concreteType, out method);

            var parameters = new List<object>();
            var extrasList = new List<object>(constructorArgs);

            Assert.That(!extrasList.Contains(null),
                "Null value given to factory constructor arguments. This is currently not allowed");

            foreach (var paramInfo in parameterInfos)
            {
                var found = false;
                var desiredType = paramInfo.ParameterType;

                foreach (var extra in extrasList)
                {
                    if (extra == null && !desiredType.IsValueType || desiredType.IsAssignableFrom(extra.GetType()))
                    {
                        found = true;
                        parameters.Add(extra);
                        extrasList.Remove(extra);
                        break;
                    }
                }

                if (!found)
                {
                    var injectInfo = InjectionInfoHelper.GetInjectInfo(paramInfo);

                    var context = new ResolveContext()
                    {
                        Target = concreteType,
                        FieldName = paramInfo.Name,
                        Identifier = injectInfo.Identifier,
                        Parents = new List<Type>(container.LookupsInProgress)
                    };

                    // Dependencies that are lists are only optional if declared as such using the inject attribute
                    bool isOptional = injectInfo.Optional;

                    object param = null;

                    if (container.HasBinding(paramInfo.ParameterType, context))
                    {
                        param = container.Resolve(paramInfo.ParameterType, context);
                    }
                    else if (ReflectionUtil.IsGenericList(desiredType))
                    // If it's a list it might map to a collection
                    {
                        var subType = desiredType.GetGenericArguments().Single();
                        param = container.ResolveMany(subType, context, isOptional);
                    }

                    if (param == null && !isOptional)
                    {
                        throw new ZenjectResolveException(
                            "Unable to find parameter with type '{0}' while constructing '{1}'.\nObject graph:\n{2}",
                            paramInfo.ParameterType.GetPrettyName(), concreteType.GetPrettyName(), container.GetCurrentObjectGraph());
                    }

                    parameters.Add(param);
                }
            }

            object newObj;

            try
            {
                newObj = method.Invoke(parameters.ToArray());
            }
            catch (Exception e)
            {
                throw new ZenjectGeneralException(
                    e, "Error occurred while instantiating object with type '" + concreteType.GetPrettyName() + "'");
            }

            FieldsInjecter.Inject(container, newObj, extrasList, true);

            return newObj;
        }
    }
}
