using System;
using UnityEngine;
using Fasterflect;

namespace ModestTree.Zenject
{
    // The class constraint is necessary to work property with Moq
    // Lazily create singleton providers and ensure that only one exists for every concrete type
    public class ReferenceBinder<TContract> : Binder<TContract> where TContract : class
    {
        public ReferenceBinder(DiContainer container, SingletonProviderMap singletonMap)
            : base(container, singletonMap)
        {
        }

        Type ContractType
        {
            get
            {
                return typeof(TContract);
            }
        }

        public BindingConditionSetter ToTransient()
        {
            return ToProvider(new TransientProvider<TContract>(_container));
        }

        public BindingConditionSetter ToTransient<TConcrete>() where TConcrete : TContract
        {
            return ToProvider(new TransientProvider<TConcrete>(_container));
        }

        public BindingConditionSetter ToSingle()
        {
            //Assert.That(!ContractType.IsSubclassOf(typeof(MonoBehaviour)),
            //    "Should not use ToSingle for Monobehaviours (when binding type " + ContractType.Name() + "), you probably want ToSingleFromPrefab or ToSingleGameObject");

            return ToProvider(_singletonMap.CreateProvider<TContract>());
        }

        public BindingConditionSetter ToSingle<TConcrete>() where TConcrete : TContract
        {
            //Assert.That(!typeof(TConcrete).IsSubclassOf(typeof(MonoBehaviour)),
            //    "Should not use ToSingle for Monobehaviours (when binding type " + ContractType.Name() + "), you probably want ToSingleFromPrefab or ToSingleGameObject");

            return ToProvider(_singletonMap.CreateProvider<TConcrete>());
        }

        public BindingConditionSetter ToSingle(Type concreteType)
        {
            if (!concreteType.DerivesFromOrEqual(ContractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'",
                    concreteType.Name(), ContractType.Name());
            }

            return ToProvider(_singletonMap.CreateProvider(concreteType));
        }

        public BindingConditionSetter To<TConcrete>(TConcrete instance) where TConcrete : TContract
        {
            if (UnityUtil.IsNull(instance) && !_container.AllowNullBindings)
            {
                string message;

                if (ContractType == typeof(TConcrete))
                {
                    message = string.Format("Received null instance during Bind command with type '{0}'", ContractType.Name());
                }
                else
                {
                    message = string.Format(
                        "Received null instance during Bind command when binding type '{0}' to '{1}'", ContractType.Name(), typeof(TConcrete).Name());
                }

                throw new ZenjectBindException(message);
            }

            return ToProvider(new InstanceProvider(typeof(TConcrete), instance));
        }

        public BindingConditionSetter ToSingle<TConcrete>(TConcrete instance) where TConcrete : TContract
        {
            if (UnityUtil.IsNull(instance) && !_container.AllowNullBindings)
            {
                string message;

                if (ContractType == typeof(TConcrete))
                {
                    message = string.Format("Received null singleton instance during Bind command with type '{0}'", ContractType.Name());
                }
                else
                {
                    message = string.Format(
                        "Received null singleton instance during Bind command when binding type '{0}' to '{1}'", ContractType.Name(), typeof(TConcrete).Name());
                }

                throw new ZenjectBindException(message);
            }

            return ToProvider(_singletonMap.CreateProvider(instance));
        }

        // we can't have this method because of the necessary where() below, so in this case they have to specify TContract twice
        //public BindingConditionSetter ToSingle(GameObject prefab)

        // Note: Here we assume that the contract is a component on the given prefab
        public BindingConditionSetter ToSingleFromPrefab<TConcrete>(GameObject prefab) where TConcrete : Component, TContract
        {
            // We have to cast to object otherwise we get SecurityExceptions when this function is run outside of unity
            if (UnityUtil.IsNull(prefab) && !_container.AllowNullBindings)
            {
                throw new ZenjectBindException("Received null prefab while binding type '{0}'", typeof(TConcrete).Name());
            }

            return ToProvider(new GameObjectSingletonProviderFromPrefab<TConcrete>(_container, prefab));
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public BindingConditionSetter ToTransientFromPrefab<TConcrete>(GameObject prefab) where TConcrete : Component, TContract
        {
            // We have to cast to object otherwise we get SecurityExceptions when this function is run outside of unity
            if (UnityUtil.IsNull(prefab) && !_container.AllowNullBindings)
            {
                throw new ZenjectBindException("Received null prefab while binding type '{0}'", typeof(TConcrete).Name());
            }

            return ToProvider(new GameObjectTransientProviderFromPrefab<TConcrete>(_container, prefab));
        }

        public BindingConditionSetter ToSingleGameObject()
        {
            return ToSingleGameObject(ContractType.Name());
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter ToSingleGameObject(string name)
        {
            if (!ContractType.IsSubclassOf(typeof(MonoBehaviour)))
            {
                throw new ZenjectBindException("Expected MonoBehaviour derived type when binding type '{0}'", ContractType.Name());
            }

            return ToProvider(new GameObjectSingletonProvider<TContract>(_container, name));
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter ToSingleGameObject<TConcrete>(string name) where TConcrete : MonoBehaviour, TContract
        {
            if (!ContractType.IsSubclassOf(typeof(MonoBehaviour)))
            {
                throw new ZenjectBindException("Expected MonoBehaviour derived type when binding type '{0}'", ContractType.Name());
            }

            return ToProvider(new GameObjectSingletonProvider<TConcrete>(_container, name));
        }
    }
}
