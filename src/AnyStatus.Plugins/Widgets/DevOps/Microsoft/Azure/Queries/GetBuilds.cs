using AnyStatus.API;
using AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Queries
{
    public class GetBuilds
    {
        public class Request : IRequest<Response>
        {
            public AzureDevOpsConnection Connection { get; set; }

            public string BuildDefinitionId { get; set; }

            public int Top { get; set; } = 1;
        }

        public class Response
        {
            public AzureDevOpsBuild[] Builds { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly ILogger _logger;

            public Handler(ILogger logger)
            {
                _logger = logger;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var builds = await AzureDevOpsUtils.Request<AzureDevOpsCollection<AzureDevOpsBuild>>(request.Connection, $"build/builds?definitions={request.BuildDefinitionId}&$top={request.Top}&api-version=2.0").ConfigureAwait(false);

                if (builds?.Value != null)
                    return new Response { Builds = builds.Value.ToArray() };

                _logger.Debug("No releases found for release-definition-id ");

                return new Response();

                //var release = releases.Value.FirstOrDefault();
            }
        }
    }
}
