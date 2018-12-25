using AnyStatus.API;
using AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Queries
{
    public class GetBuildDefinition
    {
        public class Request : IRequest<Response>
        {
            public AzureDevOpsConnection Connection { get; set; }

            public string BuildDefinitionName { get; set; }
        }

        public class Response
        {
            public AzureDevOpsBuildDefinition BuildDefinition { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var definitions = await AzureDevOpsUtils.Request<AzureDevOpsCollection<AzureDevOpsBuildDefinition>>(request.Connection, $"build/definitions?$top=1&name={Uri.EscapeDataString(request.BuildDefinitionName)}").ConfigureAwait(false);

                var definition = definitions?.Value?.Find(k => k.Name.Equals(request.BuildDefinitionName));

                if (definition == null)
                    throw new Exception($"Azure DevOps build definition \"{request.BuildDefinitionName}\" was not found.");

                return new Response { BuildDefinition = definition };
            }
        }
    }
}
