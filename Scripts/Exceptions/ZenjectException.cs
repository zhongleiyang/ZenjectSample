using System;

namespace ModestTree.Zenject
{
    public class ZenjectException : Exception
    {
        public ZenjectException(
            Exception innerException, string message)
            : base(message, innerException)
        {
        }

        public ZenjectException(string message)
            : base(message)
        {
        }

        public ZenjectException(string message, params object[] strParams)
            : base(String.Format(message, strParams))
        {
        }
    }

    public class ZenjectResolveException : ZenjectException
    {
        public ZenjectResolveException(
            Exception innerException, string message)
            : base(innerException, message)
        {
        }

        public ZenjectResolveException(string message)
            : base(message)
        {
        }

        public ZenjectResolveException(string message, params object[] strParams)
            : base(String.Format(message, strParams))
        {
        }
    }

    public class ZenjectBindException : ZenjectException
    {
        public ZenjectBindException(
            Exception innerException, string message)
            : base(innerException, message)
        {
        }

        public ZenjectBindException(string message)
            : base(message)
        {
        }

        public ZenjectBindException(string message, params object[] strParams)
            : base(String.Format(message, strParams))
        {
        }
    }
}

