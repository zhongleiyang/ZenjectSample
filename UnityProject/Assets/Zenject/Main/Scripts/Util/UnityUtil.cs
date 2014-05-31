using System;

namespace ModestTree.Zenject
{
    public static class UnityUtil
    {
        // Due to the way that Unity overrides the Equals operator,
        // normal null checks such as (x == null) do not always work as
        // expected
        // In those cases you can use this function which will also
        // work with non-unity objects
        public static bool IsNull(System.Object aObj)
        {
            return aObj == null || aObj.Equals(null);
        }
    }
}
