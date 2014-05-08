using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class ZenUtil
    {
        public static void LoadLevel(string levelName, Action<DiContainer> extraBindings)
        {
            CompositionRoot.ExtraBindingsLookup = extraBindings;
            Application.LoadLevel(levelName);
        }

        public static void LoadLevelAdditive(string levelName, Action<DiContainer> extraBindings)
        {
            CompositionRoot.ExtraBindingsLookup = extraBindings;
            Application.LoadLevelAdditive(levelName);
        }
    }
}
