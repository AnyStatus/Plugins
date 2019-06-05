using AnyStatus.API;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class RamUsageQuery : IMetricQuery<RamUsage>
    {
        [DebuggerStepThrough]
        public Task Handle(MetricQueryRequest<RamUsage> request, CancellationToken cancellationToken)
        {
            request.DataContext.Value = (int)RamInformation.GetPercentageOfMemoryInUseMiB();

            request.DataContext.State = request.DataContext.Value == -1 ? State.Error : State.Ok;
            
            return Task.CompletedTask;
        }
    }
}
