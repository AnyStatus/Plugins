using AnyStatus.API;
using AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Queries
{
    public class GetReleaseDetails
    {
        public class Request : IRequest<Response>
        {
            public AzureDevOpsConnection Connection { get; set; }

            public string ReleaseId { get; set; }
        }

        public class Response
        {
            public AzureDevOpsReleaseDetails ReleaseDetails { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var details = await AzureDevOpsUtils.Request<AzureDevOpsReleaseDetails>(request.Connection, $"release/releases/{request.ReleaseId}", true).ConfigureAwait(false);

                if (details == null)
                    throw new Exception("VSTS release details could not be found.");

                return new Response { ReleaseDetails = details };
            }
        }
    }
}
