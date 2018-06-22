using AnyStatus.API;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

//https://api.nuget.org/v3/registration3/anystatus.api/index.json

namespace AnyStatus
{
    public class NuGetPackageVersionQuery : IMetricQuery<NuGetPackageVersion>
    {
        [DebuggerStepThrough]
        public Task Handle(MetricQueryRequest<NuGetPackageVersion> request, CancellationToken cancellationToken)
        {
            //https://api.nuget.org/v3/registration3/anystatus.api/index.json

            request.DataContext.Value = "1.0.0.0";

            request.DataContext.State = State.Ok;

            return Task.CompletedTask;
        }
    }
}