using AnyStatus.API;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class PingHealthCheck : ICheckHealth<Ping>
    {
        [DebuggerStepThrough]
        public async Task Handle(HealthCheckRequest<Ping> request, CancellationToken cancellationToken)
        {
            using (var ping = new System.Net.NetworkInformation.Ping())
            {
                var reply = await ping.SendPingAsync(request.DataContext.Host)
                                      .ConfigureAwait(false);

                request.DataContext.State = (reply.Status == IPStatus.Success) ?
                    State.Ok :
                    State.Failed;
            }
        }
    }
}