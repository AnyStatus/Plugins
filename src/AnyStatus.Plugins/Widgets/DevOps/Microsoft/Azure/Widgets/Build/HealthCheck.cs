using AnyStatus.API;
using AnyStatus.API.Utils;
using AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Queries;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Widgets
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

            request.DataContext.MapTo(connection);

            if (!request.DataContext.IsInitialized)
            {
                _logger.Info($"{request.DataContext.Name} was not initialized.");

                return;
            }

            var getBuildsRequest = new GetBuilds.Request
            {
                Top = 1,
                Connection = connection,
                BuildDefinitionId = request.DataContext.BuildDefinitionId,
            };

            var response = await _m.Send(getBuildsRequest, cancellationToken).ConfigureAwait(false);

            var build = response?.Builds?.FirstOrDefault();

            request.DataContext.State = build != null ? build.State : State.Unknown;
        }
    }
}
