using AnyStatus.API;
using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public abstract class WindowsServiceHandler
    {
        protected static TimeSpan WaitForStatusTimeout = TimeSpan.FromMinutes(1);

        protected ServiceController GetServiceController(WindowsService windowsService)
        {
            return string.IsNullOrWhiteSpace(windowsService.MachineName) ?
                    new ServiceController(windowsService.ServiceName) :
                    new ServiceController(windowsService.ServiceName, windowsService.MachineName);
        }

        protected static void SetState(WindowsService windowsService, ServiceControllerStatus status)
        {
            windowsService.State = status == windowsService.Status ? State.Ok : State.Failed;
        }

        protected async Task WaitForStatusAsync(ServiceController controller, ServiceControllerStatus status, TimeSpan timeout, CancellationToken cancellationToken)
        {
            //https://stackoverflow.com/questions/38236238/how-do-do-an-async-servicecontroller-waitforstatus

            var start = DateTime.UtcNow;

            controller.Refresh();

            while (controller.Status != status)
            {
                if (DateTime.UtcNow - start > timeout)
                {
                    throw new System.TimeoutException($"Failed to wait for '{controller.ServiceName}' to change status to '{status}'.");
                }

                await Task.Delay(250, cancellationToken).ConfigureAwait(false);

                controller.Refresh();
            }
        }
    }
}