using AnyStatus.API;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class CpuUsageQuery : IMetricQuery<CpuUsage>
    {
        private const string CategoryName = "Processor";
        private const string CounterName = "% Processor Time";
        private const string InstanceName = "_Total";

        [DebuggerStepThrough]
        public async Task Handle(MetricQueryRequest<CpuUsage> request, CancellationToken cancellationToken)
        {
            request.DataContext.Value = await GetCpuUsageAsync(request.DataContext.MachineName).ConfigureAwait(false);

            request.DataContext.State = State.Ok;
        }

        private static async Task<int> GetCpuUsageAsync(string machineName)
        {
            using (var counter = string.IsNullOrWhiteSpace(machineName) ?
                new System.Diagnostics.PerformanceCounter(CategoryName, CounterName, InstanceName) :
                new System.Diagnostics.PerformanceCounter(CategoryName, CounterName, InstanceName, machineName))
            {
                counter.NextValue();

                await Task.Delay(1000).ConfigureAwait(false);

                return (int)counter.NextValue();
            }
        }
    }
}