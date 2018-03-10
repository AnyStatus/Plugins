using AnyStatus.API;
using System.Net.Sockets;

namespace AnyStatus
{
    public class PortHealthCheck : RequestHandler<HealthCheckRequest<Port>>, ICheckHealth<Port>
    {
        protected override void HandleCore(HealthCheckRequest<Port> request)
        {
            var protocol = request.DataContext.Protocol == NetworkProtocol.TCP ?
                    ProtocolType.Tcp :
                    ProtocolType.Udp;

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, protocol))
            {
                try
                {
                    socket.Connect(request.DataContext.Host, request.DataContext.PortNumber);

                    request.DataContext.State = State.Ok;
                }
                catch (SocketException)
                {
                    request.DataContext.State = State.Failed;
                }
            }
        }
    }
}