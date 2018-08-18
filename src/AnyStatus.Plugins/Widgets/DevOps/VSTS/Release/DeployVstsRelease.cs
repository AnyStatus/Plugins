using AnyStatus.API;
using System.Threading;
using System.Threading.Tasks;
using AnyStatus.API.Utils;

namespace AnyStatus.Plugins.Widgets.DevOps.VSTS.Release
{
    public class DeployVstsRelease : IStart<VSTSReleaseEnvironment>
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public DeployVstsRelease(IDialogService dialogService, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        public async Task Handle(StartRequest<VSTSReleaseEnvironment> request, CancellationToken cancellationToken)
        {
            var dialog = new ConfirmationDialog($"Are you sure you want to deploy to {request.DataContext.Name}?");

            var result = _dialogService.ShowDialog(dialog);

            if (result != DialogResult.Yes) return;

            _logger.Info($"Starting deployment to \"{request.DataContext.Name}\"...");

            var client = new VstsClient(new VstsConnection());

            request.DataContext.Parent.MapTo(client.Connection);

            await client.DeployReleaseAsync(request.DataContext.ReleaseId, request.DataContext.EnvironmentId).ConfigureAwait(false);

            _logger.Info($"Deployment to \"{request.DataContext.Name}\" has been started.");
        }
    }
}
