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

            request.DataContext.MapTo(client.Connection);

            if (request.DataContext.DefinitionId == null)
            {
                var definition = await client.GetBuildDefinitionAsync(request.DataContext.DefinitionName).ConfigureAwait(false);

                request.DataContext.DefinitionId = definition.Id;
            }

            var build = await client.GetLatestBuildAsync(request.DataContext.DefinitionId.Value)
                                    .ConfigureAwait(false);

            request.DataContext.State = build != null ? build.State : State.Unknown;
        }
    }
}