using System;
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

            var client = new AnyStatus.VSTS();

            if (request.DataContext.Parent is VSTSRelease_v1 release)
            {
                release.MapTo(client);
            }
            else
            {
                throw new InvalidOperationException("Release environment is not associated with a release.");
            }

            if (!release.ReleaseId.HasValue)
                throw new InvalidOperationException("Release id was not set.");

            var lastRelease = await client
                .GetLastReleaseAsync(release.ReleaseId.Value)
                .ConfigureAwait(false);

            if (lastRelease == null)
            {
                request.DataContext.State = State.Failed;

                _logger.Error("VSTS release definition was not released.");

                return;
            }

            var body = new
            {
                status = "inProgress"
            };

            var url = $"release/releases/{lastRelease.Id}/environments/{request.DataContext.EnvironmentId}?api-version=4.1-preview.5";

            await client.Send(url, body, true, true).ConfigureAwait(false);

            request.DataContext.State = State.Queued;

            _logger.Info($"Deployment to \"{request.DataContext.Name}\" has been queued.");
        }
    }
}
