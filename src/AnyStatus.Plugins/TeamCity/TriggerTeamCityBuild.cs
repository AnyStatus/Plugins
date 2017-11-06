using AnyStatus.API;
using AnyStatus.API.Utils;
using System.Threading.Tasks;
using System.Windows;

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

        public async Task HandleAsync(TeamCityBuild teamCityBuild)
        {
            var result = _dialogService.Show($"Are you sure you want to trigger {teamCityBuild.Name}?", "Trigger a new build", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            if (result != MessageBoxResult.Yes) return;

            _logger.Info($"Triggering \"{teamCityBuild.Name}\"...");

            var teamCityClient = new TeamCityClient(new TeamCityConnection());

            teamCityBuild.MapTo(teamCityClient.Connection);

            await teamCityClient.QueueNewBuild(teamCityBuild).ConfigureAwait(false);

            _logger.Info($"Build \"{teamCityBuild.Name}\" was triggered.");
        }
    }
}
