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
    [CategoryOrder("TeamCity", 10)]
    [DisplayName("TeamCity Build")]
    [Description("TeamCity build status")]
    [DisplayColumn("Continuous Integration")]
    public class TeamCityBuild : Build, IAmMonitored, ICanOpenInBrowser, ICanTriggerBuild
    {
        [Url]
        [Required]
        [PropertyOrder(10)]
        [DisplayName("Url")]
        [Category("TeamCity")]
        [Description("TeamCity server URL address. For example: http://teamcity:8080")]
        public string Url { get; set; }

        [Browsable(false)] //TODO: Remove property. Use Url instead.
        public string Host { get { return Url; } set { Url = value; } }

        [Required]
        [PropertyOrder(20)]
        [Category("TeamCity")]
        [DisplayName("Build Type Id")]
        [Description("TeamCity build type id (Copy from TeamCity build URL address)")]
        public string BuildTypeId { get; set; }

        [PropertyOrder(30)]
        [Category("TeamCity")]
        [DisplayName("Use Guest User")]
        [Description("Log in with TeamCity guest user. If checked, the User Name and Password are ignored")]
        public bool GuestUser { get; set; }

        [PropertyOrder(40)]
        [Category("TeamCity")]
        [DisplayName("User Name")]
        [Description("Optional.")]
        public string UserName { get; set; }

        [PropertyOrder(50)]
        [Category("TeamCity")]
        [PasswordPropertyText(true)]
        [Description("Optional.")]
        [Editor(typeof(PasswordEditor), typeof(PasswordEditor))]
        public string Password { get; set; }

        [PropertyOrder(60)]
        [DisplayName("Ignore SSL Errors")]
        public bool IgnoreSslErrors { get; set; }
    }

    public class TeamCityBuildMonitor : IMonitor<TeamCityBuild>
    {
        [DebuggerStepThrough]
        public void Handle(TeamCityBuild item)
        {
            var build = GetBuildDetailsAsync(item).Result;

            if (build.Running)
            {
                item.State = State.Running;
                return;
            }

            //if (build.CancelledInfo)
            //{
            //    item.Brush = Brushes.Gray;
            //    item.State = State.Canceled;
            //    return;
            //}

            switch (build.Status)
            {
                case "SUCCESS":
                    item.State = State.Ok;
                    break;
                case "FAILURE":
                case "ERROR":
                    item.State = State.Failed;
                    break;
                case "UNKNOWN":
                default:
                    item.State = State.Unknown;
                    break;
            }
        }

        private async Task<TeamCityBuildDetails> GetBuildDetailsAsync(TeamCityBuild item)
        {
            if (item.Url.EndsWith("/"))
            {
                //todo: move this to when monitor is created
                item.Url = item.Url.Remove(item.Url.Length - 1);
            }

            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = true;

                if (item.IgnoreSslErrors)
                {
                    handler.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                }

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string authType = string.Empty;

                    if (item.GuestUser)
                    {
                        authType = "guestAuth";
                    }
                    else
                    {
                        authType = "httpAuth";

                        if (!string.IsNullOrEmpty(item.UserName) && !string.IsNullOrEmpty(item.Password))
                        {
                            client.DefaultRequestHeaders.Authorization =
                                new AuthenticationHeaderValue("Basic",
                                    Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", item.UserName, item.Password))));
                        }
                    }

                    var apiUrl = $"{item.Url}/{authType}/app/rest/builds?locator=running:any,buildType:(id:{item.BuildTypeId}),count:1&fields=build(status,running)";

                    var response = await client.GetAsync(apiUrl);

                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();

                    var buildResponse = new JavaScriptSerializer().Deserialize<TeamCityBuildDetailsResponse>(content);

                    return buildResponse.Build.First();
                }
            }
        }

        private static void RemoveLastChar(TeamCityBuild item)
        {
            item.Url = item.Url.Remove(item.Url.Length - 1);
        }

        #region Contracts

        private class TeamCityBuildDetailsResponse
        {
            public List<TeamCityBuildDetails> Build { get; set; }
        }

        private class TeamCityBuildDetails
        {
            public bool Running { get; set; }

            public string Status { get; set; }
        }

        #endregion
    }

    public class OpenTeamCityBuildInBrowser : IOpenInBrowser<TeamCityBuild>
    {
        private readonly IProcessStarter _processStarter;

        public OpenTeamCityBuildInBrowser(IProcessStarter processStarter)
        {
            _processStarter = Preconditions.CheckNotNull(processStarter, nameof(processStarter));
        }

        public void Handle(TeamCityBuild item)
        {
            if (item == null || string.IsNullOrEmpty(item.Url) || string.IsNullOrEmpty(item.BuildTypeId))
                return;

            var url = $"{item.Url}/viewType.html?buildTypeId={item.BuildTypeId}";

            _processStarter.Start(url);
        }
    }

    public class TriggerTeamCityBuild : ITriggerBuild<TeamCityBuild>
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public TriggerTeamCityBuild(IDialogService dialogService, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        public async Task HandleAsync(TeamCityBuild build)
        {
            var result = _dialogService.Show($"Are you sure you want to trigger {build.Name}?", "Trigger a new build", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            if (result != MessageBoxResult.Yes)
                return;

            await QueueNewBuild(build);

            _logger.Info($"Build \"{build.Name}\" was triggered.");
        }

        private async Task QueueNewBuild(TeamCityBuild item)
        {
            if (item.Url.EndsWith("/"))
            {
                item.Url = item.Url.Remove(item.Url.Length - 1);
            }

            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = true;

                if (item.IgnoreSslErrors)
                {
                    handler.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                }

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                    string authType = string.Empty;

                    if (item.GuestUser)
                    {
                        authType = "guestAuth";
                    }
                    else
                    {
                        authType = "httpAuth";

                        if (!string.IsNullOrEmpty(item.UserName) && !string.IsNullOrEmpty(item.Password))
                        {
                            client.DefaultRequestHeaders.Authorization =
                                new AuthenticationHeaderValue("Basic",
                                    Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", item.UserName, item.Password))));
                        }
                    }

                    var url = $"{item.Url}/{authType}/app/rest/buildQueue";

                    var request = $"<build><buildType id=\"{item.BuildTypeId}\"/></build>";

                    var content = new StringContent(request, Encoding.UTF8, "application/xml");

                    var response = await client.PostAsync(url, content);

                    response.EnsureSuccessStatusCode();
                }
            }
        }

        #region Contracts

        public class Build
        {
            public BuildType BuildType { get; set; }
        }

        public class BuildType
        {
            [XmlAttribute]
            public string Id { get; set; }
        }

        #endregion
    }
}
