using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayColumn("Metrics")]
    [DisplayName("CPU Usage")]
    [Description("Shows the percentage of CPU usage")]
    public class CpuUsage : Sparkline, ISchedulable
    {
        public CpuUsage()
        {
            Symbol = "%";
            Interval = 10;
            Units = IntervalUnits.Seconds;
            Name = "CPU Usage";
            MaxValue = 100;
        }

        [Category("CPU Usage")]
        [DisplayName("Machine Name")]
        [Description("Optional. Leave blank for local computer.")]
        public string MachineName { get; set; }
    }
}