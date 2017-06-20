using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows;

namespace AnyStatus
{
    [DisplayName("Windows Service")]
    [Description("Test whether a windows service is running")]
    public class WindowsService : Item, IScheduledItem, ICanStartWindowsService, ICanStopWindowsService, ICanRestartWindowsService
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

    public class StartWindowsService : BaseWindowsServiceHandler, IStartWindowsService<WindowsService>
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
            var result = _dialogService.Show($"Are you sure you want to start {windowsService.Name}?", "Start Windows Service", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            if (result != MessageBoxResult.Yes)
                return;

            _logger.Info($"Starting {windowsService.Name}.");

            await Start(windowsService);
        }

        private async Task Start(WindowsService windowsService)
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

    public class StopWindowsService : BaseWindowsServiceHandler, IStopWindowsService<WindowsService>
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
            var result = _dialogService.Show($"Are you sure you want to stop {windowsService.Name}?", "Stop Windows Service", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            if (result != MessageBoxResult.Yes)
                return;

            _logger.Info($"Stopping {windowsService.Name}.");

            await Stop(windowsService);
        }

        private async Task Stop(WindowsService windowsService)
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

    public class RestartWindowsService : BaseWindowsServiceHandler, IRestartWindowsService<WindowsService>
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
            var result = _dialogService.Show($"Are you sure you want to restart {windowsService.Name}?", "Restart Windows Service", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            if (result != MessageBoxResult.Yes)
                return;

            _logger.Info($"Restarting {windowsService.Name}.");

            await Restart(windowsService);
        }

        private async Task Restart(WindowsService windowsService)
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