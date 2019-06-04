using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayColumn("Metrics")]
    [DisplayName("RAM Usage")]
    [Description("Shows the percentage of RAM usage for the local machine")]
    public class RamUsage : Sparkline, ISchedulable
    {
        public RamUsage()
        {
            Symbol = "%";
            Interval = 10;
            Units = IntervalUnits.Seconds;
            Name = "RAM Usage";
            MaxValue = 100;
        }
    }
}