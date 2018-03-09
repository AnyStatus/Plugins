using AnyStatus.API;
using System.Diagnostics;

namespace AnyStatus
{
    public class PerformanceCounterQuery : RequestHandler<MetricQueryRequest<PerformanceCounter>>, IMetricQuery<PerformanceCounter>
    {
        [DebuggerStepThrough]
        protected override void HandleCore(MetricQueryRequest<PerformanceCounter> request)
        {
            if (string.IsNullOrWhiteSpace(request.DataContext.MachineName))
                request.DataContext.MachineName = "localhost";

            request.DataContext.Value = (int)GetValue(request.DataContext);

            request.DataContext.State = State.Ok;
        }

        public float GetValue(PerformanceCounter item)
        {
            var counter = new System.Diagnostics.PerformanceCounter
            {
                CategoryName = item.CategoryName,
                CounterName = item.CounterName,
                InstanceName = item.InstanceName,
                MachineName = item.MachineName
            };

            return counter.NextValue();
        }
    }
}