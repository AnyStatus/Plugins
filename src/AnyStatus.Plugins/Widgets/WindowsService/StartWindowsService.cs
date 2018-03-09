using AnyStatus.API;
using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class StartWindowsService : WindowsServiceHandler, IStart<WindowsService>
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public StartWindowsService(IDialogService dialogService, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        public async Task Handle(StartRequest<WindowsService> request, CancellationToken cancellationToken)
        {
            var dialog = new ConfirmationDialog($"Are you sure you want to start {request.DataContext.Name}?", "AnyStatus");

            var result = _dialogService.ShowDialog(dialog);

            if (result != DialogResult.Yes)
                return;

            _logger.Info($"Starting {request.DataContext.Name}.");

            await StartAsync(request.DataContext, cancellationToken).ConfigureAwait(false);
        }

        private async Task StartAsync(WindowsService windowsService, CancellationToken cancellationToken)
        {
            using (var controller = GetServiceController(windowsService))
            {
                try
                {
                    if (controller.Status != ServiceControllerStatus.Running &&
                        controller.Status != ServiceControllerStatus.StartPending)
                    {
                        controller.Start();

                        await WaitForStatusAsync(controller, ServiceControllerStatus.Running, WaitForStatusTimeout, cancellationToken).ConfigureAwait(false);
                    }

                    SetState(windowsService, controller.Status);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"An error occurred while starting {windowsService.Name}.");
                }
                finally
                {
                    controller.Close();
                }
            }
        }
    }
}
