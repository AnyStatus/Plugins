using AnyStatus.API;
using System.ComponentModel;

namespace AnyStatus
{
    [DisplayName("CPU Usage")]
    [Description("Shows the percentage of CPU usage")]
    public class CpuUsage : Metric, ISchedulable
    {
        private const string Category = "CPU Usage";

        public CpuUsage()
        {
            Interval = 1;
        }

        [DisplayName("Machine Name")]
        [Category(Category)]
        [Description("Optional. Leave blank for local computer.")]
        public string MachineName { get; set; }
    }
}