using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayName("Page File Usage")]
    [DisplayColumn("System Information")]
    [Description("Shows the percentage of page file usage")]
    public class PageFileUsage : Sparkline, ISchedulable
    {
        public PageFileUsage()
        {
            Name = "Page File Usage";
            Symbol = "%";
            MaxValue = 100;
            Interval = 10;
            Units = IntervalUnits.Seconds;
        }

        [Category("Page File Usage")]
        [DisplayName("Machine Name")]
        [Description("Optional. Leave blank for local computer.")]
        public string MachineName { get; set; }
    }
}