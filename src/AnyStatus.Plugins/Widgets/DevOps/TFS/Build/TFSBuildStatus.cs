using AnyStatus.API;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AnyStatus
{
    public class TFSBuildStatus : TFS, ICheckHealth<TfsBuild>
    {
        public async Task Handle(HealthCheckRequest<TfsBuild> request, CancellationToken cancellationToken)
        {
            base.Handle(request.DataContext);

            var buildDetails = await GetBuildDetailsAsync(request.DataContext)
                .ConfigureAwait(false);

            switch (buildDetails.Status)
            {
                case "notStarted":
                    request.DataContext.State = State.Queued;
                    return;

                case "inProgress":
                    request.DataContext.State = State.Running;
                    return;

                default:
                    break;
            }

            switch (buildDetails.Result)
            {
                case "notStarted":
                    request.DataContext.State = State.Running;
                    break;

                case "succeeded":
                    request.DataContext.State = State.Ok;
                    break;

                case "failed":
                    request.DataContext.State = State.Failed;
                    break;

                case "partiallySucceeded":
                    request.DataContext.State = State.PartiallySucceeded;
                    break;

                case "canceled":
                    request.DataContext.State = State.Canceled;
                    break;

                default:
                    request.DataContext.State = State.Unknown;
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

                    if (!handler.UseDefaultCredentials)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{item.UserName}:{item.Password}")));
                    }

                    var url = $"{item.Url}/{Uri.EscapeDataString(item.Collection)}/{Uri.EscapeDataString(item.TeamProject)}/_apis/build/builds?definitions={item.BuildDefinitionId}&$top=1&api-version=2.0";

                    var response = await client.GetAsync(url).ConfigureAwait(false);

                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    var buildDetailsResponse = new JavaScriptSerializer().Deserialize<TfsBuildDetailsResponse>(content);

                    return buildDetailsResponse.Value.First();
                }
            }
        }
    }
}