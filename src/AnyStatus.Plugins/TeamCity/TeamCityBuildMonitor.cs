using AnyStatus.API;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AnyStatus
{
    public class TeamCityBuildMonitor : IMonitor<TeamCityBuild>
    {
        [DebuggerStepThrough]
        public void Handle(TeamCityBuild teamCityBuid)
        {
            var build = GetBuildDetailsAsync(teamCityBuid).Result;

            teamCityBuid.State = build.State;
            teamCityBuid.StateText = build.StatusText;
        }

        private async Task<TeamCityBuildDetails> GetBuildDetailsAsync(TeamCityBuild item)
        {
            if (item.Url.EndsWith("/"))
            {
                //todo: move this to when monitor is created
                item.Url = item.Url.Remove(item.Url.Length - 1);
            }

            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = true;

                if (item.IgnoreSslErrors)
                {
                    handler.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                }

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string authType = string.Empty;

                    if (item.GuestUser)
                    {
                        authType = "guestAuth";
                    }
                    else
                    {
                        authType = "httpAuth";

                        if (!string.IsNullOrEmpty(item.UserName) && !string.IsNullOrEmpty(item.Password))
                        {
                            client.DefaultRequestHeaders.Authorization =
                                new AuthenticationHeaderValue("Basic",
                                    Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", item.UserName, item.Password))));
                        }
                    }

                    var branchLocator = string.Empty;
                    if (!string.IsNullOrWhiteSpace(item.SourceControlBranch))
                    {
                        branchLocator = ",branch:" + item.SourceControlBranch;
                    }

                    var apiUrl = $"{item.Url}/{authType}/app/rest/builds?locator=running:any,buildType:(id:{item.BuildTypeId}),count:1{branchLocator}&fields=build(status,running,statusText)";

                    var response = await client.GetAsync(apiUrl);

                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();

                    var buildResponse = new JavaScriptSerializer().Deserialize<TeamCityBuildDetailsResponse>(content);

                    return buildResponse.Build.First();
                }
            }
        }
    }
}
