using AnyStatus.API;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class JenkinsClient : IJenkinsClient
    {
        private readonly ILogger _logger;

        public JenkinsClient(ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
        }

        #region IJenkinsClient 

        public async Task<JenkinsJob> GetJobAsync(IJenkinsPlugin jenkinsPlugin)
        {
            const string api = "lastBuild/api/json?tree=result,building,executor[progress]";

            using (var jenkinsRequest = new JenkinsRequest(jenkinsPlugin))
            {
                return await jenkinsRequest.GetAsync<JenkinsJob>(jenkinsPlugin, api).ConfigureAwait(false);
            }
        }

        public async Task<JenkinsView> GetViewAsync(IJenkinsPlugin jenkinsPlugin)
        {
            const string api = "api/json";

            using (var jenkinsRequest = new JenkinsRequest(jenkinsPlugin))
            {
                return await jenkinsRequest.GetAsync<JenkinsView>(jenkinsPlugin, api).ConfigureAwait(false);
            }
        }

        public async Task TriggerJobAsync(JenkinsJob_v1 jenkinsPlugin)
        {
            var api = jenkinsPlugin.HasBuildParameters ? BuildWithParametersApi(jenkinsPlugin) : "build";

            var crumb = jenkinsPlugin.CSRF ? await IssueCrumbAsync(jenkinsPlugin).ConfigureAwait(false) : null;

            using (var jenkinsRequest = new JenkinsRequest(jenkinsPlugin))
            {
                await jenkinsRequest.PostAsync(jenkinsPlugin, api, false, crumb).ConfigureAwait(false);
            }
        }

        #endregion

        #region Helpers

        private async Task<JenkinsCrumb> IssueCrumbAsync(IJenkinsPlugin jenkinsPlugin)
        {
            const string api = "crumbIssuer/api/json";

            try
            {
                using (var jenkinsRequest = new JenkinsRequest(jenkinsPlugin))
                {
                    var crumb = await jenkinsRequest.GetAsync<JenkinsCrumb>(jenkinsPlugin, api, true).ConfigureAwait(false);

                    if (!crumb.IsValid())
                    {
                        _logger.Info("Jenkins server did not return a valid crumb. Make sure your user name and API token are correct.");
                    }

                    return crumb;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Jenkins: An error occurred while requesting crumb. See inner exception.", ex);
            }
        }

        private static string BuildWithParametersApi(JenkinsJob_v1 jenkinsPlugin)
        {
            var sb = new StringBuilder("buildWithParameters?delay=0sec");

            foreach (var parameter in jenkinsPlugin.BuildParameters)
            {
                if (parameter == null || string.IsNullOrWhiteSpace(parameter.Name) || string.IsNullOrWhiteSpace(parameter.Value)) continue;

                sb.Append("&");
                sb.Append(WebUtility.UrlEncode(parameter.Name));
                sb.Append("=");
                sb.Append(WebUtility.UrlEncode(parameter.Value));
            }

            return sb.ToString();
        }

        #endregion
    }
}
