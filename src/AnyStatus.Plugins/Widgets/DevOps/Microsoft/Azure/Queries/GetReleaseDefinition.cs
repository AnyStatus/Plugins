using AnyStatus.API;
using AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Queries
{
    public class GetReleaseDefinition
    {
        public class Request : IRequest<Response>
        {
            public AzureDevOpsConnection Connection { get; set; }

            public string Name { get; set; }
        }

        public class Response
        {
            public AzureDevOpsReleaseDefinition ReleaseDefinition { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var definitions = await AzureDevOpsUtils.Request<AzureDevOpsCollection<AzureDevOpsReleaseDefinition>>(request.Connection, $"release/definitions?searchText={Uri.EscapeDataString(request.Name)}", true).ConfigureAwait(false);

                var definition = definitions?.Value?.Find(k => k.Name.Equals(request.Name));

                if (definition == null)
                    throw new Exception($"Release definition \"{request.Name}\" was not found.");

                return new Response { ReleaseDefinition = definition };
            }
        }
    }
}
