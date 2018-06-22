using AnyStatus.API;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AnyStatus
{
    public class AppVeyorBuildStatus : ICheckHealth<AppVeyorBuild>
    {
        [DebuggerStepThrough]
        public async Task Handle(HealthCheckRequest<AppVeyorBuild> request, CancellationToken cancellationToken)
        {
            var build = await GetBuildDetailsAsync(request.DataContext).ConfigureAwait(false);

            request.DataContext.State = GetState(build.Status);

            request.DataContext.StateText = "Version: " + build.Version;
        }

        private State GetState(string status)
        {
            switch (status)
            {
                case "success":
                    return State.Ok;

                case "failed":
                case "failure":
                    return State.Failed;

                case "cancelled":
                    return State.Canceled;

                case "queued":
                    return State.Queued;

                case "running":
                    return State.Running;

                default:
                    return State.Unknown;
            }
        }

        private async Task<AppVeyorBuildDetails> GetBuildDetailsAsync(AppVeyorBuild item)
        {
            const string host = @"https://ci.appveyor.com/api/projects";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", item.ApiToken);

                var apiUrl = default(string);

                if (string.IsNullOrWhiteSpace(item.SourceControlBranch))
                {
                    apiUrl = $"{host}/{item.AccountName}/{item.ProjectSlug}";
                }
                else
                {
                    apiUrl = $"{host}/{item.AccountName}/{item.ProjectSlug}/branch/{item.SourceControlBranch}";
                }

                var response = await client.GetAsync(apiUrl);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var buildResponse = new JavaScriptSerializer().Deserialize<AppVeyorBuildResponse>(content);

                return buildResponse.Build;
            }
        }
    }
}