using System;

namespace ModestTree.Zenject
{
    public class ZenjectException : Exception
    {
        public ZenjectException(string message)
            : base(message)
        {
        }

        public ZenjectException(string message, params object[] strParams)
            : base(String.Format(message, strParams))
        {
        }

        public ZenjectException(
            Exception innerException, string message, params object[] strParams)
            : base(String.Format(message, strParams), innerException)
        {
        }
    }

    public class ZenjectResolveException : ZenjectException
    {
        public ZenjectResolveException(string message)
            : base(message)
        {
        }

        public ZenjectResolveException(string message, params object[] strParams)
            : base(String.Format(message, strParams))
        {
        }

        public ZenjectResolveException(
            Exception innerException, string message, params object[] strParams)
            : base(String.Format(message, strParams), innerException)
        {
        }
    }

    public class ZenjectBindException : ZenjectException
    {
        public ZenjectBindException(string message)
            : base(message)
        {
        }

        public ZenjectBindException(string message, params object[] strParams)
            : base(String.Format(message, strParams))
        {
        }

        public ZenjectBindException(
            Exception innerException, string message, params object[] strParams)
            : base(String.Format(message, strParams), innerException)
        {
        }
    }
}

