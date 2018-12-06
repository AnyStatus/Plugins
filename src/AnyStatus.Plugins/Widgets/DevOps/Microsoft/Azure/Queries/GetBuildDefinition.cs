using AnyStatus.API;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

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
            public long Id { get; set; }

            public string Name { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var definitions = await Request<Collection<Response>>(request, $"build/definitions?$top=1&name={Uri.EscapeDataString(request.BuildDefinitionName)}").ConfigureAwait(false);

                var definition = definitions?.Value?.Find(k => k.Name.Equals(request.BuildDefinitionName));

                if (definition == null)
                    throw new Exception($"Azure DevOps build definition \"{request.BuildDefinitionName}\" was not found.");

                return definition;
            }

            public class Collection<T>
            {
                public int Count { get; set; }

                public List<T> Value { get; set; }
            }

            private static string CreateUri(AzureDevOpsConnection connection, string api, bool vsrm)
            {
                var sb = new StringBuilder();
                sb.Append("https://");
                sb.Append(connection.Account);
                if (vsrm) sb.Append(".vsrm");
                sb.Append(".visualstudio.com/");
                sb.Append(connection.Project);
                sb.Append("/_apis/");
                sb.Append(api);

                return sb.ToString();
            }

            private static async Task<T> Request<T>(Request request, string api, bool vsrm = false)
            {
                using (var handler = new WebRequestHandler())
                {
                    var httpClient = new HttpClient(handler);

                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (!string.IsNullOrEmpty(request.Connection.Password))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{request.Connection.UserName}:{request.Connection.Password}")));
                    }

                    var uri = CreateUri(request.Connection, api, vsrm);

                    var response = await httpClient.GetAsync(uri).ConfigureAwait(false);

                    EnsureSuccessStatusCode(response.StatusCode);

                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    return new JavaScriptSerializer().Deserialize<T>(content);
                }
            }

            private static void EnsureSuccessStatusCode(HttpStatusCode statusCode)
            {
                if (statusCode != HttpStatusCode.OK)
                    throw new VstsException($"Invalid HTTP response status code: {(int)statusCode} ({statusCode}). Please make sure your User Name, Password or Personal Acceess Token are correct.");
            }
        }
    }
}
