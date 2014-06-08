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

        public static void LoadScene(
            string levelName, Action<DiContainer> extraBindings)
        {
            CompositionRoot.ExtraBindingsLookup = extraBindings;
            Application.LoadLevel(levelName);
        }

        public static void LoadSceneAdditive(string levelName)
        {
            LoadSceneAdditive(levelName, null);
        }

        public static void LoadSceneAdditive(
            string levelName, Action<DiContainer> extraBindings)
        {
            CompositionRoot.ExtraBindingsLookup = extraBindings;
            Application.LoadLevelAdditive(levelName);
        }

        public static List<IInstaller> InstallInstallers(DiContainer container)
        {
            var uninstalled = container.ResolveMany<IInstaller>();

            var allInstallers = new List<IInstaller>();

            while (!uninstalled.IsEmpty())
            {
                container.ReleaseBindings<IInstaller>();

                foreach (var installer in uninstalled)
                {
                    installer.InstallBindings();
                    allInstallers.Add(installer);
                }

                uninstalled = container.ResolveMany<IInstaller>();
            }

            return allInstallers;
        }

        public static IEnumerable<ZenjectResolveException> ValidateInstallers(DiContainer container)
        {
            var allInstallers = InstallInstallers(container);

            foreach (var error in container.ValidateResolve<IDependencyRoot>())
            {
                yield return error;
            }

            foreach (var installer in allInstallers)
            {
                foreach (var error in installer.ValidateSubGraphs())
                {
                    yield return error;
                }
            }
        }
    }
}
