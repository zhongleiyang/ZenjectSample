using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    // Interface for kernel class
    // Currently there is only one (UnityKernel) but there should be another
    // eventually, once Zenject adds support for non-unity projects
    public interface IKernel
    {
        void AddTask(ITickable task);
        void AddTask(ITickable task, int priority);

        void RemoveTask(ITickable task);
    }
}

