using System;
using UnityEngine;

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

        public BindingConditionSetter ToTransient()
        {
            return To(new TransientProvider<TContract>(_container));
        }

        public BindingConditionSetter ToTransient<TConcrete>() where TConcrete : TContract
        {
            return To(new TransientProvider<TConcrete>(_container));
        }

        public BindingConditionSetter ToSingle()
        {
            //Assert.That(!typeof(TContract).IsSubclassOf(typeof(MonoBehaviour)),
            //    "Should not use ToSingle for Monobehaviours (when binding type " + typeof(TContract).GetPrettyName() + "), you probably want AsSingleFromPrefab or AsSingleGameObject");

            return To(_singletonMap.CreateProvider<TContract>());
        }

        public BindingConditionSetter ToSingle<TConcrete>() where TConcrete : TContract
        {
            //Assert.That(!typeof(TConcrete).IsSubclassOf(typeof(MonoBehaviour)),
            //    "Should not use ToSingle for Monobehaviours (when binding type " + typeof(TContract).GetPrettyName() + "), you probably want AsSingleFromPrefab or AsSingleGameObject");

            return To(_singletonMap.CreateProvider<TConcrete>());
        }

        public BindingConditionSetter ToSingle(Type concreteType)
        {
            Assert.That(concreteType.DerivesFrom(typeof(TContract)));
            return To(_singletonMap.CreateProvider(concreteType));
        }

        public BindingConditionSetter ToSingle<TConcrete>(TConcrete instance) where TConcrete : TContract
        {
            Assert.That(instance != null, "provided singleton instance is null");
            return To(new SingletonInstanceProvider(instance));
        }

        // we can't have this method because of the necessary where() below, so in this case they have to specify TContract twice
        //public BindingConditionSetter ToSingle(GameObject template)

        // Note: Here we assume that the contract is a component on the given prefab
        public BindingConditionSetter ToSingleFromPrefab<TConcrete>(GameObject template) where TConcrete : Component, TContract
        {
            Assert.IsNotNull(template, "Received null template while binding type '" + typeof(TConcrete).GetPrettyName() + "'");
            return To(new GameObjectSingletonProviderFromPrefab<TConcrete>(_container, template));
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public BindingConditionSetter ToTransientFromPrefab<TConcrete>(GameObject template) where TConcrete : Component, TContract
        {
            Assert.IsNotNull(template, "provided template instance is null");
            return To(new GameObjectTransientProviderFromPrefab<TConcrete>(_container, template));
        }

        public BindingConditionSetter ToSingleGameObject()
        {
            return ToSingleGameObject(typeof(TContract).GetPrettyName());
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter ToSingleGameObject(string name)
        {
            Assert.That(typeof(TContract).IsSubclassOf(typeof(MonoBehaviour)), "Expected MonoBehaviour derived type when binding type '" + typeof(TContract).GetPrettyName() + "'");
            return To(new GameObjectSingletonProvider<TContract>(_container, name));
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter ToSingleGameObject<TConcrete>(string name) where TConcrete : MonoBehaviour, TContract
        {
            Assert.That(typeof(TConcrete).IsSubclassOf(typeof(MonoBehaviour)), "Expected MonoBehaviour derived type when binding type '" + typeof(TConcrete).GetPrettyName() + "'");
            return To(new GameObjectSingletonProvider<TConcrete>(_container, name));
        }
    }
}
