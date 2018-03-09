using AnyStatus.API;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class WindowsServiceHealthCheck : WindowsServiceHandler, ICheckHealth<WindowsService>
    {
        public Task Handle(HealthCheckRequest<WindowsService> request, CancellationToken cancellationToken)
        {
            var serviceController = GetServiceController(request.DataContext);

            SetState(request.DataContext, serviceController.Status);

            serviceController.Close();

            return Task.CompletedTask;
        }
    }
}