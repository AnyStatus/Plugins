using System;

namespace AnyStatus
{
    public class VstsException : Exception
    {
        public VstsException()
        {
        }

        public VstsException(string message) : base(message)
        {
        }
    }
}