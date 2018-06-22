using System;

namespace AnyStatus
{
    public class VstsClientException : Exception
    {
        public VstsClientException()
        {
        }

        public VstsClientException(string message) : base(message)
        {
        }
    }
}