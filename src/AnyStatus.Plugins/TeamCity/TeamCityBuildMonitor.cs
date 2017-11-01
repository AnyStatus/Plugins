using AnyStatus.API;
using System;
using System.Collections.Generic;
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
        public void Handle(TeamCityBuild item)
        {
            var build = GetBuildDetailsAsync(item).Result;

            if (build.Running)
            {
                item.State = State.Running;
                return;
            }

            //if (build.CancelledInfo)
            //{
            //    item.Brush = Brushes.Gray;
            //    item.State = State.Canceled;
            //    return;
            //}

            switch (build.Status)
            {
                case "SUCCESS":
                    item.State = State.Ok;
                    break;
                case "FAILURE":
                case "ERROR":
                    item.State = State.Failed;
                    break;
                case "UNKNOWN":
                default:
                    item.State = State.Unknown;
                    break;
            }
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

                    var apiUrl = $"{item.Url}/{authType}/app/rest/builds?locator=running:any,buildType:(id:{item.BuildTypeId}),count:1{branchLocator}&fields=build(status,running)";

                    var response = await client.GetAsync(apiUrl);

                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();

                    var buildResponse = new JavaScriptSerializer().Deserialize<TeamCityBuildDetailsResponse>(content);

                    return buildResponse.Build.First();
                }
            }
        }

        #region Contracts

        private class TeamCityBuildDetailsResponse
        {
            public List<TeamCityBuildDetails> Build { get; set; }
        }

        private class TeamCityBuildDetails
        {
            public bool Running { get; set; }

            public string Status { get; set; }
        }

        #endregion
    }
}
