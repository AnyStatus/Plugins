using AnyStatus.API;
using System.ComponentModel;

namespace AnyStatus
{
    [Browsable(false)]
    [DisplayName("VSTS Release Environment")]
    public class VSTSReleaseEnvironment : Widget, IStartable
    {
        [ReadOnly(true)]
        [DisplayName("Release Id")]
        public long ReleaseId { get; set; }

        [ReadOnly(true)]
        [DisplayName("Environment Id")]
        public long EnvironmentId { get; set; }
    }
}