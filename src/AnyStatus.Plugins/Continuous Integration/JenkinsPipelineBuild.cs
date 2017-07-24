using AnyStatus.API;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("Jenkins Pipeline Job")]
    [Description("Jenkins Pipeline CI job status")]
    [DisplayColumn("Continuous Integration")]
    public class JenkinsPipelineBuild : Build, IMonitored, ICanOpenInBrowser
    {
        private string url;

        [Url]
        [Required]
        [PropertyOrder(10)]
        [Category("Jenkins")]
        [Description("Jenkins pipeline job URL address")]
        public string Url
        {
            get { return this.url; }
            set { this.url = EnsureEndsWithSlash(value); }
        }

        [PropertyOrder(20)]
        [Category("Jenkins")]
        [DisplayName("User Name")]
        [Description("The Jenkins user name (optional)")]
        public string UserName { get; set; }

        [PropertyOrder(30)]
        [Category("Jenkins")]
        [DisplayName("API Token")]
        [Description("The Jenkins API token (optional). The API token is available in your personal configuration page. Click your name on the top right corner on every page, then click “Configure” to see your API token.")]
        public string ApiToken { get; set; }

        [DisplayName("Ignore SSL Errors")]
        public bool IgnoreSslErrors { get; set; }

        private static string EnsureEndsWithSlash(string str)
        {
            return string.IsNullOrEmpty(str) ?
                    str :
                    str.EndsWith("/") ? str : str + "/";
        }
    }

    public class OpenJenkinsPipelineBuildInBrowser : IOpenInBrowser<JenkinsPipelineBuild>
    {
        private readonly IProcessStarter processStarter;

        public OpenJenkinsPipelineBuildInBrowser(IProcessStarter processStarter)
        {
            this.processStarter = Preconditions.CheckNotNull(processStarter, nameof(processStarter));
        }

        public void Handle(JenkinsPipelineBuild item)
        {
            this.processStarter.Start(item.Url);
        }
    }
    
    public class JenkinsPipelineBuildMonitor : IMonitor<JenkinsPipelineBuild>
    {
        [DebuggerStepThrough]
        public void Handle(JenkinsPipelineBuild item)
        {
            var build = GetPipelineDetailsAsync(item).Result;
            item.State = State.Ok;

            // HACK: This adds items, but requires a restart before "schedulers" are created by AnyStatus.
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var job in build.Jobs.OrderBy(x => x.Name))
                {
                    this.AddJob(item, job);
                }
            });
        }
        
        private void AddJob(JenkinsPipelineBuild item, JenkinsPipelineJob job)
        {
            if (item.Parent != null && item.Parent.Items.OfType<JenkinsBuild>().All(x => x.Url != job.Url))
            {
                item.Parent.Add(new JenkinsBuild
                {
                    Name = job.Name.Replace("%2F", "/"),
                    Url = job.Url,
                    UserName = item.UserName,
                    ApiToken = item.ApiToken,
                    IgnoreSslErrors = item.IgnoreSslErrors
                });
            }
        }

        private async Task<JenkinsPipelineDetails> GetPipelineDetailsAsync(JenkinsPipelineBuild item)
        {
            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = true;

                if (item.IgnoreSslErrors)
                {
                    handler.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                }

                using (var client = new HttpClient(handler))
                {
                    ConfigureHttpClientAuthorization(item, client);

                    var baseUri = new Uri(item.Url);

                    var apiUrl = new Uri(baseUri, "api/json");

                    var response = await client.GetAsync(apiUrl);

                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();

                    var jobsResult = new JavaScriptSerializer().Deserialize<JenkinsPipelineDetails>(content);
                    if (jobsResult == null)
                    {
                        throw new Exception("Invalid Jenkins Pipeline response.");
                    }

                    return jobsResult;
                }
            }
        }

        private static void ConfigureHttpClientAuthorization(JenkinsPipelineBuild item, HttpClient client)
        {
            if (string.IsNullOrEmpty(item.UserName) || string.IsNullOrEmpty(item.ApiToken))
            {
                return;
            }

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(
                        Encoding.ASCII.GetBytes(string.Format("{0}:{1}", item.UserName, item.ApiToken))));
        }
    }

    #region Contracts

    public class JenkinsPipelineDetails
    {
        public JenkinsPipelineJob[] Jobs { get; set; }
    }

    public class JenkinsPipelineJob
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    #endregion
}