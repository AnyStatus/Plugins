using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AnyStatus
{
    public interface IJenkinsClient
    {
        Task<JenkinsJob> GetJobAsync(IJenkinsPlugin jenkinsPlugin);

        Task<JenkinsView> GetViewAsync(IJenkinsPlugin jenkinsPlugin);

        Task TriggerJobAsync(JenkinsJob_v1 jenkinsJob);
    }

    public class JenkinsClient : IJenkinsClient
    {
        public async Task<JenkinsJob> GetJobAsync(IJenkinsPlugin jenkinsPlugin)
        {
            return await QueryAsync<JenkinsJob>(jenkinsPlugin, "lastBuild/api/json?tree=result,building,executor[progress]");
        }

        public async Task<JenkinsView> GetViewAsync(IJenkinsPlugin jenkinsPlugin)
        {
            return await QueryAsync<JenkinsView>(jenkinsPlugin, "api/json");
        }

        public async Task TriggerJobAsync(JenkinsJob_v1 jenkinsJob)
        {
            var api = new StringBuilder();

            api.Append("buildWithParameters?delay=0sec");

            if (jenkinsJob.BuildParameters != null && jenkinsJob.BuildParameters.Any())
            {
                foreach (var parameter in jenkinsJob.BuildParameters)
                {
                    if (parameter == null ||
                        string.IsNullOrWhiteSpace(parameter.Name) ||
                        string.IsNullOrWhiteSpace(parameter.Value))
                        continue;

                    api.Append("&");
                    api.Append(WebUtility.UrlEncode(parameter.Name));
                    api.Append("=");
                    api.Append(WebUtility.UrlEncode(parameter.Value));
                }
            }

            await PostAsync(jenkinsJob, api.ToString());
        }

        private async Task<T> QueryAsync<T>(IJenkinsPlugin jenkinsPlugin, string api, bool useBaseUri = false)
        {
            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = true;

                if (jenkinsPlugin.IgnoreSslErrors)
                {
                    handler.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                }

                using (var client = new HttpClient(handler))
                {
                    if (!string.IsNullOrEmpty(jenkinsPlugin.UserName) && !string.IsNullOrEmpty(jenkinsPlugin.ApiToken))
                    {
                        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{jenkinsPlugin.UserName}:{jenkinsPlugin.ApiToken}"));

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                    }

                    var jenkinsUri = new Uri(jenkinsPlugin.URL);

                    if (useBaseUri) jenkinsUri = new Uri(jenkinsUri.GetLeftPart(UriPartial.Authority));

                    var uri = new Uri(jenkinsUri, api);

                    var response = await client.GetAsync(uri);

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                        throw new UnauthorizedAccessException("Jenkins request was unauthorized.");

                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();

                    var result = new JavaScriptSerializer().Deserialize<T>(content);

                    if (result == null) throw new Exception("Jenkins response is null.");

                    return result;
                }
            }
        }

        private async Task PostAsync(IJenkinsPlugin jenkinsPlugin, string api, bool useBaseUri = false)
        {
            var crumb = await GetCrumb(jenkinsPlugin);

            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = true;

                if (jenkinsPlugin.IgnoreSslErrors)
                {
                    handler.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                }

                using (var client = new HttpClient(handler))
                {
                    if (!string.IsNullOrEmpty(jenkinsPlugin.UserName) && !string.IsNullOrEmpty(jenkinsPlugin.ApiToken))
                    {
                        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{jenkinsPlugin.UserName}:{jenkinsPlugin.ApiToken}"));

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                    }

                    if (crumb != null)
                    {
                        client.DefaultRequestHeaders.Add(crumb.CrumbRequestField, crumb.Crumb);
                    }

                    var jenkinsUri = new Uri(jenkinsPlugin.URL);

                    if (useBaseUri) jenkinsUri = new Uri(jenkinsUri.GetLeftPart(UriPartial.Authority));

                    var uri = new Uri(jenkinsUri, api);

                    HttpResponseMessage response = null;

                    response = await client.PostAsync(uri, new StringContent(string.Empty));

                    if (response.StatusCode == HttpStatusCode.Forbidden)
                        throw new UnauthorizedAccessException("Jenkins request was forbidden. Try enabling CSRF in the properties window.");

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                        throw new UnauthorizedAccessException("Jenkins request was unauthorized.");

                    response.EnsureSuccessStatusCode();
                }
            }
        }

#warning add error handling and logging + save crumb in plugin
        private async Task<JenkinsCrumb> GetCrumb(IJenkinsPlugin jenkinsPlugin)
        {
            JenkinsCrumb crumb = null;

            if (jenkinsPlugin.CSRF)
            {
                crumb = await QueryAsync<JenkinsCrumb>(jenkinsPlugin, "crumbIssuer/api/json", true);
            }

            return crumb;
        }
    }
}
