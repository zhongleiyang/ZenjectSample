using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    public interface ISceneInstaller
    {
        void InstallModules(DiContainer container);
    }
}
