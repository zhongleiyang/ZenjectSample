using System;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Linq;

namespace ModestTree
{
    public class AssertException : Exception
    {
        public AssertException(string message)
            : base(message)
        {
        }
    }

    public static class Assert
    {
        public static bool IsEnabled
        {
            get
            {
#if UNITY_EDITOR
                return true;
#else
                // TODO: Compile out asserts for actual builds
                return false;
#endif
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void That(bool condition)
        {
            if (!condition)
            {
                TriggerAssert("Assert hit!");
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void IsType<T>(object obj)
        {
            IsType<T>(obj, "");
        }

        [Conditional("UNITY_EDITOR")]
        public static void IsType<T>(object obj, string message)
        {
            if (!(obj is T))
            {
                TriggerAssert("Assert Hit! Wrong type found. Expected '"+ typeof(T).Name + "' but found '" + obj.GetType().Name + "'. " + message);
            }
        }

        // Use AssertEquals to get better error output (with values)
        [Conditional("UNITY_EDITOR")]
        public static void IsEqual(object left, object right)
        {
            IsEqual(left, right, "");
        }

        // Use AssertEquals to get better error output (with values)
        [Conditional("UNITY_EDITOR")]
        public static void IsEqual(object left, object right, Func<string> messageGenerator)
        {
            if (!object.Equals(left, right))
            {
                left = left ?? "<NULL>";
                right = right ?? "<NULL>";
                TriggerAssert("Assert Hit! Expected '" + right.ToString() + "' but found '" + left.ToString() + "'. " + messageGenerator());
            }
        }

        // Use AssertEquals to get better error output (with values)
        [Conditional("UNITY_EDITOR")]
        public static void IsEqual(object left, object right, string message)
        {
            if (!object.Equals(left, right))
            {
                left = left ?? "<NULL>";
                right = right ?? "<NULL>";
                TriggerAssert("Assert Hit! Expected '" + right.ToString() + "' but found '" + left.ToString() + "'. " + message);
            }
        }

        // Use Assert.IsNotEqual to get better error output (with values)
        [Conditional("UNITY_EDITOR")]
        public static void IsNotEqual(object left, object right)
        {
            IsNotEqual(left, right, "");
        }

        // Use Assert.IsNotEqual to get better error output (with values)
        [Conditional("UNITY_EDITOR")]
        public static void IsNotEqual(object left, object right, Func<string> messageGenerator)
        {
            if(object.Equals(left, right))
            {
                left = left ?? "<NULL>";
                right = right ?? "<NULL>";
                TriggerAssert("Assert Hit! Expected '" + right.ToString() + "' to differ from '" + left.ToString() + "'. " + messageGenerator());
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void IsNull(object val)
        {
            if (val != null)
            {
                TriggerAssert("Assert Hit! Expected null pointer but instead found '" + val.ToString() + "'");
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void IsNotNull(object val)
        {
            if (val == null)
            {
                TriggerAssert("Assert Hit! Found null pointer when value was expected");
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void IsNotNull(object val, string message)
        {
            if (val == null)
            {
                TriggerAssert("Assert Hit! Found null pointer when value was expected. " + message);
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void IsNotNull(object val, string message, params object[] parameters)
        {
            if (val == null)
            {
                TriggerAssert("Assert Hit! Found null pointer when value was expected. " + FormatString(message, parameters));
            }
        }

        // Use Assert.IsNotEqual to get better error output (with values)
        [Conditional("UNITY_EDITOR")]
        public static void IsNotEqual(object left, object right, string message)
        {
            if (object.Equals(left, right))
            {
                left = left ?? "<NULL>";
                right = right ?? "<NULL>";
                TriggerAssert("Assert Hit! Expected '" + right.ToString() + "' to differ from '" + left.ToString() + "'. " + message);
            }
        }

        // Pass a function instead of a string for cases that involve a lot of processing to generate a string
        // This way the processing only occurs when the assert fails
        [Conditional("UNITY_EDITOR")]
        public static void That(bool condition, Func<string> messageGenerator)
        {
            if (!condition)
            {
                TriggerAssert("Assert hit! " + messageGenerator());
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void That(
            bool condition, string message, params object[] parameters)
        {
            if (!condition)
            {
                TriggerAssert("Assert hit! " + FormatString(message, parameters));
            }
        }

        [Conditional("UNITY_EDITOR")]
        static void TriggerAssert(string message)
        {
            throw new AssertException(message);
        }

        static string FormatString(string format, params object[] parameters)
        {
            // doin this funky loop to ensure nulls are replaced with "NULL"
            // and that the original parameters array will not be modified
            if (parameters != null && parameters.Length > 0)
            {
                object[] paramToUse = parameters;

                foreach (object cur in parameters)
                {
                    if (cur == null)
                    {
                        paramToUse = new object[parameters.Length];

                        for (int i = 0; i < parameters.Length; ++i)
                        {
                            paramToUse[i] = parameters[i] ?? "NULL";
                        }

                        break;
                    }
                }

                format = string.Format(format, paramToUse);
            }

            return format;
        }
    }
}

