using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AnyStatus
{
    public class JenkinsRequest : IDisposable
    {
        private bool _disposed;
        private HttpClient _client;
        private WebRequestHandler _handler;

        public JenkinsRequest(IJenkinsPlugin jenkinsPlugin)
        {
            if (jenkinsPlugin == null)
                throw new ArgumentNullException(nameof(jenkinsPlugin));

            Initialize(jenkinsPlugin);
        }

        public async Task<T> GetAsync<T>(IJenkinsPlugin jenkinsPlugin, string api, bool useBaseUri = false)
        {
            var endpoint = GetEndpoint(jenkinsPlugin, api, useBaseUri);

            var response = await _client.GetAsync(endpoint).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (string.IsNullOrEmpty(content)) throw new Exception("Jenkins response is null or empty.");

            var result = new JavaScriptSerializer().Deserialize<T>(content);

            return result;
        }

        public async Task PostAsync(IJenkinsPlugin jenkinsPlugin, string api, bool useBaseUri = false, JenkinsCrumb crumb = null)
        {
            AddCrumbHeader(crumb);

            var endpoint = GetEndpoint(jenkinsPlugin, api, useBaseUri);

            var test = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("json", "{\"parameter\":[]}") });

            var response = await _client.PostAsync(endpoint, test).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }

        public void Dispose()
        {
            if (_disposed) return;

            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }

            if (_handler != null)
            {
                _handler.Dispose();
                _handler = null;
            }

            _disposed = true;
        }

        #region Helpers

        private void Initialize(IJenkinsPlugin jenkinsPlugin)
        {
            try
            {
                _handler = new WebRequestHandler
                {
                    UseDefaultCredentials = true
                };

                if (jenkinsPlugin.IgnoreSslErrors)
                {
                    _handler.ServerCertificateValidationCallback += OnServerCertificateValidationCallback;
                }

                _client = new HttpClient(_handler);

                if (string.IsNullOrEmpty(jenkinsPlugin.UserName) || string.IsNullOrEmpty(jenkinsPlugin.ApiToken)) return;

                Authorize(jenkinsPlugin);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating Jenkins request. See inner exception.", ex);
            }
        }

        private void Authorize(IJenkinsPlugin jenkinsPlugin)
        {
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{jenkinsPlugin.UserName}:{jenkinsPlugin.ApiToken}"));

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        }

        private void AddCrumbHeader(JenkinsCrumb crumb)
        {
            if (!crumb.IsValid()) return;

            _client.DefaultRequestHeaders.Add(crumb.CrumbRequestField, crumb.Crumb);
        }

        private static bool OnServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;

        private static Uri GetEndpoint(IJenkinsPlugin jenkinsPlugin, string api, bool useBaseUri)
        {
            var jenkinsUri = new Uri(jenkinsPlugin.URL);

            if (useBaseUri) jenkinsUri = new Uri(jenkinsUri.GetLeftPart(UriPartial.Authority));

            return new Uri(jenkinsUri, api);
        }

        #endregion Helpers
    }
}