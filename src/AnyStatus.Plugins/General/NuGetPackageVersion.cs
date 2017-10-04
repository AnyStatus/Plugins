using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace AnyStatus
{
    [Browsable(false)]
    [DisplayName("NuGet Package Version (Preview)")]
    [Description("")]
    public class NuGetPackageVersion : Metric, IMonitored
    {
        private const string Category = "NuGet Package";

        public NuGetPackageVersion()
        {
            Interval = 1;
        }

        [Required]
        [DisplayName("Package Name")]
        [Category(Category)]
        [Description("Required. Enter the NuGet package name.")]
        public string PackageName { get; set; }
    }

    public class NuGetPackageVersionMonitor : IMonitor<NuGetPackageVersion>
    {
        [DebuggerStepThrough]
        public void Handle(NuGetPackageVersion item)
        {
            //https://api.nuget.org/v3/registration3/anystatus.api/index.json

            item.Value = "1.0.0.0";

            item.State = State.Ok;
        }
    }
}