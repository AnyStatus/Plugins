using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayName("Process CPU Usage")]
    [DisplayColumn("System Information")]
    [Description("Shows the percentage of CPU usage for a single process")]
    public class ProcessCpuUsage : Sparkline, ISchedulable
    {
        public ProcessCpuUsage()
        {
            Name = "CPU Process Usage";
            MaxValue = 100;
            Interval = 10;
            Units = IntervalUnits.Seconds;
        }

        [Category("Process CPU Usage")]
        [DisplayName("Machine Name")]
        [Description("Optional. Leave blank for local computer.")]
        public string MachineName { get; set; }

        [Required]
        [Category("Process CPU Usage")]
        [DisplayName("Process Name")]
        [Description("Usually the file name without extension")]
        public string ProcessName { get; set; }

        public override string ToString()
        {
            return $"{Value}%";
        }
    }
}