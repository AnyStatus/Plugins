using AnyStatus.API;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;

namespace AnyStatus
{
    public class TriggerTfsBuild : BaseTfsBuildHandler, ITriggerBuild<TfsBuild>
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public TriggerTfsBuild(IDialogService dialogService, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        [DebuggerStepThrough]
        public override async Task HandleAsync(TfsBuild build)
        {
            var result = _dialogService.Show($"Are you sure you want to trigger {build.Name}?", "Trigger a new build", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            if (result != MessageBoxResult.Yes)
                return;

            await base.HandleAsync(build).ConfigureAwait(false);

            await QueueNewBuild(build).ConfigureAwait(false);

            _logger.Info($"Build \"{build.Name}\" was triggered.");
        }

        private async Task QueueNewBuild(TfsBuild item)
        {
            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = string.IsNullOrEmpty(item.UserName) || string.IsNullOrEmpty(item.Password); ;

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (handler.UseDefaultCredentials == false)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{item.UserName}:{item.Password}")));
                    }

                    var url = $"{item.Url}/{item.Collection}/{item.TeamProject}/_apis/build/builds?api-version=2.0";

                    var request = new QueueNewBuildRequest
                    {
                        Definition = new Definition
                        {
                            Id = item.BuildDefinitionId
                        }
                    };

                    var json = new JavaScriptSerializer().Serialize(request);

                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(url, content).ConfigureAwait(false);

                    response.EnsureSuccessStatusCode();
                }
            }
        }
    }
}
