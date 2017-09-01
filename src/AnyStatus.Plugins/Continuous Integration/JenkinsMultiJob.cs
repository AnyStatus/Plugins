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
    [DisplayName("Jenkins MultiJob (Preview)")]
    [Description("Shows the status of multiple Jenkins jobs.")]
    [DisplayColumn("Continuous Integration")]
    public class JenkinsMultiJob : Folder, IMonitored, ICanOpenInBrowser
    {
        private string url;

        [Url]
        [Required]
        [PropertyOrder(10)]
        [Category("Jenkins")]
        [Description("Jenkins multibranch job URL address")]
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

    public class OpenJenkinsMultibranchBuildInBrowser : IOpenInBrowser<JenkinsMultiJob>
    {
        private readonly IProcessStarter _processStarter;

        public OpenJenkinsMultibranchBuildInBrowser(IProcessStarter processStarter)
        {
            _processStarter = Preconditions.CheckNotNull(processStarter, nameof(processStarter));
        }

        public void Handle(JenkinsMultiJob item)
        {
            _processStarter.Start(item.Url);
        }
    }

    public class JenkinsMultibranchBuildMonitor : IMonitor<JenkinsMultiJob>
    {
        [DebuggerStepThrough]
        public void Handle(JenkinsMultiJob item)
        {
            if (item.Parent == null)
            {
                return;
            }

            var build = GetMultibranchDetailsAsync(item).Result;

            item.State = State.Ok;

            // HACK: This adds items, but requires a restart before "schedulers" are created by AnyStatus.
            var prevJobs = item.Parent.Items.OfType<JenkinsBuild>();

            Application.Current.Dispatcher.Invoke(() =>
            {
                // Add new jobs
                var newJobs = build.Jobs.Where(x => prevJobs.All(y => y.Url != x.Url)).OrderBy(x => x.Name);

                foreach (var job in newJobs)
                {
                    this.AddJob(item, job);
                }

                // Update existing jobs
                foreach (var job in build.Jobs.Except(newJobs))
                {
                    this.UpdateJob(prevJobs.Single(x => x.Url == job.Url), job);
                }

                // Remove any jobs that no longer exist
                foreach (var job in prevJobs.Where(x => build.Jobs.All(y => y.Url != x.Url)))
                {
                    this.RemoveJob(item, job);
                }
            });
        }

        private void AddJob(JenkinsMultiJob item, JenkinsJob job)
        {
            item.Add(new JenkinsBuild
            {
                Name = job.Name.Replace("%2F", "/"),
                Url = job.Url,
                Interval = item.Interval,
                UserName = item.UserName,
                ApiToken = item.ApiToken,
                IgnoreSslErrors = item.IgnoreSslErrors
            });
        }

        private void UpdateJob(JenkinsBuild item, JenkinsJob job)
        {
            // TODO: not sure what to update... => Update Item.State
        }

        private void RemoveJob(JenkinsMultiJob item, JenkinsBuild job)
        {
            item.Remove(job);
        }

        private async Task<JenkinsMultibranchDetails> GetMultibranchDetailsAsync(JenkinsMultiJob item)
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

                    var jobsResult = new JavaScriptSerializer().Deserialize<JenkinsMultibranchDetails>(content);

                    if (jobsResult == null)
                    {
                        throw new Exception("Invalid Jenkins Multibranch response.");
                    }

                    return jobsResult;
                }
            }
        }

        private static void ConfigureHttpClientAuthorization(JenkinsMultiJob item, HttpClient client)
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

    public class JenkinsMultibranchDetails
    {
        public JenkinsJob[] Jobs { get; set; }
    }

    public class JenkinsJob
    {
        public string Name { get; set; }

        public string Url { get; set; }
    }

    #endregion
}