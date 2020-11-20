using AnyStatus.API;
using AnyStatus.Plugins.Widgets.NuGet.API;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Widgets.NuGet
{
    public class NuGetPackageQuery : IMetricQuery<NuGetPackageWidget>
    {
        private readonly INotificationService _notificationService;

        public NuGetPackageQuery(INotificationService notificationService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public async Task Handle(MetricQueryRequest<NuGetPackageWidget> request, CancellationToken cancellationToken)
        {
            var api = new NuGetApi(request.DataContext.PackageSource);

            var resource = await api.GetResourceAsync("SearchQueryService", cancellationToken).ConfigureAwait(false);

            var packagesMetadata = await api.GetPackageMetadataAsync(resource, request.DataContext.PackageId, request.DataContext.PreRelease, cancellationToken).ConfigureAwait(false);

            var packageMetadata = packagesMetadata.FirstOrDefault();

            if (packageMetadata is null)
            {
                throw new Exception("NuGet package metadata was not found.");
            }

            request.DataContext.Value = packageMetadata.Version?.Split('+')[0];

            request.DataContext.Message = $"Total Downloads: {packageMetadata.TotalDownloads}";

            request.DataContext.State = State.Ok;
        }
    }
}
