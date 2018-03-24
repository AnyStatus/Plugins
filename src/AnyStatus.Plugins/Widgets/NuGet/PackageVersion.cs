using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [Browsable(false)]
    [DisplayName("NuGet Package Version (Preview)")]
    [Description("")]
    public class NuGetPackageVersion : MetricValue, ISchedulable
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
}