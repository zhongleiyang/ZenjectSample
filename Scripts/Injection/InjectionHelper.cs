using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    // Helper class to manually fill in dependencies on given objects
    public static class InjectionHelper
    {
        // Inject dependencies into child game objects
        public static void InjectChildGameObjects(
            DiContainer container, GameObject gameObject)
        {
            InjectChildGameObjects(container, gameObject, Enumerable.Empty<object>());
        }

        public static void InjectChildGameObjects(
            DiContainer container, GameObject gameObject, IEnumerable<object> extraArgs)
        {
            foreach (var monoBehaviour in gameObject.GetComponentsInChildren<MonoBehaviour>())
            {
                InjectMonoBehaviour(container, monoBehaviour, extraArgs);
            }
        }

        public static void InjectGameObject(DiContainer container, GameObject gameObj)
        {
            foreach (var monoBehaviour in gameObj.GetComponents<MonoBehaviour>())
            {
                InjectMonoBehaviour(container, monoBehaviour);
            }
        }

        public static void InjectMonoBehaviour(DiContainer container, MonoBehaviour monoBehaviour)
        {
            InjectMonoBehaviour(container, monoBehaviour, Enumerable.Empty<object>());
        }

        public static void InjectMonoBehaviour(
            DiContainer container, MonoBehaviour monoBehaviour, IEnumerable<object> extraArgs)
        {
            // null if monobehaviour link is broken
            if (monoBehaviour != null && monoBehaviour.enabled)
            {
                using (container.PushLookup(monoBehaviour.GetType()))
                {
                    FieldsInjecter.Inject(container, monoBehaviour, extraArgs);
                }
            }
        }
    }
}
