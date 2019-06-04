using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayColumn("Metrics")]
    [DisplayName("Thread Count")]
    [Description("Shows the number of running CPU threads")]
    public class ThreadCount : Sparkline, ISchedulable
    {
        public ThreadCount()
        {
            Symbol = "";
            Interval = 10;
            Units = IntervalUnits.Seconds;
            Name = "Thread Count";
        }

        [Category("Thread Count")]
        [DisplayName("Machine Name")]
        [Description("Optional. Leave blank for local computer.")]
        public string MachineName { get; set; }
    }
}