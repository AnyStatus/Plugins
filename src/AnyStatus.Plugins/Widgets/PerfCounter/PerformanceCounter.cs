using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayName("Performance Counter")]
    [Description("Experimental. Shows the value of a performance counter")]
    public class PerformanceCounter : MetricValue, ISchedulable
    {
        private const string Category = "Performance Counter";

        public PerformanceCounter()
        {
            Interval = 1;
        }

        [DisplayName("Machine Name")]
        [Category(Category)]
        [Description("Optional. Leave blank for local computer.")]
        public string MachineName { get; set; }

        [Required]
        [DisplayName("Category")]
        [Category(Category)]
        [Description("")]
        public string CategoryName { get; set; }

        [Required]
        [DisplayName("Counter")]
        [Category(Category)]
        [Description("")]
        public string CounterName { get; set; }

        [DisplayName("Instance")]
        [Category(Category)]
        [Description("")]
        public string InstanceName { get; set; }
    }
}