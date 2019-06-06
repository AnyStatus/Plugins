using AnyStatus.API;

namespace AnyStatus
{
    public class PageFileUsageQuery : RequestHandler<MetricQueryRequest<PageFileUsage>>
    {
        private const string CategoryName = "Paging File";
        private const string CounterName = "% Usage";
        private const string InstanceName = "_Total";

        protected override void HandleCore(MetricQueryRequest<PageFileUsage> request)
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