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
            request.DataContext.Value = await GetCpuUsageAsync(request.DataContext.MachineName).ConfigureAwait(false);

            request.DataContext.State = State.Ok;
        }

        private static async Task<double> GetCpuUsageAsync(string machineName)
        {
            var counter = string.IsNullOrWhiteSpace(machineName)
                ? new System.Diagnostics.PerformanceCounter("Processor", "% Processor Time", "_Total")
                : new System.Diagnostics.PerformanceCounter("Processor", "% Processor Time", "_Total", machineName);

            counter.NextValue();

            await Task.Delay(1000).ConfigureAwait(false);

            return counter.NextValue();
        }
    }
}