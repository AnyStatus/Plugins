using AnyStatus.API;
using AnyStatus.Plugins.Widgets.NuGet.API;
using RestSharp;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Widgets.NuGet
{
    [Browsable(false)]
    public class NuGetPackageWidget : Metric
    {
    }

    public class NuGetTotalDownloadsQuery : IMetricQuery<NuGetPackageWidget>
    {
        public async Task Handle(MetricQueryRequest<NuGetPackageWidget> request, CancellationToken cancellationToken)
        {
            var client = new RestClient();

            var resourcesRequest = new RestRequest("https://api.nuget.org/v3/index.json");

            var resourcesResponse = await client.ExecuteTaskAsync<NuGetIndex>(resourcesRequest);

            if (!resourcesResponse.IsSuccessful || resourcesResponse.Data == null)
            {
                return;
            }

            var url = resourcesResponse.Data.Resources.FirstOrDefault(k => k["@type"] == "SearchQueryService")["@id"];

            var metadataRequest = new RestRequest(url);

            metadataRequest.AddParameter("q", "packageid:" + "anystatus.api"); //packageid is optional. consider allowing to query a list or packages (or add another widget NuGetPackageQuery).

            var metadataResponse = await client.ExecuteTaskAsync<NuGetMetadataCollection>(metadataRequest).ConfigureAwait(false);

            if (!metadataResponse.IsSuccessful || metadataResponse.Data == null)
            {
                return;
            }

            var metadata = metadataResponse.Data.Data.FirstOrDefault();

            request.DataContext.Value = metadata.Version; //enable user select total downloads or version number
            request.DataContext.Message = $"Total Downloads: {metadata.TotalDownloads}";
            request.DataContext.State = State.Ok;
        }
    }
}
