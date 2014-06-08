using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Fasterflect;

namespace ModestTree.Zenject
{
    public class Instantiator
    {
        readonly DiContainer _container;

        public Instantiator(DiContainer container)
        {
            _container = container;
        }

        public T Instantiate<T>(
            params object[] constructorArgs)
        {
            return (T)Instantiate(typeof(T), constructorArgs);
        }

        public object Instantiate(
            Type concreteType, params object[] constructorArgs)
        {
            using (_container.PushLookup(concreteType))
            {
                return InstantiateInternal(concreteType, constructorArgs);
            }
        }

        object InstantiateInternal(
            Type concreteType, params object[] constructorArgs)
        {
            ConstructorInfo method;
            var injectInfos = InjectablesFinder.GetConstructorInjectables(concreteType, out method);

            var paramValues = new List<object>();
            var extrasList = new List<object>(constructorArgs);

            Assert.That(!extrasList.Contains(null),
                "Null value given to factory constructor arguments. This is currently not allowed");

            foreach (var injectInfo in injectInfos)
            {
                var found = false;

                foreach (var extra in extrasList)
                {
                    if (extra.GetType().DerivesFromOrEqual(injectInfo.ContractType))
                    {
                        found = true;
                        paramValues.Add(extra);
                        extrasList.Remove(extra);
                        break;
                    }
                }

                if (!found)
                {
                    paramValues.Add(_container.Resolve(injectInfo));
                }
            }

            object newObj;

            try
            {
                newObj = method.Invoke(paramValues.ToArray());
            }
            catch (Exception e)
            {
                throw new ZenjectResolveException(
                    "Error occurred while instantiating object with type '{0}'".With(concreteType.Name()), e);
            }

            FieldsInjecter.Inject(_container, newObj, extrasList, true);

            return newObj;
        }

        object ResolveFromType(
            ResolveContext context, object injectable, InjectableInfo injectInfo)
        {
            if (_container.HasBinding(injectInfo.ContractType, context))
            {
                return _container.Resolve(injectInfo.ContractType, context);
            }

            // If it's a list it might map to a collection
            if (ReflectionUtil.IsGenericList(injectInfo.ContractType))
            {
                var subType = injectInfo.ContractType.GetGenericArguments().Single();
                return _container.ResolveMany(subType, context, injectInfo.Optional);
            }

            if (!injectInfo.Optional)
            {
                throw new ZenjectResolveException(
                    "Unable to find field with type '{0}' when injecting dependencies into '{1}'. \nObject graph:\n {2}"
                        .With(injectInfo.ContractType, injectable, _container.GetCurrentObjectGraph()));
            }

            return null;
        }
    }
}
