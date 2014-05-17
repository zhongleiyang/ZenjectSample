using System;

namespace ModestTree.Zenject
{
    // An injectable is a field or property with [Inject] attribute
    // Or a constructor parameter
    internal class InjectableInfo
    {
        public bool Optional;
        public object Identifier;

        // The field name or property name from source code
        public string SourceName;

        public Type ContractType;
        public Type EnclosingType;
        // Null for constructor declared dependencies
        public Action<object, object> Setter;
    }
}
