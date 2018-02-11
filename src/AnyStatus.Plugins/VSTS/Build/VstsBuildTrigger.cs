using AnyStatus.API;
using AnyStatus.API.Utils;
using System.Threading.Tasks;
using System.Windows;

namespace AnyStatus.Plugins.VSTS.Build
{
    public class VstsBuildTrigger : ITriggerBuild<VSTSBuild_v1>
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public VstsBuildTrigger(IDialogService dialogService, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        public async Task HandleAsync(VSTSBuild_v1 build)
        {
            var dialog = new ConfirmationDialog($"Are you sure you want to trigger {build.Name}?", "Trigger a new build");

            var result = _dialogService.ShowDialog(dialog);

            if (result != DialogResult.Yes) return;

            _logger.Info($"Triggering \"{build.Name}\"...");

            var client = new VstsClient(new VstsConnection());

            build.MapTo(client.Connection);

            if (build.DefinitionId == null)
            {
                var definition = await client.GetBuildDefinitionAsync(build.DefinitionName).ConfigureAwait(false);

                build.DefinitionId = definition.Id;
            }

            await client.QueueNewBuildAsync(build.DefinitionId.Value).ConfigureAwait(false);

            _logger.Info($"Build \"{build.Name}\" has been triggered.");
        }
    }
}
