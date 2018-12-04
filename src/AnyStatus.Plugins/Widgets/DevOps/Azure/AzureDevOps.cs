using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using AnyStatus.Plugins.AzureDevOps;

namespace AnyStatus
{
    public class AzureDevOps
    {
        #region Properties

        public string Account { get; set; }

        public string Project { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        #endregion

        #region Azure DevOps

        public async Task<VSTSBuildDefinition> GetBuildDefinitionAsync(string name)
        {
            var definitions = await Request<Collection<VSTSBuildDefinition>>($"build/definitions?$top=1&name={Uri.EscapeDataString(name)}").ConfigureAwait(false);

            var definition = definitions?.Value?.Find(k => k.Name.Equals(name));

            if (definition == null)
                throw new Exception($"VSTS build definition \"{name}\" was not found.");

            return definition;
        }

        public async Task<VSTSReleaseDefinition> GetReleaseDefinitionAsync(string name)
        {
            var definitions = await Request<Collection<VSTSReleaseDefinition>>($"release/definitions?searchText={Uri.EscapeDataString(name)}", true).ConfigureAwait(false);

            var definition = definitions?.Value?.Find(k => k.Name.Equals(name));

            if (definition == null)
                throw new Exception($"Release definition {name} was not found.");

            return definition;
        }

        public async Task<VSTSRelease> GetLastReleaseAsync(long releaseDefinitionId)
        {
            var releases = await Request<Collection<VSTSRelease>>("release/releases?$top=1&definitionId=" + releaseDefinitionId, true).ConfigureAwait(false);

            if (releases?.Value == null)
                throw new Exception($"VSTS last release of release definition id \"{releaseDefinitionId}\" was not found.");

            return releases.Value.FirstOrDefault();
        }

        public async Task<VSTSReleaseDetails> GetReleaseDetailsAsync(long releaseId)
        {
            var details = await Request<VSTSReleaseDetails>($"release/releases/{releaseId}", true).ConfigureAwait(false);

            if (details == null)
                throw new Exception("VSTS release details could not be found.");

            return details;
        }

        #endregion

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

                var uri = CreateUri(api, vsrm);

                var response = await httpClient.GetAsync(uri).ConfigureAwait(false);

                EnsureSuccessStatusCode(response.StatusCode);

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return new JavaScriptSerializer().Deserialize<T>(content);
            }
        }

        private static void EnsureSuccessStatusCode(HttpStatusCode statusCode)
        {
            if (statusCode != HttpStatusCode.OK)
                throw new AzureDevOpsException($"Invalid HTTP response status code: {(int)statusCode} ({statusCode}). Please make sure your User Name, Password or Personal Acceess Token are correct.");
        }

        private string CreateUri(string api, bool vsrm)
        {
            var sb = new StringBuilder();
            sb.Append("https://");
            sb.Append(Account);
            if (vsrm) sb.Append(".vsrm");
            sb.Append(".visualstudio.com/");
            sb.Append(Project);
            sb.Append("/_apis/");
            sb.Append(api);

            return sb.ToString();
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

                var uri = CreateUri(api, vsrm);

                var json = new JavaScriptSerializer().Serialize(request);

                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = patch
                    ? await PatchAsync(httpClient, uri, httpContent, CancellationToken.None).ConfigureAwait(false)
                    : await httpClient.PostAsync(uri, httpContent).ConfigureAwait(false);

#if DEBUG
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#endif

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

        #endregion
    }
}