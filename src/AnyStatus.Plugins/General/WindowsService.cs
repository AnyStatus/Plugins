using AnyStatus.API;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace AnyStatus
{
    [DisplayName("Windows Service")]
    [Description("Test whether a windows service is running")]
    public class WindowsService : Widget, IMonitored, ICanStart, ICanStop, ICanRestart
    {
        private const string Category = "Windows Service";

        public WindowsService()
        {
            Status = ServiceControllerStatus.Running;
        }

        [Required]
        [Category(Category)]
        [DisplayName("Service Name")]
        public string ServiceName { get; set; }

        [DisplayName("Machine Name")]
        [Category(Category)]
        [Description("Optional. Leave blank for local computer.")]
        public string MachineName { get; set; }

        [Category(Category)]
        public ServiceControllerStatus Status { get; set; }

        //todo: add Started & Stopped states and add logic to following methods:

        public bool CanRestart()
        {
            return State != State.None && State != State.Error;
        }

        public bool CanStart()
        {
            return State != State.None && State != State.Error;
        }

        public bool CanStop()
        {
            return State != State.None && State != State.Error;
        }
    }

    public abstract class BaseWindowsServiceHandler
    {
        protected static TimeSpan Timeout = TimeSpan.FromMinutes(1);

        protected ServiceController GetServiceController(WindowsService windowsService)
        {
            return string.IsNullOrEmpty(windowsService.MachineName) ?
                    new ServiceController(windowsService.ServiceName) :
                    new ServiceController(windowsService.ServiceName, windowsService.MachineName);
        }

        [DebuggerStepThrough]
        protected static void SetState(WindowsService windowsService, ServiceController serviceController)
        {
            windowsService.State = serviceController.Status == windowsService.Status ? State.Ok : State.Failed;
        }
    }

    public class WindowsServiceMonitor : BaseWindowsServiceHandler, IMonitor<WindowsService>
    {
        [DebuggerStepThrough]
        public void Handle(WindowsService windowsService)
        {
            var sc = GetServiceController(windowsService);

            SetState(windowsService, sc);

            sc.Close();
        }
    }

    public class StartWindowsService : BaseWindowsServiceHandler, IStart<WindowsService>
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public StartWindowsService(IDialogService dialogService, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        public async Task HandleAsync(WindowsService windowsService)
        {
            var dialog = new ConfirmationDialog($"Are you sure you want to start {windowsService.Name}?", "Start Windows Service");

            var result = _dialogService.ShowDialog(dialog);

            if (result != DialogResult.Yes) return;

            _logger.Info($"Starting {windowsService.Name}.");

            await StartAsync(windowsService).ConfigureAwait(false);
        }

        private async Task StartAsync(WindowsService windowsService)
        {
            await Task.Run(() =>
            {
                using (var sc = GetServiceController(windowsService))
                {
                    if (sc.Status != ServiceControllerStatus.Running && sc.Status != ServiceControllerStatus.StartPending)
                    {
                        sc.Start();

                        sc.WaitForStatus(ServiceControllerStatus.Running, Timeout);
                    }

                    SetState(windowsService, sc);

                    sc.Close();
                }
            });
        }
    }

    public class StopWindowsService : BaseWindowsServiceHandler, IStop<WindowsService>
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public StopWindowsService(IDialogService dialogService, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        public async Task HandleAsync(WindowsService windowsService)
        {
            var dialog = new ConfirmationDialog($"Are you sure you want to stop {windowsService.Name}?", "Stop Windows Service");

            var result = _dialogService.ShowDialog(dialog);

            if (result != DialogResult.Yes) return;

            _logger.Info($"Stopping {windowsService.Name}.");

            await StopAsync(windowsService).ConfigureAwait(false);
        }

        private async Task StopAsync(WindowsService windowsService)
        {
            await Task.Run(() =>
            {
                using (var sc = GetServiceController(windowsService))
                {
                    if (sc.Status == ServiceControllerStatus.Running || sc.Status == ServiceControllerStatus.StartPending)
                    {
                        sc.Stop();

                        sc.WaitForStatus(ServiceControllerStatus.Stopped, Timeout);
                    }

                    SetState(windowsService, sc);

                    sc.Close();
                }
            });
        }
    }

    public class RestartWindowsService : BaseWindowsServiceHandler, IRestart<WindowsService>
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public RestartWindowsService(IDialogService dialogService, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        public async Task HandleAsync(WindowsService windowsService)
        {
            var dialog = new ConfirmationDialog($"Are you sure you want to restart {windowsService.Name}?", "Restart Windows Service");

            var result = _dialogService.ShowDialog(dialog);

            if (result != DialogResult.Yes) return;

            _logger.Info($"Restarting {windowsService.Name}.");

            await RestartAsync(windowsService).ConfigureAwait(false);
        }

        private async Task RestartAsync(WindowsService windowsService)
        {
            await Task.Run(() =>
            {
                using (var sc = GetServiceController(windowsService))
                {
                    if (sc.Status == ServiceControllerStatus.Running || sc.Status == ServiceControllerStatus.StartPending)
                    {
                        sc.Stop();

                        sc.WaitForStatus(ServiceControllerStatus.Stopped, Timeout);
                    }

                    SetState(windowsService, sc);

                    sc.Start();

                    sc.WaitForStatus(ServiceControllerStatus.Running, Timeout);

                    SetState(windowsService, sc);

                    sc.Close();
                }
            });
        }
    }
}