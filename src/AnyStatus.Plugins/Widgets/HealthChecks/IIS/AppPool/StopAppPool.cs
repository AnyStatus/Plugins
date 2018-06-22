using AnyStatus.API;
using Microsoft.Web.Administration;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class StopAppPool : IStop<IISApplicationPool>
    {
        public Task Handle(StopRequest<IISApplicationPool> request, CancellationToken cancellationToken)
        {
            using (var iis = ServerManager.OpenRemote(request.DataContext.Host))
            {
                var appPool = iis.ApplicationPools[request.DataContext.ApplicationPoolName];

                appPool.Stop();

                request.DataContext.State = appPool.State == ObjectState.Started ? State.Ok : State.Failed;

                return Task.CompletedTask;
            }
        }
    }
}
