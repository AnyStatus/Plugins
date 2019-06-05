using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayName("Thread Count")]
    [DisplayColumn("System Information")]
    [Description("Shows the number of running CPU threads")]
    public class ThreadCount : Sparkline, ISchedulable
    {
        public ThreadCount()
        {
            Name = "Thread Count";
            Interval = 10;
            Units = IntervalUnits.Seconds;
        }

        [Category("Thread Count")]
        [DisplayName("Machine Name")]
        [Description("Optional. Leave blank for local computer.")]
        public string MachineName { get; set; }
    }
}