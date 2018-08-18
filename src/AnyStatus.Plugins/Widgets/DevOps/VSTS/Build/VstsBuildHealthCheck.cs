using System.Linq;
using AnyStatus.API;
using AnyStatus.API.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    /// <summary>
    /// VSTS Build Health Check
    /// </summary>
    public class VstsBuildHealthCheck : ICheckHealth<VSTSBuild_v1>
    {
        public async Task Handle(HealthCheckRequest<VSTSBuild_v1> request, CancellationToken cancellationToken)
        {
            var client = new VstsClient();

            request.DataContext.MapTo(client);

            if (request.DataContext.DefinitionId == null)
            {
                var definition = await client.GetBuildDefinitionAsync(request.DataContext.DefinitionName).ConfigureAwait(false);

                request.DataContext.DefinitionId = definition.Id;
            }

            var builds = await client.Request<Collection<VSTSBuild>>($"build/builds?definitions={request.DataContext.DefinitionId}&$top=1&api-version=2.0").ConfigureAwait(false);

            var build = builds?.Value?.FirstOrDefault();

            request.DataContext.State = build != null ? build.State : State.Unknown;
        }
    }
}