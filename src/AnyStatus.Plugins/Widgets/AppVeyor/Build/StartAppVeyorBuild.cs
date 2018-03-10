using AnyStatus.API;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AnyStatus
{
    public class StartAppVeyorBuild : IStart<AppVeyorBuild>
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public StartAppVeyorBuild(IDialogService dialogService, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        public async Task Handle(StartRequest<AppVeyorBuild> request, CancellationToken cancellationToken)
        {
            var dialog = new ConfirmationDialog($"Are you sure you want to trigger {request.DataContext.Name}?", "Trigger a new build");

            var result = _dialogService.ShowDialog(dialog);

            if (result != DialogResult.Yes) return;

            await QueueNewBuild(request.DataContext).ConfigureAwait(false);

            _logger.Info($"Build \"{request.DataContext.Name}\" has been triggered.");
        }

        private async Task QueueNewBuild(AppVeyorBuild item)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", item.ApiToken);

                var request = default(object);

                if (string.IsNullOrWhiteSpace(item.SourceControlBranch))
                {
                    request = new
                    {
                        accountName = item.AccountName,
                        projectSlug = item.ProjectSlug
                    };
                }
                else
                {
                    request = new
                    {
                        accountName = item.AccountName,
                        projectSlug = item.ProjectSlug,
                        branch = item.SourceControlBranch
                    };
                }

                var data = new JavaScriptSerializer().Serialize(request);

                var content = new StringContent(data, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://ci.appveyor.com/api/builds", content).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();
            }
        }
    }
}