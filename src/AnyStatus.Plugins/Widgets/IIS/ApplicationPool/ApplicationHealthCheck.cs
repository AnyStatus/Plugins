using AnyStatus.API;
using Microsoft.Web.Administration;

namespace AnyStatus
{
    public class ApplicationHealthCheck : RequestHandler<HealthCheckRequest<IISApplicationPool>>, 
        ICheckHealth<IISApplicationPool>
    {
        protected override void HandleCore(HealthCheckRequest<IISApplicationPool> request)
        {
            using (var iis = ServerManager.OpenRemote(request.DataContext.Host))
            {
                var appPool = iis.ApplicationPools[request.DataContext.ApplicationPoolName];

                request.DataContext.State = appPool.State == ObjectState.Started ? State.Ok : State.Failed;
            }
        }
    }
}