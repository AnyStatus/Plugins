using AnyStatus.API;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class CpuUsageQuery : IMetricQuery<CpuUsage>
    {
        [DebuggerStepThrough]
        public async Task Handle(MetricQueryRequest<CpuUsage> request, CancellationToken cancellationToken)
        {
            var usage = await GetCpuUsageAsync(request.DataContext.MachineName).ConfigureAwait(false);

            request.DataContext.Value = usage + "%";

            request.DataContext.State = State.Ok;
        }

        public async Task<int> GetCpuUsageAsync(string machineName)
        {
            if (string.IsNullOrWhiteSpace(machineName))
                machineName = ".";

            var counter = new System.Diagnostics.PerformanceCounter("Processor", "% Processor Time", "_Total", machineName);

            counter.NextValue();

            await Task.Delay(1000).ConfigureAwait(false);

            return (int)counter.NextValue();
        }
    }
}