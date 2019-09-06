using AnyStatus.API;
using AnyStatus.API.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class StartTeamCityBuild : IStart<TeamCityBuild>
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public StartTeamCityBuild(IDialogService dialogService, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        public async Task Handle(StartRequest<TeamCityBuild> request, CancellationToken cancellationToken)
        {
            var dialog = new ConfirmationDialog($"Are you sure you want to start {request.DataContext.Name}?");

            var result = _dialogService.ShowDialog(dialog);

            if (result != DialogResult.Yes)
                return;

            _logger.Info($"Starting \"{request.DataContext.Name}\"...");

            var client = new TeamCityClient(new TeamCityConnection());

            request.DataContext.MapTo(client.Connection);

            await client.QueueNewBuild(request.DataContext).ConfigureAwait(false);

            request.DataContext.State = State.Queued;

            _logger.Info($"Build \"{request.DataContext.Name}\" has been queued.");
        }
    }
}