using System.Threading;
using System.Threading.Tasks;
using AnyStatus.API;
using AnyStatus.API.Utils;
using AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Queries;

namespace AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Widgets.Build
{
    public class AzureDevOpsBuildHealthCheck : ICheckHealth<AzureDevOpsBuildWidget>
    {
        private readonly IMediator _m;
        private readonly ILogger _logger;

        public AzureDevOpsBuildHealthCheck(ILogger logger, IMediator mediator)
        {
            _m = mediator;
            _logger = logger;
        }

        public async Task Handle(HealthCheckRequest<AzureDevOpsBuildWidget> request, CancellationToken cancellationToken)
        {
            var connection = new AzureDevOpsConnection();

            request.DataContext.CopyTo(connection);

            var getBuildsRequest = new GetBuilds.Request
            {
                Top = 1,
                Connection = connection,
                BuildDefinitionId = request.DataContext.BuildDefinitionId,
            };

            var response = await _m.Send(getBuildsRequest, cancellationToken).ConfigureAwait(false);

            if (response?.Builds != null && response.Builds.Length > 0)
            {
                request.DataContext.State = response.Builds[0].State;
            }
            else
            {
                _logger.Error($"{request.DataContext.Name} health could not be checked.");

                request.DataContext.State = State.Unknown;
            }
        }
    }
}
