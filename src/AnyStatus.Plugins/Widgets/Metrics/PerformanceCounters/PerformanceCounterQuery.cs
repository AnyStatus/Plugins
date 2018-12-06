using AnyStatus.API;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class PerformanceCounterQuery : IMetricQuery<PerformanceCounter>
    {
        public Task Handle(MetricQueryRequest<PerformanceCounter> request, CancellationToken cancellationToken)
        {
            return Handle(request.DataContext);
        }

        protected static Task Handle(PerformanceCounter counter)
        {
            var sample = new System.Diagnostics.PerformanceCounter
            {
                CategoryName = counter.CategoryName,
                CounterName = counter.CounterName,
                InstanceName = counter.InstanceName,
            };

            if (!string.IsNullOrWhiteSpace(counter.MachineName))
            {
                sample.MachineName = counter.MachineName;
            }

            counter.Value = sample.NextValue();

            counter.State = State.Ok;

            return Unit.Task;
        }
    }
}
