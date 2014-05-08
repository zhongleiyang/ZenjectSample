using System;
using System.Collections.Generic;
using System.Reflection;

namespace ModestTree.Zenject
{
    // The difference between a factory and a provider:
    // Factories create new instances, providers might return an existing instance
    public interface IFactory<T>
    {
        T Create(params object[] constructorArgs);
    }
}

