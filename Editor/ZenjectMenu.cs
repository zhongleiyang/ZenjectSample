using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Debug=UnityEngine.Debug;
using Fasterflect;

namespace ModestTree.Zenject
{
    public static class ZenjectMenu
    {
        [MenuItem("Assets/Zenject/Validate Current Scene #%v")]
        public static void ValidateCurrentScene()
        {
            var compRoots = GameObject.FindObjectsOfType<CompositionRoot>();

            if (compRoots.HasMoreThan(1))
            {
                Debug.LogError("Found multiple composition roots when only one was expected while validating current scene");
                return;
            }

            if (compRoots.IsEmpty())
            {
                Debug.LogError("Could not find composition root while validating current scene");
                return;
            }

            var compRoot = compRoots.Single();

            if (compRoot.Installers.IsEmpty())
            {
                Debug.LogError("Could not find scene installer while validating current scene");
                return;
            }

            var resolveErrors = ValidateInstallers(compRoot).Take(10);

            // Only show a few to avoid spamming the log too much
            foreach (var error in resolveErrors)
            {
                Debug.LogException(error);
            }

            if (resolveErrors.Any())
            {
                Debug.LogError("Validation Completed With Errors");
            }
            else
            {
                Debug.Log("Validation Completed Successfully");
            }
        }

        static IEnumerable<ZenjectResolveException> ValidateInstallers(CompositionRoot compRoot)
        {
            var container = new DiContainer();
            container.Bind<CompositionRoot>().ToSingle(compRoot);

            foreach (var installer in compRoot.Installers)
            {
                if (installer == null)
                {
                    yield return new ZenjectResolveException(
                        "Found null installer in properties of Composition Root");
                    yield break;
                }

                installer.Container = container;
                container.Bind<IInstaller>().To(installer);
            }

            foreach (var error in ZenUtil.ValidateInstallers(container))
            {
                yield return error;
            }

            // Also make sure we can fill in all the dependencies in the built-in scene
            foreach (var monoBehaviour in compRoot.GetComponentsInChildren<MonoBehaviour>())
            {
                if (monoBehaviour == null)
                {
                    // Be nice to give more information here
                    Log.Warn("Found null MonoBehaviour in scene");
                    continue;
                }

                foreach (var error in container.ValidateObjectGraph(monoBehaviour.GetType()))
                {
                    yield return error;
                }
            }
        }

        [MenuItem("Assets/Zenject/Output Object Graph For Current Scene")]
        public static void OutputObjectGraphForScene()
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogError("Zenject error: Must be in play mode to generate object graph.  Hit Play button and try again.");
                return;
            }

            DiContainer container;
            try
            {
                container = ZenEditorUtil.GetContainerForCurrentScene();
            }
            catch (ZenjectException e)
            {
                Debug.LogError("Unable to find container in current scene. " + e.GetFullMessage());
                return;
            }

            var ignoreTypes = Enumerable.Empty<Type>();
            var types = container.AllConcreteTypes;

            ZenEditorUtil.OutputObjectGraphForCurrentScene(container, ignoreTypes, types);
        }
    }
}
