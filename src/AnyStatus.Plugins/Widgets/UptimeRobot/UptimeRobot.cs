using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayName("Uptime Robot")]
    [DisplayColumn("Monitoring")]
    public class UptimeRobot : Widget, IHealthCheck, ISchedulable
    {
        private const string Category = "Uptime Robot";

        [Required]
        [DisplayName("API Key")]
        [Category(Category)]
        [Description("Required. Uptime Robot API key. You can get the key from \"My Settings\" page.")]
        public string ApiKey { get; set; }

        [DisplayName("Monitor Name")]
        [Category(Category)]
        [Description("Optional. Leave empty for the overall status of all monitors.")]
        public string MonitorName { get; set; }
    }
}