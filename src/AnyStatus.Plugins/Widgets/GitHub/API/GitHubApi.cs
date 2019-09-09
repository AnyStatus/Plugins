using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using RestSharp;

namespace AnyStatus.Plugins.Widgets.GitHub.API
{
    public class GitHubApi
    {
        private readonly IRestClient _client;
        private const string BaseUrl = "https://api.github.com";

        public GitHubApi()
        {
            _client = new RestClient(BaseUrl);
        }

        private async Task<T> ExecuteAsync<T>(RestRequest request)
        {
            var response = await _client.ExecuteTaskAsync<T>(request).ConfigureAwait(false);

            return response.StatusCode == HttpStatusCode.OK ? response.Data :
                throw new ApplicationException($"An error occurred while executing GitHub API request. Status Code: {response.StatusCode}. Content: {response.Content}", response.ErrorException);
        }

        public async Task<IEnumerable<GitHubIssue>> GetIssuesAsync(string owner, string repo)
        {
            var request = new RestRequest($"repos/{owner}/{repo}/issues");

            request.AddParameter("per_page", 1000);

            return await ExecuteAsync<IEnumerable<GitHubIssue>>(request).ConfigureAwait(false);
        }

        public async Task<GitHubIssue> GetIssueAsync(string owner, string repo, int issue)
        {
            var request = new RestRequest($"repos/{owner}/{repo}/issues/{issue}");

            return await ExecuteAsync<GitHubIssue>(request).ConfigureAwait(false);
        }
    }
}
