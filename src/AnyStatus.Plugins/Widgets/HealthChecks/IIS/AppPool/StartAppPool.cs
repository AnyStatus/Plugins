using AnyStatus.API;
using Microsoft.Web.Administration;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class StartAppPool : IStart<IISApplicationPool>
    {
        public Task Handle(StartRequest<IISApplicationPool> request, CancellationToken cancellationToken)
        {
            using (var iis = ServerManager.OpenRemote(request.DataContext.Host))
            {
                var appPool = iis.ApplicationPools[request.DataContext.ApplicationPoolName];

                appPool.Start();

                request.DataContext.State = appPool.State == ObjectState.Started ? State.Ok : State.Failed;

                return Task.CompletedTask;
            }
        }
    }
}
