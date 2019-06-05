using AnyStatus.API;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class RamUsageQuery : IMetricQuery<RamUsage>
    {
        [DebuggerStepThrough]
        public async Task Handle(MetricQueryRequest<RamUsage> request, CancellationToken cancellationToken)
        {
            await Task.Delay(1000).ConfigureAwait(false);

            request.DataContext.Value = (int)RamInformation.GetPercentageOfMemoryInUseMiB();

            request.DataContext.State = request.DataContext.Value == -1 ? State.Error : State.Ok;
        }
    }
}