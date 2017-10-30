using AnyStatus.API;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;

namespace AnyStatus
{
    public class TriggerAppVeyorBuild : ITriggerBuild<AppVeyorBuild>
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public TriggerAppVeyorBuild(IDialogService dialogService, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        public async Task HandleAsync(AppVeyorBuild build)
        {
            var result = _dialogService.Show($"Are you sure you want to trigger {build.Name}?", "Trigger a new build", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            if (result != MessageBoxResult.Yes)
                return;

            await QueueNewBuild(build).ConfigureAwait(false);

            _logger.Info($"Build \"{build.Name}\" was triggered.");
        }

        private async Task QueueNewBuild(AppVeyorBuild item)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", item.ApiToken);

                var request = new
                {
                    accountName = item.AccountName,
                    projectSlug = item.ProjectSlug
                };

                var data = new JavaScriptSerializer().Serialize(request);

                var content = new StringContent(data, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://ci.appveyor.com/api/builds", content).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();
            }
        }
    }
}
