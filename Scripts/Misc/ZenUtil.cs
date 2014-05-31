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
        public static void LoadScene(string levelName)
        {
            ZenUtil.LoadScene(levelName, null);
        }

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
            LoadSceneAdditive(levelName, extraBindings, null);
        }

        public static void LoadSceneAdditive(string levelName, Action<DiContainer> extraBindings, Action<DiContainer> installerBindings)
        {
            CompositionRoot.ExtraBindingsLookup = extraBindings;
            CompositionRoot.ExtraInstallerBindingsLookup = installerBindings;

            Application.LoadLevelAdditive(levelName);
        }

        public static IEnumerable<ZenjectResolveException> ValidateInstaller(ISceneInstaller installer, bool allowNullBindings)
        {
            return ValidateInstaller(installer, allowNullBindings, null);
        }

        public static IEnumerable<ZenjectResolveException> ValidateInstaller(ISceneInstaller installer, bool allowNullBindings, CompositionRoot compRoot)
        {
            var modulesContainer = new DiContainer();
            installer.InstallModules(modulesContainer);

            foreach (var error in modulesContainer.ValidateResolve<List<Module>>())
            {
                yield return error;
            }

            var allModules = modulesContainer.ResolveMany<Module>();

            var execContainer = new DiContainer();
            execContainer.AllowNullBindings = allowNullBindings;

            execContainer.Bind<CompositionRoot>().To(compRoot);

            foreach (var module in allModules)
            {
                module.Container = execContainer;
                module.AddBindings();
            }

            foreach (var error in execContainer.ValidateResolve<IDependencyRoot>())
            {
                yield return error;
            }

            foreach (var module in allModules)
            {
                foreach (var error in module.ValidateSubGraphs())
                {
                    yield return error;
                }
            }

            if (!UnityUtil.IsNull(compRoot))
            {
                // Also make sure we can fill in all the dependencies in the built-in scene
                foreach (var monoBehaviour in compRoot.GetComponentsInChildren<MonoBehaviour>())
                {
                    foreach (var error in execContainer.ValidateObjectGraph(monoBehaviour.GetType()))
                    {
                        yield return error;
                    }
                }
            }
        }
    }
}
