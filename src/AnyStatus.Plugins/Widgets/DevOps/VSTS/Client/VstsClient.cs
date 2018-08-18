using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AnyStatus
{
    public class VstsClient
    {
        public string Account { get; set; }

        public string Project { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        #region Builds

        public async Task<VSTSBuildDefinition> GetBuildDefinitionAsync(string name)
        {
            var definitions = await Request<Collection<VSTSBuildDefinition>>("build/definitions?$top=1&name=" + name).ConfigureAwait(false);

            if (definitions?.Value == null)
                throw new Exception("An error occurred while requesting  VSTS build definition.");

            var definition = definitions.Value.Find(k => k.Name.Equals(name));

            if (definition == null)
                throw new Exception($"VSTS build definition {name} was not found.");

            return definition;
        }

        #endregion Builds

        #region Releases

        public async Task<VSTSReleaseDefinition> GetReleaseDefinitionAsync(string name)
        {
            var definitions = await Request<Collection<VSTSReleaseDefinition>>("release/definitions?searchText=" + name, true).ConfigureAwait(false);

            if (definitions?.Value == null)
                throw new Exception("Invalid release definition query response.");

            var definition = definitions.Value.Find(k => k.Name.Equals(name));

            if (definition == null)
                throw new Exception($"Release definition {name} was not found.");

            return definition;
        }

        public async Task<VSTSRelease> GetLastReleaseAsync(long releaseDefinitionId)
        {
            var releases = await Request<Collection<VSTSRelease>>("release/releases?$top=1&definitionId=" + releaseDefinitionId, true).ConfigureAwait(false);

            if (releases?.Value == null)
                throw new Exception($"VSTS last release of release definition id {releaseDefinitionId} was not found.");

            return releases.Value.FirstOrDefault();
        }

        public async Task<VSTSReleaseDetails> GetReleaseDetailsAsync(long releaseId)
        {
            var details = await Request<VSTSReleaseDetails>("release/releases/" + releaseId, true).ConfigureAwait(false);

            if (details == null)
                throw new Exception("VSTS Release release details were not found.");

            return details;
        }

        #endregion Releases

        #region Helpers

        internal async Task<T> Request<T>(string api, bool vsrm = false)
        {
            using (var handler = new WebRequestHandler())
            {
                var httpClient = new HttpClient(handler);

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrEmpty(Password))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(Encoding.ASCII.GetBytes($"{UserName}:{Password}")));
                }

                var sb = new StringBuilder();
                sb.Append("https://");
                sb.Append(Account);
                if (vsrm) sb.Append(".vsrm");
                sb.Append(".visualstudio.com/");
                sb.Append(Project);
                sb.Append("/_apis/");
                sb.Append(api);

                var response = await httpClient.GetAsync(sb.ToString()).ConfigureAwait(false);

                EnsureSuccessStatusCode(response.StatusCode);

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return new JavaScriptSerializer().Deserialize<T>(content);
            }
        }

        private static void EnsureSuccessStatusCode(HttpStatusCode statusCode)
        {
            if (statusCode != HttpStatusCode.OK)
                throw new VstsClientException($"Invalid HTTP response status code: {(int)statusCode} ({statusCode}). Please verify your User Name and Password or Personal Acceess Token.");
        }

        internal async Task Send<T>(string api, T request = default(T), bool vsrm = false, bool patch = false)
        {
            using (var handler = new WebRequestHandler())
            {
                var httpClient = new HttpClient(handler);

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrEmpty(Password))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{UserName}:{Password}")));
                }

                var sb = new StringBuilder();
                sb.Append("https://");
                sb.Append(Account);
                if (vsrm) sb.Append(".vsrm");
                sb.Append(".visualstudio.com/");
                sb.Append(Project);
                sb.Append("/_apis/");
                sb.Append(api);

                var json = new JavaScriptSerializer().Serialize(request);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response;

                if (patch)
                {
                    response = await PatchAsync(httpClient, sb.ToString(), content, CancellationToken.None).ConfigureAwait(false);
                }
                else
                {
                    response = await httpClient.PostAsync(sb.ToString(), content).ConfigureAwait(false);
                }

                response.EnsureSuccessStatusCode();
            }
        }

        private static Task<HttpResponseMessage> PatchAsync(HttpClient client, string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            var method = new HttpMethod("PATCH");

            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = content
            };

            return client.SendAsync(request, cancellationToken);
        }

        #endregion Helpers
    }
}