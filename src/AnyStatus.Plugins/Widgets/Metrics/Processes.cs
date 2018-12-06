using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus.Plugins.Widgets.Metrics
{
    [DisplayColumn("Metrics")]
    [DisplayName("Processes")]
    [Description("The number of processes running on local computer.")]
    public class Processes : PerformanceCounter
    {
        public Processes()
        {
            CategoryName = "System";
            CounterName = "Processes";
        }
    }
}
