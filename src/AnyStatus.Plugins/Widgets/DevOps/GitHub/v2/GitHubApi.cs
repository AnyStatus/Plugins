using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Widgets.DevOps.GitHub
{
    public class GitHubApi
    {
        const string BaseUrl = "https://api.github.com";

        private readonly IRestClient _client;

        public GitHubApi()
        {
            _client = new RestClient(BaseUrl);
        }

        public async Task<T> ExecuteAsync<T>(RestRequest request)
        {
            var response = await _client.ExecuteTaskAsync<T>(request).ConfigureAwait(false);

            if (response.ErrorException != null)
            {
                throw new ApplicationException("An error occurred while executing GitHub API request.", response.ErrorException);
            }

            return response.Data;
        }

        public async Task<IEnumerable<GitHubIssue>> GetIssuesAsync(string owner, string repository)
        {
            var request = new RestRequest($"repos/{owner}/{repository}/issues");

            return await ExecuteAsync<IEnumerable<GitHubIssue>>(request);
        }
    }
}
