using AnyStatus.API;

namespace AnyStatus
{
    public class PerformanceCounterQuery : RequestHandler<MetricQueryRequest<PerformanceCounter>>
    {
        protected override void HandleCore(MetricQueryRequest<PerformanceCounter> request)
        {
            using (var counter = string.IsNullOrWhiteSpace(request.DataContext.MachineName)
                ? new System.Diagnostics.PerformanceCounter(request.DataContext.CategoryName, request.DataContext.CounterName, request.DataContext.InstanceName)
                : new System.Diagnostics.PerformanceCounter(request.DataContext.CategoryName, request.DataContext.CounterName, request.DataContext.InstanceName, request.DataContext.MachineName))
            {
                request.DataContext.Value = (int)counter.NextValue();

                request.DataContext.State = State.Ok;
            }
        }
    }
}
