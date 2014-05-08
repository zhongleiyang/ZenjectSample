using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Permissions;

namespace ModestTree.Zenject
{
    public interface IInitializable
    {
        void Initialize();
    }
}
