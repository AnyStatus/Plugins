using AnyStatus.API;
using AnyStatus.Plugins.Widgets.NuGet.API;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Widgets.NuGet
{
    public class NuGetPackageQuery : IMetricQuery<NuGetPackageWidget>
    {
        public async Task Handle(MetricQueryRequest<NuGetPackageWidget> request, CancellationToken cancellationToken)
        {
            var api = new NuGetApi(request.DataContext.PackageSource);

            var resource = await api.GetResourceAsync("SearchQueryService", cancellationToken).ConfigureAwait(false);

            var packagesMetadata = await api.GetPackageMetadataAsync(resource, request.DataContext.PackageId, cancellationToken).ConfigureAwait(false);

            var packageMetadata = packagesMetadata.FirstOrDefault(m => string.Equals(m.Id, request.DataContext.PackageId, StringComparison.InvariantCultureIgnoreCase));

            if (packageMetadata == null)
            {
                throw new Exception("NuGet package metadata was not found.");
            }

            request.DataContext.State = State.Ok;
            request.DataContext.Value = packageMetadata.Version;
            request.DataContext.Message = new StringBuilder()
                .Append("Total Downloads: ")
                .AppendLine(packageMetadata.TotalDownloads)
                .ToString();
        }
    }
}
