using AnyStatus.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("Jenkins Job")]
    [Description("Jenkins CI job status")]
    [DisplayColumn("Continuous Integration")]
    public class JenkinsBuild : Build, ISchedulable, ICanOpenInBrowser, ICanTriggerBuild, IReportProgress
    {
        private string _url;
        private int _progress;
        private bool _showProgress;

        public JenkinsBuild()
        {
            if (BuildParameters == null)
                BuildParameters = new List<NameValuePair>();
        }

        [Url]
        [Required]
        [PropertyOrder(10)]
        [Category("Jenkins")]
        [Description("Jenkins job URL address")]
        public string Url
        {
            get { return _url; }
            set { _url = EnsureEndsWithSlash(value); }
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

        [PropertyOrder(40)]
        [Category("Jenkins")]
        [Editor(typeof(DataGridEditor), typeof(DataGridEditor))]
        [DisplayName("Parameters")]
        [Description("Optional. Specify the build parameters to use when triggering a new build. Parameters are case-sensitive.")]
        public List<NameValuePair> BuildParameters { get; set; }

        [DisplayName("Ignore SSL Errors")]
        public bool IgnoreSslErrors { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public int Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public bool ShowProgress
        {
            get
            {
                return _showProgress;
            }
            set
            {
                _showProgress = value;
                OnPropertyChanged();
            }
        }

        public void ResetProgress()
        {
            ShowProgress = false;
            Progress = 0;
        }

        private static string EnsureEndsWithSlash(string str)
        {
            return string.IsNullOrEmpty(str) ?
                    str :
                    str.EndsWith("/") ? str : str + "/";
        }
    }

    public class OpenJenkinsBuildInBrowser : IOpenInBrowser<JenkinsBuild>
    {
        private readonly IProcessStarter _processStarter;

        public OpenJenkinsBuildInBrowser(IProcessStarter processStarter)
        {
            _processStarter = Preconditions.CheckNotNull(processStarter, nameof(processStarter));
        }

        public void Handle(JenkinsBuild item)
        {
            _processStarter.Start(item.Url);
        }
    }

    public class TriggerJenkinsBuild : ITriggerBuild<JenkinsBuild>
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public TriggerJenkinsBuild(IDialogService dialogService, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        public async Task HandleAsync(JenkinsBuild build)
        {
            var result = _dialogService.Show($"Are you sure you want to trigger {build.Name}?", "Trigger a new build", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            if (result != MessageBoxResult.Yes)
                return;

            await TriggerBuild(build);

            _logger.Info($"Build \"{build.Name}\" was triggered.");
        }

        private async Task TriggerBuild(JenkinsBuild build)
        {
            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = true;

                if (build.IgnoreSslErrors)
                {
                    handler.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                }

                using (var client = new HttpClient(handler))
                {
                    ConfigureHttpClientAuthorization(build, client);

                    var apiUri = GetApiUri(build);

                    var response = await client.PostAsync(apiUri, new StringContent(string.Empty));

                    response.EnsureSuccessStatusCode();
                }
            }
        }

        private Uri GetApiUri(JenkinsBuild build)
        {
            var relativeUri = new StringBuilder();

            relativeUri.Append("buildWithParameters?delay=0sec");

            if (build.BuildParameters != null && build.BuildParameters.Any())
            {
                foreach (var parameter in build.BuildParameters)
                {
                    if (parameter == null || 
                        string.IsNullOrWhiteSpace(parameter.Name) || 
                        string.IsNullOrWhiteSpace(parameter.Value))
                        continue;

                    relativeUri.Append("&");
                    relativeUri.Append(WebUtility.UrlEncode(parameter.Name));
                    relativeUri.Append("=");
                    relativeUri.Append(WebUtility.UrlEncode(parameter.Value));
                }
            }

            var baseUri = new Uri(build.Url);

            return new Uri(baseUri, relativeUri.ToString());
        }

        private static void ConfigureHttpClientAuthorization(JenkinsBuild item, HttpClient client)
        {
            if (string.IsNullOrEmpty(item.UserName) || string.IsNullOrEmpty(item.ApiToken))
                return;

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(
                        Encoding.ASCII.GetBytes(string.Format("{0}:{1}", item.UserName, item.ApiToken))));
        }
    }

    public class JenkinsBuildMonitor : IMonitor<JenkinsBuild>
    {
        [DebuggerStepThrough]
        public void Handle(JenkinsBuild item)
        {
            var build = GetBuildDetailsAsync(item).Result;

            if (build.IsRunning)
            {
                OnBuildRunning(item, build.Executor.Progress);
                return;
            }

            if (item.ShowProgress)
                item.ResetProgress();

            item.State = ConvertBuildResultToState(build.Result);
        }

        private static State ConvertBuildResultToState(string result)
        {
            switch (result)
            {
                case "SUCCESS":
                    return State.Ok;
                case "ABORTED":
                    return State.Canceled;
                case "FAILURE":
                    return State.Failed;
                case "UNSTABLE":
                    return State.PartiallySucceeded;
                //todo: complete this:
                //case "QUEUED":
                //    return State.Queued;
                default:
                    return State.Unknown;
            }
        }

        private static void OnBuildRunning(JenkinsBuild item, int progress)
        {
            item.State = State.Running;
            item.Progress = progress;

            if (item.ShowProgress == false)
                item.ShowProgress = true;
        }

        private async Task<JenkinsBuildDetails> GetBuildDetailsAsync(JenkinsBuild item)
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

                    var apiUrl = new Uri(baseUri, "lastBuild/api/json?tree=result,building,executor[progress]");

                    var response = await client.GetAsync(apiUrl);

                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();

                    var buildDetails = new JavaScriptSerializer().Deserialize<JenkinsBuildDetails>(content);

                    if (buildDetails == null)
                        throw new Exception("Invalid Jenkins Build response.");

                    return buildDetails;
                }
            }
        }

        private static void ConfigureHttpClientAuthorization(JenkinsBuild item, HttpClient client)
        {
            if (string.IsNullOrEmpty(item.UserName) || string.IsNullOrEmpty(item.ApiToken))
                return;

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(
                        Encoding.ASCII.GetBytes(string.Format("{0}:{1}", item.UserName, item.ApiToken))));
        }
    }

    #region Contracts

    public class JenkinsBuildDetails
    {
        public bool Building { get; set; }

        public string Result { get; set; }

        public ProgressExecutor Executor { get; set; }

        public bool IsRunning { get { return Building; } }

        public class ProgressExecutor
        {
            public int Progress { get; set; }
        }
    }

    #endregion
}