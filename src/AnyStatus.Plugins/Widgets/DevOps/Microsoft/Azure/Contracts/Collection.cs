using System.Collections.Generic;

namespace AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Contracts
{

    public class AzureDevOpsCollection<T>
    {
        public int Count { get; set; }

        public List<T> Value { get; set; }
    }
}