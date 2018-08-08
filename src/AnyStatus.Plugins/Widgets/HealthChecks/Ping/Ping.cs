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
        [Description("The host name or IP address.")]
        public string Host { get; set; }
    }
}