using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayColumn("Metrics")]
    [DisplayName("CPU Usage")]
    [Description("Shows the percentage of CPU usage")]
    public class CpuUsage : Metric, ISchedulable
    {
        public CpuUsage()
        {
            Interval = 1;
            Name = "CPU Usage";
        }

        [Category("CPU Usage")]
        [DisplayName("Machine Name")]
        [Description("Optional. Leave blank for local computer.")]
        public string MachineName { get; set; }
    }
}