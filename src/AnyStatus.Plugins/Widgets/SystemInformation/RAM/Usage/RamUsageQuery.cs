using AnyStatus.API;

namespace AnyStatus
{
    public class RamUsageQuery : RequestHandler<MetricQueryRequest<RamUsage>>
    {
        protected override void HandleCore(MetricQueryRequest<RamUsage> request)
        {
            request.DataContext.Value = (int)RamInformation.GetPercentageOfMemoryInUseMiB();

            request.DataContext.State = State.Ok;
        }
    }
}
