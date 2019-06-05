using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayName("Process Count")]
    [DisplayColumn("System Information")]
    [Description("Shows the number of running CPU processes")]
    public class ProcessCount : Sparkline, ISchedulable
    {
        public ProcessCount()
        {
            Symbol = "";
            Interval = 10;
            Units = IntervalUnits.Seconds;
            Name = "Process Count";
            MaxValue = 100;
        }

        [Category("Process Count")]
        [DisplayName("Machine Name")]
        [Description("Optional. Leave blank for local computer.")]
        public string MachineName { get; set; }
    }
}