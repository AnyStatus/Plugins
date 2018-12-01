using AnyStatus.API;
using AnyStatus.API.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Widgets.DevOps.VSTS.Release
{
    public class CreateVstsRelease : IStart<VSTSRelease_v1>
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public CreateVstsRelease(IDialogService dialogService, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        public async Task Handle(StartRequest<VSTSRelease_v1> request, CancellationToken cancellationToken)
        {
            var dialog = new ConfirmationDialog($"Are you sure you want to create a new release of {request.DataContext.Name}?");

            var result = _dialogService.ShowDialog(dialog);

            if (result != DialogResult.Yes) return;

            if (!request.DataContext.ReleaseId.HasValue)
                throw new InvalidOperationException("Release id was not set.");

            _logger.Info($"Creating a new release of \"{request.DataContext.Name}\"...");

            var client = new AnyStatus.VSTS();

            request.DataContext.MapTo(client);

            var body = new
            {
                definitionId = request.DataContext.ReleaseId
            };

            await client.Send("release/releases?api-version=4.1-preview.6", body, true).ConfigureAwait(false);

            request.DataContext.State = State.Queued;

            _logger.Info($"Release \"{request.DataContext.Name}\" has been queued.");
        }
    }
}
