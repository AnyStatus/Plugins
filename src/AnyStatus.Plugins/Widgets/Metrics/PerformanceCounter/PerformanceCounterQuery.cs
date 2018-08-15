using AnyStatus.API;

namespace AnyStatus
{
    public class PerformanceCounterQuery : RequestHandler<MetricQueryRequest<PerformanceCounter>>, IMetricQuery<PerformanceCounter>
    {
        protected override void HandleCore(MetricQueryRequest<PerformanceCounter> request)
        {
            var counter = new System.Diagnostics.PerformanceCounter
            {
                CategoryName = request.DataContext.CategoryName,
                CounterName = request.DataContext.CounterName,
                InstanceName = request.DataContext.InstanceName,
            };

            if (!string.IsNullOrWhiteSpace(request.DataContext.MachineName))
            {
                counter.MachineName = request.DataContext.MachineName;
            }

            request.DataContext.Value = counter.NextValue();

            request.DataContext.State = State.Ok;
        }
    }
}