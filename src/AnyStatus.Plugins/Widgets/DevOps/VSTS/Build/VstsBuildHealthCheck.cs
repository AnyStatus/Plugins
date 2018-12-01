using AnyStatus.API;
using AnyStatus.API.Utils;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    /// <summary>
    /// VSTS Build Health Check
    /// </summary>
    public class VstsBuildHealthCheck : ICheckHealth<VSTSBuild_v1>
    {
        private readonly ILogger _logger;

        public VstsBuildHealthCheck(ILogger logger)
        {
            _logger = logger;
        }

        public async Task Handle(HealthCheckRequest<VSTSBuild_v1> request, CancellationToken cancellationToken)
        {
            var vsts = new VSTS();

            request.DataContext.MapTo(vsts);

            if (request.DataContext.DefinitionId == null)
            {
                var definition = await vsts.GetBuildDefinitionAsync(request.DataContext.DefinitionName).ConfigureAwait(false);

                request.DataContext.DefinitionId = definition.Id;
            }

            var builds = await vsts.Request<Collection<VSTSBuild>>($"build/builds?definitions={request.DataContext.DefinitionId}&$top=1&api-version=2.0").ConfigureAwait(false);

            var build = builds?.Value?.FirstOrDefault();

            if (build != null)
            {
                request.DataContext.State = build.State;
            }
            else
            {
                request.DataContext.State = State.Unknown;

                _logger.Error($"VSTS build definition \"{request.DataContext.DefinitionName}\" no builds were found.");
            }
        }
    }
}