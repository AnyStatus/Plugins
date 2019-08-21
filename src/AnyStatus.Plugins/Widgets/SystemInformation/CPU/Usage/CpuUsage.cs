using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayName("CPU Usage")]
    [DisplayColumn("System Information")]
    [Description("Shows the percentage of CPU usage")]
    public class CpuUsage : Sparkline, ISchedulable
    {
        public CpuUsage()
        {
            Name = "CPU Usage";
            MaxValue = 100;
            Interval = 10;
            Units = IntervalUnits.Seconds;
        }

        [Category("CPU Usage")]
        [DisplayName("Machine Name")]
        [Description("Optional. Leave blank for local computer.")]
        public string MachineName { get; set; }

        public override string ToString()
        {
            return $"{Value}%";
        }
    }
}