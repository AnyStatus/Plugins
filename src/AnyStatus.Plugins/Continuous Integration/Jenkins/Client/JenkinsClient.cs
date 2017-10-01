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
                var result = await jenkinsRequest.GetAsync<JenkinsJob>(jenkinsPlugin, api).ConfigureAwait(false);

                return result;
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
            var api = string.Empty;

            if (jenkinsPlugin.HasBuildParameters)
            {
                var sb = new StringBuilder();

                sb.Append("buildWithParameters?delay=0sec");

                foreach (var parameter in jenkinsPlugin.BuildParameters)
                {
                    if (parameter == null ||
                        string.IsNullOrWhiteSpace(parameter.Name) ||
                        string.IsNullOrWhiteSpace(parameter.Value))
                        continue;

                    sb.Append("&");
                    sb.Append(WebUtility.UrlEncode(parameter.Name));
                    sb.Append("=");
                    sb.Append(WebUtility.UrlEncode(parameter.Value));
                }

                api = sb.ToString();
            }
            else
            {
                api = "build";
            }

            var crumb = jenkinsPlugin.CSRF ? await GetCrumbAsync(jenkinsPlugin).ConfigureAwait(false) : null;

            using (var jenkinsRequest = new JenkinsRequest(jenkinsPlugin))
            {
                await jenkinsRequest.PostAsync(jenkinsPlugin, api, false, crumb).ConfigureAwait(false);
            }
        }

        public async Task<JenkinsCrumb> GetCrumbAsync(IJenkinsPlugin jenkinsPlugin)
        {
            const string api = "crumbIssuer/api/json";

            try
            {
                using (var jenkinsRequest = new JenkinsRequest(jenkinsPlugin))
                {
                    return await jenkinsRequest.GetAsync<JenkinsCrumb>(jenkinsPlugin, api, true).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while requesting Jenkins crumb. See inner exception.", ex);
            }
        }

        #endregion

        //private async Task LogResponse(HttpResponseMessage response)
        //{
        //    if (response.IsSuccessStatusCode) return;
        //    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        //    var sb = new StringBuilder();
        //    sb.Append("Jenkins request has failed. ");
        //    if (response.StatusCode == HttpStatusCode.Forbidden)
        //        sb.Append("Try enabling CSRF in the properties window. ");
        //    sb.Append("Response Code: ");
        //    sb.Append(response.StatusCode);
        //    sb.Append(" ");
        //    sb.Append(Enum.GetName(typeof(HttpStatusCode), response.StatusCode));
        //    sb.AppendLine(content);
        //    _logger.Info(sb.ToString());
        //}
    }
}
