using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;

namespace AnyStatus.Plugins.Widgets.SystemInformation.Network
{
    [DisplayColumn("System Information")]
    [DisplayName("Active TCP Connections")]
    [Description("View the number of active TCP connections on the local computer.")]
    public class ActiveTcpConnections : Sparkline, ISchedulable
    {
        public ActiveTcpConnections()
        {
            Interval = 30;
            Units = IntervalUnits.Seconds;
        }
    }

    public class ActiveTcpConnectionsQuery : RequestHandler<MetricQueryRequest<ActiveTcpConnections>>
    {
        protected override void HandleCore(MetricQueryRequest<ActiveTcpConnections> request)
        {
            request.DataContext.Value = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections().Length;

            request.DataContext.State = State.Ok;
        }
    }
}
