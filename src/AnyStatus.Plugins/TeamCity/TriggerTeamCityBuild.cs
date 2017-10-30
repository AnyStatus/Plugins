using AnyStatus.API;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace AnyStatus
{
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

                    var response = await client.PostAsync(url, content).ConfigureAwait(false);

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
