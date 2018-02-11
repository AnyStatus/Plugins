using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AnyStatus
{
    public class VstsClient
    {
        public VstsClient()
        {
        }

        public VstsClient(VstsConnection connection)
        {
            Connection = connection;
        }

        public VstsConnection Connection { get; set; }

        #region Build

        public async Task<VSTSBuildDefinition> GetBuildDefinitionAsync(string name)
        {
            var definitions = await Request<Collection<VSTSBuildDefinition>>("build/definitions?$top=1&name=" + name);

            if (definitions == null || definitions.Value == null)
                throw new Exception("Invalid build definition query response.");

            var definition = definitions.Value.FirstOrDefault(k => k.Name.Equals(name));

            if (definition == null)
                throw new Exception("VSTS build definition not found.");

            return definition;
        }

        public async Task<VSTSBuild> GetLatestBuildAsync(long definitionId)
        {
            var builds = await Request<Collection<VSTSBuild>>($"build/builds?definitions={definitionId}&$top=1&api-version=2.0").ConfigureAwait(false);

            return builds?.Value?.FirstOrDefault();
        }

        public async Task QueueNewBuildAsync(long definitionId)
        {
            var request = new
            {
                Definition = new
                {
                    Id = definitionId
                }
            };

            await Send("build/builds?api-version=2.0", request).ConfigureAwait(false);
        }

        #endregion Build

        #region Release

        public async Task<VSTSReleaseDefinition> GetReleaseDefinitionAsync(string name)
        {
            var definitions = await Request<Collection<VSTSReleaseDefinition>>("release/definitions?searchText=" + name, true).ConfigureAwait(false);

            if (definitions == null || definitions.Value == null)
                throw new Exception("Invalid release definition query response.");

            var definition = definitions.Value.FirstOrDefault(k => k.Name.Equals(name));

            if (definition == null)
                throw new Exception("Release definition not found.");

            return definition;
        }

        public async Task<VSTSRelease> GetLatestReleaseAsync(long releaseDefinitionId)
        {
            var releases = await Request<Collection<VSTSRelease>>("release/releases?$top=1&definitionId=" + releaseDefinitionId, true).ConfigureAwait(false);

            if (releases == null || releases.Value == null)
                throw new Exception("VSTS release not found.");

            return releases.Value.FirstOrDefault();
        }

        public async Task<VSTSReleaseDetails> GetReleaseDetailsAsync(long releaseId)
        {
            var details = await Request<VSTSReleaseDetails>("release/releases/" + releaseId, true).ConfigureAwait(false);

            if (details == null)
                throw new Exception("VSTS Release release details were not found.");

            return details;
        }

        #endregion Release

        private async Task<T> Request<T>(string api, bool vsrm = false)
        {
            using (var handler = new WebRequestHandler())
            {
                var httpClient = new HttpClient(handler);

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrEmpty(Connection.Password))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Connection.UserName}:{Connection.Password}")));
                }

                var sb = new StringBuilder();
                sb.Append("https://");
                sb.Append(Connection.Account);
                if (vsrm) sb.Append(".vsrm");
                sb.Append(".visualstudio.com/");
                sb.Append(Connection.Project);
                sb.Append("/_apis/");
                sb.Append(api);

                var response = await httpClient.GetAsync(sb.ToString()).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return new JavaScriptSerializer().Deserialize<T>(content);
            }
        }

        private async Task Send<T>(string api, T request = default(T), bool vsrm = false)
        {
            using (var handler = new WebRequestHandler())
            {
                var httpClient = new HttpClient(handler);

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrEmpty(Connection.Password))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Connection.UserName}:{Connection.Password}")));
                }

                var sb = new StringBuilder();
                sb.Append("https://");
                sb.Append(Connection.Account);
                if (vsrm) sb.Append(".vsrm");
                sb.Append(".visualstudio.com/");
                sb.Append(Connection.Project);
                sb.Append("/_apis/");
                sb.Append(api);

                var json = new JavaScriptSerializer().Serialize(request);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(sb.ToString(), content).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();
            }
        }
    }
}