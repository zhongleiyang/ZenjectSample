using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using UnityEngine;
using Fasterflect;

namespace ModestTree.Zenject
{
    public class ZenUtil
    {
        public static void LoadScene(string levelName, Action<DiContainer> extraBindings)
        {
            CompositionRoot.ExtraBindingsLookup = extraBindings;
            Application.LoadLevel(levelName);
        }

        public static void LoadSceneAdditive(string levelName)
        {
            LoadSceneAdditive(levelName, null);
        }

        public static void LoadSceneAdditive(string levelName, Action<DiContainer> extraBindings)
        {
            CompositionRoot.ExtraBindingsLookup = extraBindings;
            Application.LoadLevelAdditive(levelName);
        }

        public static void ValidateSceneInstaller<TInstaller>(params Type[] dynamicObjectGraphs) where TInstaller : MonoBehaviour, ISceneInstaller
        {
            var self = GameObject.FindObjectOfType<TInstaller>();

            var modulesContainer = new DiContainer();
            self.InstallModules(modulesContainer);

            var container = new DiContainer();
            foreach (var module in modulesContainer.ResolveMany<IModule>())
            {
                module.AddBindings(container);
            }

            try
            {
                container.ValidateResolve<IDependencyRoot>();

                foreach (var dynType in dynamicObjectGraphs)
                {
                    container.ValidateCanCreateConcrete(dynType);
                }
            }
            catch (ZenjectResolveException e)
            {
                UnityEngine.Debug.LogError("Validation failed for object graph given by '" + typeof(TInstaller).Name() + "'");
                throw e;
            }

            UnityEngine.Debug.Log("Validation succeeded for object graph given by '" + typeof(TInstaller).Name() + "'");
        }
    }
}
