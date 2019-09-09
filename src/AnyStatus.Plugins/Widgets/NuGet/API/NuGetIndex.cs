using System.Collections.Generic;

namespace AnyStatus.Plugins.Widgets.NuGet.API
{
    public class NuGetIndex
    {
        public string Version { get; set; }

        public IEnumerable<Dictionary<string, string>> Resources { get; set; }
    }
}
