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
    public class TfsBuildMonitor : BaseTfsBuildHandler, IMonitor<TfsBuild>
    {
        [DebuggerStepThrough]
        public override void Handle(TfsBuild item)
        {
            base.Handle(item);

            var buildDetails = GetBuildDetailsAsync(item).Result;

            switch (buildDetails.Status)
            {
                case "notStarted":
                    item.State = State.Queued;
                    return;
                case "inProgress":
                    item.State = State.Running;
                    return;
                default:
                    break;
            }

            switch (buildDetails.Result)
            {
                case "notStarted":
                    item.State = State.Running;
                    break;
                case "succeeded":
                    item.State = State.Ok;
                    break;
                case "failed":
                    item.State = State.Failed;
                    break;
                case "partiallySucceeded":
                    item.State = State.PartiallySucceeded;
                    break;
                case "canceled":
                    item.State = State.Canceled;
                    break;
                default:
                    item.State = State.Unknown;
                    break;
            }
        }

        private async Task<TfsBuildDetails> GetBuildDetailsAsync(TfsBuild item)
        {
            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = string.IsNullOrEmpty(item.UserName) || string.IsNullOrEmpty(item.Password); ;

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (handler.UseDefaultCredentials == false)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{item.UserName}:{item.Password}")));
                    }

                    var url = $"{item.Url}/{item.Collection}/{item.TeamProject}/_apis/build/builds?definitions={item.BuildDefinitionId}&$top=1&api-version=2.0";

                    var response = await client.GetAsync(url);

                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();

                    var buildDetailsResponse = new JavaScriptSerializer().Deserialize<TfsBuildDetailsResponse>(content);

                    return buildDetailsResponse.Value.First();
                }
            }
        }
    }
}
