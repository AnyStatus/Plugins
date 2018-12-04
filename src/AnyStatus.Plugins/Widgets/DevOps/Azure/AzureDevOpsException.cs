using System;

namespace AnyStatus
{
    public class AzureDevOpsException : Exception
    {
        public AzureDevOpsException()
        {
        }

        public AzureDevOpsException(string message) : base(message)
        {
        }
    }
}