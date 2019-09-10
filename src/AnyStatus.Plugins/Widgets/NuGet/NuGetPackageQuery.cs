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

            var packagesMetadata = await api.GetPackageMetadataAsync(resource, request.DataContext.PackageId, cancellationToken).ConfigureAwait(false);

            var packageMetadata = packagesMetadata.FirstOrDefault(m => string.Equals(m.Id, request.DataContext.PackageId, StringComparison.InvariantCultureIgnoreCase));

            if (packageMetadata == null)
            {
                throw new Exception("NuGet package metadata was not found.");
            }

            if (request.DataContext.PackageVersion != null && request.DataContext.PackageVersion != packageMetadata.Version)
            {
                _notificationService.TrySend(new Notification($"NuGet package {request.DataContext.PackageId} has been updated.", NotificationIcon.Info));
            }

            request.DataContext.State = State.Ok;
            request.DataContext.Value = packageMetadata.Version;
            request.DataContext.PackageVersion = packageMetadata.Version;
            request.DataContext.Message = $"Total Downloads: {packageMetadata.TotalDownloads}";
        }
    }
}
