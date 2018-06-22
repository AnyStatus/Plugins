using AnyStatus.API;
using Microsoft.Web.Administration;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class RestartAppPool : IRestart<IISApplicationPool>
    {
        public Task Handle(RestartRequest<IISApplicationPool> request, CancellationToken cancellationToken)
        {
            using (var iis = ServerManager.OpenRemote(request.DataContext.Host))
            {
                var appPool = iis.ApplicationPools[request.DataContext.ApplicationPoolName];

                appPool.Recycle();

                request.DataContext.State = appPool.State == ObjectState.Started ? State.Ok : State.Failed;

                return Task.CompletedTask;
            }
        }
    }
}
