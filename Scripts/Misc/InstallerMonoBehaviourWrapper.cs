using System.Collections.Generic;
using ModestTree.Zenject;
using System;
using UnityEngine;
using Fasterflect;
using System.Linq;

namespace ModestTree.Zenject
{
    public class InstallerMonoBehaviourWrapperBase : MonoBehaviour
    {
    }

    // In some cases it's useful to not use a monobehaviour for an installer
    // for example, if you're running unit tests outside of unity, or if you
    // want to re-use an existing installer in another installer, etc.
    // So for these cases you can use a normal C# class that implements Installer,
    // and then wrap it up in this class for use in dragging and dropping it into
    // unity scenes as well
    public class InstallerMonoBehaviourWrapper<TImpl> : InstallerMonoBehaviourWrapperBase where TImpl : Installer, new()
    {
        public TImpl Impl;

        public void RegisterBindings(DiContainer container)
        {
            Assert.That(typeof(TImpl).HasAnyAttribute(typeof(SerializableAttribute)),
                "Installer with type '{0}' is not Serializable", typeof(TImpl).Name());
            Assert.IsNotNull(Impl);

            Impl.Container = container;
            Impl.RegisterBindings();
        }
    }
}
