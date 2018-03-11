using AnyStatus.API;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class StartJenkinsJob : IStart<JenkinsJob_v1>
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;
        private readonly IJenkinsClient _jenkinsClient;

        public StartJenkinsJob(IDialogService dialogService, ILogger logger, IJenkinsClient jenkinsClient)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
            _jenkinsClient = Preconditions.CheckNotNull(jenkinsClient, nameof(jenkinsClient));
        }

        public async Task Handle(StartRequest<JenkinsJob_v1> request, CancellationToken cancellationToken)
        {
            var dialog = new ConfirmationDialog($"Are you sure you want to trigger {request.DataContext.Name}?");

            var result = _dialogService.ShowDialog(dialog);

            if (result != DialogResult.Yes)
                return;

            await _jenkinsClient.TriggerJobAsync(request.DataContext).ConfigureAwait(false);

            _logger.Info($"Jenkins Job \"{request.DataContext.Name}\" has been triggered.");
        }
    }
}