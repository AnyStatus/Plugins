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
            var result = _dialogService.Show($"Are you sure you want to trigger {build.Name}?", "Trigger a new build", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            if (result != MessageBoxResult.Yes) return;

            _logger.Info($"Triggering \"{build.Name}\"...");

            var client = new VstsClient(new VstsConnection());

            build.MapTo(client.Connection);

            if (build.DefinitionId == null)
            {
                var definition = await client.GetBuildDefinitionAsync(build.DefinitionName).ConfigureAwait(false);

                build.DefinitionId = definition.Id;
            }

            await client.QueueNewBuild(build.DefinitionId.Value).ConfigureAwait(false);

            _logger.Info($"Build \"{build.Name}\" was triggered.");
        }
    }
}
