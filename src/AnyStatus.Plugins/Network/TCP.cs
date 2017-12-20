using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("TCP")]
    [DisplayColumn("Network")]
    [Description("Check TCP server connectivity")]
    public class TcpPort : Widget, IMonitored
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

    public class TcpMonitor : IMonitor<TcpPort>
    {
        public void Handle(TcpPort tcp)
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    socket.Connect(tcp.Host, tcp.Port);

                    tcp.State = State.Ok;
                }
                catch (SocketException)
                {
                    tcp.State = State.Failed;
                }
            }
        }
    }
}
