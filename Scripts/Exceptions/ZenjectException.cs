using System;

namespace ModestTree.Zenject
{
    public class ZenjectGeneralException : Exception
    {
        public ZenjectGeneralException(
            Exception innerException, string message)
            : base(message, innerException)
        {
        }

        public ZenjectGeneralException(string message)
            : base(message)
        {
        }
    }

    public class ZenjectResolveException : Exception
    {
        public ZenjectResolveException(
            Exception innerException, string message)
            : base(message, innerException)
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
}

