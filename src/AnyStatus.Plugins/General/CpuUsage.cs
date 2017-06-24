using AnyStatus.API;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace AnyStatus
{
    [DisplayName("CPU Usage")]
    [Description("Shows the percentage of CPU usage")]
    public class CpuUsage : Metric, IAmMonitored
    {
        private const string Category = "CPU Usage";

        public CpuUsage()
        {
            Interval = 1;
        }

        [DisplayName("Machine Name")]
        [Category(Category)]
        [Description("Optional. Leave blank for local computer.")]
        public string MachineName { get; set; }
    }

    public class CpuUsageMonitor : IMonitor<CpuUsage>
    {
        [DebuggerStepThrough]
        public void Handle(CpuUsage item)
        {
            item.Value = GetCpuUsage(item.MachineName) + "%";

            item.State = State.Ok;
        }

        public int GetCpuUsage(string machineName)
        {
            if (string.IsNullOrWhiteSpace(machineName))
                machineName = "localhost";

            var counter = new System.Diagnostics.PerformanceCounter("Processor", "% Processor Time", "_Total", machineName);

            counter.NextValue();

            Thread.Sleep(1000);

            return (int)counter.NextValue();
        }
    }
}