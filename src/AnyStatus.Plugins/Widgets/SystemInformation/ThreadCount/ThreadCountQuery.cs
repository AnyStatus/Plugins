using AnyStatus.API;

namespace AnyStatus
{
    public class ThreadCountQuery : RequestHandler<MetricQueryRequest<ThreadCount>>
    {
        private const string CategoryName = "System";
        private const string CounterName = "Threads";
        private const string InstanceName = "";

        protected override void HandleCore(MetricQueryRequest<ThreadCount> request)
        {
            using (var counter = string.IsNullOrWhiteSpace(request.DataContext.MachineName)
                ? new System.Diagnostics.PerformanceCounter(CategoryName, CounterName)
                : new System.Diagnostics.PerformanceCounter(CategoryName, CounterName, InstanceName, request.DataContext.MachineName))
            {
                request.DataContext.Value = (int)counter.NextValue();
                request.DataContext.State = State.Ok;
            }
        }
    }
}