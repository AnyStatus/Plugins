using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AnyStatus
{
    public class TeamCityClient
    {
        public TeamCityClient(TeamCityConnection connection)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public TeamCityConnection Connection { get; set; }

        public async Task<TeamCityContracts.BuildDetails> GetBuildDetailsAsync(TeamCityBuild item)
        {
            var branchLocator = string.IsNullOrWhiteSpace(item.SourceControlBranch) ? string.Empty : ",branch:" + item.SourceControlBranch;

            var api = $"builds?locator=running:any,buildType:(id:{item.BuildTypeId}),count:1{branchLocator}&fields=build(status,running,statusText)";

            var buildResponse = await RequestAsync<TeamCityContracts.BuildDetailsResponse>(api);

            if (buildResponse == null || buildResponse.Build == null || !buildResponse.Build.Any())
                throw new TeamCityException("Unexpected TeamCity response.");

            return buildResponse.Build.First();
        }

        public async Task QueueNewBuild(TeamCityBuild item)
        {
            //todo: move to SendAsync<T>

            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = true;

                if (item.IgnoreSslErrors)
                {
                    handler.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                }

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                    var uri = Connection.GuestUser
                        ? $"{item.Url}/guestAuth/app/rest/buildQueue"
                        : $"{item.Url}/app/rest/buildQueue";

                    if (!string.IsNullOrEmpty(item.Token))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Connection.Token);
                    }

                    var request = $"<build><buildType id=\"{item.BuildTypeId}\"/></build>";

                    var content = new StringContent(request, Encoding.UTF8, "application/xml");

                    var response = await client.PostAsync(uri, content).ConfigureAwait(false);

                    response.EnsureSuccessStatusCode();
                }
            }
        }

        #region Helpers

        private async Task<T> RequestAsync<T>(string api)
        {

            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = true;

                if (Connection.IgnoreSslErrors)
                {
                    handler.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                }

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (!Connection.GuestUser && !string.IsNullOrEmpty(Connection.Token))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Connection.Token);
                    }

                    var uri = Connection.GuestUser
                        ? $"{Connection.Url}/guestAuth/app/rest/{api}"
                        : $"{Connection.Url}/app/rest/{api}";

                    var response = await client.GetAsync(uri).ConfigureAwait(false);

                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    return new JavaScriptSerializer().Deserialize<T>(content);
                }
            }
        }

        #endregion Helpers
    }
}