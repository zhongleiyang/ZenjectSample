using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fasterflect;

namespace ModestTree
{
    public static class TypeExtensions
    {
        public static bool DerivesFrom<T>(this Type a)
        {
            return DerivesFrom(a, typeof(T));
        }

        // This seems easier to think about than IsAssignableFrom
        public static bool DerivesFrom(this Type a, Type b)
        {
            return b.IsAssignableFrom(a);
        }

        public static object GetDefaultValue(this Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        // Returns name without generic arguments
        public static string GetSimpleName(this Type type)
        {
            var name = type.Name;

            var quoteIndex = name.IndexOf("`");

            if (quoteIndex == -1)
            {
                return name;
            }

            // Remove the backtick
            return name.Substring(0, quoteIndex);
        }

        public static string GetPrettyName(this Type type)
        {
            if (type.GetGenericArguments().Length == 0)
            {
                return type.Name;
            }

            var genericArguments = type.GetGenericArguments();
            var typeDefinition = type.Name;
            var quoteIndex = typeDefinition.IndexOf("`");

            if (quoteIndex < 0)
            {
                // Not sure why this happens sometimes
                return type.Name;
            }

            var unmangledName = typeDefinition.Substring(0, quoteIndex);
            return unmangledName + "<" + String.Join(",", genericArguments.Select<Type,string>(GetPrettyName).ToArray()) + ">";
        }
    }
}
