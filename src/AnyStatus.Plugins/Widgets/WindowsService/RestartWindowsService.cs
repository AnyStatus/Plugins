using AnyStatus.API;
using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class RestartWindowsService : WindowsServiceHandler, IRestart<WindowsService>
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public RestartWindowsService(IDialogService dialogService, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        public async Task Handle(RestartRequest<WindowsService> request, CancellationToken cancellationToken)
        {
            var dialog = new ConfirmationDialog($"Are you sure you want to restart {request.DataContext.Name}?", "Restart Windows Service");

            var result = _dialogService.ShowDialog(dialog);

            if (result != DialogResult.Yes)
                return;

            _logger.Info($"Restarting {request.DataContext.Name}.");

            await RestartAsync(request.DataContext, cancellationToken).ConfigureAwait(false);
        }

        private async Task RestartAsync(WindowsService windowsService, CancellationToken cancellationToken)
        {
            using (var controller = GetServiceController(windowsService))
            {
                try
                {
                    if (controller.Status == ServiceControllerStatus.Running || controller.Status == ServiceControllerStatus.StartPending)
                    {
                        controller.Stop();

                        await WaitForStatusAsync(controller, ServiceControllerStatus.Stopped, WaitForStatusTimeout, cancellationToken)
                            .ConfigureAwait(false);
                    }

                    SetState(windowsService, controller.Status);

                    controller.Start();

                    await WaitForStatusAsync(controller, ServiceControllerStatus.Running, WaitForStatusTimeout, cancellationToken)
                        .ConfigureAwait(false);

                    SetState(windowsService, controller.Status);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"An error occurred while restarting {windowsService.Name}.");
                }
                finally
                {
                    controller.Close();
                }
            }
        }
    }
}