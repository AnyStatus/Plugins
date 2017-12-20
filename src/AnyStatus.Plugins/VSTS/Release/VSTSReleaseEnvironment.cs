using AnyStatus.API;
using System.ComponentModel;

namespace AnyStatus
{
    [Browsable(false)]
    [DisplayName("VSTS Release Environment")]
    public class VSTSReleaseEnvironment : Widget
    {
        [ReadOnly(true)]
        [DisplayName("Environment Id")]
        public long EnvironmentId { get; set; }
    }
}
