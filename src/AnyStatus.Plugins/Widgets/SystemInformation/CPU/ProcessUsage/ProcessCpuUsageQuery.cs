using AnyStatus.API;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class ProcessCpuUsageQuery : IMetricQuery<ProcessCpuUsage>
    {
        private const string CategoryName = "Process";
        private const string CounterName = "% Processor Time";

        [DebuggerStepThrough]
        public async Task Handle(MetricQueryRequest<ProcessCpuUsage> request, CancellationToken cancellationToken)
        {
            request.DataContext.Value = await GetCpuUsageAsync(request.DataContext.MachineName, request.DataContext.ProcessName).ConfigureAwait(false);

            request.DataContext.State = State.Ok;
        }

        private async Task<int> GetCpuUsageAsync(string machineName, string processName)
        {
            using (var counter = string.IsNullOrWhiteSpace(machineName)
                ? new System.Diagnostics.PerformanceCounter(CategoryName, CounterName, processName)
                : new System.Diagnostics.PerformanceCounter(CategoryName, CounterName, processName, machineName))
            {
                counter.NextValue();

                await Task.Delay(1000).ConfigureAwait(false);

                return (int)counter.NextValue();
            }
        }
    }
}