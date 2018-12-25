using AnyStatus.API;
using AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Queries;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Widgets
{

    public class AzureDevOpsBuildWidgetInitializer : IInitialize<AzureDevOpsBuildWidget>
    {
        private readonly IMediator _m;
        private readonly ILogger _logger;

        public AzureDevOpsBuildWidgetInitializer(ILogger logger, IMediator mediator)
        {
            _m = mediator;
            _logger = logger;
        }

        public async Task Handle(InitializeRequest<AzureDevOpsBuildWidget> request, CancellationToken cancellationToken)
        {
            var buildDefinitionRequest = new GetBuildDefinition.Request
            {
                Connection = request.DataContext.Connection,
                BuildDefinitionName = request.DataContext.BuildDefinitionName
            };

            var response = await _m.Send(buildDefinitionRequest, cancellationToken).ConfigureAwait(false);

            request.DataContext.BuildDefinitionId = response.BuildDefinition.Id;

            request.DataContext.IsInitialized = true;
        }
    }
}
