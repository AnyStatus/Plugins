using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayName("Ping")]
    [DisplayColumn("Health Checks")]
    [Description("Test the reachability of a host")]
    public class Ping : Widget, IHealthCheck, ISchedulable
    {
        [Required]
        [Category("Ping")]
        [Description("Host Name or IP Address")]
        public string Host { get; set; }
    }
}