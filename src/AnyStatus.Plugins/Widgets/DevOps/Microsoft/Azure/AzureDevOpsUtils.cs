using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure
{
    public static class AzureDevOpsUtils
    {
        public static async Task<T> Request<T>(AzureDevOpsConnection connection, string api, bool vsrm = false)
        {
            using (var handler = new WebRequestHandler())
            {
                var httpClient = new HttpClient(handler);

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrEmpty(connection.Password))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(Encoding.ASCII.GetBytes($"{connection.UserName}:{connection.Password}")));
                }

                var uri = CreateUri(connection, api, vsrm);

                var response = await httpClient.GetAsync(uri).ConfigureAwait(false);

                if (response.StatusCode != HttpStatusCode.OK)
                    throw new VstsException($"Invalid HTTP response status code: {(int)response.StatusCode} ({response.StatusCode}). Please make sure your User Name, Password or Personal Acceess Token are correct.");

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return new JavaScriptSerializer().Deserialize<T>(content);
            }
        }

        public static async Task Send<T>(AzureDevOpsConnection connection, string api, T request = default(T), bool vsrm = false, bool patch = false)
        {
            using (var handler = new WebRequestHandler())
            {
                var httpClient = new HttpClient(handler);

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrEmpty(connection.Password))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{connection.UserName}:{connection.Password}")));
                }

                var uri = CreateUri(connection, api, vsrm);

                var json = new JavaScriptSerializer().Serialize(request);

                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = patch
                    ? await PatchAsync(httpClient, uri, httpContent, CancellationToken.None).ConfigureAwait(false)
                    : await httpClient.PostAsync(uri, httpContent).ConfigureAwait(false);
                //#if DEBUG
                //                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                //#endif
                response.EnsureSuccessStatusCode();
            }
        }

        private static Task<HttpResponseMessage> PatchAsync(HttpClient client, string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            var method = new HttpMethod("PATCH");

            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = content
            };

            return client.SendAsync(request, cancellationToken);
        }

        private static string CreateUri(AzureDevOpsConnection connection, string api, bool vsrm)
        {
            var sb = new StringBuilder();
            sb.Append("https://");
            sb.Append(connection.Account);
            if (vsrm) sb.Append(".vsrm");
            sb.Append(".visualstudio.com/");
            sb.Append(connection.Project);
            sb.Append("/_apis/");
            sb.Append(api);

            return sb.ToString();
        }
    }
}
