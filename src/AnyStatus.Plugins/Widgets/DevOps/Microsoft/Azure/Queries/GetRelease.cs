using AnyStatus.API;
using AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Contracts;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Queries
{
    public class GetRelease
    {
        public class Request : IRequest<Response>
        {
            public AzureDevOpsConnection Connection { get; set; }

            public string ReleaseId { get; set; }

            public int Top { get; set; } = 1;
        }

        public class Response
        {
            public AzureDevOpsRelease Release { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var releases = await AzureDevOpsUtils.Request<AzureDevOpsCollection<AzureDevOpsRelease>>(request.Connection, $"release/releases?$top={request.Top}&definitionId={request.ReleaseId}", true).ConfigureAwait(false);

                if (releases?.Value == null)
                    throw new Exception($"VSTS last release of release definition id \"{request.ReleaseId}\" was not found.");

                var release = releases.Value.FirstOrDefault();

                return new Response { Release = release };
            }
        }
    }
}
