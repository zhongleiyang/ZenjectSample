using System;
using System.Collections.Generic;
using ModestTree.Zenject;

namespace ModestTree.Zenject
{
    public class ListFactory<T>
    {
        List<IFactory<T>> _singleFactories;

        public ListFactory(List<IFactory<T>> singleFactories)
        {
            _singleFactories = singleFactories;
        }

        public List<T> Create(params object[] constructorArgs)
        {
            var list = new List<T>();

            foreach (var factory in _singleFactories)
            {
                list.Add(factory.Create(constructorArgs));
            }

            return list;
        }
    }
}

