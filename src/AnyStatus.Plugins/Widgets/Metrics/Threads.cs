using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus.Plugins.Widgets.Metrics
{
    [DisplayColumn("Metrics")]
    [DisplayName("Threads")]
    [Description("The number of threads running on local computer.")]
    public class Threads : PerformanceCounter
    {
        public Threads()
        {
            CategoryName = "System";
            CounterName = "Threads";
        }
    }
}
