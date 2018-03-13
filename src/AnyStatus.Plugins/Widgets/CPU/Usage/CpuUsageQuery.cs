using AnyStatus.API;
using System.Diagnostics;
using System.Threading;

namespace AnyStatus
{
    public class CpuUsageQuery : RequestHandler<MetricQueryRequest<CpuUsage>>,
        IMetricQuery<CpuUsage>
    {
        [DebuggerStepThrough]
        protected override void HandleCore(MetricQueryRequest<CpuUsage> request)
        {
            var usage = GetCpuUsage(request.DataContext.MachineName);

            request.DataContext.Value = usage + "%";

            request.DataContext.State = State.Ok;
        }

        public static int GetCpuUsage(string machineName)
        {
            if (string.IsNullOrWhiteSpace(machineName))
                machineName = ".";

            var counter = new System.Diagnostics.PerformanceCounter("Processor", "% Processor Time", "_Total", machineName);

            counter.NextValue();

            Thread.Sleep(1000);

            return (int)counter.NextValue();
        }
    }
}