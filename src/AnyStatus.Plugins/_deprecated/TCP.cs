using AnyStatus.API;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    /// <summary>
    /// Obsolete. Use Port health check instead.
    /// </summary>
    [Browsable(false)]
    [DisplayName("TCP")]
    [DisplayColumn("Network")]
    [Description("Check TCP server connectivity")]
    public class TcpPort : Widget, IHealthCheck, ISchedulable
    {
        private const string Category = "TCP";

        [Required]
        [Category(Category)]
        [PropertyOrder(10)]
        [Description("Host Name or IP Address")]
        public string Host { get; set; }

        [Required]
        [Category(Category)]
        [PropertyOrder(20)]
        [Range(0, ushort.MaxValue, ErrorMessage = "Port must be between 0 and 65535")]
        public int Port { get; set; }
    }

    [Obsolete("Use Port health check instead.")]
    public class TcpMonitor : RequestHandler<HealthCheckRequest<TcpPort>>, ICheckHealth<TcpPort>
    {
        protected override void HandleCore(HealthCheckRequest<TcpPort> request)
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    socket.Connect(request.DataContext.Host, request.DataContext.Port);

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