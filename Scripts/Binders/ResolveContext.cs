using System;
using System.Collections.Generic;

namespace ModestTree.Zenject
{
    public class ResolveContext
    {
        public Type EnclosingType;
        // note this is null for constructor params
        public object EnclosingInstance;
        public string SourceName;
        public object Identifier;
        public List<Type> ParentTypes;

        internal ResolveContext(
            InjectableInfo injectInfo, List<Type> parents, object targetInstance)
        {
            Identifier = injectInfo.Identifier;
            SourceName = injectInfo.SourceName;
            EnclosingType = injectInfo.EnclosingType;
            EnclosingInstance = targetInstance;
            ParentTypes = parents;
        }

        internal ResolveContext(Type targetType)
        {
            ParentTypes = new List<Type>();
            EnclosingType = targetType;
        }
    }
}
