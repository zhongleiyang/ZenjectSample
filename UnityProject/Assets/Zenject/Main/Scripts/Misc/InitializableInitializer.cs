using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModestTree.Zenject
{
    // Responsibilities:
    // - Run Initialize() on all Iinitializable's, in the order specified by InitPriority
    public class InitializableHandler
    {
        List<InitializableInfo> _initializables = new List<InitializableInfo>();

        public InitializableHandler(
            [InjectOptional]
            List<IInitializable> initializables,
            [InjectOptional]
            List<Tuple<Type, int>> priorities)
        {
            var priorityMap = priorities.ToDictionary(x => x.First, x => x.Second);

            foreach (var initializable in initializables)
            {
                int priority;
                bool success = priorityMap.TryGetValue(initializable.GetType(), out priority);

                if (!success)
                {
                    Debug.LogWarning(
                        String.Format("IInitializable with type '{0}' does not have a priority assigned",
                            initializable.GetType()));
                }

                _initializables.Add(
                    new InitializableInfo(initializable, success ? (int?)priority : null));
            }
        }

        int SortCompare(InitializableInfo e1, InitializableInfo e2)
        {
            // Initialize initializables with null priorities last
            if (!e1.Priority.HasValue)
            {
                return 1;
            }

            if (!e2.Priority.HasValue)
            {
                return -1;
            }

            return e1.Priority.Value.CompareTo(e2.Priority.Value);
        }

        public void Initialize()
        {
            _initializables.Sort(SortCompare);

            if (Assert.IsEnabled)
            {
                foreach (var initializable in _initializables.Select(x => x.Initializable).FindDuplicates())
                {
                    Assert.That(false, "Found duplicate IInitializable with type '" + initializable.GetType() + "'");
                }
            }

            foreach (var initializable in _initializables)
            {
                //Debug.Log("Initializing initializable with type '" + initializable.GetType() + "'");

                try
                {
                    initializable.Initializable.Initialize();
                }
                catch (Exception e)
                {
                    throw new ZenjectGeneralException(
                        e, "Error occurred while initializing IInitializable with type '" + initializable.GetType().GetPrettyName() + "'");
                }
            }
        }

        class InitializableInfo
        {
            public IInitializable Initializable;
            public int? Priority;

            public InitializableInfo(IInitializable initializable, int? priority)
            {
                Initializable = initializable;
                Priority = priority;
            }
        }
    }
}
