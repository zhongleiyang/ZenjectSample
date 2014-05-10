using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ModestTree.Zenject
{
    // Responsibilities:
    // - Expose methods to configure object graph via Bind() methods
    // - Build object graphs via Resolve() method
    public class DiContainer
    {
        readonly Dictionary<Type, List<ProviderBase>> _providers = new Dictionary<Type, List<ProviderBase>>();
        readonly SingletonProviderMap _singletonMap;

        Stack<Type> _lookupsInProgress = new Stack<Type>();
        bool _hasDisposed;

        public DiContainer()
        {
            _singletonMap = new SingletonProviderMap(this);
            Bind<DiContainer>().ToSingle(this);
        }

        public IEnumerable<Type> AllContracts
        {
            get
            {
                return _providers.Keys;
            }
        }

        // This is the list of concrete types that are in the current object graph
        // Useful for error messages (and complex binding conditions)
        internal Stack<Type> LookupsInProgress
        {
            get
            {
                return _lookupsInProgress;
            }
        }

        internal string GetCurrentObjectGraph()
        {
            Assert.That(!_hasDisposed);
            if (_lookupsInProgress.Count == 0)
            {
                return "";
            }

            return _lookupsInProgress.Select(t => t.GetPrettyName()).Reverse().Aggregate((i, str) => i + "\n" + str);
        }

        public void Dispose()
        {
            Assert.That(!_hasDisposed);

            foreach (var disposable in ResolveMany<IDisposable>())
            {
                disposable.Dispose();
            }

            _hasDisposed = true;
        }

        // This occurs so often that we might as well have a convenience method
        public BindingConditionSetter BindFactory<TContract>()
        {
            Assert.That(!_hasDisposed);
            return Bind<IFactory<TContract>>().ToSingle<Factory<TContract>>();
        }

        public BindingConditionSetter BindFactory<TContract, TConcrete>() where TConcrete : TContract
        {
            Assert.That(!_hasDisposed);
            return Bind<IFactory<TContract>>().ToSingle<Factory<TContract, TConcrete>>();
        }

        public ValueBinder<TContract> BindValue<TContract>() where TContract : struct
        {
            Assert.That(!_hasDisposed);
            return new ValueBinder<TContract>(this, _singletonMap);
        }

        public ReferenceBinder<TContract> Bind<TContract>() where TContract : class
        {
            Assert.That(!_hasDisposed);
            return new ReferenceBinder<TContract>(this, _singletonMap);
        }

        public CustomScope CreateScope()
        {
            Assert.That(!_hasDisposed);
            return new CustomScope(this, _singletonMap);
        }

        // See comment in LookupInProgressAdder
        internal LookupInProgressAdder PushLookup(Type type)
        {
            Assert.That(!_hasDisposed);
            return new LookupInProgressAdder(this, type);
        }

        public void RegisterProvider<TContract>(ProviderBase provider)
        {
            Assert.That(!_hasDisposed);
            if (_providers.ContainsKey(typeof (TContract)))
            {
                // Prevent duplicate singleton bindings:
                Assert.That(_providers[typeof(TContract)].Find(item => ReferenceEquals(item, provider)) == null,
                "Found duplicate singleton binding for contract '" + typeof (TContract) + "'");

                _providers[typeof (TContract)].Add(provider);
            }
            else
            {
                _providers.Add(typeof (TContract), new List<ProviderBase> {provider});
            }
        }

        public void UnregisterProvider(ProviderBase provider)
        {
            Assert.That(!_hasDisposed);
            int numRemoved = 0;

            foreach (var keyValue in _providers)
            {
                numRemoved += keyValue.Value.RemoveAll(x => x == provider);
            }

            Assert.That(numRemoved > 0, "Tried to unregister provider that was not registered");

            // Remove any empty contracts
            foreach (var contractType in _providers.Where(x => x.Value.IsEmpty()).Select(x => x.Key).ToList())
            {
                _providers.Remove(contractType);
            }

            provider.Dispose();
        }

        public List<TContract> ResolveMany<TContract>()
        {
            Assert.That(!_hasDisposed);
            return ResolveMany<TContract>(new ResolveContext());
        }

        public List<TContract> ResolveMany<TContract>(ResolveContext context)
        {
            Assert.That(!_hasDisposed);
            return (List<TContract>) ResolveMany(typeof (TContract), context);
        }

        public object ResolveMany(Type contract)
        {
            Assert.That(!_hasDisposed);
            return ResolveMany(contract, new ResolveContext());
        }

        List<object> ResolveInternalList(Type contract, ResolveContext context)
        {
            Assert.That(!_hasDisposed);
            var instances = new List<object>();

            List<ProviderBase> providers;
            if (_providers.TryGetValue(contract, out providers))
            {
                foreach (var provider in providers.Where(x => x.Matches(context)))
                {
                    instances.Add(provider.GetInstance());
                }
            }

            return instances;
        }

        public bool HasBinding(Type contract)
        {
            Assert.That(!_hasDisposed);
            return HasBinding(contract, new ResolveContext());
        }

        public bool HasBinding(Type contract, ResolveContext context)
        {
            Assert.That(!_hasDisposed);
            List<ProviderBase> providers;

            if (!_providers.TryGetValue(contract, out providers))
            {
                return false;
            }

            return providers.Where(x => x.Matches(context)).HasAtLeast(1);
        }

        public bool HasBinding<TContract>()
        {
            Assert.That(!_hasDisposed);
            return HasBinding(typeof(TContract));
        }

        public object ResolveMany(Type contract, ResolveContext context)
        {
            Assert.That(!_hasDisposed);
            return ResolveMany(contract, context, true);
        }

        public object ResolveMany(Type contract, ResolveContext context, bool optional)
        {
            Assert.That(!_hasDisposed);
            // Note that different types can map to the same provider (eg. a base type to a concrete class and a concrete class to itself)

            if (_providers.ContainsKey(contract))
            {
                return ReflectionUtil.CreateGenericList(contract, ResolveInternalList(contract, context).ToArray());
            }

            Assert.That(optional, () =>
                    "Could not find required dependency with type '" + contract.GetPrettyName() + "' \nObject graph:\n" + GetCurrentObjectGraph());

            // All many-dependencies are optional, return an empty list
            return ReflectionUtil.CreateGenericList(contract, new object[] {});
        }

        public List<Type> ResolveTypeMany(Type contract)
        {
            Assert.That(!_hasDisposed);
            if (_providers.ContainsKey(contract))
            {
                // TODO: fix this to work with providers that have conditions
                var context = new ResolveContext();

                return (from provider in _providers[contract] where provider.Matches(context) select provider.GetInstanceType()).ToList();
            }

            return new List<Type> {};
        }

        // Return single instance of requested type or assert
        public TContract Resolve<TContract>()
        {
            Assert.That(!_hasDisposed);
            return Resolve<TContract>(new ResolveContext());
        }

        public TContract Resolve<TContract>(ResolveContext context)
        {
            Assert.That(!_hasDisposed);
            return (TContract) Resolve(typeof (TContract), context);
        }

        public object Resolve(Type contract)
        {
            Assert.That(!_hasDisposed);
            return Resolve(contract, new ResolveContext());
        }

        public object Resolve(Type contract, ResolveContext context)
        {
            Assert.That(!_hasDisposed);
            return ResolveInternalSingle(contract, context, false);
        }

        object ResolveInternalSingle(Type contract, ResolveContext context, bool optional)
        {
            Assert.That(!_hasDisposed);
            // Note that different types can map to the same provider (eg. a base type to a concrete class and a concrete class to itself)

            var objects = ResolveInternalList(contract, context);

            if (!objects.Any())
            {
                if (!optional)
                {
                    throw new ZenjectResolveException(
                        "Unable to resolve type '{0}'. \nObject graph:\n{1}", contract.GetPrettyName(), GetCurrentObjectGraph());
                }

                return null;
            }

            if (objects.Count > 1)
            {
                if (!optional)
                {
                    throw new ZenjectResolveException(
                        "Found multiple matches when only one was expected for type '{0}'. \nObject graph:\n {1}",
                            contract.GetPrettyName(), GetCurrentObjectGraph());
                }

                return null;
            }

            return objects.First();
        }

        public IEnumerable<Type> GetDependencyContracts<TContract>()
        {
            Assert.That(!_hasDisposed);
            return GetDependencyContracts(typeof(TContract));
        }

        public IEnumerable<Type> GetDependencyContracts(Type contract)
        {
            Assert.That(!_hasDisposed);
            foreach (var param in InjectionInfoHelper.GetConstructorDependencies(contract, false))
            {
                yield return param.ParameterType;
            }

            foreach (var fieldInfo in InjectionInfoHelper.GetFieldDependencies(contract))
            {
                yield return fieldInfo.FieldType;
            }

            foreach (var propInfo in InjectionInfoHelper.GetPropertyDependencies(contract))
            {
                yield return propInfo.PropertyType;
            }
        }
    }
}
