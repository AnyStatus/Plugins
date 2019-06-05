using AnyStatus.API;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class PerformanceCounterQuery : BasePerformanceCounterQuery, IMetricQuery<PerformanceCounter>
    {
        public Task Handle(MetricQueryRequest<PerformanceCounter> request, CancellationToken cancellationToken)
        {
            return Handle(request.DataContext);
        }
    }
}
