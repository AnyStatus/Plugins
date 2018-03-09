using AnyStatus.API;
using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class StopWindowsService : WindowsServiceHandler, IStop<WindowsService>
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public StopWindowsService(IDialogService dialogService, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        public async Task Handle(StopRequest<WindowsService> request, CancellationToken cancellationToken)
        {
            var dialog = new ConfirmationDialog($"Are you sure you want to stop {request.DataContext.Name}?", "Stop Windows Service");

            var result = _dialogService.ShowDialog(dialog);

            if (result != DialogResult.Yes)
                return;

            _logger.Info($"Stopping {request.DataContext.Name}.");

            await StopAsync(request.DataContext, cancellationToken).ConfigureAwait(false);
        }

        private async Task StopAsync(WindowsService windowsService, CancellationToken cancellationToken)
        {
            using (var sc = GetServiceController(windowsService))
            {
                try
                {
                    if (sc.Status == ServiceControllerStatus.Running || sc.Status == ServiceControllerStatus.StartPending)
                    {
                        sc.Stop();

                        await WaitForStatusAsync(sc, ServiceControllerStatus.Stopped, WaitForStatusTimeout, cancellationToken)
                            .ConfigureAwait(false);
                    }

                    SetState(windowsService, sc.Status);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"An error occurred while stopping {windowsService.Name}.");
                }
                finally
                {
                    sc.Close();
                }
            }
        }
    }
}