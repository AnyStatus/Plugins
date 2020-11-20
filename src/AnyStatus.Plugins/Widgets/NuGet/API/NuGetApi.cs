using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Widgets.NuGet.API
{
    internal class NuGetApi
    {
        private readonly RestClient _client;
        private readonly string _packageSource;

        internal NuGetApi(string packageSource)
        {
            if (string.IsNullOrEmpty(packageSource))
            {
                throw new ArgumentException("NuGet package source cannot be null or empty.");
            }

            _packageSource = packageSource;

            _client = new RestClient();
        }

        internal async Task<NuGetResource> GetResourceAsync(string name, CancellationToken cancellationToken)
        {
            var resources = await GetResourcesAsync(cancellationToken).ConfigureAwait(false);

            var resource = resources.FirstOrDefault(r => r.Name == name);

            if (resource == null)
                throw new ApplicationException($"Resource name ${name} was not found.");

            return resource;
        }

        internal async Task<IEnumerable<NuGetResource>> GetResourcesAsync(CancellationToken cancellationToken)
        {
            var request = new RestRequest(_packageSource);

            var response = await _client.ExecuteTaskAsync<NuGetIndex>(request, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessful || response.Data == null)
                throw new Exception("An error occurred while getting NuGet resources.");

            return response.Data.Resources;
        }

        internal async Task<IEnumerable<NuGetMetadata>> GetPackagesMetadataAsync(NuGetResource searchQueryService, string packageId, CancellationToken cancellationToken)
        {
            var metadataRequest = new RestRequest(searchQueryService.URL);

            metadataRequest.AddParameter("q", packageId);

            var metadataResponse = await _client.ExecuteTaskAsync<NuGetMetadataCollection>(metadataRequest, cancellationToken).ConfigureAwait(false);

            if (!metadataResponse.IsSuccessful || metadataResponse.Data == null)
            {
                throw new Exception("An error occurred while getting NuGet package metadata.");
            }

            return metadataResponse.Data.Data;
        }

        internal async Task<IEnumerable<NuGetMetadata>> GetPackageMetadataAsync(NuGetResource searchQueryService, string packageId, bool prerelease, CancellationToken cancellationToken)
        {
            var metadataRequest = new RestRequest(searchQueryService.URL);

            metadataRequest.AddParameter("semVerLevel", "2.0.0");

            if (prerelease)
            {
                metadataRequest.AddParameter("prerelease", prerelease);
            }

            metadataRequest.AddParameter("q", $"packageid:{packageId}");

            var metadataResponse = await _client.ExecuteTaskAsync<NuGetMetadataCollection>(metadataRequest, cancellationToken).ConfigureAwait(false);

            if (!metadataResponse.IsSuccessful || metadataResponse.Data == null)
            {
                throw new Exception("An error occurred while getting NuGet package metadata.");
            }

            return metadataResponse.Data.Data;
        }
    }
}
