using AnyStatus.API;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AnyStatus
{
    public class GitHubIssueStateCheck : ICheckHealth<GitHubIssue>
    {
        [DebuggerStepThrough]
        public async Task Handle(HealthCheckRequest<GitHubIssue> request, CancellationToken cancellationToken)
        {
            var state = await GetGitHubIssueStateAsync(request.DataContext).ConfigureAwait(false);

            switch (state)
            {
                case GitHubIssueState.Open:
                    request.DataContext.State = State.Open;
                    break;

                case GitHubIssueState.Closed:
                    request.DataContext.State = State.Closed;
                    break;

                default:
                    request.DataContext.State = State.Unknown;
                    break;
            }
        }

        private async Task<GitHubIssueState> GetGitHubIssueStateAsync(GitHubIssue issue)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("ANYSTATUS", "1.0"));

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(issue.URL).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var githubIssue = new JavaScriptSerializer().Deserialize<GitHubIssueDetails>(content);

                return githubIssue.State;
            }
        }

        private class GitHubIssueDetails
        {
            public GitHubIssueState State { get; set; }
        }

        public enum GitHubIssueState
        {
            None,
            Open,
            Closed,
            All
        }
    }
}