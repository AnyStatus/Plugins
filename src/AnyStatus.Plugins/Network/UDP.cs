using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("UDP")]
    [DisplayColumn("Network")]
    [Description("Check UDP server connectivity")]
    public class Udp : Plugin, IAmMonitored
    {
        private const string Category = "UDP";

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

    public class UdpMonitor : IMonitor<Udp>
    {
        public void Handle(Udp udp)
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp))
            {
                try
                {
                    socket.Connect(udp.Host, udp.Port);

                    udp.State = State.Ok;
                }
                catch (SocketException)
                {
                    udp.State = State.Failed;
                }
            }
        }
    }
}
