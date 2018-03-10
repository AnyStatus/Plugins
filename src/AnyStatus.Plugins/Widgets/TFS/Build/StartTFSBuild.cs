using AnyStatus.API;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AnyStatus
{
    public class StartTFSBuild : TFSBuildHandler, IStart<TfsBuild>
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public StartTFSBuild(IDialogService dialogService, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        public async Task Handle(StartRequest<TfsBuild> request, CancellationToken cancellationToken)
        {
            var dialog = new ConfirmationDialog($"Are you sure you want to start {request.DataContext.Name}?", "Trigger a new request.DataContext");

            var result = _dialogService.ShowDialog(dialog);

            if (result != DialogResult.Yes)
                return;

            _logger.Info($"Starting \"{request.DataContext.Name}\"...");

            await base.HandleAsync(request.DataContext).ConfigureAwait(false);

            await QueueNewBuildAsync(request.DataContext).ConfigureAwait(false);

            _logger.Info($"request.DataContext \"{request.DataContext.Name}\" has been triggered.");
        }

        private async Task QueueNewBuildAsync(TfsBuild item)
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