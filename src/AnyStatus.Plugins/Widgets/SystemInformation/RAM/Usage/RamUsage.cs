using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayName("RAM Usage")]
    [DisplayColumn("System Information")]
    [Description("Shows the percentage of RAM usage for the local machine")]
    public class RamUsage : Sparkline, ISchedulable
    {
        public RamUsage()
        {
            Name = "RAM Usage";
            MaxValue = 100;
            Interval = 10;
            Units = IntervalUnits.Seconds;
        }

        public override string ToString()
        {
            return $"{Value}%";
        }
    }
}
