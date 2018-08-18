using AnyStatus.API;
using AnyStatus.API.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.VSTS.Build
{
    public class VstsBuildTrigger : IStart<VSTSBuild_v1>
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public VstsBuildTrigger(IDialogService dialogService, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        public async Task Handle(StartRequest<VSTSBuild_v1> request, CancellationToken cancellationToken)
        {
            var dialog = new ConfirmationDialog($"Are you sure you want to start {request.DataContext.Name}?");

            var result = _dialogService.ShowDialog(dialog);

            if (result != DialogResult.Yes) return;

            _logger.Info($"Starting \"{request.DataContext.Name}\"...");

            var client = new VstsClient();

            request.DataContext.MapTo(client);

            if (request.DataContext.DefinitionId == null)
            {
                var definition = await client.GetBuildDefinitionAsync(request.DataContext.DefinitionName).ConfigureAwait(false);

                request.DataContext.DefinitionId = definition.Id;
            }

            var body = new
            {
                Definition = new
                {
                    Id = request.DataContext.DefinitionId
                }
            };

            await client.Send("build/builds?api-version=2.0", body).ConfigureAwait(false);

            request.DataContext.State = State.Queued;

            _logger.Info($"Build \"{request.DataContext.Name}\" has been triggered.");
        }
    }
}