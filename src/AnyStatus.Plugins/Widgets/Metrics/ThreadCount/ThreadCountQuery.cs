using AnyStatus.API;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class ThreadCountQuery : IMetricQuery<ThreadCount>
    {
        private const string CategoryName = "System";
        private const string CounterName = "Threads";

        [DebuggerStepThrough]
        public async Task Handle(MetricQueryRequest<ThreadCount> request, CancellationToken cancellationToken)
        {
            request.DataContext.Value = await GetProcessCountAsync(request.DataContext.MachineName).ConfigureAwait(false);

            request.DataContext.State = State.Ok;
        }

        private async Task<int> GetProcessCountAsync(string machineName)
        {
            using (var counter = string.IsNullOrWhiteSpace(machineName)
                ? new System.Diagnostics.PerformanceCounter(CategoryName, CounterName)
                : new System.Diagnostics.PerformanceCounter(CategoryName, CounterName, string.Empty, machineName))
            {
                counter.NextValue();

                await Task.Delay(1000).ConfigureAwait(false);

                return (int)counter.NextValue();
            }
        }
    }
}